// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Collections.Immutable;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;

namespace Deepstaging.Roslyn;

/// <summary>
/// Code fix action helpers for project-level modifications (MSBuild properties, files, etc.).
/// </summary>
public static class ProjectCodeFixActions
{
    /// <summary>
    /// Creates a code action that adds an MSBuild property to the project file.
    /// The property is inserted into the first <c>&lt;PropertyGroup&gt;</c>.
    /// </summary>
    /// <param name="project">The project to modify.</param>
    /// <param name="propertyName">The MSBuild property name (e.g., "UserSecretsId").</param>
    /// <param name="propertyValue">The property value.</param>
    /// <param name="comment">Optional XML comment placed above the property element.</param>
    public static CodeAction AddProjectPropertyAction(this Project project, string propertyName, string propertyValue, string? comment = null)
    {
        var title = $"Add <{propertyName}> to project";

        return new ModifyProjectFileCodeAction(
            title,
            project.FilePath,
            doc => AddProperty(doc, propertyName, propertyValue, comment));
    }

    /// <summary>
    /// Creates a code action that writes a file to the project directory.
    /// If the file already exists, it is overwritten.
    /// </summary>
    /// <param name="project">The project to modify.</param>
    /// <param name="relativePath">File path relative to the project directory (e.g., "appsettings.schema.json").</param>
    /// <param name="content">The file content to write.</param>
    /// <param name="title">Optional title for the code action. Defaults to "Create {relativePath}".</param>
    public static CodeAction WriteFileAction(this Project project, string relativePath, string content, string? title = null)
    {
        title ??= $"Create {relativePath}";
        return new WriteProjectFileCodeAction(title, project.FilePath, relativePath, content);
    }

    /// <summary>
    /// Creates a code action that writes multiple files to the project directory in a single operation.
    /// If any file already exists, it is overwritten.
    /// </summary>
    /// <param name="project">The project to modify.</param>
    /// <param name="title">The title for the code action.</param>
    /// <param name="files">Pairs of (relativePath, content) for each file to write.</param>
    public static CodeAction WriteFilesAction(this Project project, string title, ImmutableArray<(string RelativePath, string Content)> files) =>
        new WriteProjectFilesCodeAction(title, project.FilePath, files);

    /// <summary>
    /// Creates a builder for composing multiple file operations into a single code action.
    /// Supports writing (overwrite), conditional writing (skip if exists), and appending lines.
    /// </summary>
    /// <param name="project">The project to modify.</param>
    /// <param name="title">The title for the resulting code action.</param>
    public static ProjectFileActionsBuilder FileActions(this Project project, string title) =>
        new(title, project.FilePath);

    /// <summary>
    /// Creates a code action that modifies an XML file relative to the project directory.
    /// If the file does not exist, creates it with an empty <c>&lt;Project&gt;</c> root element.
    /// </summary>
    /// <param name="project">The project to modify.</param>
    /// <param name="title">The title for the code action.</param>
    /// <param name="relativePath">File path relative to the project directory.</param>
    /// <param name="modify">Action to modify the XML document.</param>
    public static CodeAction ModifyXmlFileAction(this Project project, string title, string relativePath, Action<XDocument> modify)
    {
        var projectDir = project.FilePath is not null ? Path.GetDirectoryName(project.FilePath) : null;
        if (projectDir is null)
            return CodeAction.Create(title, _ => Task.FromResult(project.Solution));

        var fullPath = Path.Combine(projectDir, relativePath);
        return new ModifyOrCreateXmlFileCodeAction(title, fullPath, modify);
    }

    #region MSBuild Property Helpers

    private static void AddProperty(XDocument document, string propertyName, string propertyValue, string? comment)
    {
        var root = document.Root;
        if (root is null) return;

        var propertyGroup = root.Elements("PropertyGroup").FirstOrDefault();
        if (propertyGroup is null) return;

        // Don't add if property already exists
        if (propertyGroup.Element(propertyName) is not null) return;

        // Infer indentation from existing children
        var indent = InferChildIndent(propertyGroup);

        if (comment is not null)
        {
            propertyGroup.Add(new XText(indent));
            propertyGroup.Add(new XComment($" {comment} "));
        }

        propertyGroup.Add(new XText(indent));
        propertyGroup.Add(new XElement(propertyName, propertyValue));
        propertyGroup.Add(new XText("\n"));
    }

    private static string InferChildIndent(XElement parent)
    {
        // Look at whitespace text nodes before existing child elements
        XNode? prev = null;

        foreach (var node in parent.Nodes())
        {
            if (node is XElement && prev is XText text)
            {
                var lines = text.Value.Split('\n');
                if (lines.Length > 1)
                    return "\n" + lines[lines.Length - 1];
            }

            prev = node;
        }

        return "\n        ";
    }

    #endregion

    #region Code Actions

    private sealed class ModifyProjectFileCodeAction(string title, string? projectFilePath, Action<XDocument> modify)
        : CodeAction
    {
        public override string Title => title;
        public override string? EquivalenceKey => title;

        protected override Task<ImmutableArray<CodeActionOperation>> ComputeOperationsAsync(
            IProgress<CodeAnalysisProgress> progress,
            CancellationToken cancellationToken)
        {
            if (projectFilePath is null)
                return Task.FromResult(ImmutableArray<CodeActionOperation>.Empty);

            return Task.FromResult(
                ImmutableArray.Create<CodeActionOperation>(
                    new ModifyProjectFileOperation(projectFilePath, modify)));
        }
    }

    private sealed class WriteProjectFileCodeAction(
        string title,
        string? projectFilePath,
        string relativePath,
        string content) : CodeAction
    {
        public override string Title => title;
        public override string? EquivalenceKey => title;

        protected override Task<ImmutableArray<CodeActionOperation>> ComputeOperationsAsync(
            IProgress<CodeAnalysisProgress> progress,
            CancellationToken cancellationToken)
        {
            if (projectFilePath is null)
                return Task.FromResult(ImmutableArray<CodeActionOperation>.Empty);

            var projectDir = Path.GetDirectoryName(projectFilePath);

            if (projectDir is null)
                return Task.FromResult(ImmutableArray<CodeActionOperation>.Empty);

            var fullPath = Path.Combine(projectDir, relativePath);

            return Task.FromResult(
                ImmutableArray.Create<CodeActionOperation>(
                    new WriteFileOperation(fullPath, content)));
        }
    }

    private sealed class WriteProjectFilesCodeAction(
        string title,
        string? projectFilePath,
        ImmutableArray<(string RelativePath, string Content)> files) : CodeAction
    {
        public override string Title => title;
        public override string? EquivalenceKey => title;

        protected override Task<ImmutableArray<CodeActionOperation>> ComputeOperationsAsync(
            IProgress<CodeAnalysisProgress> progress,
            CancellationToken cancellationToken)
        {
            if (projectFilePath is null)
                return Task.FromResult(ImmutableArray<CodeActionOperation>.Empty);

            var projectDir = Path.GetDirectoryName(projectFilePath);

            if (projectDir is null)
                return Task.FromResult(ImmutableArray<CodeActionOperation>.Empty);

            var operations = files
                .Select(f => (CodeActionOperation)new WriteFileOperation(
                    Path.Combine(projectDir, f.RelativePath), f.Content))
                .ToImmutableArray();

            return Task.FromResult(operations);
        }
    }

    private sealed class ModifyOrCreateXmlFileCodeAction(string title, string fullPath, Action<XDocument> modify)
        : CodeAction
    {
        public override string Title => title;
        public override string? EquivalenceKey => title;

        protected override Task<ImmutableArray<CodeActionOperation>> ComputeOperationsAsync(
            IProgress<CodeAnalysisProgress> progress,
            CancellationToken cancellationToken) =>
            Task.FromResult(
                ImmutableArray.Create<CodeActionOperation>(
                    new ModifyOrCreateXmlFileOperation(fullPath, modify)));
    }

    #endregion

    #region Operations

    private sealed class ModifyProjectFileOperation(string projectFilePath, Action<XDocument> modify) : CodeActionOperation
    {
        public override void Apply(Workspace workspace, CancellationToken cancellationToken)
        {
            if (!File.Exists(projectFilePath))
                return;

            var document = XDocument.Load(projectFilePath, LoadOptions.PreserveWhitespace);
            modify(document);
            document.Save(projectFilePath);
        }
    }

    private sealed class ModifyOrCreateXmlFileOperation(string fullPath, Action<XDocument> modify) : CodeActionOperation
    {
        public override void Apply(Workspace workspace, CancellationToken cancellationToken)
        {
            var directory = Path.GetDirectoryName(fullPath);

            if (directory is not null)
                Directory.CreateDirectory(directory);

            var document = File.Exists(fullPath)
                ? XDocument.Load(fullPath, LoadOptions.PreserveWhitespace)
                : new XDocument(new XElement("Project"));

            modify(document);
            document.Save(fullPath);
        }
    }

    private sealed class WriteFileOperation(string fullPath, string content) : CodeActionOperation
    {
        public override void Apply(Workspace workspace, CancellationToken cancellationToken)
        {
            var directory = Path.GetDirectoryName(fullPath);

            if (directory is not null)
                Directory.CreateDirectory(directory);

            File.WriteAllText(fullPath, content);
        }
    }

    #endregion
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Collections.Immutable;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;

namespace Deepstaging.Roslyn;

/// <summary>
/// Fluent builder for composing multiple file operations into a single <see cref="CodeAction"/>.
/// Supports writing (overwrite), conditional writing (skip if exists), and appending lines.
/// </summary>
/// <remarks>
/// Created via <c>project.FileActions("title")</c>. Example:
/// <code>
/// return project.FileActions("Generate configuration files")
///     .Write("schema.json", schemaContent)
///     .WriteIfNotExists("settings.json", "{}")
///     .AppendLine(".gitignore", "secrets.json")
///     .ToCodeAction();
/// </code>
/// </remarks>
public sealed class ProjectFileActionsBuilder(string title, string? projectFilePath)
{
    private readonly List<(string RelativePath, CodeActionOperation Operation)> _operations = [];

    /// <summary>
    /// Writes a file to the project directory, overwriting any existing content.
    /// </summary>
    public ProjectFileActionsBuilder Write(string relativePath, string content)
    {
        var fullPath = ResolvePath(relativePath);
        if (fullPath is not null)
            _operations.Add((relativePath, new WriteFileOperation(fullPath, content)));
        return this;
    }

    /// <summary>
    /// Writes a file to the project directory only if it does not already exist.
    /// </summary>
    public ProjectFileActionsBuilder WriteIfNotExists(string relativePath, string content)
    {
        var fullPath = ResolvePath(relativePath);
        if (fullPath is not null)
            _operations.Add((relativePath, new WriteFileIfNotExistsOperation(fullPath, content)));
        return this;
    }

    /// <summary>
    /// Appends a line to a file if the line is not already present.
    /// Creates the file if it does not exist.
    /// </summary>
    public ProjectFileActionsBuilder AppendLine(string relativePath, string line)
    {
        var fullPath = ResolvePath(relativePath);
        if (fullPath is not null)
            _operations.Add((relativePath, new AppendLineToFileOperation(fullPath, line)));
        return this;
    }

    /// <summary>
    /// Deep merges template JSON into an existing file, adding missing keys while preserving existing values.
    /// If the file does not exist, writes the template as-is.
    /// </summary>
    public ProjectFileActionsBuilder MergeJsonFile(string relativePath, string templateContent)
    {
        var fullPath = ResolvePath(relativePath);
        if (fullPath is not null)
            _operations.Add((relativePath, new MergeJsonFileOperation(fullPath, templateContent)));
        return this;
    }

    /// <summary>
    /// Synchronizes a JSON file with the template: adds missing keys, preserves existing values for matching keys,
    /// and removes keys not present in the template. Keys starting with <c>$</c> (e.g. <c>$schema</c>) are always preserved.
    /// If the file does not exist, writes the template as-is.
    /// </summary>
    public ProjectFileActionsBuilder SyncJsonFile(string relativePath, string templateContent)
    {
        var fullPath = ResolvePath(relativePath);
        if (fullPath is not null)
            _operations.Add((relativePath, new SyncJsonFileOperation(fullPath, templateContent)));
        return this;
    }

    /// <summary>
    /// Modifies the project file (.csproj) using the provided action on the XML document.
    /// The action receives the <see cref="XDocument"/> and can add, remove, or modify elements.
    /// </summary>
    public ProjectFileActionsBuilder ModifyProjectFile(Action<XDocument> modify)
    {
        if (projectFilePath is not null)
            _operations.Add(("project", new ModifyProjectFileOperation(projectFilePath, modify)));
        return this;
    }

    /// <summary>
    /// Modifies an XML file relative to the project directory using the provided action.
    /// If the file does not exist, creates it with an empty <c>&lt;Project&gt;</c> root element.
    /// </summary>
    public ProjectFileActionsBuilder ModifyXmlFile(string relativePath, Action<XDocument> modify)
    {
        var fullPath = ResolvePath(relativePath);
        if (fullPath is not null)
            _operations.Add((relativePath, new ModifyOrCreateXmlFileOperation(fullPath, modify)));
        return this;
    }

    /// <summary>
    /// Builds the composed operations into a single <see cref="CodeAction"/>.
    /// </summary>
    public CodeAction ToCodeAction() =>
        new CompositeFileCodeAction(title, [.._operations.Select(o => o.Operation)]);

    private string? ResolvePath(string relativePath)
    {
        if (projectFilePath is null)
            return null;

        var projectDir = Path.GetDirectoryName(projectFilePath);
        return projectDir is null ? null : Path.Combine(projectDir, relativePath);
    }

    #region Operations

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

    private sealed class WriteFileIfNotExistsOperation(string fullPath, string content) : CodeActionOperation
    {
        public override void Apply(Workspace workspace, CancellationToken cancellationToken)
        {
            if (File.Exists(fullPath))
                return;

            var directory = Path.GetDirectoryName(fullPath);

            if (directory is not null)
                Directory.CreateDirectory(directory);

            File.WriteAllText(fullPath, content);
        }
    }

    private sealed class AppendLineToFileOperation(string fullPath, string line) : CodeActionOperation
    {
        public override void Apply(Workspace workspace, CancellationToken cancellationToken)
        {
            var directory = Path.GetDirectoryName(fullPath);

            if (directory is not null)
                Directory.CreateDirectory(directory);

            if (File.Exists(fullPath))
            {
                var existing = File.ReadAllText(fullPath);
                if (existing.Contains(line))
                    return;

                var prefix = existing.Length > 0 && !existing.EndsWith("\n") ? "\n" : "";
                File.AppendAllText(fullPath, prefix + line + "\n");
            }
            else
            {
                File.WriteAllText(fullPath, line + "\n");
            }
        }
    }

    private sealed class MergeJsonFileOperation(string fullPath, string templateContent) : CodeActionOperation
    {
        public override void Apply(Workspace workspace, CancellationToken cancellationToken)
        {
            var directory = Path.GetDirectoryName(fullPath);

            if (directory is not null)
                Directory.CreateDirectory(directory);

            if (File.Exists(fullPath))
            {
                var existing = File.ReadAllText(fullPath);
                var merged = JsonMerge.Apply(existing, templateContent);
                File.WriteAllText(fullPath, merged);
            }
            else
            {
                File.WriteAllText(fullPath, templateContent);
            }
        }
    }

    private sealed class SyncJsonFileOperation(string fullPath, string templateContent) : CodeActionOperation
    {
        public override void Apply(Workspace workspace, CancellationToken cancellationToken)
        {
            var directory = Path.GetDirectoryName(fullPath);

            if (directory is not null)
                Directory.CreateDirectory(directory);

            if (File.Exists(fullPath))
            {
                var existing = File.ReadAllText(fullPath);
                var synced = JsonMerge.Sync(existing, templateContent);
                File.WriteAllText(fullPath, synced);
            }
            else
            {
                File.WriteAllText(fullPath, templateContent);
            }
        }
    }

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

    #endregion

    #region Code Action

    private sealed class CompositeFileCodeAction(string title, ImmutableArray<CodeActionOperation> operations)
        : CodeAction
    {
        public override string Title => title;
        public override string? EquivalenceKey => title;

        protected override Task<ImmutableArray<CodeActionOperation>> ComputeOperationsAsync(
            IProgress<CodeAnalysisProgress> progress,
            CancellationToken cancellationToken) =>
            Task.FromResult(operations);
    }

    #endregion
}

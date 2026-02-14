// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;

namespace Deepstaging.Roslyn;

/// <summary>
/// Represents a file to be added to a project by a code fix.
/// </summary>
public readonly struct AdditionalDocument
{
    /// <summary>
    /// The relative file path (e.g., "Templates/MyProject/Widget.scriban-cs").
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// The file content.
    /// </summary>
    public string Content { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AdditionalDocument"/> struct.
    /// </summary>
    public AdditionalDocument(string path, string content)
    {
        Path = path;
        Content = content;
    }
}

/// <summary>
/// Base class for code fix providers that add additional documents to a project.
/// Uses declarative configuration via <see cref="CodeFixAttribute"/>.
/// </summary>
/// <remarks>
/// Unlike <see cref="SyntaxCodeFix{TSyntax}"/> which modifies existing source,
/// this base class creates new files in the project (as additional documents).
/// Override <see cref="CreateDocument"/> to define the file path and content.
/// </remarks>
public abstract class AdditionalDocumentCodeFix : CodeFixProvider
{
    /// <inheritdoc />
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; }

    /// <inheritdoc />
    public override FixAllProvider? GetFixAllProvider() => null;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdditionalDocumentCodeFix"/> class.
    /// </summary>
    protected AdditionalDocumentCodeFix()
    {
        var codeFixAttrs = GetType()
            .GetCustomAttributes<CodeFixAttribute>()
            .ToArray();

        if (codeFixAttrs.Length == 0)
            throw new InvalidOperationException(
                $"CodeFix {GetType().Name} must have at least one [CodeFix] attribute.");

        FixableDiagnosticIds = [..codeFixAttrs.Select(a => a.DiagnosticId)];
    }

    /// <inheritdoc />
    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics[0];
        var compilation = await context.Document.Project
            .GetCompilationAsync(context.CancellationToken)
            .ConfigureAwait(false);

        if (compilation is null) return;

        var result = CreateDocument(compilation, diagnostic);
        if (result is not { } doc) return;

        var title = GetTitle(doc, diagnostic);

        context.RegisterCodeFix(
            CodeAction.Create(
                title,
                _ => Task.FromResult(AddDocument(context.Document.Project, doc)),
                equivalenceKey: title),
            diagnostic);
    }

    /// <summary>
    /// Creates the additional document to add to the project.
    /// </summary>
    /// <param name="compilation">The current compilation (for reading metadata).</param>
    /// <param name="diagnostic">The diagnostic being fixed.</param>
    /// <returns>The document to add, or null to skip.</returns>
    protected abstract AdditionalDocument? CreateDocument(Compilation compilation, Diagnostic diagnostic);

    /// <summary>
    /// Gets the title for the code action shown in the IDE lightbulb menu.
    /// Default returns "Create file: {path}".
    /// </summary>
    /// <param name="document">The document that will be created.</param>
    /// <param name="diagnostic">The diagnostic being fixed.</param>
    protected virtual string GetTitle(AdditionalDocument document, Diagnostic diagnostic)
    {
        return $"Create file: {document.Path}";
    }

    private static Solution AddDocument(Project project, AdditionalDocument doc)
    {
        return project.Solution.AddAdditionalDocument(
            DocumentId.CreateNewId(project.Id),
            doc.Path,
            SourceText.From(doc.Content),
            filePath: doc.Path);
    }
}

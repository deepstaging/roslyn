// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Deepstaging.Roslyn.Scriban;

/// <summary>
/// Code fix that creates a Scriban template file from the scaffold content
/// advertised by a generator via <see cref="ScaffoldEmitter"/>.
/// </summary>
/// <remarks>
/// This code fix responds to the scaffold available diagnostic (DSRK005).
/// It reads the scaffold content from the compilation's assembly metadata and creates
/// a <c>Templates/{Name}.scriban-cs</c> file in the project.
/// </remarks>
[Shared]
[CodeFix(ScaffoldDiagnostics.ScaffoldAvailable)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ScaffoldTemplateCodeFix))]
public sealed class ScaffoldTemplateCodeFix : AdditionalDocumentCodeFix
{
    /// <inheritdoc />
    protected override AdditionalDocument? CreateDocument(Compilation compilation, Diagnostic diagnostic)
    {
        if (!diagnostic.Properties.TryGetValue("TemplateName", out var templateName) ||
            templateName is null)
            return null;

        var scaffolds = ScaffoldMetadata.ReadFrom(compilation);
        var scaffold = scaffolds.FirstOrDefault(s => s.TemplateName == templateName);
        if (scaffold.Scaffold is null) return null;

        var path = $"Templates/{templateName}{ScribanExtension.CSharp.Value}";
        return new AdditionalDocument(path, scaffold.Scaffold);
    }

    /// <inheritdoc />
    protected override string GetTitle(AdditionalDocument document, Diagnostic diagnostic)
    {
        return $"Create template: {document.Path}";
    }
}

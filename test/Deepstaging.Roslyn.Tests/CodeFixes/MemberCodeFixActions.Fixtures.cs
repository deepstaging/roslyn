// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.Roslyn.Tests.CodeFixes;

// Reuses NeedsTypeFixAttribute / NeedsTypeFixAnalyzer from TypeCodeFixActions.Fixtures.cs

// ── Attribute code fixes ─────────────────────────────────────────────────────

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class AddObsoleteAttributeFix : ClassCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.AddAttributeAction(syntax, "Obsolete");
}

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class AddObsoleteWithArgsFix : ClassCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.AddAttributeAction(syntax, "Obsolete", "\"Use NewClass instead\"");
}

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class RemoveNeedsTypeFixAttributeFix : ClassCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.RemoveAttributeAction(syntax, "NeedsTypeFix");
}

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class ReplaceNeedsTypeFixAttributeFix : ClassCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.ReplaceAttributeAction(syntax, "NeedsTypeFix", "Serializable");
}
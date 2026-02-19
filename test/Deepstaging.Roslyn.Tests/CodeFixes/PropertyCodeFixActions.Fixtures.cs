// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Analyzers;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Roslyn.Tests.CodeFixes;

// ── Trigger attribute ────────────────────────────────────────────────────────

[AttributeUsage(AttributeTargets.Property)]
public sealed class NeedsPropertyFixAttribute : Attribute;

// ── Analyzer ─────────────────────────────────────────────────────────────────

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports("TEST_PROP", "Property needs fix",
    Message = "Property '{0}' needs a fix",
    Severity = DiagnosticSeverity.Warning)]
public sealed class NeedsPropertyFixAnalyzer : PropertyAnalyzer
{
    public const string DiagnosticId = "TEST_PROP";

    protected override bool ShouldReport(ValidSymbol<IPropertySymbol> property) =>
        property.HasAttribute<NeedsPropertyFixAttribute>();
}

// ── Code fixes ───────────────────────────────────────────────────────────────

[CodeFix(NeedsPropertyFixAnalyzer.DiagnosticId)]
public sealed class AddPartialToPropertyFix : PropertyCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<PropertyDeclarationSyntax> syntax) =>
        document.AddPartialModifierAction(syntax);
}

[CodeFix(NeedsPropertyFixAnalyzer.DiagnosticId)]
public sealed class AddRequiredToPropertyFix : PropertyCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<PropertyDeclarationSyntax> syntax) =>
        document.AddRequiredModifierAction(syntax);
}

[CodeFix(NeedsPropertyFixAnalyzer.DiagnosticId)]
public sealed class MakePropertyInitOnlyFix : PropertyCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<PropertyDeclarationSyntax> syntax) =>
        document.MakePropertyInitOnlyAction(syntax);
}

[CodeFix(NeedsPropertyFixAnalyzer.DiagnosticId)]
public sealed class RenamePropertyFix : PropertyCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<PropertyDeclarationSyntax> syntax) =>
        document.RenamePropertyAction(syntax, "NewName");
}
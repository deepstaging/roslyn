// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Analyzers;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Roslyn.Tests.CodeFixes;

// ── Trigger attribute ────────────────────────────────────────────────────────

[AttributeUsage(AttributeTargets.Field)]
public sealed class NeedsFieldFixAttribute : Attribute;

// ── Analyzer ─────────────────────────────────────────────────────────────────

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports("TEST_FIELD", "Field needs fix",
    Message = "Field '{0}' needs a fix",
    Severity = DiagnosticSeverity.Warning)]
public sealed class NeedsFieldFixAnalyzer : FieldAnalyzer
{
    public const string DiagnosticId = "TEST_FIELD";

    protected override bool ShouldReport(ValidSymbol<IFieldSymbol> field) =>
        field.HasAttribute<NeedsFieldFixAttribute>();
}

// ── Code fixes ───────────────────────────────────────────────────────────────

[CodeFix(NeedsFieldFixAnalyzer.DiagnosticId)]
public sealed class MakeFieldPrivateFix : FieldCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<FieldDeclarationSyntax> syntax) =>
        document.MakeFieldPrivateAction(syntax);
}

[CodeFix(NeedsFieldFixAnalyzer.DiagnosticId)]
public sealed class AddReadonlyToFieldFix : FieldCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<FieldDeclarationSyntax> syntax) =>
        document.AddFieldReadonlyModifierAction(syntax);
}

[CodeFix(NeedsFieldFixAnalyzer.DiagnosticId)]
public sealed class RenameFieldFix : FieldCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<FieldDeclarationSyntax> syntax) =>
        document.RenameFieldAction(syntax, "_value");
}

[CodeFix(NeedsFieldFixAnalyzer.DiagnosticId)]
public sealed class RenameFieldWithIndexFix : FieldCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<FieldDeclarationSyntax> syntax) =>
        document.RenameFieldAction(syntax, 1, "_second");
}

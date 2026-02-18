// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Analyzers;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Roslyn.Tests.CodeFixes;

// ── Trigger attribute ────────────────────────────────────────────────────────

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class NeedsTypeFixAttribute : Attribute;

// ── Analyzer ─────────────────────────────────────────────────────────────────

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports("TEST_TYPE", "Type needs fix",
    Message = "Type '{0}' needs a fix",
    Severity = DiagnosticSeverity.Warning)]
public sealed class NeedsTypeFixAnalyzer : TypeAnalyzer
{
    public const string DiagnosticId = "TEST_TYPE";

    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<NeedsTypeFixAttribute>();
}

// ── Modifier code fixes ──────────────────────────────────────────────────────

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class AddPartialToClassFix : ClassCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.AddPartialModifierAction(syntax);
}

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class AddSealedToClassFix : ClassCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.AddSealedModifierAction(syntax);
}

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class AddStaticToClassFix : ClassCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.AddStaticModifierAction(syntax);
}

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class AddAbstractToClassFix : ClassCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.AddAbstractModifierAction(syntax);
}

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class AddReadonlyToStructFix : StructCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<StructDeclarationSyntax> syntax) =>
        document.AddReadonlyModifierAction(syntax);
}

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class RemoveSealedFromClassFix : ClassCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.RemoveModifierAction(syntax, SyntaxKind.SealedKeyword, "Remove 'sealed' modifier");
}

// ── Member insertion code fixes ──────────────────────────────────────────────

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class AddMembersToClassFix : ClassCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax)
    {
        var method = MethodBuilder
            .Parse("public void DoWork()")
            .WithExpressionBody("null")
            .Build();

        return document.AddMembersAction(syntax, "Add DoWork method", method);
    }
}

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class AddMembersFromSourceToClassFix : ClassCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.AddMembersFromSourceAction(syntax, "Add members from source",
            "public int Value { get; set; }");
}

// ── Rename code fix ──────────────────────────────────────────────────────────

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class RenameClassFix : ClassCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.RenameTypeAction(syntax, "Renamed");
}

// ── Base type code fixes ─────────────────────────────────────────────────────

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class AddBaseTypeToClassFix : ClassCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.AddBaseTypeAction(syntax, "BaseClass");
}

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class AddInterfaceToClassFix : ClassCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.AddInterfaceAction(syntax, "IDisposable");
}
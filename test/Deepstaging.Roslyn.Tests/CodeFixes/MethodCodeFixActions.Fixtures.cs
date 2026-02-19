// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Analyzers;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Roslyn.Tests.CodeFixes;

// ── Trigger attribute ────────────────────────────────────────────────────────

[AttributeUsage(AttributeTargets.Method)]
public sealed class NeedsMethodFixAttribute : Attribute;

// ── Analyzer ─────────────────────────────────────────────────────────────────

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports("TEST_METHOD", "Method needs fix",
    Message = "Method '{0}' needs a fix",
    Severity = DiagnosticSeverity.Warning)]
public sealed class NeedsMethodFixAnalyzer : MethodAnalyzer
{
    public const string DiagnosticId = "TEST_METHOD";

    protected override bool ShouldReport(ValidSymbol<IMethodSymbol> method) =>
        method.HasAttribute<NeedsMethodFixAttribute>();
}

// ── Modifier code fixes ──────────────────────────────────────────────────────

[CodeFix(NeedsMethodFixAnalyzer.DiagnosticId)]
public sealed class AddPartialToMethodFix : MethodCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<MethodDeclarationSyntax> syntax) =>
        document.AddPartialModifierAction(syntax);
}

[CodeFix(NeedsMethodFixAnalyzer.DiagnosticId)]
public sealed class AddAsyncToMethodFix : MethodCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<MethodDeclarationSyntax> syntax) =>
        document.AddAsyncModifierAction(syntax);
}

[CodeFix(NeedsMethodFixAnalyzer.DiagnosticId)]
public sealed class AddVirtualToMethodFix : MethodCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<MethodDeclarationSyntax> syntax) =>
        document.AddVirtualModifierAction(syntax);
}

[CodeFix(NeedsMethodFixAnalyzer.DiagnosticId)]
public sealed class AddOverrideToMethodFix : MethodCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<MethodDeclarationSyntax> syntax) =>
        document.AddOverrideModifierAction(syntax);
}

[CodeFix(NeedsMethodFixAnalyzer.DiagnosticId)]
public sealed class AddStaticToMethodFix : MethodCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<MethodDeclarationSyntax> syntax) =>
        document.AddStaticMethodModifierAction(syntax);
}

[CodeFix(NeedsMethodFixAnalyzer.DiagnosticId)]
public sealed class RemoveStaticFromMethodFix : MethodCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<MethodDeclarationSyntax> syntax) =>
        document.RemoveMethodModifierAction(syntax, SyntaxKind.StaticKeyword, "Remove 'static' modifier");
}

// ── Rename code fixes ────────────────────────────────────────────────────────

[CodeFix(NeedsMethodFixAnalyzer.DiagnosticId)]
public sealed class RenameMethodFix : MethodCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<MethodDeclarationSyntax> syntax) =>
        document.RenameMethodAction(syntax, "NewName");
}

[CodeFix(NeedsMethodFixAnalyzer.DiagnosticId)]
public sealed class AddAsyncSuffixFix : MethodCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<MethodDeclarationSyntax> syntax) =>
        document.AddAsyncSuffixAction(syntax);
}

[CodeFix(NeedsMethodFixAnalyzer.DiagnosticId)]
public sealed class RemoveAsyncSuffixFix : MethodCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<MethodDeclarationSyntax> syntax) =>
        document.RemoveAsyncSuffixAction(syntax);
}

// ── Return type code fixes ───────────────────────────────────────────────────

[CodeFix(NeedsMethodFixAnalyzer.DiagnosticId)]
public sealed class ChangeReturnTypeFix : MethodCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<MethodDeclarationSyntax> syntax) =>
        document.ChangeReturnTypeAction(syntax, "string");
}

[CodeFix(NeedsMethodFixAnalyzer.DiagnosticId)]
public sealed class WrapReturnTypeInTaskFix : MethodCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<MethodDeclarationSyntax> syntax) =>
        document.WrapReturnTypeInTaskAction(syntax);
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.Roslyn.Tests.CodeFixes;

// Reuses NeedsTypeFixAttribute / NeedsTypeFixAnalyzer from TypeCodeFixActions.Fixtures.cs

// ── Document-level code fixes ────────────────────────────────────────────────

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class AddUsingFix : ClassCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.AddUsingAction("System.Collections.Generic");
}

public sealed class SuppressWithPragmaFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [NeedsTypeFixAnalyzer.DiagnosticId];
    public override FixAllProvider? GetFixAllProvider() => null;

    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics[0];
        var action = context.Document.SuppressWithPragmaAction(diagnostic);
        context.RegisterCodeFix(action, diagnostic);
        return Task.CompletedTask;
    }
}
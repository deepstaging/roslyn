// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Analyzers;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Roslyn.Tests.CodeFixes;

// ── Toy attribute (lives in test assembly, referenced by test source) ────────

[AttributeUsage(AttributeTargets.Class)]
public sealed class GenerateHelperAttribute : Attribute;

// ── Toy analyzer ─────────────────────────────────────────────────────────────

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports("TEST001", "Class can generate helper",
    Message = "Class '{0}' has [GenerateHelper] and can generate a helper file",
    Severity = DiagnosticSeverity.Info)]
public sealed class GenerateHelperAnalyzer : TypeAnalyzer
{
    public const string DiagnosticId = "TEST001";

    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<GenerateHelperAttribute>();
}

// ── Toy code fix (non-generic) ──────────────────────────────────────────────

[CodeFix(GenerateHelperAnalyzer.DiagnosticId)]
public sealed class GenerateHelperCodeFix : SourceDocumentCodeFix
{
    protected override SourceDocument? CreateDocument(Compilation compilation, Diagnostic diagnostic)
    {
        if (compilation.GetSymbolAtDiagnostic(diagnostic).IsNotValid(out var symbol))
            return null;

        var emit = TypeBuilder
            .Parse($"public static class {symbol.Name}Helper")
            .InNamespace(symbol.Namespace ?? "Global")
            .AddMethod(MethodBuilder
                .Parse($"public static string Describe()")
                .WithExpressionBody($"\"Helper for {symbol.Name}\""))
            .Emit();

        if (emit.IsNotValid(out var source))
            return null;

        return new SourceDocument($"{symbol.Name}Helper.g.cs", source.Code);
    }
}

// ── Toy code fix (generic, with symbol resolution) ──────────────────────────

[CodeFix(GenerateHelperAnalyzer.DiagnosticId)]
public sealed class GenerateHelperWithSymbolCodeFix : SourceDocumentCodeFix<INamedTypeSymbol>
{
    protected override SourceDocument? CreateDocument(Compilation compilation, ValidSymbol<INamedTypeSymbol> symbol)
    {
        var emit = TypeBuilder
            .Parse($"public static class {symbol.Name}Helper")
            .InNamespace(symbol.Namespace ?? "Global")
            .AddMethod(MethodBuilder
                .Parse($"public static string Describe()")
                .WithExpressionBody($"\"Helper for {symbol.Name}\""))
            .Emit();

        if (emit.IsNotValid(out var source))
            return null;

        return new SourceDocument($"{symbol.Name}Helper.g.cs", source.Code);
    }

    protected override string GetTitle(SourceDocument document, Diagnostic diagnostic) =>
        $"Generate helper: {document.Path}";
}

// ── Toy code fix that returns null (no fix) ─────────────────────────────────

[CodeFix(GenerateHelperAnalyzer.DiagnosticId)]
public sealed class NullSourceDocumentCodeFix : SourceDocumentCodeFix
{
    protected override SourceDocument? CreateDocument(Compilation compilation, Diagnostic diagnostic) => null;
}
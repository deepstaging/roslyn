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
        var tree = diagnostic.Location.SourceTree;
        if (tree is null) return null;

        var root = tree.GetRoot();
        var node = root.FindNode(diagnostic.Location.SourceSpan);
        var semanticModel = compilation.GetSemanticModel(tree);
        var symbol = semanticModel.GetDeclaredSymbol(node);
        if (symbol is null) return null;

        var ns = symbol.ContainingNamespace?.ToDisplayString() ?? "Global";
        var name = symbol.Name;

        var content = $$"""
                        namespace {{ns}};

                        public static class {{name}}Helper
                        {
                            public static string Describe() => "Helper for {{name}}";
                        }
                        """;

        return new SourceDocument($"{name}Helper.g.cs", content);
    }
}

// ── Toy code fix (generic, with symbol resolution) ──────────────────────────

[CodeFix(GenerateHelperAnalyzer.DiagnosticId)]
public sealed class GenerateHelperWithSymbolCodeFix : SourceDocumentCodeFix<INamedTypeSymbol>
{
    protected override SourceDocument? CreateDocument(Compilation compilation, ValidSymbol<INamedTypeSymbol> symbol)
    {
        var content =
            $$"""
              namespace {{symbol.Namespace ?? "Global"}};

              public static class {{symbol.Name}}Helper
              {
                  public static string Describe() => "Helper for {{symbol.Name}}";
              }
              """;

        return new SourceDocument($"{symbol.Name}Helper.g.cs", content);
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
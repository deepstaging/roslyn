// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Roslyn.Testing;

/// <summary>
/// Unified base class for testing Roslyn components (symbols, analyzers, generators, code fixes).
/// Provides entry points to all testing contexts with a clean, consistent API.
/// </summary>
public abstract class RoslynTestBase
{
    /// <summary>
    /// Create a symbol test context from source code for testing queries and projections.
    /// </summary>
    /// <param name="source">The C# source code to compile.</param>
    /// <returns>A context for querying symbols from the compilation.</returns>
    protected SymbolTestContext SymbolsFor(string source) =>
        new(CompilationHelper.CreateCompilation(source));

    /// <summary>
    /// Get the compilation for source code.
    /// </summary>
    /// <param name="source">The C# source code to compile.</param>
    /// <returns>The compilation.</returns>
    protected Compilation CompilationFor(string source) =>
        SymbolsFor(source).Compilation;

    /// <summary>
    /// Run an analyzer against source code and return a fluent assertion context.
    /// </summary>
    /// <typeparam name="TAnalyzer">The analyzer type to test.</typeparam>
    /// <param name="source">The C# source code to analyze.</param>
    /// <returns>A context for making fluent assertions about diagnostics.</returns>
    protected AnalyzerTestContext AnalyzeWith<TAnalyzer>(string source)
        where TAnalyzer : DiagnosticAnalyzer, new() =>
        new(source, new TAnalyzer());

    /// <summary>
    /// Run a generator against source code and return a fluent assertion context.
    /// </summary>
    /// <typeparam name="TGenerator">The generator type to test.</typeparam>
    /// <param name="source">The C# source code to generate from.</param>
    /// <returns>A context for making fluent assertions about generated code.</returns>
    protected GeneratorTestContext GenerateWith<TGenerator>(string source)
        where TGenerator : new() =>
        new(source, new TGenerator());

    /// <summary>
    /// Start a code fix test with the provided source code.
    /// </summary>
    /// <typeparam name="TCodeFix">The code fix provider type to test.</typeparam>
    /// <param name="source">The C# source code containing a diagnostic to fix.</param>
    /// <returns>A context for applying code fixes and making assertions.</returns>
    protected CodeFixTestContext FixWith<TCodeFix>(string source)
        where TCodeFix : CodeFixProvider, new() =>
        new(source, new TCodeFix());

    /// <summary>
    /// Start a code fix test with an analyzer that produces the diagnostics.
    /// Use this when testing code fixes for analyzer diagnostics (not compiler diagnostics).
    /// </summary>
    /// <typeparam name="TAnalyzer">The analyzer type that produces the diagnostics.</typeparam>
    /// <typeparam name="TCodeFix">The code fix provider type to test.</typeparam>
    /// <param name="source">The C# source code to analyze and fix.</param>
    /// <returns>A context for applying code fixes and making assertions.</returns>
    protected CodeFixTestContext AnalyzeAndFixWith<TAnalyzer, TCodeFix>(string source)
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new() =>
        new(source, new TCodeFix(), new TAnalyzer());

    /// <summary>
    /// Create a template test context for rendering templates with a compilation.
    /// </summary>
    /// <param name="source">The C# source code to compile.</param>
    /// <returns>A context for rendering templates with symbol queries.</returns>
    protected TemplateTestContext RenderTemplateFrom<TGenerator>(string source) =>
        new(CompilationFor(source), TemplateName.ForGenerator<TGenerator>());
}
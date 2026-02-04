// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.Reflection;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Roslyn.Analyzers;

/// <summary>
/// Base class for symbol-based analyzers that use declarative configuration via <see cref="ReportsAttribute"/>.
/// </summary>
/// <typeparam name="TSymbol">The symbol type to analyze.</typeparam>
public abstract class SymbolAnalyzer<TSymbol> : DiagnosticAnalyzer
    where TSymbol : class, ISymbol
{
    private readonly DiagnosticDescriptor _rule;
    private readonly SymbolKind _symbolKind;

    /// <summary>
    /// Gets the diagnostic rule for this analyzer.
    /// </summary>
    protected DiagnosticDescriptor Rule => _rule;

    /// <inheritdoc />
    public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [_rule];

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolAnalyzer{TSymbol}"/> class.
    /// </summary>
    protected SymbolAnalyzer()
    {
        var reportsAttr = GetType().GetCustomAttribute<ReportsAttribute>()
            ?? throw new InvalidOperationException(
                $"Analyzer {GetType().Name} must have a [Reports] attribute.");

        _rule = reportsAttr.ToDescriptor();
        _symbolKind = InferSymbolKind();
    }

    /// <inheritdoc />
    public sealed override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeSymbol, _symbolKind);
    }

    private void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        if (context.Symbol is not TSymbol symbol)
            return;

        var valid = ValidSymbol<TSymbol>.From(symbol);
        if (!ShouldReport(valid))
            return;

        var location = GetLocation(valid);
        var messageArgs = GetMessageArgs(valid);

        context.ReportDiagnostic(Diagnostic.Create(_rule, location, messageArgs));
    }

    /// <summary>
    /// Determines whether a diagnostic should be reported for the given symbol.
    /// </summary>
    /// <param name="symbol">The validated symbol to analyze.</param>
    /// <returns><c>true</c> if a diagnostic should be reported; otherwise, <c>false</c>.</returns>
    protected abstract bool ShouldReport(ValidSymbol<TSymbol> symbol);

    /// <summary>
    /// Gets the message format arguments for the diagnostic.
    /// Default returns the symbol name as the first argument.
    /// </summary>
    /// <param name="symbol">The validated symbol.</param>
    /// <returns>The message format arguments.</returns>
    protected virtual object[] GetMessageArgs(ValidSymbol<TSymbol> symbol) => [symbol.Name];

    /// <summary>
    /// Gets the location for the diagnostic.
    /// Default returns the first declared location.
    /// </summary>
    /// <param name="symbol">The validated symbol.</param>
    /// <returns>The diagnostic location.</returns>
    protected virtual Location GetLocation(ValidSymbol<TSymbol> symbol) => symbol.Location;

    private static SymbolKind InferSymbolKind()
    {
        var symbolType = typeof(TSymbol);

        if (symbolType == typeof(INamedTypeSymbol)) return SymbolKind.NamedType;
        if (symbolType == typeof(IMethodSymbol)) return SymbolKind.Method;
        if (symbolType == typeof(IPropertySymbol)) return SymbolKind.Property;
        if (symbolType == typeof(IFieldSymbol)) return SymbolKind.Field;
        if (symbolType == typeof(IEventSymbol)) return SymbolKind.Event;
        if (symbolType == typeof(IParameterSymbol)) return SymbolKind.Parameter;
        if (symbolType == typeof(INamespaceSymbol)) return SymbolKind.Namespace;

        throw new InvalidOperationException(
            $"Cannot infer SymbolKind from {symbolType.Name}. Use a more specific symbol type.");
    }
}

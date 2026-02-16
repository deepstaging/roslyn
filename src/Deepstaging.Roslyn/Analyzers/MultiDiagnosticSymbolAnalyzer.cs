// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Reflection;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Roslyn.Analyzers;

/// <summary>
/// Base class for symbol-based analyzers that report multiple diagnostics per symbol.
/// Each item returned from <see cref="GetDiagnosticItems"/> produces one diagnostic.
/// </summary>
/// <typeparam name="TSymbol">The symbol type to analyze.</typeparam>
/// <typeparam name="TItem">The item type representing each diagnostic occurrence.</typeparam>
public abstract class MultiDiagnosticSymbolAnalyzer<TSymbol, TItem> : DiagnosticAnalyzer
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
    /// Initializes a new instance of the <see cref="MultiDiagnosticSymbolAnalyzer{TSymbol, TItem}"/> class.
    /// </summary>
    protected MultiDiagnosticSymbolAnalyzer()
    {
        var reportsAttr = GetType().GetCustomAttribute<ReportsAttribute>() ??
                          throw new InvalidOperationException(
                              $"Analyzer {GetType().Name} must have a [Reports] attribute.");

        _rule = reportsAttr.ToDescriptor();
        _symbolKind = SymbolAnalyzer<TSymbol>.InferSymbolKindFor<TSymbol>();
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

        foreach (var item in GetDiagnosticItems(valid))
        {
            var location = GetLocation(valid, item);
            var properties = GetProperties(valid, item);
            var messageArgs = GetMessageArgs(valid, item);

            context.ReportDiagnostic(Diagnostic.Create(_rule, location, properties, messageArgs));
        }
    }

    /// <summary>
    /// Returns the items that should each produce a diagnostic for the given symbol.
    /// Return an empty sequence to report nothing.
    /// </summary>
    /// <param name="symbol">The validated symbol to analyze.</param>
    /// <returns>An item for each diagnostic to report.</returns>
    protected abstract IEnumerable<TItem> GetDiagnosticItems(ValidSymbol<TSymbol> symbol);

    /// <summary>
    /// Gets the message format arguments for a specific diagnostic item.
    /// Default returns the symbol name as the first argument.
    /// </summary>
    /// <param name="symbol">The validated symbol.</param>
    /// <param name="item">The diagnostic item.</param>
    /// <returns>The message format arguments.</returns>
    protected virtual object[] GetMessageArgs(ValidSymbol<TSymbol> symbol, TItem item) => [symbol.Name];

    /// <summary>
    /// Gets the location for a specific diagnostic item.
    /// Default returns the first declared location of the symbol.
    /// </summary>
    /// <param name="symbol">The validated symbol.</param>
    /// <param name="item">The diagnostic item.</param>
    /// <returns>The diagnostic location.</returns>
    protected virtual Location GetLocation(ValidSymbol<TSymbol> symbol, TItem item) => symbol.Location;

    /// <summary>
    /// Gets the properties dictionary for a specific diagnostic item.
    /// Code fixes can read these via <c>diagnostic.Properties</c>.
    /// Default returns <c>null</c> (no properties).
    /// </summary>
    /// <param name="symbol">The validated symbol.</param>
    /// <param name="item">The diagnostic item.</param>
    /// <returns>An immutable dictionary of property key-value pairs, or <c>null</c>.</returns>
    protected virtual ImmutableDictionary<string, string?>? GetProperties(ValidSymbol<TSymbol> symbol, TItem item) =>
        null;
}
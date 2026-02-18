# MultiDiagnosticSymbolAnalyzer

`MultiDiagnosticSymbolAnalyzer<TSymbol, TItem>` is for analyzers that report **zero or more diagnostics per symbol** — one for each problematic item found.

## Quick Start

```csharp
[Reports("DSRK001", "Pipeline model property uses ImmutableArray<T>",
    Message = "Property '{0}' on '{1}' uses ImmutableArray<T> — use EquatableArray<T>",
    Category = "PipelineModel")]
public sealed class ImmutableArrayAnalyzer : MultiDiagnosticTypeAnalyzer<ValidSymbol<IPropertySymbol>>
{
    protected override IEnumerable<ValidSymbol<IPropertySymbol>> GetDiagnosticItems(
        ValidSymbol<INamedTypeSymbol> type)
    {
        if (type.LacksAttribute<PipelineModelAttribute>())
            yield break;

        foreach (var prop in type.QueryProperties().ThatAreInstance()
                     .Where(x => x.Type.IsImmutableArrayType()).GetAll())
            yield return prop;
    }

    protected override object[] GetMessageArgs(
        ValidSymbol<INamedTypeSymbol> symbol, ValidSymbol<IPropertySymbol> item)
        => [item.Name, symbol.Name];

    protected override Location GetLocation(
        ValidSymbol<INamedTypeSymbol> symbol, ValidSymbol<IPropertySymbol> item)
        => item.Location;
}
```

## How It Works

1. Declare the diagnostic via `[Reports]` — one rule shared by all items
2. For each symbol, `GetDiagnosticItems` returns items that should produce diagnostics
3. Each item becomes one `Diagnostic.Create` call with custom location and message args

## Specialized Aliases

| Alias | Equivalent To |
|-------|---------------|
| `MultiDiagnosticTypeAnalyzer<TItem>` | `MultiDiagnosticSymbolAnalyzer<INamedTypeSymbol, TItem>` |
| `MultiDiagnosticMethodAnalyzer<TItem>` | `MultiDiagnosticSymbolAnalyzer<IMethodSymbol, TItem>` |
| `MultiDiagnosticPropertyAnalyzer<TItem>` | `MultiDiagnosticSymbolAnalyzer<IPropertySymbol, TItem>` |

## Abstract & Virtual Members

| Member | Kind | Default | Description |
|--------|------|---------|-------------|
| `GetDiagnosticItems(symbol)` | abstract | — | Return items that should each produce a diagnostic |
| `GetMessageArgs(symbol, item)` | virtual | `[symbol.Name]` | Message format arguments |
| `GetLocation(symbol, item)` | virtual | `symbol.Location` | Diagnostic location |
| `GetProperties(symbol, item)` | virtual | `null` | Properties forwarded to code fixes |

## Comparison with SymbolAnalyzer

| | `SymbolAnalyzer<T>` | `MultiDiagnosticSymbolAnalyzer<T, I>` |
|---|---|---|
| **Diagnostics per symbol** | One | Zero or more |
| **Override** | `ShouldReport(symbol) → bool` | `GetDiagnosticItems(symbol) → IEnumerable<TItem>` |
| **Location** | Type declaration | Custom via `GetLocation()` |
| **Message args** | Type name only | Custom via `GetMessageArgs()` |
| **Properties** | None | Custom via `GetProperties()` |

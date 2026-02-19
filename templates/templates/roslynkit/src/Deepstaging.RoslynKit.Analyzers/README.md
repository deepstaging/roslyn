# Deepstaging.RoslynKit.Analyzers

Diagnostic analyzers that validate `[AutoNotify]` usage at compile time.

## Diagnostics

| ID | Severity | Description | Fix |
|----|----------|-------------|-----|
| RK001 | Error | Type with `[AutoNotify]` must be `partial` | Add `partial` modifier |
| RK002 | Error | Backing field in `[AutoNotify]` type must be `private` | Make field `private` |

## Implementation

Both analyzers extend `SymbolAnalyzer<T>` from Deepstaging.Roslyn. The entire analyzer is a single `ShouldReport` predicate:

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports("RK001", "Type must be partial", ...)]
public sealed class MustBePartialAnalyzer : SymbolAnalyzer<INamedTypeSymbol>
{
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> symbol) =>
        symbol.HasAttribute<AutoNotifyAttribute>() && !symbol.IsPartial;
}
```

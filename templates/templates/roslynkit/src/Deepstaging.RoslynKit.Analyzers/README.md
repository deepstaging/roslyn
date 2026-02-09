# Deepstaging.RoslynKit.Analyzers

Diagnostic analyzers that validate correct usage of RoslynKit attributes. Uses the [Deepstaging.Roslyn](https://github.com/deepstaging/roslyn) framework and shares projections with the generators.

## Diagnostics

All diagnostic IDs are centralized in `Diagnostics.cs`:

| ID | Constant | Severity | Description | Fix Available |
|----|----------|----------|-------------|---------------|
| RK1001 | `Diagnostics.GenerateWithMustBePartial` | Error | Type with `[GenerateWith]` must be declared as `partial` | ✅ |
| RK1002 | `Diagnostics.AutoNotifyMustBePartial` | Error | Type with `[AutoNotify]` must be declared as `partial` | ✅ |
| RK1003 | `Diagnostics.AutoNotifyFieldMustBePrivate` | Warning | AutoNotify backing field should be `private` | ✅ |

## Analyzers

| Analyzer | Diagnostic |
|----------|------------|
| `GenerateWithAnalyzer` | RK1001 |
| `AutoNotifyMustBePartialAnalyzer` | RK1002 |
| `AutoNotifyFieldMustBePrivateAnalyzer` | RK1003 |

## Architecture

Analyzers use the Projection layer's `BackingFieldConventions` for consistent field naming validation:

```
Symbol → BackingFieldConventions.IsBackingFieldName() → Validation → Diagnostic
```

## Related Projects

- [RoslynKit](../Deepstaging.RoslynKit/) - Attribute definitions
- [RoslynKit.Projection](../Deepstaging.RoslynKit.Projection/) - Shared queries, models, and conventions
- [RoslynKit.CodeFixes](../Deepstaging.RoslynKit.CodeFixes/) - Code fixes for these diagnostics
- [RoslynKit.Tests](../Deepstaging.RoslynKit.Tests/) - Analyzer tests
- [Project README](../../README.md) - Full documentation

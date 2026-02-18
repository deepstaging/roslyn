# Analyzers

The `Deepstaging.Roslyn.Analyzers` namespace provides base classes and utilities for building Roslyn diagnostic analyzers with minimal boilerplate.

## Architecture

```
DiagnosticAnalyzer (Roslyn)
├── SymbolAnalyzer<TSymbol>              — Per-symbol, single diagnostic
│   ├── TypeAnalyzer                     — INamedTypeSymbol
│   ├── MethodAnalyzer                   — IMethodSymbol
│   ├── PropertyAnalyzer                 — IPropertySymbol
│   ├── FieldAnalyzer                    — IFieldSymbol
│   ├── EventAnalyzer                    — IEventSymbol
│   ├── ParameterAnalyzer                — IParameterSymbol
│   ├── NamespaceAnalyzer                — INamespaceSymbol
│   └── TypeParameterAnalyzer            — ITypeParameterSymbol
├── MultiDiagnosticSymbolAnalyzer<S,I>   — Per-symbol, multiple diagnostics
│   ├── MultiDiagnosticTypeAnalyzer<I>   — INamedTypeSymbol
│   ├── MultiDiagnosticMethodAnalyzer<I> — IMethodSymbol
│   └── MultiDiagnosticPropertyAnalyzer<I> — IPropertySymbol
├── AssemblyAttributeAnalyzer<TItem>     — Assembly-level attributes
└── TrackedFileTypeAnalyzer              — File existence/staleness tracking
```

## Choosing the Right Base Class

| Scenario | Base Class |
|----------|------------|
| Single yes/no check per symbol | [SymbolAnalyzer](symbol-analyzer.md) (or a specialized alias) |
| Multiple diagnostics per symbol (one per item) | [MultiDiagnosticSymbolAnalyzer](multi-diagnostic-analyzer.md) |
| Assembly-level `[assembly: ...]` attributes | [AssemblyAttributeAnalyzer](assembly-attribute-analyzer.md) |
| File existence / staleness tracking | [TrackedFileTypeAnalyzer](tracked-file-analyzer.md) |
| Custom registration patterns | Extend `DiagnosticAnalyzer` directly |

## Declarative Configuration

All base classes use attribute-based configuration instead of manual `DiagnosticDescriptor` fields:

- **[Reports]** — Declares diagnostic ID, title, message, severity, category. Used by `SymbolAnalyzer`, `MultiDiagnosticSymbolAnalyzer`, and `AssemblyAttributeAnalyzer`.
- **[TracksFiles]** — Declares paired missing/stale diagnostics. Used by `TrackedFileTypeAnalyzer`.

See [ReportsAttribute](reports-attribute.md) and [TracksFilesAttribute](tracked-file-analyzer.md#tracksfilesattribute) for details.

## Utilities

- [BuildPropertyExtensions](build-property-extensions.md) — Typed access to MSBuild `CompilerVisibleProperty` values
- [TrackedFiles](tracked-file-analyzer.md#trackedfiles) — Additional file discovery with embedded hash tracking
- [PipelineModelAttribute](pipeline-model.md) — Marks records as incremental generator pipeline models

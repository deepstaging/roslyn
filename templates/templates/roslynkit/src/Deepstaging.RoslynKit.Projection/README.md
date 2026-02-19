# Deepstaging.RoslynKit.Projection

Shared projection layer — the single source of truth for extracting data from Roslyn symbols.

Both the generator and analyzer consume models from this project. This avoids duplicating symbol-walking logic and ensures consistent interpretation of attributes.

## Key Files

| File | Purpose |
|------|---------|
| `AutoNotifyProjection.cs` | `ValidSymbol<INamedTypeSymbol>.QueryAutoNotify()` extension |
| `Models/AutoNotifyModel.cs` | Strongly-typed model for the class |
| `Models/AutoNotifyFieldModel.cs` | Strongly-typed model per field |
| `Attributes/AlsoNotifyAttributeQuery.cs` | Typed accessor for `[AlsoNotify]` constructor args |

## The Pattern

```
Roslyn Symbol → QueryAutoNotify() → AutoNotifyModel → Generator / Analyzer
```

Models are `[PipelineModel]` records with `EquatableArray<T>` fields, making them safe for incremental generator caching.

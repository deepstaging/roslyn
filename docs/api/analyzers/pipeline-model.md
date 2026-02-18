# PipelineModelAttribute

`[PipelineModel]` marks a sealed record as a model used in incremental source generator pipelines. It enables compile-time validation of equality semantics required for Roslyn's caching to work correctly.

## Usage

```csharp
[PipelineModel]
public sealed record MyModel(string Name, int Count, EquatableArray<string> Items);
```

## Why It Matters

Roslyn's incremental generator pipeline caches models between compilations. If a model's `Equals` implementation is incorrect (e.g., uses reference equality for collections), the pipeline won't detect changes and will serve stale generated code.

`[PipelineModel]` enables the built-in analyzers to catch common mistakes:

| ID | Diagnostic | Description |
|----|-----------|-------------|
| `DSRK001` | Uses `ImmutableArray<T>` | Use `EquatableArray<T>` instead (implements value equality) |
| `DSRK002` | Uses `ValidSymbol<T>` | Use snapshot types (`TypeSnapshot`, `MethodSnapshot`) instead |
| `DSRK003` | Uses `ISymbol` | Extract data during projection, don't store symbols |
| `DSRK004` | Non-`IEquatable<T>` property | All properties must implement `IEquatable<T>` |

## Rules

- Must be a `sealed record` (value equality via records)
- Use `EquatableArray<T>` instead of `ImmutableArray<T>` for collections
- Don't store Roslyn symbols — use snapshot types or extract primitive data
- All property types must implement `IEquatable<T>`

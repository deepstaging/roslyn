# PipelineModel

The `[PipelineModel]` attribute marks sealed records used in incremental generator pipelines. It ships with four analyzers that enforce correct equality semantics — preventing the most common causes of broken incremental caching.

> **See also:** [EquatableArray](equatable-array.md) | [Snapshots](snapshots.md) | [Projections Overview](index.md)

## The Problem

Incremental source generators cache pipeline outputs and skip re-emit when inputs haven't changed. This only works if the pipeline model's `Equals` method returns `true` for semantically identical data. Three common patterns silently break this:

1. **`ImmutableArray<T>`** uses reference equality in records
2. **`ValidSymbol<T>`** wraps live `ISymbol` — broken equality + memory retention
3. **`ISymbol`** retains the entire `Compilation` in memory
4. **Non-`IEquatable<T>` types** fall back to `object.Equals` (reference equality)

## Usage

```csharp
using Deepstaging.Roslyn;

[PipelineModel]
public sealed record MyModel
{
    public required string Name { get; init; }
    public required EquatableArray<MethodSnapshot> Methods { get; init; }
    public required TypeSnapshot TargetType { get; init; }
}
```

The attribute itself has no runtime behavior — it's a marker for the analyzers.

---

## Analyzer Rules

The `Deepstaging.Roslyn` NuGet package bundles these analyzers automatically. They run at compile time on any type decorated with `[PipelineModel]`.

### DSRK001 — ImmutableArray in Pipeline Model

| | |
|---|---|
| **Severity** | Error |
| **Category** | PipelineModel |
| **Message** | Property '{name}' on pipeline model '{type}' uses ImmutableArray&lt;T&gt; which has reference equality — use EquatableArray&lt;T&gt; instead |

`ImmutableArray<T>` is a struct with reference equality. In a `record`, this means two models with identical array contents compare as unequal, defeating caching.

**Fix:** Replace `ImmutableArray<T>` with [`EquatableArray<T>`](equatable-array.md).

```csharp
// ❌ DSRK001
public ImmutableArray<string> Names { get; init; }

// ✅ Fixed
public EquatableArray<string> Names { get; init; }
```

---

### DSRK002 — ValidSymbol in Pipeline Model

| | |
|---|---|
| **Severity** | Error |
| **Category** | PipelineModel |
| **Message** | Property '{name}' on pipeline model '{type}' uses ValidSymbol&lt;T&gt; which retains the Compilation — use a snapshot type instead |

`ValidSymbol<T>` holds a reference to an `ISymbol`, which holds the entire `Compilation` — every syntax tree, symbol table, and metadata reference. Storing it in a pipeline model prevents garbage collection of old compilations.

**Fix:** Replace with the appropriate [snapshot type](snapshots.md) and call `.ToSnapshot()` during projection.

```csharp
// ❌ DSRK002
public required ValidSymbol<INamedTypeSymbol> TargetType { get; init; }

// ✅ Fixed
public required TypeSnapshot TargetType { get; init; }
```

---

### DSRK003 — ISymbol in Pipeline Model

| | |
|---|---|
| **Severity** | Error |
| **Category** | PipelineModel |
| **Message** | Property '{name}' on pipeline model '{type}' uses an ISymbol type which retains the Compilation — extract data during the projection step |

Same memory retention problem as DSRK002, but for raw Roslyn symbol types (`ISymbol`, `INamedTypeSymbol`, `IMethodSymbol`, etc.).

**Fix:** Extract the data you need as strings or snapshot types during the projection step.

```csharp
// ❌ DSRK003
public required INamedTypeSymbol Symbol { get; init; }

// ✅ Fixed — extract only what's needed
public required string SymbolName { get; init; }
public required string SymbolFullyQualifiedName { get; init; }

// ✅ Or use a snapshot if you need the full picture
public required TypeSnapshot Symbol { get; init; }
```

---

### DSRK004 — Non-IEquatable Field

| | |
|---|---|
| **Severity** | Warning |
| **Category** | PipelineModel |
| **Message** | Property '{name}' on pipeline model '{type}' has type '{propertyType}' which does not implement IEquatable&lt;T&gt; — equality will be broken |

Types that don't implement `IEquatable<T>` fall back to `object.Equals`, which uses reference equality for classes. This silently breaks caching.

**Fix:** Ensure the property type implements `IEquatable<T>`. For custom types, use a `sealed record` (which auto-implements it) or implement the interface manually.

```csharp
// ❌ DSRK004 — XElement doesn't implement IEquatable
public XElement? Documentation { get; init; }

// ✅ Fixed — use DocumentationSnapshot (a record with correct equality)
public DocumentationSnapshot Documentation { get; init; }
```

!!! note
    DSRK004 excludes types already caught by DSRK001–DSRK003 to avoid duplicate diagnostics.

---

## Guidelines

### What Makes a Good Pipeline Model

- **`sealed record`** — provides value equality via compiler-generated `Equals`
- **`EquatableArray<T>`** instead of `ImmutableArray<T>` — sequence equality
- **Snapshot types** instead of `ValidSymbol<T>` or `ISymbol` — no Compilation retention
- **All field types implement `IEquatable<T>`** — no reference equality fallback
- **Computed properties use `=>`** — record equality ignores expression-bodied properties

### Model Lifecycle

```
ISymbol (live, compilation-bound)
    ↓  .ToSnapshot() or .Snapshots()
Snapshot types (pipeline-safe, equatable)
    ↓  stored in
[PipelineModel] record (cached by pipeline)
    ↓  read by
Writer (emits code via TypeRef.From(snapshot))
```

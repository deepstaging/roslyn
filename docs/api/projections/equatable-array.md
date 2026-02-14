# EquatableArray<T>

A drop-in replacement for `ImmutableArray<T>` that provides structural (sequence) equality. Use this in pipeline models instead of `ImmutableArray<T>` to ensure incremental generator caching works correctly.

> **See also:** [Projections Overview](index.md) | [Snapshots](snapshots.md) | [PipelineModel](pipeline-model.md)

## The Problem

`ImmutableArray<T>` is a `struct` that uses **reference equality** — two arrays with identical contents are considered unequal. When used in `record` types, this breaks the auto-generated `Equals` implementation, causing incremental generators to re-emit on every compilation even when nothing has changed.

```csharp
var a = ImmutableArray.Create(1, 2, 3);
var b = ImmutableArray.Create(1, 2, 3);
a.Equals(b); // false — reference equality!
```

## The Solution

`EquatableArray<T>` wraps `ImmutableArray<T>` and implements `IEquatable<EquatableArray<T>>` with element-wise comparison:

```csharp
var a = new EquatableArray<int>(ImmutableArray.Create(1, 2, 3));
var b = new EquatableArray<int>(ImmutableArray.Create(1, 2, 3));
a.Equals(b); // true — sequence equality!
```

## Usage

### In Pipeline Models

```csharp
[PipelineModel]
public sealed record MyModel
{
    // ❌ Breaks caching — reference equality
    public ImmutableArray<string> Names { get; init; }
    
    // ✅ Correct — sequence equality
    public EquatableArray<string> Names { get; init; }
}
```

### Creating

```csharp
// From ImmutableArray (implicit conversion)
ImmutableArray<string> immutable = ImmutableArray.Create("a", "b");
EquatableArray<string> equatable = immutable;

// From IEnumerable
EquatableArray<string> equatable = names.ToEquatableArray();

// From ImmutableArray explicitly
EquatableArray<string> equatable = immutable.ToEquatableArray();

// Empty
EquatableArray<string> empty = EquatableArray<string>.Empty;

// Collection expression
EquatableArray<string> items = ["a", "b", "c"];
```

### Projecting

```csharp
// Transform and wrap in one call
EquatableArray<string> names = symbols.SelectEquatable(s => s.Name);

// From ImmutableArray with projection
EquatableArray<MethodSnapshot> methods = immutableMethods.SelectEquatable(m => m.ToSnapshot());
```

### Unwrapping

When Roslyn APIs require `ImmutableArray<T>`:

```csharp
EquatableArray<string> equatable = ...;

// Implicit conversion
ImmutableArray<string> immutable = equatable;

// Explicit unwrap
ImmutableArray<string> immutable = equatable.AsImmutableArray();
```

### Iterating

`EquatableArray<T>` implements `IReadOnlyList<T>`:

```csharp
EquatableArray<MethodSnapshot> methods = ...;

// Indexer
var first = methods[0];

// Count
var count = methods.Count;

// foreach
foreach (var method in methods)
    Console.WriteLine(method.Name);

// LINQ
var names = methods.Where(m => m.IsPublic).Select(m => m.Name);
```

## Constraint

`EquatableArray<T>` requires `T : IEquatable<T>`. This is satisfied by:

- All primitive types (`int`, `string`, `bool`, etc.)
- Enums
- All `sealed record` types (records auto-implement `IEquatable<T>`)
- All snapshot types (`TypeSnapshot`, `MethodSnapshot`, etc.)

## API Reference

| Member | Description |
|--------|-------------|
| `EquatableArray(ImmutableArray<T>)` | Constructor |
| `Empty` | Static property — empty array |
| `Count` | Number of elements |
| `this[int]` | Indexer |
| `AsImmutableArray()` | Unwrap to `ImmutableArray<T>` |
| `Equals(EquatableArray<T>)` | Element-wise equality |
| `GetHashCode()` | Combined element hashes |
| `GetEnumerator()` | `IReadOnlyList<T>` enumerator |

### Extension Methods

| Method | Description |
|--------|-------------|
| `source.ToEquatableArray()` | From `IEnumerable<T>` or `ImmutableArray<T>` |
| `source.SelectEquatable(selector)` | Project and wrap in one call |

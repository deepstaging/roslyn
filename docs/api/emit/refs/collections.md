# CollectionRefs & ImmutableCollectionRefs

Generic and immutable collection type references.

> **See also:** [Refs Overview](index.md) | [TypeRef & Primitives](../type-ref.md)

---

## CollectionRefs

`System.Collections.Generic` types.

### Types

| Method | Produces |
|--------|----------|
| `List(element)` | `List<T>` |
| `Dictionary(key, value)` | `Dictionary<TKey, TValue>` |
| `HashSet(element)` | `HashSet<T>` |
| `KeyValuePair(key, value)` | `KeyValuePair<TKey, TValue>` |

### Interfaces

| Method | Produces |
|--------|----------|
| `IEnumerable(element)` | `IEnumerable<T>` |
| `ICollection(element)` | `ICollection<T>` |
| `IList(element)` | `IList<T>` |
| `IDictionary(key, value)` | `IDictionary<TKey, TValue>` |
| `ISet(element)` | `ISet<T>` |
| `IReadOnlyList(element)` | `IReadOnlyList<T>` |
| `IReadOnlyCollection(element)` | `IReadOnlyCollection<T>` |
| `IReadOnlyDictionary(key, value)` | `IReadOnlyDictionary<TKey, TValue>` |

### Examples

```csharp
// Return type
method.WithReturnType(CollectionRefs.IReadOnlyList("string"))
// → global::System.Collections.Generic.IReadOnlyList<string>

// Parameter type
method.AddParameter("items", CollectionRefs.List("int"))
// → global::System.Collections.Generic.List<int>

// Nested generics
CollectionRefs.Dictionary("string", CollectionRefs.List("int"))
// → global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.List<int>>

// Field
builder.AddField("_cache",
    CollectionRefs.Dictionary("string", TaskRefs.Task("byte[]")))
```

---

## ImmutableCollectionRefs

`System.Collections.Immutable` types.

### Types

| Method | Produces |
|--------|----------|
| `ImmutableArray(element)` | `ImmutableArray<T>` |
| `ImmutableList(element)` | `ImmutableList<T>` |
| `ImmutableDictionary(key, value)` | `ImmutableDictionary<TKey, TValue>` |

### Examples

```csharp
// Return type
method.WithReturnType(ImmutableCollectionRefs.ImmutableArray("Diagnostic"))
// → global::System.Collections.Immutable.ImmutableArray<Diagnostic>

// Snapshot-friendly collections in pipeline models
property.WithType(ImmutableCollectionRefs.ImmutableDictionary("string", "object"))
```

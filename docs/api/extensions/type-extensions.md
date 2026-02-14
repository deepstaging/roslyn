# Type Extensions

Extensions on `ITypeSymbol` organized by category for discoverability. Each category lives in its own file (`ITypeSymbol.{Category}.cs`).

> **See also:** [Symbol Extensions](symbol-extensions.md) | [Extensions Overview](index.md)

---

## Async (`ITypeSymbol.Async.cs`)

Check Task and ValueTask types:

```csharp
typeSymbol.IsTaskType()              // Task or ValueTask (with or without T)
typeSymbol.IsValueTaskType()         // ValueTask or ValueTask<T>
typeSymbol.IsGenericTaskType()       // Task<T>
typeSymbol.IsGenericValueTaskType()  // ValueTask<T>
typeSymbol.IsNonGenericTaskType()    // Task (no type arg)
typeSymbol.IsNonGenericValueTaskType() // ValueTask (no type arg)
```

---

## Collections (`ITypeSymbol.Collections.cs`)

### Mutable Interfaces

```csharp
typeSymbol.IsEnumerableType()        // IEnumerable<T>
typeSymbol.IsCollectionType()        // ICollection<T>
typeSymbol.IsListType()              // IList<T>
typeSymbol.IsDictionaryType()        // IDictionary<TKey, TValue>
typeSymbol.IsSetType()               // ISet<T>
```

### Read-Only Interfaces

```csharp
typeSymbol.IsReadOnlyListType()          // IReadOnlyList<T>
typeSymbol.IsReadOnlyCollectionType()    // IReadOnlyCollection<T>
typeSymbol.IsReadOnlyDictionaryType()    // IReadOnlyDictionary<TKey, TValue>
```

### Concrete Collections

```csharp
typeSymbol.IsHashSetType()               // HashSet<T>
typeSymbol.IsReadOnlyCollectionConcreteType() // ReadOnlyCollection<T>
```

### Immutable Collections

```csharp
typeSymbol.IsImmutableArrayType()        // ImmutableArray<T>
typeSymbol.IsImmutableListType()         // ImmutableList<T>
typeSymbol.IsImmutableDictionaryType()   // ImmutableDictionary<TKey, TValue>
typeSymbol.IsImmutableHashSetType()      // ImmutableHashSet<T>
```

### Query & Observable

```csharp
typeSymbol.IsQueryableType()             // IQueryable<T>
typeSymbol.IsObservableType()            // IObservable<T>
```

---

## Classification (`ITypeSymbol.Classification.cs`)

### Type Kinds

```csharp
typeSymbol.IsEnumType()          // enum
typeSymbol.IsInterfaceType()     // interface
typeSymbol.IsRecordType()        // record class or record struct
typeSymbol.IsStructType()        // struct (excluding enums)
typeSymbol.IsClassType()         // class
typeSymbol.IsArrayType()         // T[]
typeSymbol.IsPointerType()       // T*
typeSymbol.IsTupleType()         // ValueTuple
typeSymbol.IsNullableValueType() // Nullable<T>
```

### Modifiers

```csharp
typeSymbol.IsAbstractType()
typeSymbol.IsSealedType()
typeSymbol.IsStaticType()
```

---

## Delegates (`ITypeSymbol.Delegates.cs`)

```csharp
typeSymbol.IsFuncType()          // Func<...>
typeSymbol.IsActionType()        // Action or Action<...>
typeSymbol.IsDelegateType()      // any delegate type
typeSymbol.IsExpressionType()    // Expression<T>
```

---

## Inheritance (`ITypeSymbol.Inheritance.cs`)

```csharp
// Check type hierarchy
typeSymbol.ImplementsOrInheritsFrom(baseType)    // ITypeSymbol comparison
typeSymbol.IsOrInheritsFrom("Exception", "System") // by name + optional namespace
typeSymbol.InheritsFrom("Controller", "Microsoft.AspNetCore.Mvc") // excludes self

// Navigate hierarchy
ITypeSymbol? base = typeSymbol.GetBaseTypeByName("DbContext");
ITypeSymbol? arg = typeSymbol.GetSingleTypeArgument();  // T from Generic<T>
```

---

## Equality (`ITypeSymbol.Equality.cs`)

```csharp
typeSymbol.ImplementsIEquatable()
```

Handles all common cases transparently:

- Primitives (`int`, `string`, `bool`, etc.)
- Enums
- Types explicitly implementing `IEquatable<T>`
- `Nullable<T>` — delegates to inner type

---

## Roslyn Types (`ITypeSymbol.Roslyn.cs`)

Deepstaging-specific checks for identifying Roslyn and library types:

```csharp
typeSymbol.IsValidSymbolType()   // Deepstaging.Roslyn.ValidSymbol<T>
typeSymbol.IsRoslynSymbolType()  // Microsoft.CodeAnalysis.ISymbol or implementors
```

Used by the [PipelineModel analyzers](../projections/pipeline-model.md) (DSRK002 and DSRK003).

---

## Common Types (`ITypeSymbol.CommonTypes.cs`)

Well-known value types and framework types:

### Time

```csharp
typeSymbol.IsTimeSpanType()          // System.TimeSpan
typeSymbol.IsDateTimeType()          // System.DateTime
typeSymbol.IsDateTimeOffsetType()    // System.DateTimeOffset
```

### Identifiers

```csharp
typeSymbol.IsGuidType()              // System.Guid
typeSymbol.IsUriType()               // System.Uri
```

### Threading

```csharp
typeSymbol.IsCancellationTokenType() // System.Threading.CancellationToken
```

### Wrappers

```csharp
typeSymbol.IsLazyType()              // Lazy<T>
```

---

## Span & Memory (`ITypeSymbol.SpanMemory.cs`)

```csharp
typeSymbol.IsSpanType()              // Span<T>
typeSymbol.IsReadOnlySpanType()      // ReadOnlySpan<T>
typeSymbol.IsMemoryType()            // Memory<T>
typeSymbol.IsReadOnlyMemoryType()    // ReadOnlyMemory<T>
```

---

## Queries (`ITypeSymbol.Queries.cs`)

Start query builders from any `ITypeSymbol`:

```csharp
typeSymbol.QueryMethods()        // MethodQuery
typeSymbol.QueryProperties()     // PropertyQuery
typeSymbol.QueryFields()         // FieldQuery
typeSymbol.QueryConstructors()   // ConstructorQuery
typeSymbol.QueryEvents()         // EventQuery
typeSymbol.QueryAttributes()     // ImmutableArray<AttributeData>
```

---

## File Organization

Extensions are organized by category in the filesystem for easy scanning:

```
Extensions/Roslyn/
├── ITypeSymbol.Async.cs          — Task, ValueTask
├── ITypeSymbol.Classification.cs — enum, interface, record, struct, modifiers
├── ITypeSymbol.Collections.cs    — all collection type checks
├── ITypeSymbol.CommonTypes.cs    — TimeSpan, DateTime, Guid, Uri, etc.
├── ITypeSymbol.Delegates.cs      — Func, Action, Expression, delegate
├── ITypeSymbol.Equality.cs       — ImplementsIEquatable
├── ITypeSymbol.Inheritance.cs    — base type navigation
├── ITypeSymbol.Queries.cs        — query builder starters
├── ITypeSymbol.Roslyn.cs         — ValidSymbol, ISymbol checks
└── ITypeSymbol.SpanMemory.cs     — Span, Memory
```

# Types

Type-safe wrappers for common .NET generic types. Each wrapper is a `readonly record struct` that carries constituent type information and produces globally-qualified output.

> **See also:** [TypeRef & Primitives](emit/type-ref.md) | [Expressions](expressions.md) | [LanguageExt Types](languageext/types.md)

## Overview

When you write `TypeRef.Global("System.Collections.Generic.List<string>")`, the element type `string` is baked into the string — lost to downstream code. Typed wrappers preserve this structure:

```csharp
using Deepstaging.Roslyn.Types;

// ❌ Flat string — can't recover the element type
TypeRef list = TypeRef.Global("System.Collections.Generic.List<string>");

// ✅ Typed wrapper — element type is accessible
var list = new ListTypeRef("string");
list.ElementType  // → "string"
list.ToString()   // → "global::System.Collections.Generic.List<string>"
```

Every wrapper has:

- **Constituent type properties** — access the inner types that built the wrapper
- **Implicit conversion to `TypeRef`** — use anywhere a `TypeRef` is expected
- **Implicit conversion to `string`** — use in string interpolation
- **Globally-qualified `ToString()`** — always produces `global::*` output

---

## Async

### TaskTypeRef

Represents `Task<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ResultType` | `TypeRef` | The result type |

```csharp
var task = new TaskTypeRef("string");

task.ResultType   // → "string"
task.ToString()   // → "global::System.Threading.Tasks.Task<string>"

method.WithReturnType(task);  // implicit TypeRef conversion
```

### ValueTaskTypeRef

Represents `ValueTask<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ResultType` | `TypeRef` | The result type |

```csharp
var vtask = new ValueTaskTypeRef("int");
vtask.ToString()  // → "global::System.Threading.Tasks.ValueTask<int>"
```

---

## Collections

### ListTypeRef

Represents `List<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var list = new ListTypeRef("Order");
list.ElementType  // → "Order"
list.ToString()   // → "global::System.Collections.Generic.List<Order>"
```

### DictionaryTypeRef

Represents `Dictionary<TKey, TValue>`.

| Property | Type | Description |
|----------|------|-------------|
| `KeyType` | `TypeRef` | The key type |
| `ValueType` | `TypeRef` | The value type |

```csharp
var dict = new DictionaryTypeRef("string", "int");
dict.KeyType    // → "string"
dict.ValueType  // → "int"
dict.ToString() // → "global::System.Collections.Generic.Dictionary<string, int>"
```

### HashSetTypeRef

Represents `HashSet<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var set = new HashSetTypeRef("string");
set.ToString()  // → "global::System.Collections.Generic.HashSet<string>"
```

---

## Nullable

### NullableTypeRef

Represents `Nullable<T>` (value types only).

| Property | Type | Description |
|----------|------|-------------|
| `InnerType` | `TypeRef` | The underlying value type |

```csharp
var nullable = new NullableTypeRef("int");
nullable.InnerType  // → "int"
nullable.ToString() // → "global::System.Nullable<int>"
```

---

## Lazy

### LazyTypeRef

Represents `Lazy<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ValueType` | `TypeRef` | The lazily-initialized type |

```csharp
var lazy = new LazyTypeRef("ExpensiveService");
lazy.ValueType  // → "ExpensiveService"
lazy.ToString() // → "global::System.Lazy<ExpensiveService>"
```

---

## Comparison

### EqualityComparerTypeRef

Represents `EqualityComparer<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ComparedType` | `TypeRef` | The type being compared |

```csharp
var eq = new EqualityComparerTypeRef("string");
eq.ComparedType  // → "string"
eq.ToString()    // → "global::System.Collections.Generic.EqualityComparer<string>"
```

### ComparerTypeRef

Represents `Comparer<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ComparedType` | `TypeRef` | The type being compared |

```csharp
var cmp = new ComparerTypeRef("int");
cmp.ToString()  // → "global::System.Collections.Generic.Comparer<int>"
```

---

## Delegates

### FuncTypeRef

Represents `Func<T1, ..., TResult>`.

| Property | Type | Description |
|----------|------|-------------|
| `ParameterTypes` | `ImmutableArray<TypeRef>` | The parameter types |
| `ReturnType` | `TypeRef` | The return type |

```csharp
// Func<string, bool>
var func = new FuncTypeRef([TypeRef.From("string")], TypeRef.From("bool"));
func.ToString()  // → "global::System.Func<string, bool>"

// Func<int> (no parameters)
var producer = new FuncTypeRef(TypeRef.From("int"));
producer.ToString()  // → "global::System.Func<int>"
```

### ActionTypeRef

Represents `Action` or `Action<T1, ...>`.

| Property | Type | Description |
|----------|------|-------------|
| `ParameterTypes` | `ImmutableArray<TypeRef>` | The parameter types |

```csharp
// Action<string>
var action = new ActionTypeRef(TypeRef.From("string"));
action.ToString()  // → "global::System.Action<string>"

// Action (no parameters)
var noParam = new ActionTypeRef(ImmutableArray<TypeRef>.Empty);
noParam.ToString()  // → "global::System.Action"
```

---

## Immutable Collections

### ImmutableArrayTypeRef

Represents `ImmutableArray<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var arr = new ImmutableArrayTypeRef("string");
arr.ToString()  // → "global::System.Collections.Immutable.ImmutableArray<string>"
```

### ImmutableDictionaryTypeRef

Represents `ImmutableDictionary<TKey, TValue>`.

| Property | Type | Description |
|----------|------|-------------|
| `KeyType` | `TypeRef` | The key type |
| `ValueType` | `TypeRef` | The value type |

```csharp
var idict = new ImmutableDictionaryTypeRef("string", "int");
idict.ToString()  // → "global::System.Collections.Immutable.ImmutableDictionary<string, int>"
```

---

## Composability

Typed wrappers compose naturally because they implicitly convert to `TypeRef`:

```csharp
// Nested generics
var nested = new EqualityComparerTypeRef((TypeRef)new ListTypeRef("int"));
nested.ToString()
// → "global::System.Collections.Generic.EqualityComparer<global::System.Collections.Generic.List<int>>"

// Use in builders
var task = new TaskTypeRef((TypeRef)new ListTypeRef("Order"));
method.WithReturnType(task);  // Task<List<Order>>

// Use in expression factories
var eq = EqualityComparerExpression.DefaultEquals("string", "_name", "value");
// → "global::System.Collections.Generic.EqualityComparer<string>.Default.Equals(_name, value)"
```

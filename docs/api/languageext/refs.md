# Refs

Type-safe wrappers for LanguageExt types, created through the `LanguageExtRefs` factory.

> **See also:** [Overview](index.md) | [TypeRef & Primitives](../../api/emit/type-ref.md) | [Refs Overview](../../api/emit/refs/index.md)

---

## LanguageExtRefs Factory

All refs are created through `LanguageExtRefs` — the central factory class:

```csharp
using Deepstaging.Roslyn.LanguageExt.Refs;
```

### Type Factories

| Factory Method | Returns | Produces |
|----------------|---------|----------|
| `Eff(rt, result)` | `EffTypeRef` | `global::LanguageExt.Eff<RT, A>` |
| `Option(innerType)` | `OptionTypeRef` | `global::LanguageExt.Option<T>` |
| `Fin(innerType)` | `FinTypeRef` | `global::LanguageExt.Fin<A>` |
| `Seq(elementType)` | `SeqTypeRef` | `global::LanguageExt.Seq<A>` |
| `Either(left, right)` | `EitherTypeRef` | `global::LanguageExt.Either<L, R>` |
| `HashMap(key, value)` | `HashMapTypeRef` | `global::LanguageExt.HashMap<K, V>` |
| `Unit` | `TypeRef` | `global::LanguageExt.Unit` |

### Namespace & Using Helpers

| Member | Returns | Produces |
|--------|---------|----------|
| `Namespace` | `NamespaceRef` | `LanguageExt` |
| `EffectsNamespace` | `NamespaceRef` | `LanguageExt.Effects` |
| `PreludeStatic` | `string` | `static LanguageExt.Prelude` |

---

## Type-Safe Ref Structs

Every ref is a `readonly record struct` with:

- **Constituent type properties** — access the inner types that built the ref
- **Implicit conversion to `TypeRef`** — use anywhere a `TypeRef` is expected
- **Implicit conversion to `string`** — use in string interpolation
- **Globally qualified `ToString()`** — always produces `global::LanguageExt.*` output

### EffTypeRef

Represents `Eff<RT, A>` — an effect that requires a runtime `RT` and produces a result `A`.

| Property | Type | Description |
|----------|------|-------------|
| `Rt` | `TypeRef` | The runtime type parameter |
| `Result` | `TypeRef` | The result type |

```csharp
var eff = LanguageExtRefs.Eff("RT", "int");

eff.Rt       // → "RT"
eff.Result   // → "int"
eff.ToString() // → "global::LanguageExt.Eff<RT, int>"

// Use as TypeRef in a method builder
method.WithReturnType(eff);  // implicit TypeRef conversion
```

### OptionTypeRef

Represents `Option<T>` — a value that may or may not be present.

| Property | Type | Description |
|----------|------|-------------|
| `InnerType` | `TypeRef` | The wrapped type |

```csharp
var opt = LanguageExtRefs.Option("User");

opt.InnerType  // → "User"
opt.ToString() // → "global::LanguageExt.Option<User>"
```

### FinTypeRef

Represents `Fin<A>` — a result that is either a success value or an `Error`.

| Property | Type | Description |
|----------|------|-------------|
| `InnerType` | `TypeRef` | The success type |

```csharp
var fin = LanguageExtRefs.Fin("int");

fin.InnerType  // → "int"
fin.ToString() // → "global::LanguageExt.Fin<int>"
```

### EitherTypeRef

Represents `Either<L, R>` — a discriminated union of left (error) and right (success).

| Property | Type | Description |
|----------|------|-------------|
| `Left` | `TypeRef` | The left (error) type |
| `Right` | `TypeRef` | The right (success) type |

```csharp
var either = LanguageExtRefs.Either("Error", "int");

either.Left    // → "Error"
either.Right   // → "int"
either.ToString() // → "global::LanguageExt.Either<Error, int>"
```

### SeqTypeRef

Represents `Seq<A>` — an immutable lazy sequence.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var seq = LanguageExtRefs.Seq("string");

seq.ElementType // → "string"
seq.ToString()  // → "global::LanguageExt.Seq<string>"
```

### HashMapTypeRef

Represents `HashMap<K, V>` — an immutable hash map.

| Property | Type | Description |
|----------|------|-------------|
| `KeyType` | `TypeRef` | The key type |
| `ValueType` | `TypeRef` | The value type |

```csharp
var map = LanguageExtRefs.HashMap("string", "int");

map.KeyType    // → "string"
map.ValueType  // → "int"
map.ToString() // → "global::LanguageExt.HashMap<string, int>"
```

---

## Implicit Conversions

All `*TypeRef` structs convert implicitly to `TypeRef` and `string`. This means they drop into any API that accepts those types:

```csharp
var option = LanguageExtRefs.Option("User");

// As a return type (accepts TypeRef)
method.WithReturnType(option);

// As a property type (accepts TypeRef)
type.AddProperty("Result", option);

// In string interpolation (accepts string)
var code = $"public {option} Find(int id)";
// → "public global::LanguageExt.Option<User> Find(int id)"

// In EffLift methods (accepts OptionTypeRef specifically)
lift.AsyncOptional(option, "rt.Service.FindAsync(id)");
```

!!! tip "Why globally qualified?"
    All refs produce `global::LanguageExt.*` output. This prevents conflicts with user-defined types that may share names like `Option` or `Either`, making the generated code robust in any namespace context.

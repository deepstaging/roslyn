# Expressions

Prelude-style construction expressions for LanguageExt types. All methods return `ExpressionRef` for composability.

> **See also:** [Overview](index.md) | [Refs](refs.md) | [Eff Lifting](eff-lifting.md)

---

## OptionExpression

Builds `Option<T>` construction expressions. Requires `using static LanguageExt.Prelude` in generated code.

| Method | Returns | Produces |
|--------|---------|----------|
| `Optional(expr)` | `ExpressionRef` | `Optional(expr)` |
| `Some(expr)` | `ExpressionRef` | `Some(expr)` |
| `None` | `ExpressionRef` | `None` |

```csharp
using Deepstaging.Roslyn.LanguageExt.Expressions;

OptionExpression.Optional("entity");  // → Optional(entity)
OptionExpression.Some("value");       // → Some(value)
OptionExpression.None;                // → None
```

!!! tip "Optional vs Some"
    `Optional(x)` safely wraps a nullable — returns `None` if `x` is null. `Some(x)` asserts non-null and throws if given null. Use `Optional` for database lookups and `Some` when you know the value exists.

---

## EitherExpression

Builds `Either<L, R>` construction expressions.

| Method | Returns | Produces |
|--------|---------|----------|
| `Right(expr)` | `ExpressionRef` | `Right(expr)` |
| `Left(expr)` | `ExpressionRef` | `Left(expr)` |

```csharp
EitherExpression.Right("entity");       // → Right(entity)
EitherExpression.Left("Error.New(ex)"); // → Left(Error.New(ex))
```

---

## FinExpression

Builds `Fin<A>` construction expressions.

| Method | Returns | Produces |
|--------|---------|----------|
| `FinSucc(expr)` | `ExpressionRef` | `FinSucc(expr)` |
| `FinFail(expr)` | `ExpressionRef` | `FinFail(expr)` |
| `FinFailMessage(message)` | `ExpressionRef` | `FinFail(Error.New(message))` |

```csharp
FinExpression.FinSucc("42");              // → FinSucc(42)
FinExpression.FinFail("Error.New(ex)");   // → FinFail(Error.New(ex))
FinExpression.FinFailMessage("\"oops\""); // → FinFail(Error.New("oops"))
```

---

## SeqExpression

Builds `Seq<A>` construction and conversion expressions.

| Method | Returns | Produces |
|--------|---------|----------|
| `Seq(items...)` | `ExpressionRef` | `Seq(a, b, c)` |
| `toSeq(expr)` | `ExpressionRef` | `toSeq(expr)` |
| `Empty(seqType)` | `ExpressionRef` | `global::LanguageExt.Seq<A>.Empty` |

```csharp
SeqExpression.Seq("a", "b", "c");  // → Seq(a, b, c)
SeqExpression.toSeq("items");      // → toSeq(items)

var seqType = LanguageExtRefs.Seq("string");
SeqExpression.Empty(seqType);      // → global::LanguageExt.Seq<string>.Empty
```

!!! tip "Seq vs toSeq"
    `Seq(...)` constructs from explicit items. `toSeq(expr)` converts an `IEnumerable<T>` — use it when bridging from LINQ results or collections.

---

## HashMapExpression

Builds `HashMap<K, V>` construction and conversion expressions.

| Method | Returns | Produces |
|--------|---------|----------|
| `HashMap(pairs...)` | `ExpressionRef` | `HashMap(pair1, pair2)` |
| `toHashMap(expr)` | `ExpressionRef` | `toHashMap(expr)` |
| `Empty(hashMapType)` | `ExpressionRef` | `global::LanguageExt.HashMap<K, V>.Empty` |

```csharp
HashMapExpression.HashMap("(\"key\", 1)", "(\"other\", 2)");
// → HashMap(("key", 1), ("other", 2))

HashMapExpression.toHashMap("pairs");
// → toHashMap(pairs)

var mapType = LanguageExtRefs.HashMap("string", "int");
HashMapExpression.Empty(mapType);
// → global::LanguageExt.HashMap<string, int>.Empty
```

---

## Composability

All expressions return `ExpressionRef`, which supports chaining via the standard emit API:

```csharp
// Chain with .Call()
OptionExpression.Optional("value").Call("Map", "x => x.Name");
// → Optional(value).Map(x => x.Name)

// Chain with .Await()
EitherExpression.Right("entity").Await();
// → await Right(entity)

// Chain with .Member()
FinExpression.FinSucc("result").Member("IsSucc");
// → FinSucc(result).IsSucc

// Use in body statements
body.AddReturn(OptionExpression.Optional("entity"));
// → return Optional(entity);
```

All expression methods accept `ExpressionRef` parameters, which have implicit conversion from `string` — so you can pass string literals directly.

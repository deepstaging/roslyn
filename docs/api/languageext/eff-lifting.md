# Eff Lifting

Builders for lifting operations into the LanguageExt `Eff` monad — the core pattern for effect-based source generators.

> **See also:** [Overview](index.md) | [Refs](refs.md) | [Expressions](expressions.md) | [Extensions](extensions.md)

---

## Overview

LanguageExt's `Eff<RT, A>` monad captures side effects as data. Wrapping an operation into `Eff` is called **lifting**. There are two lifting APIs:

| Builder | Pattern | When to use |
|---------|---------|-------------|
| `EffLift` | `liftEff<RT, A>(rt => ...)` | General-purpose — result type specified per call |
| `EffLiftIO` | `Eff<RT, A>.LiftIO(rt => ...)` | Terminal operations — Eff type already known |

Both are created through the `EffExpression` entry point:

```csharp
using Deepstaging.Roslyn.LanguageExt.Expressions;
using Deepstaging.Roslyn.LanguageExt.Refs;

var lift   = EffExpression.Lift("RT", "rt");
var liftIO = EffExpression.LiftIO(LanguageExtRefs.Eff("RT", "int"), "rt");
```

---

## EffLift

Builds `liftEff<RT, A>(...)` expressions. The result type is specified per method call.

### Methods

| Method | Signature | Produces |
|--------|-----------|----------|
| `Async` | `Async(result, expr)` | `liftEff<RT, A>(async rt => await expr)` |
| `AsyncVoid` | `AsyncVoid(expr)` | `liftEff<RT, Unit>(async rt => { await expr; return unit; })` |
| `AsyncOptional` | `AsyncOptional(optionType, expr)` | `liftEff<RT, Option<T>>(async rt => Optional(await expr))` |
| `AsyncNonNull` | `AsyncNonNull(result, expr)` | `liftEff<RT, A>(async rt => (await expr)!)` |
| `Sync` | `Sync(result, expr)` | `liftEff<RT, A>(rt => expr)` |
| `SyncVoid` | `SyncVoid(expr)` | `liftEff<RT, Unit>(rt => { expr; return unit; })` |
| `SyncOptional` | `SyncOptional(optionType, expr)` | `liftEff<RT, Option<T>>(rt => Optional(expr))` |
| `SyncNonNull` | `SyncNonNull(result, expr)` | `liftEff<RT, A>(rt => (expr)!)` |
| `Body` | `Body(result, lambdaBody)` | `liftEff<RT, A>(lambdaBody)` — escape hatch |

### Examples

```csharp
var lift = EffExpression.Lift("RT", "rt");

// Async value
lift.Async("int", "rt.Db.Users.CountAsync()");
// → liftEff<RT, int>(async rt => await rt.Db.Users.CountAsync())

// Async void (returns Unit)
lift.AsyncVoid("rt.Db.SaveChangesAsync()");
// → liftEff<RT, Unit>(async rt => { await rt.Db.SaveChangesAsync(); return unit; })

// Async optional (nullable → Option)
lift.AsyncOptional(LanguageExtRefs.Option("User"), "rt.Db.Users.FindAsync(id)");
// → liftEff<RT, global::LanguageExt.Option<User>>(async rt => Optional(await rt.Db.Users.FindAsync(id)))

// Async non-null (null-forgiving assertion)
lift.AsyncNonNull("User", "rt.Db.Users.FindAsync(id)");
// → liftEff<RT, User>(async rt => (await rt.Db.Users.FindAsync(id))!)

// Sync value
lift.Sync("string", "rt.Config.ConnectionString");
// → liftEff<RT, string>(rt => rt.Config.ConnectionString)

// Custom body (escape hatch)
lift.Body("int", "rt => { var x = rt.Get(); return x + 1; }");
// → liftEff<RT, int>(rt => { var x = rt.Get(); return x + 1; })
```

!!! tip "AsyncOptional requires OptionTypeRef"
    `AsyncOptional` and `SyncOptional` accept `OptionTypeRef`, not `string`. This prevents mistakes like passing a raw type name — you must go through `LanguageExtRefs.Option()` which ensures the `Option<T>` wrapping is correct.

---

## EffLiftIO

Builds `Eff<RT, A>.LiftIO(...)` expressions. The Eff type is captured at construction — individual methods don't need the result type.

### Methods

| Method | Signature | Produces |
|--------|-----------|----------|
| `Async` | `Async(expr)` | `Eff<RT, A>.LiftIO(async rt => await expr)` |
| `AsyncVoid` | `AsyncVoid(expr)` | `Eff<RT, A>.LiftIO(async rt => { await expr; return unit; })` |
| `AsyncOptional` | `AsyncOptional(expr)` | `Eff<RT, A>.LiftIO(async rt => Optional(await expr))` |
| `AsyncNonNull` | `AsyncNonNull(expr)` | `Eff<RT, A>.LiftIO(async rt => (await expr)!)` |
| `Sync` | `Sync(expr)` | `Eff<RT, A>.LiftIO(rt => expr)` |
| `SyncVoid` | `SyncVoid(expr)` | `Eff<RT, A>.LiftIO(rt => { expr; return unit; })` |
| `SyncOptional` | `SyncOptional(expr)` | `Eff<RT, A>.LiftIO(rt => Optional(expr))` |
| `SyncNonNull` | `SyncNonNull(expr)` | `Eff<RT, A>.LiftIO(rt => (expr)!)` |
| `Body` | `Body(lambdaBody)` | `Eff<RT, A>.LiftIO(lambdaBody)` — escape hatch |

### Examples

```csharp
var effType = LanguageExtRefs.Eff("RT", "int");
var io = EffExpression.LiftIO(effType, "rt");

io.Async("query(rt).CountAsync(token)");
// → global::LanguageExt.Eff<RT, int>.LiftIO(async rt => await query(rt).CountAsync(token))

io.SyncVoid("rt.Cache.Clear()");
// → global::LanguageExt.Eff<RT, int>.LiftIO(rt => { rt.Cache.Clear(); return unit; })
```

---

## LiftingStrategy

An enum that describes **how** a method call should be lifted, paired with a dispatch method that routes to the correct `EffLift` call.

### Variants

| Strategy | Async/Sync | Return shape | Lambda pattern |
|----------|-----------|--------------|----------------|
| `AsyncValue` | Async | Value | `async rt => await expr` |
| `AsyncVoid` | Async | Unit | `async rt => { await expr; return unit; }` |
| `AsyncOptional` | Async | Option | `async rt => Optional(await expr)` |
| `AsyncNonNull` | Async | Non-null | `async rt => (await expr)!` |
| `SyncValue` | Sync | Value | `rt => expr` |
| `SyncVoid` | Sync | Unit | `rt => { expr; return unit; }` |
| `SyncOptional` | Sync | Option | `rt => Optional(expr)` |
| `SyncNonNull` | Sync | Non-null | `rt => (expr)!` |

### Dispatch

`EffLift.Lift()` is the dispatch method — call it with a strategy, result type, and expression:

```csharp
var lift = EffExpression.Lift("RT", "rt");

// Dispatch based on a strategy determined at analysis time
LiftingStrategy strategy = LiftingStrategy.AsyncOptional;

string expr = lift.Lift(strategy, "User", "rt.Service.FindAsync(id)");
// → liftEff<RT, global::LanguageExt.Option<User>>(async rt => Optional(await rt.Service.FindAsync(id)))
```

!!! tip "When to use LiftingStrategy"
    Use `LiftingStrategy` when the lifting pattern is determined dynamically — for example, by analyzing a method's return type and async nature. This eliminates `switch` statements in your generator code.

### EffReturnType

Computes the Eff return type for a given strategy — essential for building method signatures:

```csharp
LiftingStrategy.AsyncValue.EffReturnType("int");
// → "int" (passthrough)

LiftingStrategy.AsyncOptional.EffReturnType("User");
// → global::LanguageExt.Option<User>

LiftingStrategy.AsyncVoid.EffReturnType("ignored");
// → global::LanguageExt.Unit
```

| Strategy category | EffReturnType behavior |
|-------------------|----------------------|
| `*Value`, `*NonNull` | Returns `resultType` as-is |
| `*Optional` | Wraps in `Option<T>` via `LanguageExtRefs.Option()` |
| `*Void` | Returns `Unit` via `LanguageExtRefs.Unit` |

---

## LiftingStrategyAnalysis

Extension methods on `ValidSymbol<IMethodSymbol>` that automatically determine the lifting strategy and result type from a Roslyn method symbol. This eliminates manual analysis logic in your generator.

```csharp
using Deepstaging.Roslyn.LanguageExt.Expressions;
```

### DetermineLiftingStrategy

Analyzes a method's async nature and return type nullability to pick the correct `LiftingStrategy`:

```csharp
// Given a ValidSymbol<IMethodSymbol> from Roslyn analysis:
LiftingStrategy strategy = method.DetermineLiftingStrategy();
```

| Method signature | Determined strategy |
|-----------------|-------------------|
| `Task SendAsync(string to)` | `AsyncVoid` |
| `Task<int> CountAsync()` | `AsyncValue` |
| `Task<User?> FindAsync(int id)` | `AsyncOptional` |
| `void Clear()` | `SyncVoid` |
| `int Count()` | `SyncValue` |
| `User? Find(int id)` | `SyncOptional` |

The analysis inspects `AsyncMethodKind`, `InnerTaskType`, nullable annotations, and `ReturnsVoid` — all via the `ValidSymbol` projection from `Deepstaging.Roslyn`.

### EffectResultType

Computes the raw (unwrapped) result type for a method given its strategy:

```csharp
LiftingStrategy strategy = method.DetermineLiftingStrategy();
string resultType = method.EffectResultType(strategy);
```

| Strategy category | Result |
|-------------------|--------|
| `*Void` | `"Unit"` |
| `AsyncValue`, `AsyncOptional`, `AsyncNonNull` | Inner type of `Task<T>` / `ValueTask<T>` |
| `SyncValue`, `SyncOptional`, `SyncNonNull` | The method's return type directly |

The returned type is always the **unwrapped inner type** (e.g., `"User"` not `"Option<User>"`). Wrapping is handled downstream by `EffReturnType` and `Lift`.

!!! tip "DetermineLiftingStrategy + EffectResultType + Lift = zero manual analysis"
    These three methods form a complete pipeline: analyze the method, extract its result type, then generate the lift expression — no `switch` statements needed in your generator code.

---

## Putting It Together

A complete generator pattern that uses `LiftingStrategyAnalysis` to fully automate effect method generation from Roslyn symbols:

```csharp
using Deepstaging.Roslyn.LanguageExt.Expressions;
using Deepstaging.Roslyn.LanguageExt.Extensions;
using Deepstaging.Roslyn.LanguageExt.Refs;

var lift = EffExpression.Lift("RT", "rt");

// For each method on the source interface:
foreach (var method in sourceType.QueryMethods())
{
    // 1. Determine how the method should be lifted
    var strategy = method.DetermineLiftingStrategy();
    var resultType = method.EffectResultType(strategy);

    // 2. Build the call expression
    var callExpr = $"rt.Service.{method.Name}({paramList})";

    // 3. Generate the effect method — strategy drives everything
    type = type.AddMethod(method.Name, m => m
        .AsEffMethod("RT", "IHasService", strategy.EffReturnType(resultType))
        .WithExpressionBody(lift.Lift(strategy, resultType, callExpr)));
}
```

This generates correct code regardless of whether the source method is async/sync, returns void, returns nullable, or returns a value — the strategy handles all the variation.

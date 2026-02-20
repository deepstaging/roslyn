# Deepstaging.Roslyn.LanguageExt — Agent Guide

LanguageExt type references and emit patterns for Roslyn source generators built with Deepstaging.Roslyn.

This is a satellite package — it extends the core `Deepstaging.Roslyn` emit API with LanguageExt-specific builders.

## Type References

Type refs carry constituent types for compile-time introspection, not just string names:

```csharp
using Deepstaging.Roslyn.LanguageExt;

var eff    = LanguageExtTypes.Eff("RT", "int");           // Eff<RT, int>
var option = LanguageExtTypes.Option("string");            // Option<string>
var either = LanguageExtTypes.Either("Error", "int");      // Either<Error, int>
var fin    = LanguageExtTypes.Fin("int");                  // Fin<int>
var seq    = LanguageExtTypes.Seq("string");               // Seq<string>
var hash   = LanguageExtTypes.HashMap("string", "int");    // HashMap<string, int>
var unit   = LanguageExtTypes.Unit;                        // Unit
```

All refs produce globally qualified type strings (e.g., `global::LanguageExt.Eff<RT, int>`).

## Expression Builders

Prelude-style constructors returning `ExpressionRef` for composability:

```csharp
using Deepstaging.Roslyn.LanguageExt.Expressions;

OptionExpression.Some("entity");              // Some(entity)
OptionExpression.None;                        // None
EitherExpression.Right("value");              // Right(value)
FinExpression.FinSucc("42");                  // FinSucc(42)
SeqExpression.toSeq("items");                // toSeq(items)

// Chain with ExpressionRef methods
OptionExpression.Optional("value").Call("Map", "x => x.Name");
```

## Effect Lifting

Build `liftEff` and `Eff.LiftIO` expressions:

```csharp
var lift = EffExpression.Lift("RT", "rt");

// Dispatch by strategy — eliminates manual switch statements
lift.Lift(LiftingStrategy.Async, "int", "rt.Db.CountAsync()");
lift.Lift(LiftingStrategy.AsyncOptional, "User", "rt.Db.FindAsync(id)");
lift.Lift(LiftingStrategy.SyncVoid, null, "rt.Service.Reset()");

// Compute Eff return type from strategy
var returnType = LiftingStrategy.AsyncOptional.EffReturnType("User");
// → global::LanguageExt.Option<User>
```

### LiftingStrategy Values

| Strategy        | Lambda Shape                               | Return Type |
|-----------------|--------------------------------------------|-------------|
| `Async`         | `async rt => await expr`                   | `A`         |
| `AsyncOptional` | `async rt => Optional(await expr)`         | `Option<A>` |
| `AsyncVoid`     | `async rt => { await expr; return unit; }` | `Unit`      |
| `SyncValue`     | `rt => expr`                               | `A`         |
| `SyncVoid`      | `rt => { expr; return unit; }`             | `Unit`      |

## Method Builder Extension

Convert any method to the standard `Eff<RT, A>` shape:

```csharp
using Deepstaging.Roslyn.LanguageExt.Extensions;

MethodBuilder
    .Parse("public void GetCount()")
    .AsEffMethod("RT", "IHasDb", "int")
    .WithExpressionBody(lift.Async("int", "rt.Db.CountAsync()"));
// → public static Eff<RT, int> GetCount<RT>() where RT : IHasDb
```

## Package Structure

| Folder         | Purpose                                       |
|----------------|-----------------------------------------------|
| `Types/`       | Type-safe wrappers carrying constituent types |
| `Expressions/` | Prelude-style expression builders             |
| `Extensions/`  | `TypeBuilder` / `MethodBuilder` extensions + `LanguageExtExtensions` query helpers |

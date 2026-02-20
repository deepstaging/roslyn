# Deepstaging.Roslyn.LanguageExt

Type-safe references, expression builders, and emit patterns for [LanguageExt](https://github.com/louthy/language-ext) source generators.

## What is this?

This satellite package provides everything you need to generate LanguageExt functional code from Roslyn source generators. Instead of assembling raw strings for types like `Eff<RT, Option<User>>`, you use type-safe wrappers that carry constituent types and prevent construction mistakes at compile time.

The package is organized into three layers:

| Layer | Namespace | Purpose |
|-------|-----------|---------|
| **[Types](types.md)** | `Types` | Type-safe wrappers for LanguageExt types (`EffTypeRef`, `OptionTypeRef`, etc.) |
| **[Expressions](expressions.md)** | `Expressions` | Prelude-style construction expressions and [Eff lifting](eff-lifting.md) |
| **[Extensions](extensions.md)** | `Extensions` | `TypeBuilder` / `MethodBuilder` extensions for common patterns |

## Installation

```bash
dotnet add package Deepstaging.Roslyn.LanguageExt --prerelease
```

## Quick Start

A typical generator that produces LanguageExt effect methods:

```csharp
using Deepstaging.Roslyn.LanguageExt;
using Deepstaging.Roslyn.LanguageExt.Expressions;
using Deepstaging.Roslyn.LanguageExt.Extensions;

// 1. Build the lift expression
var lift = EffExpression.Lift("RT", "rt");

// 2. Build a method with the standard Eff shape
var method = MethodBuilder
    .Parse("public void GetCount()")
    .AsEffMethod("RT", "IHasDb", "int")
    .WithExpressionBody(lift.Async("int", "rt.Db.Users.CountAsync()"));

// 3. Build the type with LanguageExt usings
var type = TypeBuilder.StaticClass("DbEffects")
    .AddLanguageExtUsings()
    .AddMethod(method);
```

For generators that analyze Roslyn symbols, use `LiftingStrategyAnalysis` to avoid manual `switch` statements:

```csharp
// Automatically determine strategy from the method symbol
var strategy = method.DetermineLiftingStrategy();
var resultType = method.EffectResultType(strategy);

// Strategy drives both the return type and the lift expression
var effMethod = MethodBuilder
    .Parse($"public void {method.Name}()")
    .AsEffMethod("RT", "IHasDb", strategy.EffReturnType(resultType))
    .WithExpressionBody(lift.Lift(strategy, resultType, callExpr));
```

This generates:

```csharp
using LanguageExt;
using LanguageExt.Effects;
using static LanguageExt.Prelude;

public static class DbEffects
{
    public static Eff<RT, int> GetCount<RT>() where RT : IHasDb
        => liftEff<RT, int>(async rt => await rt.Db.Users.CountAsync());
}
```

## Why type-safe refs?

A plain `TypeRef` or `string` tells you nothing about the type's structure. When you pass `"Option<User>"` into an expression builder, there's no way to extract `"User"` back out — and no way to prevent passing `"Eff<RT, int>"` where an `Option` was expected.

Type-safe refs solve this:

```csharp
// ❌ String-based — can't introspect, can't validate
string returnType = "Option<User>";
string liftExpr = $"liftEff<RT, {returnType}>(async rt => Optional(await ...))";

// ✅ Type-safe — inner type accessible, compiler validates usage
OptionTypeRef returnType = LanguageExtTypes.Option("User");
returnType.InnerType  // → "User" (accessible for downstream use)
string liftExpr = lift.AsyncOptional(returnType, "rt.Service.FindAsync(id)");
```

Every `*TypeRef` has implicit conversions to `TypeRef` and `string`, so they drop in anywhere a `TypeRef` is accepted — no casting needed.

## Pages

- **[Types](types.md)** — `LanguageExtTypes` factory and type-safe ref structs
- **[Expressions](expressions.md)** — Option, Either, Fin, Seq, HashMap expression builders
- **[Eff Lifting](eff-lifting.md)** — `EffLift`, `EffLiftIO`, `LiftingStrategy`, and `LiftingStrategyAnalysis`
- **[Extensions](extensions.md)** — `AddLanguageExtUsings`, `AsEffMethod`

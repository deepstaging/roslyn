# Deepstaging.Roslyn.LanguageExt

LanguageExt type references and emit patterns for Roslyn source generators built
with [Deepstaging.Roslyn](https://github.com/deepstaging/roslyn).

## Usage

```csharp
using Deepstaging.Roslyn.LanguageExt.Types;

// Type references carry constituent types for compile-time introspection
var effType = LanguageExtRefs.Eff("RT", "int");        // EffTypeRef → global::LanguageExt.Eff<RT, int>
var optionType = LanguageExtRefs.Option("string");      // OptionTypeRef → global::LanguageExt.Option<string>
var eitherType = LanguageExtRefs.Either("Error", "int");// EitherTypeRef → global::LanguageExt.Either<Error, int>
var finType = LanguageExtRefs.Fin("int");               // FinTypeRef → global::LanguageExt.Fin<int>
var seqType = LanguageExtRefs.Seq("string");            // SeqTypeRef → global::LanguageExt.Seq<string>
var hashMapType = LanguageExtRefs.HashMap("string", "int"); // HashMapTypeRef
var unitType = LanguageExtRefs.Unit;                    // global::LanguageExt.Unit

// Namespaces and Prelude
var ns = LanguageExtRefs.Namespace;                     // LanguageExt
var prelude = LanguageExtRefs.PreludeStatic;            // static LanguageExt.Prelude
```

## Expressions

Prelude-style construction expressions that return `ExpressionRef` for composability:

```csharp
using Deepstaging.Roslyn.LanguageExt.Expressions;

// Option
OptionExpression.Optional("value");           // Optional(value)
OptionExpression.Some("entity");              // Some(entity)
OptionExpression.None;                        // None

// Either
EitherExpression.Right("entity");             // Right(entity)
EitherExpression.Left("Error.New(ex)");       // Left(Error.New(ex))

// Fin
FinExpression.FinSucc("42");                  // FinSucc(42)
FinExpression.FinFail("Error.New(ex)");       // FinFail(Error.New(ex))

// Seq
SeqExpression.Seq("a", "b", "c");            // Seq(a, b, c)
SeqExpression.toSeq("items");                // toSeq(items)
SeqExpression.Empty(LanguageExtRefs.Seq("string"));  // global::LanguageExt.Seq<string>.Empty

// HashMap
HashMapExpression.toHashMap("pairs");         // toHashMap(pairs)
HashMapExpression.Empty(LanguageExtRefs.HashMap("string", "int"));

// All return ExpressionRef — chain with .Call(), .Await(), .Member(), etc.
OptionExpression.Optional("value").Call("Map", "x => x.Name");
```

## Effect Expressions

Builders for `liftEff` and `Eff.LiftIO` patterns:

```csharp
var lift = EffExpression.Lift("RT", "rt");

lift.Async("int", "rt.Service.GetCountAsync()");
// → liftEff<RT, int>(async rt => await rt.Service.GetCountAsync())

lift.AsyncOptional(LanguageExtRefs.Option("User"), "rt.Service.FindAsync(id)");
// → liftEff<RT, Option<User>>(async rt => Optional(await rt.Service.FindAsync(id)))

lift.SyncVoid("rt.Service.Reset()");
// → liftEff<RT, Unit>(rt => { rt.Service.Reset(); return unit; })

// LiftIO for terminal operations (type already known)
var io = EffExpression.LiftIO(LanguageExtRefs.Eff("RT", "int"), "rt");
io.Async("query(rt).CountAsync(token)");
// → global::LanguageExt.Eff<RT, int>.LiftIO(async rt => await query(rt).CountAsync(token))
```

## Patterns

### Lifting Strategy

Dispatch to the correct lift method based on the operation shape — eliminates manual switch statements:

```csharp
using Deepstaging.Roslyn.LanguageExt.Expressions;

var lift = EffExpression.Lift("RT", "rt");

// Dispatch based on strategy
var expr = lift.Lift(LiftingStrategy.AsyncOptional, "User", "rt.Service.FindAsync(id)");
// → liftEff<RT, Option<User>>(async rt => Optional(await rt.Service.FindAsync(id)))

// Compute the Eff return type for the same strategy
var returnType = LiftingStrategy.AsyncOptional.EffReturnType("User");
// → global::LanguageExt.Option<User>
```

### Eff Method Shape

Build the standard `Eff<RT, A>` method shape with a single call:

```csharp
using Deepstaging.Roslyn.LanguageExt.Extensions;

MethodBuilder
    .Parse("public void GetCount()")
    .AsEffMethod("RT", "IHasDb", "int")
    .WithExpressionBody(lift.Async("int", "rt.Db.Users.CountAsync()"));
// → public static Eff<RT, int> GetCount<RT>() where RT : IHasDb
//       => liftEff<RT, int>(async rt => await rt.Db.Users.CountAsync())

// Combine both patterns for full automation:
var strategy = LiftingStrategy.AsyncOptional;
MethodBuilder
    .Parse("public void FindUser(int id)")
    .AsEffMethod("RT", "IHasDb", strategy.EffReturnType("User"))
    .WithExpressionBody(lift.Lift(strategy, "User", "rt.Service.FindAsync(id)"));
```

## Package Structure

This is a satellite package following the `Deepstaging.Roslyn.{Library}` convention:

- **Refs/** — Type-safe wrappers for LanguageExt types (carry constituent types)
- **Expressions/** — Expression builders for Prelude functions and Eff lifting
- **Extensions/** — TypeBuilder/MethodBuilder extensions for common LanguageExt patterns

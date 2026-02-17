# Extensions

LanguageExt convenience extensions for `TypeBuilder` and `MethodBuilder`.

> **See also:** [Overview](index.md) | [Eff Lifting](eff-lifting.md) | [TypeBuilder](../../api/emit/type-builder.md) | [MethodBuilder](../../api/emit/method-builder.md)

---

## TypeBuilder Extensions

### AddLanguageExtUsings

Adds the standard LanguageExt using directives for effect code generation:

```csharp
using Deepstaging.Roslyn.LanguageExt.Extensions;

var type = TypeBuilder.StaticClass("Effects")
    .AddLanguageExtUsings();
```

Produces:

```csharp
using LanguageExt;
using LanguageExt.Effects;
using static LanguageExt.Prelude;
```

These three usings cover the vast majority of LanguageExt effect code — `Eff`, `Unit`, `Option`, `Seq`, all Prelude functions (`liftEff`, `Optional`, `Some`, `Seq`, etc.), and the `Traits` module.

---

## MethodBuilder Extensions

### AsEffMethod

Configures a method as a standard LanguageExt effect method:

```csharp
using Deepstaging.Roslyn.LanguageExt.Extensions;

var method = MethodBuilder
    .Parse("public void GetCount()")
    .AsEffMethod("RT", "IHasDb", "int");
```

Produces:

```csharp
public static Eff<RT, int> GetCount<RT>() where RT : IHasDb
```

`AsEffMethod` applies three transformations:

| Transformation | What it does |
|---------------|--------------|
| `.AsStatic()` | Makes the method `static` (effect methods are always static) |
| `.AddTypeParameter(rt, ...)` | Adds a generic `<RT>` with a `where RT : IHasDb` constraint |
| `.WithReturnType(...)` | Sets the return type to `Eff<RT, A>` using `LanguageExtRefs.Eff()` |

### Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `rt` | `string` | The runtime type parameter name (e.g., `"RT"`) |
| `capability` | `TypeRef` | The capability interface constraint (e.g., `"IHasDb"`) |
| `resultType` | `TypeRef` | The Eff result type (e.g., `"int"`, `LanguageExtRefs.Option("User")`) |

### Combining with LiftingStrategy

`AsEffMethod` and `LiftingStrategy.EffReturnType` work together to fully automate effect method signatures:

```csharp
LiftingStrategy strategy = LiftingStrategy.AsyncOptional;

var method = MethodBuilder
    .Parse("public void FindUser(int id)")
    .AsEffMethod("RT", "IHasDb", strategy.EffReturnType("User"))
    .WithExpressionBody(lift.Lift(strategy, "User", "rt.Db.Users.FindAsync(id)"));
```

Produces:

```csharp
public static Eff<RT, Option<User>> FindUser<RT>(int id) where RT : IHasDb
    => liftEff<RT, global::LanguageExt.Option<User>>(
        async rt => Optional(await rt.Db.Users.FindAsync(id)));
```

### Full Example

A typical generator building multiple effect methods from analyzed source:

```csharp
var lift = EffExpression.Lift("RT", "rt");

var type = TypeBuilder.StaticClass("UserEffects")
    .InNamespace("MyApp.Effects")
    .AddLanguageExtUsings();

// Value-returning async method
type = type.AddMethod(MethodBuilder
    .Parse("public void GetCount()")
    .AsEffMethod("RT", "IHasUsers", "int")
    .WithExpressionBody(lift.Async("int", "rt.Users.CountAsync()")));

// Optional-returning async method
type = type.AddMethod(MethodBuilder
    .Parse("public void Find(int id)")
    .AsEffMethod("RT", "IHasUsers", LanguageExtRefs.Option("User"))
    .WithExpressionBody(
        lift.AsyncOptional(LanguageExtRefs.Option("User"), "rt.Users.FindAsync(id)")));

// Void async method
type = type.AddMethod(MethodBuilder
    .Parse("public void Delete(int id)")
    .AsEffMethod("RT", "IHasUsers", LanguageExtRefs.Unit)
    .WithExpressionBody(lift.AsyncVoid("rt.Users.DeleteAsync(id)")));
```

Produces:

```csharp
using LanguageExt;
using LanguageExt.Effects;
using static LanguageExt.Prelude;

namespace MyApp.Effects;

public static class UserEffects
{
    public static Eff<RT, int> GetCount<RT>() where RT : IHasUsers
        => liftEff<RT, int>(async rt => await rt.Users.CountAsync());

    public static Eff<RT, Option<User>> Find<RT>(int id) where RT : IHasUsers
        => liftEff<RT, global::LanguageExt.Option<User>>(
            async rt => Optional(await rt.Users.FindAsync(id)));

    public static Eff<RT, Unit> Delete<RT>(int id) where RT : IHasUsers
        => liftEff<RT, Unit>(async rt => { await rt.Users.DeleteAsync(id); return unit; });
}
```

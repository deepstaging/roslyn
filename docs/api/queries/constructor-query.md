# ConstructorQuery

Find constructors on a type.

> **See also:** [Queries Overview](index.md) | [MethodQuery](method-query.md) | [TypeQuery](type-query.md)

```csharp
var publicCtors = typeSymbol.QueryConstructors()
    .ThatArePublic()
    .ThatAreInstance()
    .GetAll();

var parameterless = typeSymbol.QueryConstructors()
    .WithNoParameters()
    .FirstOrDefault();
```

## Accessibility Filters

| Method | Description |
|--------|-------------|
| `ThatArePublic()` | Public constructors |
| `ThatAreNotPublic()` | Non-public constructors |
| `ThatArePrivate()` | Private constructors |
| `ThatAreNotPrivate()` | Non-private constructors |
| `ThatAreProtected()` | Protected constructors |
| `ThatAreNotProtected()` | Non-protected constructors |
| `ThatAreInternal()` | Internal constructors |
| `ThatAreNotInternal()` | Non-internal constructors |
| `ThatAreProtectedOrInternal()` | Protected internal constructors |
| `ThatAreNotProtectedOrInternal()` | Non-protected internal constructors |

## Modifier Filters

| Method | Description |
|--------|-------------|
| `ThatAreStatic()` | Static constructors |
| `ThatAreInstance()` | Instance constructors |

## Parameter Filters

| Method | Description |
|--------|-------------|
| `WithParameterCount(int)` | Exact parameter count |
| `WithNoParameters()` | Parameterless constructor |
| `WithAtLeastParameters(int)` | Minimum parameter count |
| `WithFirstParameterOfType(ITypeSymbol)` | First parameter matches type |
| `WithParameter(Func<IParameterSymbol, bool>)` | Any parameter matches predicate |
| `WhereAllParameters(Func<IParameterSymbol, bool>)` | All parameters match predicate |

## Attribute Filters

| Method | Description |
|--------|-------------|
| `WithAttribute(string)` | Has attribute by name |

## Materialization

Same as [TypeQuery](type-query.md#materialization): `GetAll()`, `GetAllSymbols()`, `Select<T>()`, `SelectMany<T>()`, `FirstOrDefault()`, `First()`, `Any()`, `Count()`

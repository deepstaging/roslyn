# ParameterQuery

Find parameters on a method.

> **See also:** [Queries Overview](index.md) | [MethodQuery](method-query.md) | [ConstructorQuery](constructor-query.md)

```csharp
var optionalParams = ParameterQuery.From(methodSymbol)
    .ThatAreOptional()
    .GetAll();

var refParams = methodSymbol.QueryParameters()
    .ThatAreRef()
    .GetAll();
```

## Factory Methods

| Method | Description |
|--------|-------------|
| `From(IMethodSymbol)` | Query parameters on a method |

## Modifier Filters

| Method | Description |
|--------|-------------|
| `ThatAreRef()` | Ref parameters |
| `ThatAreNotRef()` | Non-ref parameters |
| `ThatAreOut()` | Out parameters |
| `ThatAreNotOut()` | Non-out parameters |
| `ThatAreIn()` | In parameters |
| `ThatAreNotIn()` | Non-in parameters |
| `ThatAreParams()` | Params array parameters |
| `ThatAreNotParams()` | Non-params parameters |
| `ThatAreOptional()` | Optional parameters (with defaults) |
| `ThatAreRequired()` | Required parameters |
| `ThatAreThis()` | Extension method 'this' parameter |
| `ThatAreNotThis()` | Non-this parameters |
| `ThatAreDiscards()` | Discard parameters (named `_`) |
| `ThatAreNotDiscards()` | Non-discard parameters |

## Name Filters

| Method | Description |
|--------|-------------|
| `WithName(string)` | Exact name match |
| `WithNamePrefix(string)` | Name starts with prefix |
| `WithNameSuffix(string)` | Name ends with suffix |
| `WithNameMatching(Func<string, bool>)` | Custom name predicate |

## Type Filters

| Method | Description |
|--------|-------------|
| `WithType<T>()` | Parameters of type T |
| `WithType(string)` | Parameters with type name |
| `ThatAreGenericType()` | Parameters with generic types |
| `ThatAreNotGenericType()` | Parameters without generic types |
| `ThatAreNullable()` | Parameters with nullable annotation |
| `ThatAreNotNullable()` | Parameters without nullable annotation |

## Position Filters

| Method | Description |
|--------|-------------|
| `AtPosition(int)` | Parameter at specific index |
| `ThatAreFirst()` | First parameter |
| `ThatAreLast()` | Last parameter |

## Attribute Filters

| Method | Description |
|--------|-------------|
| `WithAttribute<TAttribute>()` | Has attribute type |
| `WithAttribute(string)` | Has attribute by name |
| `WithoutAttribute<TAttribute>()` | Lacks attribute type |

## Materialization

Same as [TypeQuery](type-query.md#materialization): `GetAll()`, `GetAllSymbols()`, `Select<T>()`, `SelectMany<T>()`, `FirstOrDefault()`, `First()`, `Any()`, `Count()`

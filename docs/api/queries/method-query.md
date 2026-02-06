# MethodQuery

Find methods on a type.

> **See also:** [Queries Overview](index.md) | [TypeQuery](type-query.md) | [ParameterQuery](parameter-query.md)

```csharp
// Static factory
var methods = MethodQuery.From(typeSymbol)
    .ThatAreAsync()
    .ThatArePublic()
    .ReturningTask()
    .GetAll();

// Extension method (preferred)
var handlers = typeSymbol.QueryMethods()
    .WithNameEndingWith("Handler")
    .WithParameterCount(1)
    .GetAll();
```

## Factory Methods

| Method | Description |
|--------|-------------|
| `From(ITypeSymbol)` | Query methods on a type |

## Accessibility Filters

| Method | Description |
|--------|-------------|
| `ThatArePublic()` | Public methods |
| `ThatAreNotPublic()` | Non-public methods |
| `ThatArePrivate()` | Private methods |
| `ThatAreNotPrivate()` | Non-private methods |
| `ThatAreProtected()` | Protected methods |
| `ThatAreNotProtected()` | Non-protected methods |
| `ThatAreInternal()` | Internal methods |
| `ThatAreNotInternal()` | Non-internal methods |
| `ThatAreProtectedOrInternal()` | Protected internal methods |
| `ThatAreNotProtectedOrInternal()` | Non-protected internal methods |

## Modifier Filters

| Method | Description |
|--------|-------------|
| `ThatAreStatic()` | Static methods |
| `ThatAreInstance()` | Instance methods |
| `ThatAreAsync()` | Async methods |
| `ThatAreNotAsync()` | Non-async methods |
| `ThatAreGeneric()` | Generic methods |
| `ThatAreNotGeneric()` | Non-generic methods |
| `ThatAreVirtual()` | Virtual methods |
| `ThatAreNotVirtual()` | Non-virtual methods |
| `ThatAreAbstract()` | Abstract methods |
| `ThatAreNotAbstract()` | Non-abstract methods |
| `ThatAreOverrides()` | Override methods |
| `ThatAreNotOverrides()` | Non-override methods |
| `ThatAreSealed()` | Sealed methods |
| `ThatAreNotSealed()` | Non-sealed methods |

## Name Filters

| Method | Description |
|--------|-------------|
| `WithName(string)` | Exact name match |
| `WithNameStartingWith(string)` | Name starts with prefix |
| `WithNameEndingWith(string)` | Name ends with suffix |
| `WithNameContaining(string)` | Name contains substring |
| `WithNameMatching(Regex)` | Name matches regex pattern |

## Parameter Filters

| Method | Description |
|--------|-------------|
| `WithParameterCount(int)` | Exact parameter count |
| `WithNoParameters()` | No parameters |
| `WithParameters()` | At least one parameter |
| `WithFirstParameterOfType(string)` | First parameter matches type name |
| `WithParameters(Func<ImmutableArray<IParameterSymbol>, bool>)` | Custom parameter predicate |

## Return Type Filters

| Method | Description |
|--------|-------------|
| `WithReturnType(string)` | Return type name match |
| `WithReturnType(Func<ITypeSymbol, bool>)` | Custom return type predicate |
| `ReturningVoid()` | Returns void |
| `ReturningTask()` | Returns Task or ValueTask |
| `ReturningValueTask()` | Returns ValueTask |
| `ReturningGenericTask()` | Returns Task<T> or ValueTask<T> |

## Attribute Filters

| Method | Description |
|--------|-------------|
| `WithAttribute<TAttribute>()` | Has attribute type |
| `WithAttribute(string)` | Has attribute by name |
| `WithoutAttribute<TAttribute>()` | Lacks attribute type |

## Materialization

Same as [TypeQuery](type-query.md#materialization): `GetAll()`, `GetAllSymbols()`, `Select<T>()`, `SelectMany<T>()`, `FirstOrDefault()`, `First()`, `Any()`, `Count()`

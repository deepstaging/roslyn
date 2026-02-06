# EventQuery

Find events on a type.

> **See also:** [Queries Overview](index.md) | [PropertyQuery](property-query.md) | [TypeQuery](type-query.md)

```csharp
var publicEvents = typeSymbol.QueryEvents()
    .ThatArePublic()
    .WithType("EventHandler")
    .GetAll();
```

## Accessibility Filters

| Method | Description |
|--------|-------------|
| `ThatArePublic()` | Public events |
| `ThatAreNotPublic()` | Non-public events |
| `ThatArePrivate()` | Private events |
| `ThatAreNotPrivate()` | Non-private events |
| `ThatAreProtected()` | Protected events |
| `ThatAreNotProtected()` | Non-protected events |
| `ThatAreInternal()` | Internal events |
| `ThatAreNotInternal()` | Non-internal events |

## Modifier Filters

| Method | Description |
|--------|-------------|
| `ThatAreStatic()` | Static events |
| `ThatAreInstance()` | Instance events |
| `ThatAreAbstract()` | Abstract events |
| `ThatAreNotAbstract()` | Non-abstract events |
| `ThatAreVirtual()` | Virtual events |
| `ThatAreNotVirtual()` | Non-virtual events |
| `ThatAreSealed()` | Sealed events |
| `ThatAreNotSealed()` | Non-sealed events |
| `ThatAreOverride()` | Override events |
| `ThatAreNotOverride()` | Non-override events |

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
| `WithType<T>()` | Events of type T |
| `WithType(string)` | Events with type name |

## Attribute Filters

| Method | Description |
|--------|-------------|
| `WithAttribute<TAttribute>()` | Has attribute type |
| `WithAttribute(string)` | Has attribute by name |
| `WithoutAttribute<TAttribute>()` | Lacks attribute type |

## Materialization

Same as [TypeQuery](type-query.md#materialization): `GetAll()`, `GetAllSymbols()`, `Select<T>()`, `SelectMany<T>()`, `FirstOrDefault()`, `First()`, `Any()`, `Count()`

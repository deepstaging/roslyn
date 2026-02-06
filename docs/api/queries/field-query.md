# FieldQuery

Find fields on a type.

> **See also:** [Queries Overview](index.md) | [PropertyQuery](property-query.md) | [TypeQuery](type-query.md)

```csharp
var constants = typeSymbol.QueryFields()
    .ThatAreConst()
    .ThatArePublic()
    .GetAll();

var injectableFields = typeSymbol.QueryFields()
    .WithAttribute("Inject")
    .ThatArePrivate()
    .GetAll();
```

## Accessibility Filters

| Method | Description |
|--------|-------------|
| `ThatArePublic()` | Public fields |
| `ThatAreNotPublic()` | Non-public fields |
| `ThatArePrivate()` | Private fields |
| `ThatAreNotPrivate()` | Non-private fields |
| `ThatAreProtected()` | Protected fields |
| `ThatAreNotProtected()` | Non-protected fields |
| `ThatAreInternal()` | Internal fields |
| `ThatAreNotInternal()` | Non-internal fields |

## Modifier Filters

| Method | Description |
|--------|-------------|
| `ThatAreStatic()` | Static fields |
| `ThatAreInstance()` | Instance fields |
| `ThatAreReadOnly()` | Readonly fields |
| `ThatAreNotReadOnly()` | Non-readonly fields |
| `ThatAreConst()` | Const fields |
| `ThatAreNotConst()` | Non-const fields |
| `ThatAreVolatile()` | Volatile fields |
| `ThatAreNotVolatile()` | Non-volatile fields |

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
| `WithType<T>()` | Fields of type T |
| `WithType(string)` | Fields with type name |
| `ThatAreGenericType()` | Fields with generic types |
| `ThatAreNotGenericType()` | Fields without generic types |
| `ThatAreNullable()` | Fields with nullable annotation |
| `ThatAreNotNullable()` | Fields without nullable annotation |

## Attribute Filters

| Method | Description |
|--------|-------------|
| `WithAttribute<TAttribute>()` | Has attribute type |
| `WithAttribute(string)` | Has attribute by name |
| `WithoutAttribute<TAttribute>()` | Lacks attribute type |

## Materialization

Same as [TypeQuery](type-query.md#materialization): `GetAll()`, `GetAllSymbols()`, `Select<T>()`, `SelectMany<T>()`, `FirstOrDefault()`, `First()`, `Any()`, `Count()`

# PropertyQuery

Find properties on a type.

> **See also:** [Queries Overview](index.md) | [TypeQuery](type-query.md) | [FieldQuery](field-query.md)

```csharp
var requiredProps = typeSymbol.QueryProperties()
    .ThatAreRequired()
    .ThatArePublic()
    .GetAll();

var readOnlyProps = typeSymbol.QueryProperties()
    .ThatAreReadOnly()
    .WithoutAttribute<ObsoleteAttribute>()
    .GetAll();
```

## Accessibility Filters

| Method | Description |
|--------|-------------|
| `ThatArePublic()` | Public properties |
| `ThatAreNotPublic()` | Non-public properties |
| `ThatArePrivate()` | Private properties |
| `ThatAreNotPrivate()` | Non-private properties |
| `ThatAreProtected()` | Protected properties |
| `ThatAreNotProtected()` | Non-protected properties |
| `ThatAreInternal()` | Internal properties |
| `ThatAreNotInternal()` | Non-internal properties |
| `ThatAreProtectedOrInternal()` | Protected internal properties |
| `ThatAreNotProtectedOrInternal()` | Non-protected internal properties |

## Modifier Filters

| Method | Description |
|--------|-------------|
| `ThatAreStatic()` | Static properties |
| `ThatAreInstance()` | Instance properties |
| `ThatAreVirtual()` | Virtual properties |
| `ThatAreNotVirtual()` | Non-virtual properties |
| `ThatAreAbstract()` | Abstract properties |
| `ThatAreNotAbstract()` | Non-abstract properties |
| `ThatAreOverride()` | Override properties |
| `ThatAreNotOverride()` | Non-override properties |
| `ThatAreSealed()` | Sealed properties |
| `ThatAreNotSealed()` | Non-sealed properties |
| `ThatAreReadOnly()` | Read-only (no setter) |
| `ThatAreWriteOnly()` | Write-only (no getter) |
| `ThatAreReadWrite()` | Has getter and setter |
| `WithInitOnlySetter()` | Init-only setter |
| `ThatAreRequired()` | Required properties |
| `ThatAreNotRequired()` | Non-required properties |

## Name Filters

| Method | Description |
|--------|-------------|
| `WithName(string)` | Exact name match |
| `WithNameStartingWith(string)` | Name starts with prefix |
| `WithNameContaining(string)` | Name contains substring |
| `WithNameEndingWith(string)` | Name ends with suffix |
| `WithNameMatching(Regex)` | Name matches regex pattern |

## Type Filters

| Method | Description |
|--------|-------------|
| `OfType(ITypeSymbol)` | Exact type match |
| `OfTypeName(string)` | Type name match |
| `OfType(Func<ITypeSymbol, bool>)` | Custom type predicate |

## Attribute Filters

| Method | Description |
|--------|-------------|
| `WithAttribute<TAttribute>()` | Has attribute type |
| `WithAttribute(string)` | Has attribute by name |
| `WithoutAttribute<TAttribute>()` | Lacks attribute type |

## Materialization

Same as [TypeQuery](type-query.md#materialization): `GetAll()`, `GetAllSymbols()`, `Select<T>()`, `SelectMany<T>()`, `FirstOrDefault()`, `First()`, `Any()`, `Count()`

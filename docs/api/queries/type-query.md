# TypeQuery

Find types in a compilation or namespace.

> **See also:** [Queries Overview](index.md) | [MethodQuery](method-query.md) | [PropertyQuery](property-query.md)

```csharp
// From a compilation
var types = TypeQuery.From(compilation)
    .ThatArePublic()
    .ThatAreClasses()
    .WithAttribute("MyAttribute")
    .GetAll();

// From a namespace symbol
var types = TypeQuery.From(namespaceSymbol)
    .ThatAreInterfaces()
    .InNamespaceStartingWith("MyApp.Domain")
    .GetAll();

// Using extension method
var types = compilation.QueryTypes()
    .ThatAreRecords()
    .GetAll();
```

## Factory Methods

| Method | Description |
|--------|-------------|
| `From(Compilation)` | Query all types in the compilation |
| `From(INamespaceSymbol)` | Query types from a specific namespace |

## Accessibility Filters

| Method | Description |
|--------|-------------|
| `ThatArePublic()` | Public types |
| `ThatAreNotPublic()` | Non-public types |
| `ThatAreInternal()` | Internal types |
| `ThatAreNotInternal()` | Non-internal types |
| `ThatArePrivate()` | Private types (nested only) |
| `ThatAreNotPrivate()` | Non-private types |
| `ThatAreProtected()` | Protected types (nested only) |
| `ThatAreNotProtected()` | Non-protected types |

## Type Kind Filters

| Method | Description |
|--------|-------------|
| `ThatAreClasses()` | Class types |
| `ThatAreNotClasses()` | Non-class types |
| `ThatAreInterfaces()` | Interface types |
| `ThatAreNotInterfaces()` | Non-interface types |
| `ThatAreStructs()` | Struct types |
| `ThatAreNotStructs()` | Non-struct types |
| `ThatAreEnums()` | Enum types |
| `ThatAreNotEnums()` | Non-enum types |
| `ThatAreDelegates()` | Delegate types |
| `ThatAreNotDelegates()` | Non-delegate types |
| `ThatAreRecords()` | Record types (class or struct) |
| `ThatAreNotRecords()` | Non-record types |

## Modifier Filters

| Method | Description |
|--------|-------------|
| `ThatAreStatic()` | Static types |
| `ThatAreNotStatic()` | Non-static types |
| `ThatAreAbstract()` | Abstract types |
| `ThatAreNotAbstract()` | Non-abstract types |
| `ThatAreSealed()` | Sealed types |
| `ThatAreNotSealed()` | Non-sealed types |
| `ThatAreGeneric()` | Generic types |
| `ThatAreNotGeneric()` | Non-generic types |
| `ThatArePartial()` | Partial types |
| `ThatAreNotPartial()` | Non-partial types |
| `ThatAreRefStructs()` | Ref struct types |
| `ThatAreNotRefStructs()` | Non-ref struct types |
| `ThatAreReadOnlyStructs()` | Readonly struct types |
| `ThatAreNotReadOnlyStructs()` | Non-readonly struct types |

## Name Filters

| Method | Description |
|--------|-------------|
| `WithName(string)` | Exact name match |
| `WithNameStartingWith(string)` | Name starts with prefix |
| `WithNameContaining(string)` | Name contains substring |
| `WithNameEndingWith(string)` | Name ends with suffix |
| `WithNameMatching(Regex)` | Name matches regex pattern |

## Inheritance Filters

| Method | Description |
|--------|-------------|
| `InheritingFrom(INamedTypeSymbol)` | Types inheriting from base type |
| `ImplementingInterface(INamedTypeSymbol)` | Types implementing interface |
| `ImplementingInterface(string)` | Types implementing interface by name |

## Namespace Filters

| Method | Description |
|--------|-------------|
| `InNamespace(string)` | Exact namespace match |
| `InNamespaceStartingWith(string)` | Namespace starts with prefix |
| `IncludeNestedNamespaces()` | Include types in nested namespaces |

## Attribute Filters

| Method | Description |
|--------|-------------|
| `WithAttribute(string)` | Types with attribute (with or without "Attribute" suffix) |

## Materialization

| Method | Returns | Description |
|--------|---------|-------------|
| `GetAll()` | `ImmutableArray<ValidSymbol<INamedTypeSymbol>>` | All matches as validated wrappers |
| `GetAllSymbols()` | `ImmutableArray<INamedTypeSymbol>` | Raw symbols |
| `Select<T>(Func)` | `ImmutableArray<T>` | Project each to a model |
| `SelectMany<T>(Func)` | `ImmutableArray<T>` | Project and flatten |
| `FirstOrDefault()` | `OptionalSymbol<INamedTypeSymbol>` | First match or empty |
| `First()` | `ValidSymbol<INamedTypeSymbol>` | First match (throws if none) |
| `Any()` | `bool` | True if any match |
| `Count()` | `int` | Count of matches |

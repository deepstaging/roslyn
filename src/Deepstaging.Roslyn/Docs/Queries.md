# Queries

Fluent builders for finding types, methods, properties, fields, constructors, events, and parameters.

> **See also:** [Projections](Projections.md) | [Emit](Emit.md) | [Extensions](Extensions.md) | [Roslyn Toolkit README](../README.md)

## Overview

Query builders let you compose chainable filters on Roslyn symbols:

- **Immutable** — each method returns a new instance
- **Lazy** — filters are applied when you call `GetAll()`, `First()`, etc.
- **Safe** — materialization returns `ValidSymbol<T>` wrappers with guaranteed non-null access

```csharp
// Find all public async methods returning Task<T>
var methods = typeSymbol.QueryMethods()
    .ThatArePublic()
    .ThatAreAsync()
    .ReturningGenericTask()
    .GetAll();

// Project results directly to your model
var models = TypeQuery.From(compilation)
    .ThatAreClasses()
    .WithAttribute("Entity")
    .Select(t => new EntityModel(t.Name, t.Namespace));
```

---

## TypeQuery

Find types in a compilation or namespace.

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

### Factory Methods

| Method | Description |
|--------|-------------|
| `From(Compilation)` | Query all types in the compilation |
| `From(INamespaceSymbol)` | Query types from a specific namespace |

### Accessibility Filters

| Method | Description |
|--------|-------------|
| `ThatArePublic()` | Public types |
| `ThatAreInternal()` | Internal types |
| `ThatArePrivate()` | Private types (nested only) |
| `ThatAreProtected()` | Protected types (nested only) |

### Type Kind Filters

| Method | Description |
|--------|-------------|
| `ThatAreClasses()` | Class types |
| `ThatAreInterfaces()` | Interface types |
| `ThatAreStructs()` | Struct types |
| `ThatAreEnums()` | Enum types |
| `ThatAreDelegates()` | Delegate types |
| `ThatAreRecords()` | Record types (class or struct) |

### Modifier Filters

| Method | Description |
|--------|-------------|
| `ThatAreStatic()` | Static types |
| `ThatAreAbstract()` | Abstract types |
| `ThatAreSealed()` | Sealed types |
| `ThatAreGeneric()` | Generic types |
| `ThatArePartial()` | Partial types |
| `ThatAreRefStructs()` | Ref struct types |
| `ThatAreReadOnlyStructs()` | Readonly struct types |

### Name Filters

| Method | Description |
|--------|-------------|
| `WithName(string)` | Exact name match |
| `WithNameStartingWith(string)` | Name starts with prefix |
| `WithNameContaining(string)` | Name contains substring |
| `WithNameEndingWith(string)` | Name ends with suffix |

### Inheritance Filters

| Method | Description |
|--------|-------------|
| `InheritingFrom(INamedTypeSymbol)` | Types inheriting from base type |
| `ImplementingInterface(INamedTypeSymbol)` | Types implementing interface |
| `ImplementingInterface(string)` | Types implementing interface by name |

### Namespace Filters

| Method | Description |
|--------|-------------|
| `InNamespace(string)` | Exact namespace match |
| `InNamespaceStartingWith(string)` | Namespace starts with prefix |
| `IncludeNestedNamespaces()` | Include types in nested namespaces |

### Attribute Filters

| Method | Description |
|--------|-------------|
| `WithAttribute(string)` | Types with attribute (with or without "Attribute" suffix) |

### Materialization

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

---

## MethodQuery

Find methods on a type.

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

### Factory Methods

| Method | Description |
|--------|-------------|
| `From(ITypeSymbol)` | Query methods on a type |

### Accessibility Filters

| Method | Description |
|--------|-------------|
| `ThatArePublic()` | Public methods |
| `ThatArePrivate()` | Private methods |
| `ThatAreProtected()` | Protected methods |
| `ThatAreInternal()` | Internal methods |
| `ThatAreProtectedOrInternal()` | Protected internal methods |

### Modifier Filters

| Method | Description |
|--------|-------------|
| `ThatAreStatic()` | Static methods |
| `ThatAreInstance()` | Instance methods |
| `ThatAreAsync()` | Async methods |
| `ThatAreGeneric()` | Generic methods |
| `ThatAreVirtual()` | Virtual methods |
| `ThatAreAbstract()` | Abstract methods |
| `ThatAreOverrides()` | Override methods |
| `ThatAreSealed()` | Sealed methods |

### Name Filters

| Method | Description |
|--------|-------------|
| `WithName(string)` | Exact name match |
| `WithNameStartingWith(string)` | Name starts with prefix |
| `WithNameEndingWith(string)` | Name ends with suffix |
| `WithNameContaining(string)` | Name contains substring |

### Parameter Filters

| Method | Description |
|--------|-------------|
| `WithParameterCount(int)` | Exact parameter count |
| `WithNoParameters()` | No parameters |
| `WithParameters()` | At least one parameter |
| `WithFirstParameterOfType(string)` | First parameter matches type name |
| `WithParameters(Func<ImmutableArray<IParameterSymbol>, bool>)` | Custom parameter predicate |

### Return Type Filters

| Method | Description |
|--------|-------------|
| `WithReturnType(string)` | Return type name match |
| `WithReturnType(Func<ITypeSymbol, bool>)` | Custom return type predicate |
| `ReturningVoid()` | Returns void |
| `ReturningTask()` | Returns Task or ValueTask |
| `ReturningValueTask()` | Returns ValueTask |
| `ReturningGenericTask()` | Returns Task<T> or ValueTask<T> |

### Attribute Filters

| Method | Description |
|--------|-------------|
| `WithAttribute<TAttribute>()` | Has attribute type |
| `WithAttribute(string)` | Has attribute by name |
| `WithoutAttribute<TAttribute>()` | Lacks attribute type |

### Materialization

Same as TypeQuery: `GetAll()`, `GetAllSymbols()`, `Select<T>()`, `SelectMany<T>()`, `FirstOrDefault()`, `First()`, `Any()`, `Count()`

---

## PropertyQuery

Find properties on a type.

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

### Accessibility Filters

| Method | Description |
|--------|-------------|
| `ThatArePublic()` | Public properties |
| `ThatArePrivate()` | Private properties |
| `ThatAreProtected()` | Protected properties |
| `ThatAreInternal()` | Internal properties |
| `ThatAreProtectedOrInternal()` | Protected internal properties |

### Modifier Filters

| Method | Description |
|--------|-------------|
| `ThatAreStatic()` | Static properties |
| `ThatAreInstance()` | Instance properties |
| `ThatAreVirtual()` | Virtual properties |
| `ThatAreAbstract()` | Abstract properties |
| `ThatAreOverride()` | Override properties |
| `ThatAreSealed()` | Sealed properties |
| `ThatAreReadOnly()` | Read-only (no setter) |
| `ThatAreWriteOnly()` | Write-only (no getter) |
| `ThatAreReadWrite()` | Has getter and setter |
| `WithInitOnlySetter()` | Init-only setter |
| `ThatAreRequired()` | Required properties |

### Name Filters

| Method | Description |
|--------|-------------|
| `WithName(string)` | Exact name match |
| `WithNameStartingWith(string)` | Name starts with prefix |
| `WithNameContaining(string)` | Name contains substring |
| `WithNameEndingWith(string)` | Name ends with suffix |

### Type Filters

| Method | Description |
|--------|-------------|
| `OfType(ITypeSymbol)` | Exact type match |
| `OfTypeName(string)` | Type name match |
| `OfType(Func<ITypeSymbol, bool>)` | Custom type predicate |

### Attribute Filters

| Method | Description |
|--------|-------------|
| `WithAttribute<TAttribute>()` | Has attribute type |
| `WithAttribute(string)` | Has attribute by name |
| `WithoutAttribute<TAttribute>()` | Lacks attribute type |

---

## FieldQuery

Find fields on a type.

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

### Accessibility Filters

| Method | Description |
|--------|-------------|
| `ThatArePublic()` | Public fields |
| `ThatArePrivate()` | Private fields |
| `ThatAreProtected()` | Protected fields |
| `ThatAreInternal()` | Internal fields |

### Modifier Filters

| Method | Description |
|--------|-------------|
| `ThatAreStatic()` | Static fields |
| `ThatAreInstance()` | Instance fields |
| `ThatAreReadOnly()` | Readonly fields |
| `ThatAreConst()` | Const fields |
| `ThatAreVolatile()` | Volatile fields |

### Name Filters

| Method | Description |
|--------|-------------|
| `WithName(string)` | Exact name match |
| `WithNamePrefix(string)` | Name starts with prefix |
| `WithNameSuffix(string)` | Name ends with suffix |
| `WithNameMatching(Func<string, bool>)` | Custom name predicate |

### Type Filters

| Method | Description |
|--------|-------------|
| `WithType<T>()` | Fields of type T |
| `WithType(string)` | Fields with type name |
| `ThatAreGenericType()` | Fields with generic types |
| `ThatAreNullable()` | Fields with nullable annotation |

### Attribute Filters

| Method | Description |
|--------|-------------|
| `WithAttribute<TAttribute>()` | Has attribute type |
| `WithAttribute(string)` | Has attribute by name |
| `WithoutAttribute<TAttribute>()` | Lacks attribute type |

---

## ConstructorQuery

Find constructors on a type.

```csharp
var publicCtors = typeSymbol.QueryConstructors()
    .ThatArePublic()
    .ThatAreInstance()
    .GetAll();

var parameterless = typeSymbol.QueryConstructors()
    .WithNoParameters()
    .FirstOrDefault();
```

### Accessibility Filters

| Method | Description |
|--------|-------------|
| `ThatArePublic()` | Public constructors |
| `ThatArePrivate()` | Private constructors |
| `ThatAreProtected()` | Protected constructors |
| `ThatAreInternal()` | Internal constructors |
| `ThatAreProtectedOrInternal()` | Protected internal constructors |

### Modifier Filters

| Method | Description |
|--------|-------------|
| `ThatAreStatic()` | Static constructors |
| `ThatAreInstance()` | Instance constructors |

### Parameter Filters

| Method | Description |
|--------|-------------|
| `WithParameterCount(int)` | Exact parameter count |
| `WithNoParameters()` | Parameterless constructor |
| `WithAtLeastParameters(int)` | Minimum parameter count |
| `WithFirstParameterOfType(ITypeSymbol)` | First parameter matches type |
| `WithParameter(Func<IParameterSymbol, bool>)` | Any parameter matches predicate |
| `WhereAllParameters(Func<IParameterSymbol, bool>)` | All parameters match predicate |

### Attribute Filters

| Method | Description |
|--------|-------------|
| `WithAttribute(string)` | Has attribute by name |

---

## EventQuery

Find events on a type.

```csharp
var publicEvents = typeSymbol.QueryEvents()
    .ThatArePublic()
    .WithType("EventHandler")
    .GetAll();
```

### Accessibility Filters

| Method | Description |
|--------|-------------|
| `ThatArePublic()` | Public events |
| `ThatArePrivate()` | Private events |
| `ThatAreProtected()` | Protected events |
| `ThatAreInternal()` | Internal events |

### Modifier Filters

| Method | Description |
|--------|-------------|
| `ThatAreStatic()` | Static events |
| `ThatAreInstance()` | Instance events |
| `ThatAreAbstract()` | Abstract events |
| `ThatAreVirtual()` | Virtual events |
| `ThatAreSealed()` | Sealed events |
| `ThatAreOverride()` | Override events |

### Name Filters

| Method | Description |
|--------|-------------|
| `WithName(string)` | Exact name match |
| `WithNamePrefix(string)` | Name starts with prefix |
| `WithNameSuffix(string)` | Name ends with suffix |
| `WithNameMatching(Func<string, bool>)` | Custom name predicate |

### Type Filters

| Method | Description |
|--------|-------------|
| `WithType<T>()` | Events of type T |
| `WithType(string)` | Events with type name |

### Attribute Filters

| Method | Description |
|--------|-------------|
| `WithAttribute<TAttribute>()` | Has attribute type |
| `WithAttribute(string)` | Has attribute by name |
| `WithoutAttribute<TAttribute>()` | Lacks attribute type |

---

## ParameterQuery

Find parameters on a method.

```csharp
var optionalParams = ParameterQuery.From(methodSymbol)
    .ThatAreOptional()
    .GetAll();

var refParams = methodSymbol.QueryParameters()
    .ThatAreRef()
    .GetAll();
```

### Factory Methods

| Method | Description |
|--------|-------------|
| `From(IMethodSymbol)` | Query parameters on a method |

### Modifier Filters

| Method | Description |
|--------|-------------|
| `ThatAreRef()` | Ref parameters |
| `ThatAreOut()` | Out parameters |
| `ThatAreIn()` | In parameters |
| `ThatAreParams()` | Params array parameters |
| `ThatAreOptional()` | Optional parameters (with defaults) |
| `ThatAreRequired()` | Required parameters |
| `ThatAreThis()` | Extension method 'this' parameter |
| `ThatAreDiscards()` | Discard parameters (named `_`) |

### Name Filters

| Method | Description |
|--------|-------------|
| `WithName(string)` | Exact name match |
| `WithNamePrefix(string)` | Name starts with prefix |
| `WithNameSuffix(string)` | Name ends with suffix |
| `WithNameMatching(Func<string, bool>)` | Custom name predicate |

### Type Filters

| Method | Description |
|--------|-------------|
| `WithType<T>()` | Parameters of type T |
| `WithType(string)` | Parameters with type name |
| `ThatAreGenericType()` | Parameters with generic types |
| `ThatAreNullable()` | Parameters with nullable annotation |

### Position Filters

| Method | Description |
|--------|-------------|
| `AtPosition(int)` | Parameter at specific index |
| `ThatAreFirst()` | First parameter |
| `ThatAreLast()` | Last parameter |

### Attribute Filters

| Method | Description |
|--------|-------------|
| `WithAttribute<TAttribute>()` | Has attribute type |
| `WithAttribute(string)` | Has attribute by name |
| `WithoutAttribute<TAttribute>()` | Lacks attribute type |

---

## Custom Filters

All query builders support `Where()` for custom predicates:

```csharp
var special = TypeQuery.From(compilation)
    .Where(t => t.GetMembers().Length > 10)
    .Where(t => t.ContainingNamespace?.Name == "Domain")
    .GetAll();
```

---

## Common Patterns

### Filter and Project

```csharp
var models = TypeQuery.From(compilation)
    .ThatAreClasses()
    .WithAttribute("Entity")
    .Select(type => new EntityModel
    {
        Name = type.Name,
        Properties = type.Value.QueryProperties().ThatArePublic().GetAll()
    });
```

### Early Exit with FirstOrDefault

```csharp
var handler = typeSymbol.QueryMethods()
    .WithName("Handle")
    .WithParameterCount(1)
    .FirstOrDefault();

if (handler.IsNotValid(out var valid))
    return; // No handler found

// Use valid.Name, valid.Value, etc.
```

### Chaining Queries

```csharp
var targetMethods = new HashSet<string>(
    attribute.TargetType.QueryMethods()
        .ThatArePublic()
        .Select(m => m.Name)
);
```

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](../../../LICENSE) for the full legal text.

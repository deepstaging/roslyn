# Queries

Fluent builders for finding types, methods, properties, fields, constructors, events, and parameters.

> **See also:** [Projections](projections.md) | [Emit](emit.md) | [Extensions](extensions.md) | [Roslyn Toolkit README](../index.md)

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
| `ThatAreNotPublic()` | Non-public types |
| `ThatAreInternal()` | Internal types |
| `ThatAreNotInternal()` | Non-internal types |
| `ThatArePrivate()` | Private types (nested only) |
| `ThatAreNotPrivate()` | Non-private types |
| `ThatAreProtected()` | Protected types (nested only) |
| `ThatAreNotProtected()` | Non-protected types |

### Type Kind Filters

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

### Modifier Filters

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
| `ThatAreNotPublic()` | Non-public methods |
| `ThatArePrivate()` | Private methods |
| `ThatAreNotPrivate()` | Non-private methods |
| `ThatAreProtected()` | Protected methods |
| `ThatAreNotProtected()` | Non-protected methods |
| `ThatAreInternal()` | Internal methods |
| `ThatAreNotInternal()` | Non-internal methods |
| `ThatAreProtectedOrInternal()` | Protected internal methods |
| `ThatAreNotProtectedOrInternal()` | Non-protected internal methods |

### Modifier Filters

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
| `ThatAreNotPublic()` | Non-public properties |
| `ThatArePrivate()` | Private properties |
| `ThatAreNotPrivate()` | Non-private properties |
| `ThatAreProtected()` | Protected properties |
| `ThatAreNotProtected()` | Non-protected properties |
| `ThatAreInternal()` | Internal properties |
| `ThatAreNotInternal()` | Non-internal properties |
| `ThatAreProtectedOrInternal()` | Protected internal properties |
| `ThatAreNotProtectedOrInternal()` | Non-protected internal properties |

### Modifier Filters

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
| `ThatAreNotPublic()` | Non-public fields |
| `ThatArePrivate()` | Private fields |
| `ThatAreNotPrivate()` | Non-private fields |
| `ThatAreProtected()` | Protected fields |
| `ThatAreNotProtected()` | Non-protected fields |
| `ThatAreInternal()` | Internal fields |
| `ThatAreNotInternal()` | Non-internal fields |

### Modifier Filters

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
| `ThatAreNotGenericType()` | Fields without generic types |
| `ThatAreNullable()` | Fields with nullable annotation |
| `ThatAreNotNullable()` | Fields without nullable annotation |

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
| `ThatAreNotPublic()` | Non-public constructors |
| `ThatArePrivate()` | Private constructors |
| `ThatAreNotPrivate()` | Non-private constructors |
| `ThatAreProtected()` | Protected constructors |
| `ThatAreNotProtected()` | Non-protected constructors |
| `ThatAreInternal()` | Internal constructors |
| `ThatAreNotInternal()` | Non-internal constructors |
| `ThatAreProtectedOrInternal()` | Protected internal constructors |
| `ThatAreNotProtectedOrInternal()` | Non-protected internal constructors |

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
| `ThatAreNotPublic()` | Non-public events |
| `ThatArePrivate()` | Private events |
| `ThatAreNotPrivate()` | Non-private events |
| `ThatAreProtected()` | Protected events |
| `ThatAreNotProtected()` | Non-protected events |
| `ThatAreInternal()` | Internal events |
| `ThatAreNotInternal()` | Non-internal events |

### Modifier Filters

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
| `ThatAreNotGenericType()` | Parameters without generic types |
| `ThatAreNullable()` | Parameters with nullable annotation |
| `ThatAreNotNullable()` | Parameters without nullable annotation |

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

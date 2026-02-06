# Queries

Fluent builders for finding types, methods, properties, fields, constructors, events, and parameters.

> **See also:** [Projections](../projections/index.md) | [Emit](../emit/index.md) | [Extensions](../extensions/index.md)

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

## Query Types

| Query | Description |
|-------|-------------|
| [TypeQuery](type-query.md) | Find types in a compilation or namespace |
| [MethodQuery](method-query.md) | Find methods on a type |
| [PropertyQuery](property-query.md) | Find properties on a type |
| [FieldQuery](field-query.md) | Find fields on a type |
| [ConstructorQuery](constructor-query.md) | Find constructors on a type |
| [EventQuery](event-query.md) | Find events on a type |
| [ParameterQuery](parameter-query.md) | Find parameters on a method |

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

See [LICENSE](https://github.com/deepstaging/roslyn/blob/main/LICENSE) for the full legal text.

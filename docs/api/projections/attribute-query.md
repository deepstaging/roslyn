# AttributeQuery

Base record for strongly-typed attribute query wrappers in Projection projects.

> **See also:** [Projections Overview](index.md) | [ValidAttribute](valid-attribute.md)

## Overview

`AttributeQuery` provides a base class for creating domain-specific wrappers around `AttributeData`. Instead of accessing raw attribute arguments everywhere, you define a query class once with typed properties.

```csharp
// Define a query wrapper
public sealed record RetryAttributeQuery(AttributeData AttributeData) 
    : AttributeQuery(AttributeData)
{
    public int MaxRetries => NamedArg<int>("MaxRetries").OrDefault(3);
    public int DelayMs => NamedArg<int>("DelayMs").OrDefault(1000);
    public bool Exponential => NamedArg<bool>("Exponential").OrDefault(false);
}

// Use it
var query = validAttr.AsQuery<RetryAttributeQuery>();
var retries = query.MaxRetries;  // Strongly typed, with default
```

## Protected Helpers

AttributeQuery provides three protected helpers for accessing attribute data:

| Method | Description |
|--------|-------------|
| `ConstructorArg<T>(index)` | Gets a constructor argument by position |
| `NamedArg<T>(name)` | Gets a named argument (property) by name |
| `TypeArg(index)` | Gets a type argument from a generic attribute |

All return `Optional*` types for safe chaining:

```csharp
public sealed record HandlerAttributeQuery(AttributeData AttributeData) 
    : AttributeQuery(AttributeData)
{
    // Constructor argument
    public ValidSymbol<INamedTypeSymbol> RequestType => 
        ConstructorArg<INamedTypeSymbol>(0)
            .Map(s => s.AsValidNamedType())
            .OrThrow("Request type required");
    
    // Named argument with default
    public int Priority => NamedArg<int>("Priority").OrDefault(0);
    
    // Generic type argument (for Handler<TRequest>)
    public ValidSymbol<INamedTypeSymbol> GenericRequestType =>
        TypeArg(0).OrThrow("Type argument required").AsValidNamedType();
}
```

## AsQuery Extension

The `AsQuery<TQuery>()` extension on `ValidAttribute` converts to any query type:

```csharp
// Single attribute
var query = symbol
    .GetAttribute<MyAttribute>()
    .Map(attr => attr.AsQuery<MyAttributeQuery>())
    .OrThrow();

// Multiple attributes
var queries = symbol
    .GetAttributes<MyAttribute>()
    .Select(attr => attr.AsQuery<MyAttributeQuery>())
    .ToImmutableArray();
```

The conversion uses a cached compiled expression tree, so performance is equivalent to calling `new` directly after the first invocation.

## Typical Projection Structure

A Projection project typically has:

```
MyProject.Projection/
├── Attributes/
│   ├── MyAttributeQuery.cs      # Inherits AttributeQuery
│   └── OtherAttributeQuery.cs
├── Models/
│   └── MyModel.cs               # Domain model built from queries
├── Queries.cs                   # Extension methods using AsQuery<T>
└── MyModels.cs                  # Builds models from symbols
```

### Queries.cs Pattern

```csharp
public static class Queries
{
    extension(ValidAttribute attribute)
    {
        public MyAttributeQuery QueryMyAttribute() =>
            attribute.AsQuery<MyAttributeQuery>();
    }

    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        public ImmutableArray<MyAttributeQuery> MyAttributes() =>
        [
            ..symbol.GetAttributes<MyAttribute>()
                .Select(attr => attr.AsQuery<MyAttributeQuery>())
        ];
    }
}
```

## Requirements

Query types must:

1. Inherit from `AttributeQuery`
2. Have a constructor that takes `AttributeData`

```csharp
// ✅ Correct - primary constructor with AttributeData
public sealed record MyQuery(AttributeData AttributeData) : AttributeQuery(AttributeData);

// ❌ Wrong - missing constructor parameter
public sealed record MyQuery() : AttributeQuery(null!);
```

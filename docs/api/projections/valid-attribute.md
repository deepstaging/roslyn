# ValidAttribute

A validated attribute with guaranteed non-null `AttributeData`.

> **See also:** [Projections Overview](index.md) | [OptionalAttribute](optional-attribute.md) | [AttributeQuery](attribute-query.md)

## Getting Arguments

Same argument extraction methods as OptionalAttribute:

```csharp
OptionalArgument<string> arg = validAttr.ConstructorArg<string>(0);
OptionalArgument<int> retries = validAttr.NamedArg<int>("MaxRetries");
validAttr.GetNamedArgument<bool>("Enabled");  // alternate syntax
```

## Direct Access

```csharp
validAttr.Value             // AttributeData
validAttr.AttributeClass    // INamedTypeSymbol
```

## Generic Attribute Type Arguments

For generic attributes, type arguments return `ValidSymbol`:

```csharp
ImmutableArray<ValidSymbol<INamedTypeSymbol>> typeArgs = validAttr.GetTypeArguments();
```

## Converting to Query Types

Use `AsQuery<TQuery>()` to convert to a strongly-typed query wrapper:

```csharp
// Convert to a domain-specific query type
var query = validAttr.AsQuery<MyAttributeQuery>();

// Access typed properties with defaults
var maxRetries = query.MaxRetries;  // int, not Optional
```

See [AttributeQuery](attribute-query.md) for details on creating query types.

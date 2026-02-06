# ValidAttribute

A validated attribute with guaranteed non-null `AttributeData`.

> **See also:** [Projections Overview](index.md) | [OptionalAttribute](optional-attribute.md)

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

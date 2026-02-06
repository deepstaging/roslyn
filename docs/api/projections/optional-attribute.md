# OptionalAttribute

Wraps an `AttributeData` that may or may not be present.

> **See also:** [Projections Overview](index.md) | [ValidAttribute](valid-attribute.md) | [OptionalArgument](optional-argument.md)

## Creating

```csharp
OptionalAttribute.WithValue(attributeData)
OptionalAttribute.Empty()
OptionalAttribute.FromNullable(maybeNull)
```

## Getting Arguments

```csharp
// Constructor arguments by index
OptionalArgument<string> name = attr.ConstructorArg<string>(0);
OptionalArgument<int> count = attr.ConstructorArg<int>(1);

// Named arguments
OptionalArgument<int> retries = attr.NamedArg<int>("MaxRetries");
OptionalArgument<string> message = attr.NamedArg<string>("Message");
```

## Generic Attribute Type Arguments

For generic attributes like `[MyAttribute<TRuntime, TEvent>]`:

```csharp
attr.GetTypeArguments()         // ImmutableArray<OptionalSymbol<INamedTypeSymbol>>
attr.GetTypeArgument(0)         // OptionalArgument<INamedTypeSymbol>
attr.GetTypeArgumentSymbol(0)   // OptionalSymbol<ITypeSymbol>
attr.AttributeClass             // OptionalSymbol<INamedTypeSymbol>
```

## Transforming

```csharp
// Map to a result type
OptionalArgument<MyConfig> config = attr.Map(a => new MyConfig(a));

// Extract multiple arguments at once
OptionalArgument<MyConfig> config = attr.WithArgs(a => new MyConfig
{
    Name = a.ConstructorArg<string>(0).OrDefault("Default"),
    Retries = a.NamedArg<int>("MaxRetries").OrDefault(3)
});
```

## Validation

```csharp
if (attr.IsValid(out var valid)) { /* use valid */ }
if (attr.IsNotValid(out var valid)) return;

attr.Validate();           // OptionalAttribute → ValidAttribute?
attr.ValidateOrThrow();    // throws if empty
attr.TryValidate(out var valid);
```

## Other Methods

```csharp
attr.Do(a => Console.WriteLine(a.AttributeClass?.Name));
attr.OrElse(() => fallbackAttribute);
attr.OrNull();
attr.OrThrow("Attribute required");
attr.OrDefault(fallbackValue);
attr.Match(whenPresent: ..., whenEmpty: ...);
attr.PropertyName           // string? — suggested property name
attr.ParameterName          // string? — suggested parameter name
```

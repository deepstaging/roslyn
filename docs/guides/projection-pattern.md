# The Projection Pattern

The Projection layer converts Roslyn symbols into strongly-typed models through three components. All examples in this guide are drawn from the [Deepstaging](https://github.com/deepstaging/deepstaging) source generator suite.

## Overview

The pattern flows in one direction:

```
Symbol → AttributeQuery → Model
```

| Component | Role | Location |
|-----------|------|----------|
| **AttributeQuery** | Wraps `AttributeData` with typed properties and defaults | `Projection/Domain/Attributes/` |
| **Model** | A `[PipelineModel]` record capturing everything for generation | `Projection/Domain/Models/` |
| **Query** | Extension methods that chain queries into models | `Projection/Domain/Queries.cs` |

## 1. AttributeQuery Types

An `AttributeQuery` wraps `AttributeData` and exposes typed properties with safe defaults. Here's the real `StrongIdAttributeQuery`:

```csharp
public sealed record StrongIdAttributeQuery(AttributeData AttributeData)
    : AttributeQuery(AttributeData)
{
    public BackingType BackingType =>
        NamedArg<int>(nameof(StrongIdAttribute.BackingType))
            .ToEnum<BackingType>()
            .OrDefault(BackingType.Guid);

    public ValidSymbol<INamedTypeSymbol> BackingTypeSymbol(SemanticModel model) =>
        BackingType switch
        {
            BackingType.Guid => model.WellKnownSymbols.Guid,
            BackingType.Int => model.WellKnownSymbols.Int32,
            BackingType.Long => model.WellKnownSymbols.Int64,
            BackingType.String => model.WellKnownSymbols.String,
            _ => throw new ArgumentOutOfRangeException(nameof(BackingType))
        };

    public IdConverters Converters =>
        NamedArg<int>(nameof(StrongIdAttribute.Converters))
            .ToEnum<IdConverters>()
            .OrDefault(IdConverters.None);
}
```

Key patterns:

- **`NamedArg<T>(name)`** — reads a named attribute argument with type safety
- **`ConstructorArg<T>(index)`** — reads a positional constructor argument
- **`.OrDefault(value)`** — provides a fallback when the argument is missing
- **`.OrNull()`** — returns null for truly optional values
- **`.OrThrow(message)`** — fails explicitly for required values
- **`.ToEnum<T>()`** — converts int arguments to enum types (Roslyn stores enums as ints)

### More AttributeQuery Examples

A query that reads a type argument from a generic attribute:

```csharp
public sealed record HttpClientAttributeQuery(AttributeData AttributeData)
    : AttributeQuery(AttributeData)
{
    public OptionalSymbol<INamedTypeSymbol> ConfigurationType =>
        AttributeData.Query()
            .GetTypeArgument(0)
            .Map(symbol => symbol.AsNamedType())
            .OrDefault(OptionalSymbol<INamedTypeSymbol>.Empty);

    public string? BaseAddress => NamedArg<string>("BaseAddress").OrNull();
}
```

A query that resolves referenced types:

```csharp
public sealed record UsesAttributeQuery(AttributeData AttributeData)
    : AttributeQuery(AttributeData)
{
    public ValidSymbol<INamedTypeSymbol> ModuleType =>
        ConstructorArg<INamedTypeSymbol>(0)
            .Map(symbol => symbol.AsValidNamedType())
            .OrThrow("UsesAttribute must have a valid module type.");

    public ImmutableArray<EffectsModuleModel> EffectsModules =>
        ModuleType.QueryEffectsModules();
}
```

## 2. Models

Models are `[PipelineModel]` records that capture everything needed for generation. They use `required` properties and [`EquatableArray<T>`](../api/projections/equatable-array.md) for correct incremental caching.

```csharp
[PipelineModel]
public sealed record StrongIdModel
{
    public required string Namespace { get; init; }
    public required string TypeName { get; init; }
    public required string Accessibility { get; init; }
    public required BackingType BackingType { get; init; }
    public required IdConverters Converters { get; init; }
    public required TypeSnapshot BackingTypeSnapshot { get; init; }
}
```

!!! tip "When to use snapshots vs. strings"
    Use **[snapshot types](../api/projections/snapshots.md)** (`TypeSnapshot`, `MethodSnapshot`) when your writer needs multiple properties from a symbol — they capture everything in one call. Use **plain strings** when you only need a name or type reference.

### Nested Models

Complex features use nested models:

```csharp
[PipelineModel]
public sealed record ConfigModel
{
    public required string Namespace { get; init; }
    public required TypeRef TypeName { get; init; }
    public required string Accessibility { get; init; }
    public required string Section { get; init; }
    public EquatableArray<ConfigTypeModel> ExposedConfigurationTypes { get; init; } = [];
    public bool HasSecrets => ExposedConfigurationTypes.Any(ct => ct.Properties.Any(p => p.IsSecret));
}
```

### Model Rules

1. **Always use `[PipelineModel]`** — it generates equality members for incremental caching
2. **Use `EquatableArray<T>`** instead of `ImmutableArray<T>` for collections
3. **Use snapshot types** instead of `ISymbol` for symbol data — symbols are not safe across pipeline stages
4. **Use `required`** on all properties that must be set

## 3. Query Extensions

Query extensions are the glue — they chain from symbols to attribute queries to models. Here's the real StrongId query:

```csharp
extension(ValidSymbol<INamedTypeSymbol> symbol)
{
    public StrongIdModel ToStrongIdModel(SemanticModel model) =>
        symbol.GetAttribute<StrongIdAttribute>()
            .Map(attr => attr.AsQuery<StrongIdAttributeQuery>())
            .Map(attr => new StrongIdModel
            {
                Namespace = symbol.Namespace ?? "",
                TypeName = symbol.Name,
                Accessibility = symbol.AccessibilityString,
                BackingType = attr.BackingType,
                BackingTypeSnapshot = attr.BackingTypeSymbol(model).ToSnapshot(),
                Converters = attr.Converters
            })
            .OrThrow($"Expected '{symbol.FullyQualifiedName}' to have StrongIdAttribute.");
}
```

### Querying Multiple Attributes

When a symbol can have multiple instances of the same attribute:

```csharp
extension(ValidSymbol<INamedTypeSymbol> symbol)
{
    public ImmutableArray<EffectsModuleAttributeQuery> EffectsModuleAttributes() =>
    [
        ..symbol.GetAttributes<EffectsModuleAttribute>()
            .Select(attr => attr.AsQuery<EffectsModuleAttributeQuery>())
    ];
}
```

### Querying Methods and Parameters

Queries compose across symbol types — from types to methods to parameters:

```csharp
extension(ValidSymbol<INamedTypeSymbol> symbol)
{
    public HttpClientModel? QueryHttpClient() =>
        symbol.GetAttribute<HttpClientAttribute>()
            .OrElse(() => symbol.GetAttribute(typeof(HttpClientAttribute<>)))
            .Map(attr => attr.AsQuery<HttpClientAttributeQuery>())
            .Map(query => new HttpClientModel
            {
                Namespace = symbol.Namespace ?? "",
                TypeName = symbol.Name,
                Accessibility = symbol.AccessibilityString,
                ConfigurationType = query.ConfigurationType.FullyQualifiedName,
                BaseAddress = query.BaseAddress,
                Requests =
                [
                    ..symbol.QueryMethods()
                        .ThatArePartialDefinitions()
                        .Select(method => method.QueryHttpRequest())
                        .Where(model => model.HasValue)
                        .Select(model => model.Value)
                ]
            })
            .OrNull();
}
```

## Fluent Query Chains

Compose symbol queries naturally:

```csharp
// Good: fluent chain
var methods = type.QueryMethods()
    .ThatArePublic()
    .ThatAreNotStatic()
    .WithAttribute<EffectAttribute>()
    .GetAll();

// Avoid: manual LINQ over raw Roslyn APIs
var methods = type.GetMembers()
    .OfType<IMethodSymbol>()
    .Where(m => m.DeclaredAccessibility == Accessibility.Public)
    .Where(m => !m.IsStatic);
```

### Early Exit with IsNotValid

Use the projection pattern for null-safety:

```csharp
var attr = symbol.GetAttribute<StrongIdAttribute>();

if (attr.IsNotValid(out var valid))
    return null;  // Early exit

// valid is guaranteed non-null here
var query = valid.AsQuery<StrongIdAttributeQuery>();
```

### Chaining with Map and OrDefault

```csharp
// OrDefault for optional values with fallbacks
var maxRetries = attr.NamedArg<int>("MaxRetries").OrDefault(3);

// OrNull for truly optional values
var prefix = attr.NamedArg<string>("Prefix").OrNull();

// OrThrow for required values
var id = attr.NamedArg<string>("Id").OrThrow("Id is required");

// OrElse for fallback chains
var target = symbol.GetAttribute<PrimaryAttribute>()
    .OrElse(() => symbol.GetAttribute<FallbackAttribute>());
```

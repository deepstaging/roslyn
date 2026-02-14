# Projections

Optional and validated wrappers that make null-checking less painful.

> **See also:** [Queries](../queries/index.md) | [Emit](../emit/index.md) | [Extensions](../extensions/index.md)

## Overview

Roslyn symbols are often nullable, requiring constant null checks. Projections wrap these nullable values in types that provide safe access and fluent transformations:

| Type | Description |
|------|-------------|
| [OptionalSymbol](optional-symbol.md) | A symbol that may or may not be present |
| [ValidSymbol](valid-symbol.md) | A symbol guaranteed to be non-null |
| [OptionalAttribute](optional-attribute.md) | An attribute that may or may not be present |
| [ValidAttribute](valid-attribute.md) | An attribute guaranteed to be non-null |
| [AttributeQuery](attribute-query.md) | Base for strongly-typed attribute query wrappers |
| [OptionalArgument](optional-argument.md) | An attribute argument that may or may not exist |
| [OptionalValue](optional-value.md) | A general-purpose optional wrapper |
| [Syntax Wrappers](syntax-wrappers.md) | Optional/Valid wrappers for syntax nodes |
| [XmlDocumentation](xml-documentation.md) | Parsed XML documentation from a symbol |
| [EquatableArray](equatable-array.md) | Drop-in `ImmutableArray<T>` replacement with sequence equality |
| [Snapshots](snapshots.md) | Pipeline-safe materializations of Roslyn symbols |
| [PipelineModel](pipeline-model.md) | `[PipelineModel]` attribute and DSRK analyzers |

## The Pattern

```csharp
// Without projections — null checks everywhere
var attr = symbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "MyAttribute");
if (attr == null) return;
var value = attr.ConstructorArguments.FirstOrDefault().Value;
if (value is not string s) return;
// finally use s

// With projections — fluent, null-safe operations
var value = symbol
    .GetAttribute("MyAttribute")
    .ConstructorArg<string>(0)
    .OrDefault("fallback");
```

---

## Real-World Examples

### Extract Attribute Configuration

```csharp
var config = symbol
    .GetAttribute("RetryAttribute")
    .WithArgs(a => new RetryConfig
    {
        MaxRetries = a.NamedArg<int>("MaxRetries").OrDefault(3),
        DelayMs = a.NamedArg<int>("DelayMs").OrDefault(1000),
        ExponentialBackoff = a.NamedArg<bool>("Exponential").OrDefault(false)
    })
    .OrDefault(RetryConfig.Default);
```

### Early Exit Pattern in Analyzers

```csharp
protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type)
{
    var target = GetFirstInvalidTarget(type);
    return target.HasValue;
}

private static OptionalSymbol<INamedTypeSymbol> GetFirstInvalidTarget(ValidSymbol<INamedTypeSymbol> type)
{
    return OptionalSymbol<INamedTypeSymbol>.FromNullable(
        type.GetAttributes("EffectsModule")
            .FirstOrDefault(t => !t.TargetType.IsInterface)
            ?.TargetType.Value
    );
}
```

### Safe Type Navigation

```csharp
var elementType = typeSymbol
    .AsOptional()
    .Where(t => t.IsGenericType && t.Name == "List")
    .Map(t => t.SingleTypeArgument)
    .OrDefault(OptionalSymbol<ITypeSymbol>.Empty());
```

### Validate Before Processing

```csharp
public void Process(OptionalSymbol<IMethodSymbol> method)
{
    if (method.IsNotValid(out var valid))
    {
        ReportError("Method symbol required");
        return;
    }
    
    // valid is ValidSymbol<IMethodSymbol> — no null checks needed
    var name = valid.Name;
    var isAsync = valid.IsAsync;
    var parameters = valid.Value.Parameters;
}
```

### Chain Optional Operations

```csharp
var serviceName = symbol
    .GetAttribute("ServiceAttribute")
    .ConstructorArg<INamedTypeSymbol>(0)
    .Map(t => t.Name)
    .OrDefault(() => symbol.Name + "Service");
```

### Work with Generic Attributes

```csharp
// For [Handler<TRequest, TResponse>]
var attr = symbol.GetAttribute("Handler");
var requestType = attr.GetTypeArgument(0).OrThrow("Request type required");
var responseType = attr.GetTypeArgument(1).OrThrow("Response type required");
```

### Check Type Hierarchy

```csharp
// Check if a type implements IDisposable
if (typeSymbol.ImplementsInterface("IDisposable"))
{
    // Generate dispose pattern
}

// Check if inherits from a specific base class
if (typeSymbol.InheritsFrom("ControllerBase"))
{
    // Handle controller-specific generation
}

// Iterate all base types
foreach (var baseType in typeSymbol.GetBaseTypes())
{
    Console.WriteLine($"Inherits from: {baseType.Name}");
}

// Get all interfaces including inherited ones
var allInterfaces = typeSymbol.GetAllInterfaces()
    .Select(i => i.FullyQualifiedName)
    .ToList();
```

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](https://github.com/deepstaging/roslyn/blob/main/LICENSE) for the full legal text.

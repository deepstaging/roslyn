# Extensions

Convenience methods for common Roslyn operations.

> **See also:** [Queries](../queries/index.md) | [Projections](../projections/index.md) | [Emit](../emit/index.md)

## Overview

Extension methods organized by the type they extend. These wrap common Roslyn operations in fluent, null-safe APIs.

| Category | Description |
|----------|-------------|
| [Symbol Extensions](symbol-extensions.md) | ISymbol, INamedTypeSymbol, IMethodSymbol |
| [Type Extensions](type-extensions.md) | ITypeSymbol — collections, async, classification, equality, and more |
| [Compilation Extensions](compilation-extensions.md) | Compilation and namespace queries |
| [Attribute Extensions](attribute-extensions.md) | AttributeData extraction |
| [String Extensions](string-extensions.md) | Case conversion utilities |
| [Domain Extensions](domain-extensions.md) | Entity Framework, LanguageExt, JSON utilities |

---

## Usage Examples

### Find Async Methods Returning Entity Types

```csharp
var asyncMethods = typeSymbol.QueryMethods()
    .ThatArePublic()
    .ThatAreAsync()
    .Where(m => m.ReturnType.IsGenericTaskType())
    .GetAll();
```

### Check If Type Is Repository

```csharp
bool isRepo = typeSymbol.IsInterfaceType() &&
              typeSymbol.Name.EndsWith("Repository") &&
              ((INamedTypeSymbol)typeSymbol).ImplementsInterface("IRepository");
```

### Extract Attribute Configuration

```csharp
var config = symbol.GetAttributesByName("Configure")
    .FirstOrDefault()
    ?.Query()
    .WithArgs(attr => new Config
    {
        Name = attr.ConstructorArg<string>(0).OrDefault("Default"),
        Enabled = attr.NamedArg<bool>("Enabled").OrDefault(true),
        Priority = attr.NamedArg<int>("Priority").OrDefault(0)
    })
    .OrDefault(Config.Default);
```

### Check Type Hierarchy

```csharp
if (typeSymbol.InheritsFrom("ControllerBase", "Microsoft.AspNetCore.Mvc"))
{
    var actions = typeSymbol.QueryMethods()
        .ThatArePublic()
        .ThatAreInstance()
        .WithoutAttribute<NonActionAttribute>()
        .GetAll();
}
```

### Analyze Async Method Signatures

```csharp
var method = typeSymbol.QueryMethods()
    .WithName("ProcessAsync")
    .FirstOrDefault();

if (method.IsValid(out var valid))
{
    var asyncKind = valid.Value.GetAsyncKind();
    var innerType = valid.Value.GetAsyncReturnType();
    
    if (valid.Value.IsAsyncVoid())
    {
        // Report diagnostic: async void is problematic
    }
}
```

### Filter by Generated Code

```csharp
var userDefinedTypes = TypeQuery.From(compilation)
    .ThatAreClasses()
    .Where(t => t.IsNotGeneratedCode())
    .GetAll();
```

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](https://github.com/deepstaging/roslyn/blob/main/LICENSE) for the full legal text.

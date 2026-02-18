# TemplateName

`TemplateName` is a `readonly struct` that uniquely identifies an embedded Scriban template by its resource name and assembly.

## Overview

```
TemplateName
├── Value      → "MyProject.Templates.MyTemplate.scriban-cs"
└── Assembly   → Assembly containing the embedded resource
```

## Creating a Factory

The most common pattern creates a factory function scoped to a generator's namespace:

```csharp
public class MyGenerator : IIncrementalGenerator
{
    // Creates: "MyNamespace.Templates.{name}"
    private static readonly Func<string, TemplateName> Named =
        TemplateName.ForGenerator<MyGenerator>();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Each call creates a TemplateName for a specific template
        var template = Named("MyTemplate.scriban-cs");
    }
}
```

`ForGenerator<T>()` infers the namespace from `T` and the assembly from `T`'s declaring assembly. The template name is constructed as `{Namespace}.Templates.{fileName}`.

## Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | Fully qualified embedded resource name |
| `Assembly` | `Assembly` | Assembly containing the embedded resource |

## Operators

| Operator | Description |
|----------|-------------|
| `implicit operator string` | Converts to the `Value` string |
| `==` / `!=` | Value equality based on `Value` and `Assembly` |

## Conventions

Templates must be embedded resources in a `Templates/` directory:

```xml
<ItemGroup>
    <EmbeddedResource Include="Templates\*.scriban-cs" />
</ItemGroup>
```

Directory structure:

```
MyProject/
├── MyGenerator.cs
└── Templates/
    ├── Class.scriban-cs
    └── Interface.scriban-cs
```

The embedded resource name follows the standard .NET convention: `{RootNamespace}.Templates.{FileName}` with dots replacing path separators.

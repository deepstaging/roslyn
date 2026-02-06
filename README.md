# Deepstaging.Roslyn

A fluent toolkit for building Roslyn source generators, analyzers, and code fixes.

[![Documentation](https://img.shields.io/badge/docs-deepstaging.github.io%2Froslyn-blue)](https://deepstaging.github.io/roslyn)

Roslyn APIs are powerful but awkward. This library wraps them in something that feels natural.

## Packages

| Package | Purpose |
|---------|---------|
| `Deepstaging.Roslyn` | Core toolkit: Queries, Projections, Emit builders |
| `Deepstaging.Roslyn.Scriban` | Scriban template integration for source generators |
| `Deepstaging.Roslyn.Workspace` | Code fix provider infrastructure |
| `Deepstaging.Roslyn.Testing` | Test utilities for generators and analyzers |

## Installation

```bash
dotnet add package Deepstaging.Roslyn
dotnet add package Deepstaging.Roslyn.Scriban  # Optional: Scriban templates
```

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        Your Generator                           │
├─────────────────────────────────────────────────────────────────┤
│  Queries          │  Projections       │  Emit                  │
│  ───────          │  ───────────       │  ────                  │
│  TypeQuery        │  OptionalSymbol<T> │  TypeBuilder           │
│  MethodQuery      │  ValidSymbol<T>    │  MethodBuilder         │
│  PropertyQuery    │  OptionalAttribute │  PropertyBuilder       │
│  FieldQuery       │  ValidAttribute    │  FieldBuilder          │
│  ParameterQuery   │  XmlDocumentation  │  ConstructorBuilder    │
│  ConstructorQuery │                    │  AttributeBuilder      │
│  EventQuery       │                    │  BodyBuilder           │
└─────────────────────────────────────────────────────────────────┘
```

## Quick Examples

### Queries — Find Symbols

```csharp
// Find all public partial classes with a specific attribute
var types = TypeQuery.From(compilation)
    .ThatArePublic()
    .ThatAreClasses()
    .ThatArePartial()
    .WithAttribute("GenerateAttribute")
    .GetAll();

// Find async methods returning Task<T>
var methods = MethodQuery.From(typeSymbol)
    .ThatArePublic()
    .ThatAreAsync()
    .WithReturnTypeMatching(t => t.Name == "Task")
    .GetAll();
```

### Projections — Safe Symbol Access

```csharp
// OptionalSymbol<T> — May or may not have a value
var optional = symbol.GetAttribute("MyAttribute");

// Early-exit pattern
if (optional.IsNotValid(out var valid))
    return;

// valid is now ValidSymbol<T> with guaranteed non-null access
var typeName = valid.Name;

// Chain attribute access with defaults
var maxRetries = symbol
    .GetAttribute("RetryAttribute")
    .NamedArg("MaxRetries")
    .OrDefault(3);
```

### Emit — Generate Code

```csharp
var code = TypeBuilder.Class("CustomerDto")
    .InNamespace("MyApp.Models")
    .AsPartial()
    .WithUsing("System")
    .AddProperty("Id", "int", p => p
        .WithGetter()
        .WithSetter())
    .AddProperty("Name", "string", p => p
        .WithGetter()
        .WithInitOnlySetter())
    .AddMethod("Validate", "bool", m => m
        .AsPublic()
        .WithBody(b => b
            .AddStatement("return !string.IsNullOrEmpty(Name)")))
    .Emit();
```

### Scriban Templates

```csharp
// In your generator
context.AddFromTemplate(
    TemplateName.ForGenerator<MyGenerator>()("MyTemplate.scriban-cs"),
    $"{typeName}.g.cs",
    new { TypeName = typeName, Properties = properties });
```

```scriban
// Templates/MyTemplate.scriban-cs
namespace {{ namespace }};

public partial class {{ type_name }}
{
{{~ for prop in properties ~}}
    public {{ prop.type }} {{ prop.name }} { get; set; }
{{~ end ~}}
}
```

## Documentation

- **[Full Documentation](https://deepstaging.github.io/roslyn)** — Complete API reference
  - [Queries](https://deepstaging.github.io/roslyn/api/queries/) — Find types, methods, properties, and more
  - [Projections](https://deepstaging.github.io/roslyn/api/projections/) — Safe nullable symbol wrappers
  - [Emit](https://deepstaging.github.io/roslyn/api/emit/) — Generate C# code with fluent builders
  - [Extensions](https://deepstaging.github.io/roslyn/api/extensions/) — Convenience methods for Roslyn types
- **[Scriban Templates](https://deepstaging.github.io/roslyn/packages/scriban/)** — Template infrastructure
- **[Workspace / Code Fixes](https://deepstaging.github.io/roslyn/packages/workspace/)** — Code fix providers
- **[Testing](https://deepstaging.github.io/roslyn/packages/testing/)** — Test infrastructure for analyzers and generators

## Build & Test

```bash
dotnet build Deepstaging.Roslyn.slnx
dotnet test Deepstaging.Roslyn.slnx
```

## Philosophy

This is utility code, not a framework. It should feel like Roslyn's missing standard library.

- When you call `.GetAll()`, you get actual Roslyn symbols you can use normally
- When you call `.Emit()`, you get valid `CompilationUnitSyntax` you can use with Roslyn's APIs
- Reading and writing are symmetric: TypeQuery finds types → TypeBuilder creates types

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

See [LICENSE](LICENSE) for the full legal text.

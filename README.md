# Deepstaging.Roslyn

A fluent toolkit for building Roslyn source generators, analyzers, and code fixes.

[![Documentation](https://img.shields.io/badge/docs-deepstaging.github.io%2Froslyn-blue)](https://deepstaging.github.io/roslyn)

Roslyn APIs are powerful but awkward. This library wraps them in something that feels natural.

## Packages

| Package | Purpose |
|---------|---------|
| `Deepstaging.Roslyn` | Core toolkit: Queries, Projections, Types, Expressions, Emit builders, Scriban templates, Code fixes |

| `Deepstaging.Roslyn.LanguageExt` | LanguageExt type references, expression builders, and effect lifting |
| `Deepstaging.Roslyn.Testing` | Test utilities for generators and analyzers |

## Installation

```bash
dotnet add package Deepstaging.Roslyn
dotnet add package Deepstaging.Roslyn.LanguageExt  # Optional: LanguageExt support
```

## Architecture

```
┌──────────────────────────────────────────────────────────────────────────────┐
│                              Your Generator                                  │
├──────────────────────────────────────────────────────────────────────────────┤
│  Queries          │  Projections       │  Types & Expressions                │
│  ───────          │  ───────────       │  ──────────────────                 │
│  TypeQuery        │  OptionalSymbol<T> │  TaskTypeRef, ListTypeRef, ...      │
│  MethodQuery      │  ValidSymbol<T>    │  TaskExpression, CollectionExpr, ...│
│  PropertyQuery    │  OptionalAttribute │                                     │
│  FieldQuery       │  ValidAttribute    │  Emit                               │
│  ParameterQuery   │  XmlDocumentation  │  ────                               │
│  ConstructorQuery │                    │  TypeBuilder, MethodBuilder          │
│  EventQuery       │                    │  PropertyBuilder, FieldBuilder       │
│                   │                    │  ConstructorBuilder, BodyBuilder     │
└──────────────────────────────────────────────────────────────────────────────┘
```

Reading and writing are symmetric: `TypeQuery` finds types ↔ `TypeBuilder` creates types.

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

### Types & Expressions — Type-Safe References

```csharp
using Deepstaging.Roslyn.Types;
using Deepstaging.Roslyn.Expressions;

// Typed wrappers carry constituent types, not just strings
var taskType = new TaskTypeRef("Customer");           // Task<Customer>
var listType = new ListTypeRef("Order");              // List<Order>
var eqType = new EqualityComparerTypeRef("string");   // EqualityComparer<string>

// Expression factories produce code fragments
var completed = TaskExpression.FromResult("result");   // Task.FromResult(result)
var equals = EqualityComparerExpression.DefaultEquals("string", "_name", "value");
// → EqualityComparer<string>.Default.Equals(_name, value)
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
  - [Types](https://deepstaging.github.io/roslyn/api/types/) — Typed wrappers for common .NET types
  - [Expressions](https://deepstaging.github.io/roslyn/api/expressions/) — Expression factories and builder extensions
  - [Emit](https://deepstaging.github.io/roslyn/api/emit/) — Generate C# code with fluent builders
  - [Extensions](https://deepstaging.github.io/roslyn/api/extensions/) — Convenience methods for Roslyn types
- **[LanguageExt](https://deepstaging.github.io/roslyn/api/languageext/)** — LanguageExt types, expressions, and effect lifting
- **[Scriban Templates](https://deepstaging.github.io/roslyn/packages/scriban/)** — Template infrastructure
- **[Code Fixes](https://deepstaging.github.io/roslyn/packages/workspace/)** — Code fix providers
- **[Testing](https://deepstaging.github.io/roslyn/packages/testing/)** — Test infrastructure for analyzers and generators

## Build & Test

```bash
dotnet build Deepstaging.Roslyn.slnx
dotnet run --project test/Deepstaging.Roslyn.Tests -c Release
```

## Philosophy

This is utility code, not a framework. It should feel like Roslyn's missing standard library.

- When you call `.GetAll()`, you get actual Roslyn symbols you can use normally
- When you call `.Emit()`, you get valid `CompilationUnitSyntax` you can use with Roslyn's APIs
- `TypeRef` wrappers carry constituent types for compile-time introspection, not just strings

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

See [LICENSE](LICENSE) for the full legal text.

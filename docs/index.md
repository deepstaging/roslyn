# Deepstaging.Roslyn

A fluent toolkit for building Roslyn source generators, analyzers, and code fixes.

Roslyn APIs are powerful but awkward. This library wraps them in something that feels natural.

## Quick Start

```bash
dotnet new classlib -n MyProject.Generators -f netstandard2.0
cd MyProject.Generators
dotnet add package Deepstaging.Roslyn
```

## What You Get

### Queries — Find Symbols

```csharp
var types = TypeQuery.From(compilation)
    .ThatArePublic()
    .ThatAreClasses()
    .ThatArePartial()
    .WithAttribute("GenerateAttribute")
    .GetAll();
```

### Projections — Safe Symbol Access

```csharp
var optional = symbol.GetAttribute("MyAttribute");

if (optional.IsNotValid(out var valid))
    return;

var typeName = valid.Name;
```

### Emit — Generate Code

```csharp
var code = TypeBuilder.Class("CustomerDto")
    .InNamespace("MyApp.Models")
    .AsPartial()
    .AddProperty("Id", "int", p => p.WithGetter().WithSetter())
    .Emit();
```

## Packages

| Package | Purpose |
|---------|---------|
| [`Deepstaging.Roslyn`](getting-started.md) | Core toolkit — [Queries](api/queries/index.md), [Projections](api/projections/index.md), [Emit](api/emit/index.md), [Scriban](api/scriban/index.md), [Workspace](api/workspace/index.md) |
| [`Deepstaging.Roslyn.Testing`](api/testing/index.md) | Test utilities for generators, analyzers, and code fixes |
| [`Deepstaging.Roslyn.LanguageExt`](api/languageext/index.md) | LanguageExt integration — Eff lifting, expressions, refs |

Scriban and Workspace are bundled inside the core package — no separate install needed.

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
│  ParameterQuery   │  AttributeQuery    │  ConstructorBuilder    │
│  ConstructorQuery │  XmlDocumentation  │  AttributeBuilder      │
│  EventQuery       │                    │  BodyBuilder           │
└─────────────────────────────────────────────────────────────────┘
```

## Philosophy

This is utility code, not a framework. It should feel like Roslyn's missing standard library.

- When you call `.GetAll()`, you get actual Roslyn symbols you can use normally
- When you call `.Emit()`, you get valid `CompilationUnitSyntax` you can use with Roslyn's APIs
- Reading and writing are symmetric: TypeQuery finds types → TypeBuilder creates types

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

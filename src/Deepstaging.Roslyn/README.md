# Deepstaging.Roslyn

Roslyn APIs are powerful but awkward. This library wraps them in something that feels natural.

📚 **[Full Documentation](https://deepstaging.github.io/roslyn)**

## What is this?

Three things:

**Query builders** for finding symbols without writing loops:

```csharp
TypeQuery.From(compilation)
    .ThatArePublic()
    .ThatAreClasses()
    .WithAttribute("MyAttribute")
    .GetAll()
```

**Projections** for working with nullable Roslyn data safely:

```csharp
OptionalSymbol<INamedTypeSymbol> type = GetTypeOrNull();
var name = type
    .Where(t => t.IsPublic)
    .Map(t => t.FullyQualifiedName)
    .OrNull();
```

**Emit builders** for generating C# code with fluent syntax:

```csharp
TypeBuilder
    .Class("Customer")
    .AddProperty("Name", "string", prop => prop
        .WithAccessibility(Accessibility.Public)
        .WithAutoPropertyAccessors())
    .Emit()
```

## Features

- **[Queries](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn/Docs/Queries.md)** - Fluent
  builders for finding types, methods, properties, fields, constructors, events, and parameters
- **[Projections](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn/Docs/Projections.md)** -
  Optional/validated wrappers that make null-checking less painful
- **[Emit](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn/Docs/Emit.md)** - Fluent builders for
  generating compilable C# code
- **[Extensions](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn/Docs/Extensions.md)** -
  Convenience methods for common Roslyn operations

## Why?

Writing Roslyn analyzers and source generators means dealing with:

- Nullable symbols everywhere
- Casting between ISymbol types
- Manually looping through members
- Verbose syntax for common checks
- Complex SyntaxFactory calls for code generation

This library doesn't hide Roslyn, it just makes it less annoying. You still get ISymbol references back when you need
them, and you get valid CompilationUnitSyntax when generating code.

## Installation

Reference this project in your analyzer. It targets netstandard2.0 for broad compatibility.

## Quick Examples

**Find all async methods that return Task:**

```csharp
MethodQuery.From(typeSymbol)
    .ThatAreAsync()
    .WithReturnType("System.Threading.Tasks.Task")
    .GetAll()
```

**Get an attribute argument or fall back to default:**

```csharp
var maxRetries = symbol
    .GetAttribute("RetryAttribute")
    .NamedArg<int>("MaxRetries")
    .OrDefault(3);
```

**Check if a type implements an interface:**

```csharp
bool isDisposable = ((INamedTypeSymbol)typeSymbol)
    .ImplementsInterface("System.IDisposable");
```

**Generate a complete class with properties:**

```csharp
var result = TypeBuilder
    .Class("Customer")
    .InNamespace("MyApp.Domain")
    .AddProperty("Id", "Guid", prop => prop
        .WithAccessibility(Accessibility.Public)
        .WithAutoPropertyAccessors())
    .AddProperty("Name", "string", prop => prop
        .WithAccessibility(Accessibility.Public)
        .WithAutoPropertyAccessors())
    .Emit();

if (result.IsValid(out var validEmit))
{
    string code = validEmit.Code;  // Valid, compilable C#
}
```

## Documentation

Each feature has its own doc file with examples:

- **[Queries.md](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn/Docs/Queries.md)** - How to use
  each query builder
- **[Projections.md](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn/Docs/Projections.md)** -
  OptionalSymbol, ValidSymbol, and friends
- **[Emit.md](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn/Docs/Emit.md)** - Fluent builders
  for generating C# code
- **[Extensions.md](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn/Docs/Extensions.md)** -
  Helper methods for common tasks

The test project is also useful documentation - every test shows real usage.

## Philosophy

This is utility code, not a framework. It should feel like Roslyn's missing standard library, not something layered on
top.

When you call `.GetAll()`, you get an array of actual Roslyn symbols you can use normally. When you call `.Emit()`, you
get valid CompilationUnitSyntax you can use with Roslyn's APIs.

**Reading and writing are symmetric:**

- TypeQuery finds types → TypeBuilder creates types
- ValidSymbol wraps symbols → ValidEmit wraps generated code
- String-based filters → String-based type references
- Fluent, immutable → Fluent, immutable

## Related Documentation

- **[Main README](https://github.com/deepstaging/roslyn/blob/main/README.md)** - Project overview and effects system
- **[Testing](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn.Testing/README.md)** - Test
  infrastructure for Roslyn components
- **[Scriban Templates](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn.Scriban/README.md)** -
  Template infrastructure for source generators
- **[Workspace / Code Fixes](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn.Workspace/README.md)
  ** - Code fix provider infrastructure
- **[Emit API Overview](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn/Emit/README.md)** - Quick
  reference for the Emit API

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service
or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we
think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](https://github.com/deepstaging/roslyn/blob/main/LICENSE) for the full legal text.

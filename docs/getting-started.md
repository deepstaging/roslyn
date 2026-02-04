# Getting Started

This guide walks you through building your first source generator with Deepstaging.Roslyn.

## Installation

Add the package to your analyzer/generator project:

```bash
dotnet add package Deepstaging.Roslyn
```

Your `.csproj` should target `netstandard2.0` for Roslyn compatibility:

```xml
<PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>
</PropertyGroup>
```

## Your First Generator

Here's a complete incremental generator that finds classes with `[AutoNotify]` and generates property change notifications:

```csharp
using Deepstaging.Roslyn.Generators;

namespace Deepstaging.Generators;

/// <inheritdoc />
public class AutoNotifyAttribute : Attribute;

/// <inheritdoc />
[Generator]
public class AutoNotifyGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.ForEach(
            query: ctx => ctx.ForAttribute<AutoNotifyAttribute>().Map((c, _) => new AutoNotifyModel(c.TargetSymbol)),
            generate: (ctx, model) =>
            {
                var hint = new HintName(model.Namespace);

                // Build the partial class
                var code = TypeBuilder.Class(model.TypeName)
                    .InNamespace(model.Namespace)
                    .AsPartial()
                    .AddUsing("System.ComponentModel")
                    .WithEach(model.Fields, (builder, field) => builder
                        .AddProperty(field.PropertyName,
                            field.Type.FullyQualifiedName, p => p
                                .WithGetter(b => b.AddStatement($"return {field.Name}"))
                                .WithSetter(b => b
                                    .AddStatement($"{field.Name} = value")
                                    .AddStatement($"OnPropertyChanged(nameof({field.ParameterName}))"))));

                ctx.AddFromEmit(
                    hint.Filename(model.TypeName),
                    code.Emit()
                );
            });
    }
}

internal record AutoNotifyModel(ISymbol? Type)
{
    public ISymbol? Type { get; init; } = Type;
    private readonly ValidSymbol<INamedTypeSymbol> _symbol = Type.AsValidNamedType();

    public string TypeName => _symbol.Name;
    public string Namespace => _symbol.Namespace ?? "Global";

    public ImmutableArray<ValidSymbol<IFieldSymbol>> Fields => _symbol.QueryFields()
        .ThatArePrivate()
        .ThatAreInstance()
        .GetAll();
}
```

### Generator Context Extensions

The `ForAttribute` extension simplifies attribute-based symbol discovery:

```csharp
// By generic type (requires attribute assembly reference)
context.ForAttribute<MyAttribute>()
    .Map((ctx, ct) => BuildModel(ctx));

// By fully qualified name (no assembly reference needed)
context.ForAttribute("MyNamespace.MyAttribute")
    .Map((ctx, ct) => BuildModel(ctx));

// With syntax predicate for filtering
context.ForAttribute<MyAttribute>()
    .Where(
        syntaxPredicate: (node, ct) => node is ClassDeclarationSyntax,
        builder: (ctx, ct) => BuildModel(ctx));
```

For non-attribute-based discovery, use `MapTypes`:

```csharp
context.MapTypes((compilation, ct) =>
    TypeQuery.From(compilation)
        .ThatAreClasses()
        .ThatArePartial()
        .WithName("*Repository")
        .Select(t => new RepositoryModel(t.Value)));
```

## Core Concepts

### Queries

Find symbols with fluent, composable filters:

```csharp
// Find all public async methods
var methods = type.QueryMethods()
    .ThatArePublic()
    .ThatAreAsync()
    .GetAll();
```

See [Queries](api/queries.md) for the full API.

### Projections

Work safely with nullable Roslyn data:

```csharp
var attr = symbol.GetAttribute("MyAttribute");

// Early-exit pattern
if (attr.IsNotValid(out var valid))
    return;

// Now you have guaranteed non-null access
var name = valid.NamedArg("Name").OrDefault("default");
```

See [Projections](api/projections.md) for details.

### Emit

Generate C# code with fluent builders:

```csharp
var code = TypeBuilder.Class("Generated")
    .AddMethod("Execute", "void", m => m
        .AsPublic()
        .WithBody(b => b.AddStatement("Console.WriteLine(\"Hello\")")))
    .Emit();
```

See [Emit](api/emit.md) for the full API.

## Next Steps

- [Queries API Reference](api/queries.md)
- [Projections API Reference](api/projections.md)
- [Emit API Reference](api/emit.md)
- [Testing your generators](packages/testing.md)

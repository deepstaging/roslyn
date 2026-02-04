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
using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Generators;
using Microsoft.CodeAnalysis;

[Generator]
public class AutoNotifyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Use ForAttribute extension to find types with the attribute
        var models = context
            .ForAttribute<AutoNotifyAttribute>()
            .Map((ctx, ct) => new AutoNotifyModel(ctx.TargetSymbol as INamedTypeSymbol));

        context.RegisterSourceOutput(models.Collect(), Generate);
    }

    private void Generate(SourceProductionContext context, 
                          ImmutableArray<AutoNotifyModel> models)
    {
        foreach (var model in models)
        {
            if (model.Type is null) continue;

            // Query for fields to wrap
            var fields = model.Type.QueryFields()
                .ThatArePrivate()
                .ThatAreNotStatic()
                .GetAll();

            // Build the partial class
            var code = TypeBuilder.Class(model.Type.Name)
                .InNamespace(model.Type.ContainingNamespace.ToDisplayString())
                .AsPartial()
                .AddUsing("System.ComponentModel");

            foreach (var field in fields)
            {
                var propName = ToPascalCase(field.Value.Name);
                code = code.AddProperty(propName, field.Value.Type.ToDisplayString(), p => p
                    .WithGetter(b => b.AddStatement($"return {field.Value.Name}"))
                    .WithSetter(b => b
                        .AddStatement($"{field.Value.Name} = value")
                        .AddStatement($"OnPropertyChanged(nameof({propName}))")));
            }

            var result = code.Emit();
            if (result.IsValid(out var valid))
            {
                context.AddSource($"{model.Type.Name}.g.cs", valid.Code);
            }
        }
    }

    private static string ToPascalCase(string fieldName) =>
        fieldName.TrimStart('_') is { Length: > 0 } s 
            ? char.ToUpper(s[0]) + s[1..] 
            : fieldName;
}

record AutoNotifyModel(INamedTypeSymbol? Type);
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

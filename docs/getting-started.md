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
using Microsoft.CodeAnalysis;

[Generator]
public class AutoNotifyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var types = context.SyntaxProvider
            .ForTypesWithAttribute("AutoNotifyAttribute")
            .Collect();

        context.RegisterSourceOutput(types, Generate);
    }

    private void Generate(SourceProductionContext context, 
                          ImmutableArray<INamedTypeSymbol> types)
    {
        foreach (var type in types)
        {
            // Query for fields to wrap
            var fields = type.QueryFields()
                .ThatArePrivate()
                .ThatAreNotStatic()
                .GetAll();

            // Build the partial class
            var code = TypeBuilder.Class(type.Name)
                .InNamespace(type.ContainingNamespace.ToDisplayString())
                .AsPartial()
                .WithUsing("System.ComponentModel");

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
                context.AddSource($"{type.Name}.g.cs", valid.Code);
            }
        }
    }
}
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

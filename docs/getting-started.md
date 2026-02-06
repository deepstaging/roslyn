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

Here's an incremental generator that finds classes with `[AutoNotify]` and generates property change notifications. This example follows the recommended **Projection Pattern** with separate model, projection, and writer layers.

### The Attribute

```csharp
namespace MyProject;

[AttributeUsage(AttributeTargets.Class)]
public class AutoNotifyAttribute : Attribute;
```

### The Model

Models are simple records that capture the data needed for code generation:

```csharp
namespace MyProject.Projection.Models;

public sealed record AutoNotifyModel
{
    public required string Namespace { get; init; }
    public required string TypeName { get; init; }
    public required Accessibility Accessibility { get; init; }
    public required ImmutableArray<NotifyPropertyModel> Properties { get; init; }
}

public sealed record NotifyPropertyModel
{
    public required string PropertyName { get; init; }
    public required string FieldName { get; init; }
    public required string TypeName { get; init; }
}
```

### The Projection

Projections query symbols and build models. This layer is the single source of truth for interpreting attributes:

```csharp
using Deepstaging.Roslyn;

namespace MyProject.Projection;

public static class AutoNotify
{
    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        public AutoNotifyModel? QueryAutoNotify()
        {
            var properties = symbol.QueryNotifyProperties();
            if (properties.IsEmpty)
                return null;

            return new AutoNotifyModel
            {
                Namespace = symbol.Namespace ?? "",
                TypeName = symbol.Name,
                Accessibility = symbol.Accessibility,
                Properties = properties
            };
        }
    }

    private static ImmutableArray<NotifyPropertyModel> QueryNotifyProperties(
        this ValidSymbol<INamedTypeSymbol> symbol)
    {
        return symbol.QueryFields()
            .ThatAreInstance()
            .ThatArePrivate()
            .Where(f => f.Name.StartsWith("_"))
            .Select(field => new NotifyPropertyModel
            {
                PropertyName = field.Name.TrimStart('_').ToPascalCase(),
                FieldName = field.Name,
                TypeName = field.Type?.FullyQualifiedName!
            });
    }
}
```

### The Writer

Writers use the Emit API to generate code from models:

```csharp
using Deepstaging.Roslyn.Emit;

namespace MyProject.Generators.Writers;

public static class AutoNotifyWriter
{
    extension(AutoNotifyModel model)
    {
        public OptionalEmit WriteAutoNotifyClass()
        {
            return TypeBuilder
                .Class(model.TypeName)
                .AsPartial()
                .InNamespace(model.Namespace)
                .WithAccessibility(model.Accessibility)
                .AddUsing("System.ComponentModel")
                .Implements("INotifyPropertyChanged")
                .AddEvent("PropertyChanged", "PropertyChangedEventHandler?")
                .AddMethod(MethodBuilder.Parse("protected void OnPropertyChanged(string name)")
                    .WithBody(b => b.AddStatement(
                        "PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name))")))
                .WithEach(model.Properties, AddProperty)
                .Emit();
        }
    }

    private static TypeBuilder AddProperty(TypeBuilder type, NotifyPropertyModel prop)
    {
        return type.AddProperty(prop.PropertyName, prop.TypeName, p => p
            .WithGetter(b => b.AddStatement($"return {prop.FieldName}"))
            .WithSetter(b => b
                .AddStatement($"{prop.FieldName} = value")
                .AddStatement($"OnPropertyChanged(nameof({prop.PropertyName}))")));
    }
}
```

### The Generator

The generator ties it all together with minimal code:

```csharp
using Deepstaging.Roslyn.Generators;
using MyProject.Projection;
using MyProject.Generators.Writers;

namespace MyProject.Generators;

[Generator]
public sealed class AutoNotifyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var models = context.ForAttribute<AutoNotifyAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryAutoNotify());

        context.RegisterSourceOutput(models, static (ctx, model) => model
            .WriteAutoNotifyClass()
            .RegisterSourceWith(ctx, HintName.From(model.Namespace, model.TypeName)));
    }
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

See [Queries](api/queries/index.md) for the full API.

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

See [Projections](api/projections/index.md) for details.

### Emit

Generate C# code with fluent builders:

```csharp
var code = TypeBuilder.Class("Generated")
    .AddMethod("Execute", "void", m => m
        .AsPublic()
        .WithBody(b => b.AddStatement("Console.WriteLine(\"Hello\")")))
    .Emit();
```

See [Emit](api/emit/index.md) for the full API.

## Next Steps

- [Queries API Reference](api/queries/index.md)
- [Projections API Reference](api/projections/index.md)
- [Emit API Reference](api/emit/index.md)
- [Testing your generators](packages/testing.md)

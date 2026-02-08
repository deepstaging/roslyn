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
using Deepstaging.Roslyn.Emit;
using Deepstaging.Roslyn.Emit.Interfaces.Observable;
using Deepstaging.Roslyn.Generators;
using Microsoft.CodeAnalysis;

namespace MyProject;

[AttributeUsage(AttributeTargets.Class)]
public class AutoNotifyAttribute : Attribute;

[Generator]
public sealed class AutoNotifyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var models = context.ForAttribute<AutoNotifyAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryAutoNotify());

        context.RegisterSourceOutput(models, static (ctx, model) => model
            .WriteAutoNotifyClass()
            .AddSourceTo(ctx, HintName.From(model.Namespace, model.TypeName)));
    }
}

internal record AutoNotifyModel(string Namespace, string TypeName, ImmutableArray<FieldModel> Fields);
internal record FieldModel(string FieldName, string PropertyName, string TypeName);

file static class AutoNotifyExtensions
{
    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        public AutoNotifyModel QueryAutoNotify() => new(
            symbol.Namespace ?? "Global",
            symbol.Name,
            symbol.QueryFields()
                .ThatArePrivate()
                .ThatAreInstance()
                .Where(f => f.Name.StartsWith("_"))
                .Select(f => new FieldModel(
                    f.Name,
                    f.Name.TrimStart('_').ToPascalCase(),
                    f.Type?.FullyQualifiedName ?? "object")));
    }

    extension(AutoNotifyModel model)
    {
        public OptionalEmit WriteAutoNotifyClass() => TypeBuilder
            .Class(model.TypeName)
            .AsPartial()
            .InNamespace(model.Namespace)
            .ImplementsINotifyPropertyChanged()
            .WithEach(model.Fields, (type, field) => type
                .AddProperty(field.PropertyName, field.TypeName, p => p
                    .WithGetter(b => b.AddStatement($"return {field.FieldName}"))
                    .WithSetter(b => b
                        .AddStatement($"{field.FieldName} = value")
                        .AddStatement($"OnPropertyChanged()"))))
            .Emit();
    }
}
```

The `ImplementsINotifyPropertyChanged()` extension adds:

- The `INotifyPropertyChanged` interface
- The `PropertyChanged` event
- A protected `OnPropertyChanged` helper with `[CallerMemberName]` support

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

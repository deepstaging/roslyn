# Getting Started

This guide walks you through building your first source generator with Deepstaging.Roslyn.

## Installation

Create a new `netstandard2.0` class library (required for Roslyn analyzer/generator compatibility):

```bash
dotnet new classlib -n MyProject.Generators -f netstandard2.0
cd MyProject.Generators
dotnet add package Deepstaging.Roslyn --prerelease
dotnet add package PolySharp
```

[PolySharp](https://github.com/Sergio0694/PolySharp) provides compiler polyfills (`record`, `init`, etc.) for `netstandard2.0` targets.

Then configure `MyProject.Generators.csproj`:

```xml
<PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <LangVersion>preview</LangVersion>
    <EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>
</PropertyGroup>
```

## Your First Generator

Delete the default `Class1.cs` and create `AutoNotifyGenerator.cs` with the following content — a complete incremental generator that finds classes with `[AutoNotify]` and generates property change notifications:

```csharp
using System;
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

[PipelineModel]
internal sealed record AutoNotifyModel(string Namespace, string TypeName, EquatableArray<FieldModel> Fields);

[PipelineModel]
internal sealed record FieldModel(string FieldName, string PropertyName, string TypeName);

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
                    f.Type.FullyQualifiedName))
                .ToEquatableArray());
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

// By Type — useful for generic attributes without backtick-arity strings
context.ForAttribute(typeof(MyAttribute<>))
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
        .Select(t => new RepositoryModel(t.Name, t.Namespace ?? "")));
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
- [Testing your generators](api/testing/index.md)

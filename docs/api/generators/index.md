# Generators

The `Deepstaging.Roslyn.Generators` namespace provides utilities for building Roslyn incremental source generators with less boilerplate.

## Components

| Type | Description |
|------|-------------|
| [ForAttribute / AttributeMapper](for-attribute.md) | Fluent API for `ForAttributeWithMetadataName` |
| [Combine Overloads](combine.md) | Multi-arity `Combine` with flat tuples and auto-collect |
| [HintName](hint-name.md) | Consistent hint name generation for output files |
| [OptionalEmit.AddSourceTo](source-output.md) | Safe source output with diagnostic reporting |

## The Thin Generator Pattern

Generators should be thin wiring between Projection and Emit layers:

```csharp
[Generator]
public sealed class StrongIdGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var models = context.ForAttribute<StrongIdAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol
                .AsValidNamedType()
                .ToStrongIdModel(ctx.SemanticModel));

        context.RegisterSourceOutput(models, static (ctx, model) =>
        {
            model.WriteStrongId()
                .AddSourceTo(ctx, HintName.From(model.Namespace, model.TypeName));
        });
    }
}
```

See the [Generator Guide](../../guides/generators.md) for patterns, writer classes, and best practices.

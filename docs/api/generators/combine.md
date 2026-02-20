# Combine Overloads

Multi-arity `Combine` extensions that flatten nested tuples and auto-collect plural providers.

## The Problem

Roslyn's built-in `Combine` only pairs two providers, producing nested tuples:

```csharp
// Each .Combine() wraps in another layer
a.Combine(b)                        // (A, B)
a.Combine(b).Combine(c)             // ((A, B), C)
a.Combine(b).Combine(c).Combine(d)  // (((A, B), C), D)

// Destructuring gets painful fast
var (((webApp, dispatches), commands), queries) = tuple;
```

You also need manual `.Collect()` calls to convert `IncrementalValuesProvider<T>` (plural) to `IncrementalValueProvider<ImmutableArray<T>>` (singular) before combining.

## The Solution

The `CombineExtensions` class provides overloads that accept multiple providers, call `.Collect()` internally, and return flat tuples:

```csharp
using Deepstaging.Roslyn.Generators;

var combined = modules
    .Combine(commandHandlers, queryHandlers)
    .Select(static (tuple, _) =>
    {
        var (module, commands, queries) = tuple;
        // commands: ImmutableArray<CommandHandlerGroupModel>
        // queries:  ImmutableArray<QueryHandlerGroupModel>
        ...
    });
```

## Available Overloads

All overloads accept `IncrementalValuesProvider<T>` (plural) parameters and collect them automatically.

### On `IncrementalValuesProvider<T>` (plural source)

```csharp
// 2 additional providers → flat 3-tuple
source.Combine(second, third)
// → IncrementalValuesProvider<(T1, ImmutableArray<T2>, ImmutableArray<T3>)>

// 3 additional providers → flat 4-tuple
source.Combine(second, third, fourth)
// → IncrementalValuesProvider<(T1, ImmutableArray<T2>, ImmutableArray<T3>, ImmutableArray<T4>)>
```

### On `IncrementalValueProvider<T>` (singular source)

```csharp
// 2 additional providers → flat 3-tuple
source.Combine(second, third)
// → IncrementalValueProvider<(T1, ImmutableArray<T2>, ImmutableArray<T3>)>

// 3 additional providers → flat 4-tuple
source.Combine(second, third, fourth)
// → IncrementalValueProvider<(T1, ImmutableArray<T2>, ImmutableArray<T3>, ImmutableArray<T4>)>
```

## Full Example

```csharp
[Generator]
public sealed class WebEndpointGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var webApps = context.ForAttribute<WebAppAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryWebApp());

        var dispatches = context.ForAttribute<DispatchModuleAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryDispatchModule());

        var commands = context.ForAttribute<CommandHandlerAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryCommandHandlerGroup());

        var queries = context.ForAttribute<QueryHandlerAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryQueryHandlerGroup());

        var combined = webApps
            .Combine(dispatches, commands, queries)
            .Select(static (tuple, _) =>
            {
                var (webApp, dispatches, commands, queries) = tuple;
                if (dispatches.Length == 0) return null;

                var dispatch = dispatches[0] with
                {
                    CommandHandlers = commands,
                    QueryHandlers = queries
                };

                return webApp.WithRoutes(dispatch);
            })
            .Where(static model => model is not null)
            .Select(static (model, _) => model!);

        context.RegisterSourceOutput(combined, static (ctx, model) =>
        {
            EndpointWriter.WriteEndpoints(model)
                .AddSourceTo(ctx, HintName.From(model.Namespace, $"{model.TypeName}.WebApp"));
        });
    }
}
```

# Deepstaging.RoslynKit.Generators

Source generators that produce code based on RoslynKit attributes. Uses the [Deepstaging.Roslyn](https://github.com/deepstaging/roslyn) framework and consumes projections from the Projection layer.

## Generators

| Generator | Attribute | Output |
|-----------|-----------|--------|
| `WithMethodsGenerator` | `[GenerateWith]` | Immutable `With*()` methods for each `init` property |
| `AutoNotifyGenerator` | `[AutoNotify]` | `INotifyPropertyChanged` implementation with properties |

## Architecture

```
Symbol → Projection Query → Model → Template → Generated Code
```

Each generator:
1. Queries the Projection layer for a strongly-typed model
2. Passes the model to a template/writer
3. Emits the generated source

## Key Files

| File | Purpose |
|------|---------|
| `WithMethodsGenerator.cs` | Generator for `[GenerateWith]` |
| `AutoNotifyGenerator.cs` | Generator for `[AutoNotify]` |
| `Templates/` | Source generation templates |
| `Writers/` | Code emission helpers |

## Generated Output Example

For a type with `[GenerateWith]`:

```csharp
// Input
[GenerateWith]
public partial class Person
{
    public string Name { get; init; } = "";
}

// Generated
public partial class Person
{
    public Person WithName(string value) => new Person { Name = value };
}
```

## Related Projects

- [RoslynKit](../Deepstaging.RoslynKit/) - Attribute definitions
- [RoslynKit.Projection](../Deepstaging.RoslynKit.Projection/) - Queries and models
- [RoslynKit.Tests](../Deepstaging.RoslynKit.Tests/) - Generator tests
- [Project README](../../README.md) - Full documentation

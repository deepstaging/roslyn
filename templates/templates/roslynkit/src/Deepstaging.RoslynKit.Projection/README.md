# Deepstaging.RoslynKit.Projection

Shared projection layer containing queries and models for extracting attribute data from Roslyn symbols. This project is the **single source of truth** for attribute interpretation, used by both generators and analyzers.

## Architecture

```
Roslyn Symbols → Queries → Models → Generators/Analyzers
```

The Projection pattern ensures consistent behavior across all Roslyn tools by:
- Defining strongly-typed models instead of passing raw symbols
- Providing reusable query extension methods
- Centralizing validation logic

## Key Files

| File | Purpose |
|------|---------|
| `Attributes/` | `AttributeQuery` extensions for finding attributes on symbols |
| `Queries.cs` | Extension methods on `ValidSymbol<T>` for extracting data |
| `Models/` | Strongly-typed models representing projected data |
| `AutoNotify.cs` | Projection for `[AutoNotify]` attribute |
| `WithMethods.cs` | Projection for `[GenerateWith]` attribute |

## Usage

```csharp
using Deepstaging.RoslynKit.Projection;

// In a generator or analyzer:
var model = validSymbol.QueryAutoNotify();
if (model is not null)
{
    foreach (var property in model.Properties)
    {
        // Use strongly-typed property data
    }
}
```

## Related Projects

- [RoslynKit](../Deepstaging.RoslynKit/) - Attribute definitions
- [RoslynKit.Generators](../Deepstaging.RoslynKit.Generators/) - Consumes projections for code generation
- [RoslynKit.Analyzers](../Deepstaging.RoslynKit.Analyzers/) - Consumes projections for diagnostics
- [Project README](../../README.md) - Full documentation

# RoslynKit.Sample

Example project demonstrating RoslynKit attribute usage. This sample shows how end users consume the generators, analyzers, and code fixes.

## Files

| File | Demonstrates |
|------|--------------|
| `Models.cs` | `[GenerateWith]` for immutable record-like types |
| `ViewModels.cs` | `[AutoNotify]` and `[AlsoNotify]` for MVVM |
| `Program.cs` | Usage of generated code |
| `Generated/` | Committed generated output (for reference) |

## Running

```bash
cd samples/RoslynKit.Sample
dotnet run
```

## Example Output

```
Original: Person { Name = Alice, Age = 30 }
Updated:  Person { Name = Alice, Age = 31 }
```

## Related

- [Project README](../../README.md) - Full documentation and architecture overview
- [RoslynKit Attributes](../../src/Deepstaging.RoslynKit/) - Attribute definitions

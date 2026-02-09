# Deepstaging.RoslynKit.CodeFixes

Code fix providers for RoslynKit analyzer diagnostics. Each fix references diagnostic IDs from the centralized `Diagnostics` class.

## Code Fixes

| Provider | Fixes | Action |
|----------|-------|--------|
| `MakePartialClassCodeFixProvider` | RK1001, RK1002 | Adds `partial` modifier to class |
| `MakePartialStructCodeFixProvider` | RK1001, RK1002 | Adds `partial` modifier to struct |
| `MakePrivateCodeFixProvider` | RK1003 | Changes field accessibility to `private` |

## Example

Before fix (RK1001):
```csharp
[GenerateWith]
public class Person  // Error: must be partial
{
    public string Name { get; init; }
}
```

After fix:
```csharp
[GenerateWith]
public partial class Person
{
    public string Name { get; init; }
}
```

## Related Projects

- [RoslynKit.Analyzers](../Deepstaging.RoslynKit.Analyzers/) - Analyzers that report these diagnostics
- [RoslynKit.Tests](../Deepstaging.RoslynKit.Tests/) - Code fix tests
- [Project README](../../README.md) - Full documentation

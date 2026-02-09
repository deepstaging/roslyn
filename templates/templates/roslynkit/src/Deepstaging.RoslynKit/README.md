# Deepstaging.RoslynKit

Attribute library for the RoslynKit project. Contains marker attributes consumed by the source generators and analyzers.

## Attributes

| Attribute | Target | Description |
|-----------|--------|-------------|
| `[GenerateWith]` | Class/Struct | Generates immutable `With*()` methods for all `init` properties |
| `[AutoNotify]` | Class | Generates `INotifyPropertyChanged` implementation from backing fields |
| `[AlsoNotify]` | Field | Used with `[AutoNotify]` to trigger additional property notifications |

## Usage

```csharp
using Deepstaging.RoslynKit;

// Simple immutable updates
[GenerateWith]
public partial class Person
{
    public string Name { get; init; } = "";
    public int Age { get; init; }
}

// MVVM property change notifications
[AutoNotify]
public partial class PersonViewModel
{
    [AlsoNotify(nameof(FullName))]
    private string _firstName = "";
    
    private string _lastName = "";
    
    public string FullName => $"{FirstName} {LastName}";
}
```

## Related Projects

- [RoslynKit.Projection](../Deepstaging.RoslynKit.Projection/) - Queries and models for attribute data extraction
- [RoslynKit.Generators](../Deepstaging.RoslynKit.Generators/) - Source generators that consume these attributes
- [RoslynKit.Analyzers](../Deepstaging.RoslynKit.Analyzers/) - Analyzers that validate attribute usage
- [Project README](../../README.md) - Full documentation

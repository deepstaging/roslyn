# Deepstaging.RoslynKit

Attribute library consumed by end users. Contains `[AutoNotify]` and `[AlsoNotify]`.

This project targets `netstandard2.0` with no dependencies beyond PolySharp (compile-time only). It's the only assembly consumers reference directly.

## Attributes

| Attribute | Target | Purpose |
|-----------|--------|---------|
| `[AutoNotify]` | Class | Generates `INotifyPropertyChanged` from private backing fields |
| `[AlsoNotify]` | Field | Raises additional `PropertyChanged` notifications |

## Usage

```csharp
[AutoNotify]
public partial class PersonViewModel
{
    [AlsoNotify("FullName")]
    private string _firstName = "";
    private int _age;
}
```

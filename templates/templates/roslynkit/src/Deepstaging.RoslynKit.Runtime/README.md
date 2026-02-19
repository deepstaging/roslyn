# Deepstaging.RoslynKit.Runtime

Runtime base classes that ship alongside the generated code. This project targets `net10.0` and is bundled into the NuGet package under `lib/net10.0/`.

## ObservableObject

Base class providing `INotifyPropertyChanged`, `OnPropertyChanged`, and `SetField<T>`. The generator emits classes that inherit from `ObservableObject` and call `SetField` in property setters — no inline INPC boilerplate needed.

```csharp
[AutoNotify]
public partial class PersonViewModel
{
    private string _name = "";
    private int _age;
}
```

The generator produces:

```csharp
public partial class PersonViewModel : ObservableObject
{
    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }
}
```

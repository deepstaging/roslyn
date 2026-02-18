# HintName

`HintName` provides consistent hint name generation for source generator output files. Hint names are used by Roslyn to uniquely identify generated files.

## Static Usage

The most common pattern — create a hint name from namespace and type name:

```csharp
HintName.From("MyApp.Models", "UserModel")
// → "MyApp.Models/UserModel.g.cs"

HintName.From("MyApp.Models", "UserModel", ".json")
// → "MyApp.Models/UserModel.g.json"
```

## Instance Usage

When generating multiple files with the same root:

```csharp
var hints = new HintName("MyApp.Models");

hints.Filename("UserModel")
// → "MyApp.Models/UserModel.g.cs"

hints.Filename("ViewModels", "UserViewModel")
// → "MyApp.Models.ViewModels/UserViewModel.g.cs"
```

## API

| Method | Returns | Description |
|--------|---------|-------------|
| `From(root, name, ext?)` | `string` | Static helper: `{root}/{name}.g{ext}` |
| `Filename(name)` | `string` | Instance: `{root}/{name}.g{ext}` |
| `Filename(append, name)` | `string` | Instance with sub-path: `{root}.{append}/{name}.g{ext}` |

The default extension is `.cs`.

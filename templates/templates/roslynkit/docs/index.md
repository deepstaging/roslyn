# Deepstaging.RoslynKit

A starter kit for building Roslyn source generators, analyzers, and code fixes with [Deepstaging.Roslyn](https://deepstaging.github.io/roslyn).

## What It Does

The `[AutoNotify]` attribute generates `INotifyPropertyChanged` implementations from private backing fields:

```csharp
[AutoNotify]
public partial class PersonViewModel
{
    [AlsoNotify("FullName")]
    private string _firstName = "";
    private string _lastName = "";
    private int _age;

    public string FullName => $"{FirstName} {LastName}".Trim();
}
```

The generator emits a partial class with:

- Public properties with equality-checked setters
- The `PropertyChanged` event and `OnPropertyChanged` helper
- Additional `OnPropertyChanged` calls for `[AlsoNotify]` targets

Analyzers enforce that `[AutoNotify]` classes are `partial` and fields are `private`, with one-click code fixes for both.

## Quick Links

- [Getting Started](getting-started.md) — Build, test, and extend the project
- [Architecture](guides/architecture.md) — How the Projection pattern works
- [Testing](guides/testing.md) — Write tests with RoslynTestBase
- [Packaging & Publishing](guides/packaging.md) — Ship to NuGet

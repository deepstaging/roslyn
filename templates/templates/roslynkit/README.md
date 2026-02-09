# Deepstaging.RoslynKit

A complete Roslyn project template demonstrating source generators, analyzers, and code fixes using [Deepstaging.Roslyn](https://github.com/deepstaging/roslyn).

## Quick Start

```bash
# Create a new project from this template
dotnet new roslynkit -n MyCompany.MyProject

# Build and test
cd MyCompany.MyProject
dotnet build
dotnet test
```

## What's Included

| Project | Description |
|---------|-------------|
| `*.RoslynKit` | Attribute library containing `[GenerateWith]`, `[AutoNotify]`, `[AlsoNotify]` |
| `*.RoslynKit.Projection` | Queries and models for extracting attribute data (shared across generators/analyzers) |
| `*.RoslynKit.Generators` | Source generators for `With*()` methods and `INotifyPropertyChanged` |
| `*.RoslynKit.Analyzers` | Analyzers that validate attribute usage |
| `*.RoslynKit.CodeFixes` | Code fixes for analyzer diagnostics |
| `*.RoslynKit.Tests` | Unit tests using TUnit and Deepstaging.Roslyn.Testing |

## Architecture: The Projection Pattern

This template demonstrates the **Projection Pattern** for Roslyn tools:

```
┌─────────────────────────────────────────────────────────────────┐
│                     RoslynKit (Attributes)                       │
│  [GenerateWith], [AutoNotify], [AlsoNotify]                     │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                  RoslynKit.Projection                            │
│  ┌─────────────────┐  ┌────────────────┐  ┌──────────────────┐  │
│  │ AttributeQuery  │  │    Queries     │  │     Models       │  │
│  │ - AutoNotify    │  │ extension      │  │ - AutoNotifyModel│  │
│  │ - AlsoNotify    │  │ methods on     │  │ - NotifyProperty │  │
│  │   AttributeQuery│  │ ValidSymbol<T> │  │   Model          │  │
│  └─────────────────┘  └────────────────┘  └──────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
           │                    │                    │
           ▼                    ▼                    ▼
    ┌──────────┐         ┌───────────┐        ┌──────────┐
    │Generators│         │ Analyzers │        │ CodeFixes│
    └──────────┘         └───────────┘        └──────────┘
```

**Benefits:**
- **Single source of truth** for attribute interpretation
- **Consistent validation** across generators and analyzers
- **Strongly-typed models** instead of raw Roslyn symbols
- **Testable projections** independent of code generation

## Example 1: `[GenerateWith]` (Simple)

```csharp
using Deepstaging.RoslynKit;

[GenerateWith]
public partial class Person
{
    public string Name { get; init; } = "";
    public int Age { get; init; }
    public string? Email { get; init; }
}
```

The source generator produces:

```csharp
public partial class Person
{
    public Person WithName(string value) => new Person
    {
        Name = value,
        Age = this.Age,
        Email = this.Email,
    };

    public Person WithAge(int value) => new Person
    {
        Name = this.Name,
        Age = value,
        Email = this.Email,
    };

    public Person WithEmail(string? value) => new Person
    {
        Name = this.Name,
        Age = this.Age,
        Email = value,
    };
}
```

Usage:

```csharp
var person = new Person { Name = "Alice", Age = 30 };
var updated = person.WithAge(31).WithEmail("alice@example.com");
```

## Example 2: `[AutoNotify]` (Complex with Projections)

This example demonstrates the full Projection pattern with queries and models.

```csharp
using Deepstaging.RoslynKit;

[AutoNotify]
public partial class PersonViewModel
{
    [AlsoNotify(nameof(FullName))]
    private string _firstName = "";

    [AlsoNotify(nameof(FullName))]
    private string _lastName = "";

    private int _age;

    public string FullName => $"{FirstName} {LastName}".Trim();
}
```

The source generator produces:

```csharp
public partial class PersonViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public string FirstName
    {
        get => _firstName;
        set
        {
            if (SetField(ref _firstName, value))
            {
                OnPropertyChanged(nameof(FullName));
            }
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            if (SetField(ref _lastName, value))
            {
                OnPropertyChanged(nameof(FullName));
            }
        }
    }

    public int Age
    {
        get => _age;
        set => SetField(ref _age, value);
    }
}
```

### How the Projection Works

The `AutoNotify.cs` projection builds a model from the symbol:

```csharp
// In Deepstaging.RoslynKit.Projection/AutoNotify.cs
public static AutoNotifyModel? QueryAutoNotify(this ValidSymbol<INamedTypeSymbol> symbol)
{
    var attributes = symbol.AutoNotifyAttributes();
    if (attributes.IsEmpty)
        return null;

    return new AutoNotifyModel
    {
        TypeName = symbol.Name,
        Properties = symbol.QueryNotifyProperties()  // Queries fields with naming convention
    };
}
```

This model is then used by:
- **Generator**: To emit properties with change notification
- **Analyzer**: To validate field accessibility
- **CodeFix**: To understand what needs fixing

## Analyzer Diagnostics

| ID | Severity | Description |
|----|----------|-------------|
| RK1001 | Error | Type with `[GenerateWith]` must be declared as `partial` |
| RK1002 | Error | Type with `[AutoNotify]` must be declared as `partial` |
| RK1003 | Warning | AutoNotify backing field should be `private` |

## Project Structure

```
├── src/
│   ├── Deepstaging.RoslynKit/            # Attributes
│   ├── Deepstaging.RoslynKit.Projection/ # Queries and Models (shared)
│   ├── Deepstaging.RoslynKit.Generators/ # Source generators
│   ├── Deepstaging.RoslynKit.Analyzers/  # Diagnostic analyzers
│   ├── Deepstaging.RoslynKit.CodeFixes/  # Code fix providers
│   └── Deepstaging.RoslynKit.Tests/      # Unit tests
├── samples/
│   └── RoslynKit.Sample/                 # Example usage
├── build/                                # Build configuration
└── Deepstaging.RoslynKit.slnx           # Solution file
```

## Building

### Prerequisites

For local development with Deepstaging packages, set the `DEEPSTAGING_PACKAGES_DIR` environment variable:

```bash
# Add to ~/.zshrc or ~/.bashrc
export DEEPSTAGING_PACKAGES_DIR="$HOME/org/deepstaging/artifacts/packages"
```

### Commands

```bash
# Build all projects
dotnet build

# Run tests
dotnet test

# Create NuGet packages
./build/pack.sh
```

## License

RPL-1.5 - This template uses [Deepstaging.Roslyn](https://github.com/deepstaging/roslyn) which is licensed under RPL-1.5.

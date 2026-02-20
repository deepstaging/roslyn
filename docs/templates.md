# Templates

Deepstaging.Roslyn ships project templates to scaffold complete Roslyn toolkits in seconds.

## Available Templates

| Template | Short Name | Description |
|----------|------------|-------------|
| Deepstaging Roslyn Kit | `roslynkit` | Source generator, analyzer, and code fix project |

## RoslynKit

A complete starter project with an `[AutoNotify]` example that generates `INotifyPropertyChanged` implementations from private backing fields.

### Install

```bash
dotnet new install Deepstaging.Roslyn.Templates
```

### Create a Project

```bash
# Standard — analyzers + source generator only
dotnet new roslynkit -n MyLib

# With runtime base classes (ObservableObject)
dotnet new roslynkit -n MyLib --includeRuntime
```

### What You Get

```
src/
├── MyLib/                  Attributes — [AutoNotify], [AlsoNotify]
├── MyLib.Projection/       Queries + Models — extract data from symbols
├── MyLib.Generators/       Source generator — emit code from models
├── MyLib.Analyzers/        Analyzers — validate attribute usage
├── MyLib.CodeFixes/        Code fixes — auto-fix diagnostics
└── MyLib.Runtime/          Base classes (--includeRuntime only)
test/
└── MyLib.Tests/            15 tests covering all of the above
build/                      MSBuild props, pack.sh, docs.sh
docs/                       MkDocs Material site
.github/                    CI workflows + Dependabot
```

### Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `--name` / `-n` | string | required | Root namespace and project name |
| `--includeRuntime` | bool | `false` | Add a Runtime project with `ObservableObject` base class |

### With vs Without Runtime

**Without `--includeRuntime`** (default):

The generator emits the full `INotifyPropertyChanged` implementation inline — the event, `OnPropertyChanged` method, and a private `SetProperty<T>` helper for equality-checked setters. No runtime dependency.

```csharp
// Generated output (simplified)
public partial class Person : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name
    {
        get => _name;
        set { SetProperty(ref _name, value); }
    }

    private bool SetProperty<T>(ref T field, T value, ...) { ... }
    protected void OnPropertyChanged(...) { ... }
}
```

**With `--includeRuntime`**:

The generator emits a class that inherits from `ObservableObject` and delegates to `SetField`. Much cleaner output — the base class handles all INPC boilerplate.

```csharp
// Generated output (simplified)
public partial class Person : ObservableObject
{
    public string Name
    {
        get => _name;
        set { SetField(ref _name, value); }
    }
}
```

The Runtime project ships inside the NuGet package under `lib/net10.0/`, so consumers get it automatically.

### Packaging

The template uses single-package bundling — one NuGet package contains everything:

- Attributes → `lib/netstandard2.0/`
- Generator + Projection → `analyzers/dotnet/cs/`
- Analyzers + CodeFixes → `analyzers/dotnet/cs/`
- Runtime (if included) → `lib/net10.0/`
- Projection (satellite) → `satellite/netstandard2.0/`
- Build props → `build/`

```bash
# Pack with dev version suffix
./build/pack.sh

# Pack for release
./build/pack.sh --no-version-suffix
```

The `satellite/` folder enables downstream packages to reference your Projection layer for building generators that extend your attributes. See [Satellite Projection](guides/project-organization.md#satellite-projection) for details.

### Documentation Site

Each scaffolded project includes a MkDocs Material site under `docs/` and a GitHub Actions workflow to deploy to GitHub Pages.

```bash
# Preview locally
./build/docs.sh serve

# Build static site
./build/docs.sh build
```

To enable GitHub Pages deployment, go to repo **Settings → Pages** and set **Source** to **GitHub Actions**.

### CI/CD

The template includes three GitHub Actions workflows:

| Workflow | Trigger | Purpose |
|----------|---------|---------|
| `build.yml` | Push, PR | Build + test |
| `publish.yml` | Manual dispatch | Pack + publish to NuGet |
| `docs.yml` | Push to main (docs changes) | Deploy docs to GitHub Pages |

Dependabot is configured for NuGet, GitHub Actions, and pip (docs) dependencies.

# Deepstaging Templates

Project templates for building .NET applications using the Deepstaging ecosystem.

## Installation

```bash
# Install from local source
dotnet new install .

# Or install from NuGet (when published)
dotnet new install Deepstaging.Templates
```

## Available Templates

| Template | Short Name | Description |
|----------|------------|-------------|
| [Deepstaging Roslyn Kit](templates/roslynkit/) | `roslynkit` | Complete Roslyn project with source generator, analyzer, and codefix |

## Usage

```bash
# Create a new Roslyn project
dotnet new roslynkit -n MyProject

# See template options
dotnet new roslynkit --help
```

## Template Details

### roslynkit - Roslyn Project Template

A complete Roslyn project demonstrating:
- **Source Generator** - `[GenerateWith]` → immutable `With*()` methods
- **Analyzer** - RK1001: Type must be `partial`
- **Code Fix** - Adds `partial` modifier

See [templates/roslynkit/README.md](templates/roslynkit/README.md) for full documentation.

## License

RPL-1.5 - See [LICENSE](LICENSE) for details.

All templates depend on [Deepstaging.Roslyn](https://github.com/deepstaging/roslyn) which is also licensed under RPL-1.5.

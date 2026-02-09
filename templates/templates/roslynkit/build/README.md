# Build Configuration

This directory contains modular MSBuild configuration files imported by `Directory.Build.props`.

## Files

| File | Purpose |
|------|---------|
| `Build.Paths.props` | Controls where build outputs go (bin, obj, packages) |
| `Build.Language.props` | C# language settings (nullable, implicit usings, etc.) |
| `Build.Packaging.props` | NuGet package metadata and versioning |
| `Build.SourceLink.props` | SourceLink and symbol configuration |
| `Build.GitHooks.props` | Git hooks via Husky.Net (optional) |

## Scripts

| Script | Purpose |
|--------|---------|
| `pack.sh` | Build and create NuGet packages |

## Usage

```bash
# Pack with dev version suffix
./build/pack.sh

# Pack for release (no suffix)
./build/pack.sh --no-version-suffix

# See all options
./build/pack.sh --help
```

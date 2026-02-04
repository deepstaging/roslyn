# Build Configuration

> **See also:** [Main README](../README.md) | [Roslyn Toolkit](../roslyn/Deepstaging.Roslyn/README.md) | [Testing](../roslyn/Deepstaging.Roslyn.Testing/README.md)

This directory contains modular MSBuild configuration files that are imported by `Directory.Build.props`.

## Files

### Build.Paths.props
Controls build output locations:
- `ArtifactsDir` - Root directory for all build outputs
- `BaseOutputPath` - Compiled binaries (.dll, .exe)
- `BaseIntermediateOutputPath` - Temporary build files (.obj)
- `PackageOutputPath` - NuGet packages (.nupkg)

**When to edit:** Changing where build outputs go

### Build.Language.props
C# language and code quality settings:
- `LangVersion` - C# language version
- `Nullable` - Nullable reference types
- `ImplicitUsings` - Auto-import common namespaces
- `TreatWarningsAsErrors` - Build fails on warnings

**When to edit:** Adjusting language features or strictness

### Build.Packaging.props
NuGet package metadata and versioning:
- Package metadata (Authors, Company, Description, etc.)
- License and repository URLs
- Package tags and assets (README, icon)
- Version prefix

**When to edit:** Customizing package information before publishing

### Build.SourceLink.props
Debugging and documentation:
- XML documentation generation
- SourceLink integration (source code debugging)
- Symbol package configuration (.snupkg)

**When to edit:** Rarely - defaults work for most projects

## Import Order

Files are imported in this order by `Directory.Build.props`:
1. Build.Paths.props
2. Build.Language.props
3. Build.Packaging.props
4. Build.SourceLink.props
5. Directory.Build.Dev.props (if exists, Debug only)

## Adding New Configuration

To add a new category:
1. Create `Build.YourCategory.props`
2. Add `<Import Project="$(MSBuildThisFileDirectory)build/Build.YourCategory.props" />`
3. Document it in this README

## Learn More

- [Customize your build](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory)
- [Common MSBuild properties](https://learn.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-properties)

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](../LICENSE) for the full legal text.

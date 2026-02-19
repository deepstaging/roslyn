# Deepstaging.RoslynKit

A starter kit for building Roslyn source generators, analyzers, and code fixes with [Deepstaging.Roslyn](https://github.com/deepstaging/roslyn).

## Philosophy

Every Roslyn tool does three things: **query** symbols, **validate** them, and **emit** code or diagnostics. This project separates those concerns into distinct projects so each piece is testable and reusable.

The **Projection** layer is the key insight — it extracts strongly-typed models from Roslyn symbols once, then both the generator and analyzer consume the same models. No duplicated symbol-walking logic.

## Project Map

```
src/
├── RoslynKit/              Attributes — what consumers put on their code
├── RoslynKit.Projection/   Queries + Models — extract data from symbols
├── RoslynKit.Generators/   Source generator — emit code from models
├── RoslynKit.Analyzers/    Analyzers — validate symbols, report diagnostics
├── RoslynKit.CodeFixes/    Code fixes — auto-fix analyzer diagnostics
<!--#if (includeRuntime) -->
└── RoslynKit.Runtime/      Base classes (ObservableObject) for generated code
<!--#else -->
└── (optional: RoslynKit.Runtime — add with --includeRuntime)
<!--#endif -->
test/
└── RoslynKit.Tests/        Tests for all of the above
```


## What It Does

The `[AutoNotify]` attribute generates `INotifyPropertyChanged` implementations from private backing fields:

```csharp
[AutoNotify]
public partial class Person
{
    [AlsoNotify("FullName")]
    private string _firstName = "";
    private string _lastName = "";
    private int _age;
}
```

<!--#if (includeRuntime) -->
The generator emits a partial class that inherits from `ObservableObject` and uses `SetField` for equality-checked property setters. The `[AlsoNotify]` attribute teaches the generator to raise additional property notifications.
<!--#else -->
The generator emits a partial class with public properties, `INotifyPropertyChanged` implementation, and a `SetProperty<T>` helper for equality-checked setters. The `[AlsoNotify]` attribute teaches the generator to raise additional property notifications.
<!--#endif -->

Analyzers enforce that `[AutoNotify]` classes are `partial` (RK001) and fields are `private` (RK002), with one-click code fixes for both.

## Build & Test

```bash
dotnet build
dotnet run --project test/Deepstaging.RoslynKit.Tests -c Release
```

## Packaging

<!--#if (includeRuntime) -->
This project produces a single NuGet package that bundles everything — attributes, source generator, analyzers, code fixes, and the runtime library (`ObservableObject`). Consumers only need one reference:
<!--#else -->
This project produces a single NuGet package that bundles everything — attributes, source generator, analyzers, and code fixes. Consumers only need one reference:
<!--#endif -->

```xml
<PackageReference Include="Deepstaging.RoslynKit" Version="1.0.0" />
```

```bash
# Pack with a dev version suffix (default: dev.<commit-count>)
./build/pack.sh

# Pack a release version (no suffix)
./build/pack.sh --no-version-suffix

# Custom suffix
./build/pack.sh --version-suffix beta.1
```

Packages are written to `artifacts/packages/`.

## Local Development Loop

Tests cover correctness, but sometimes you need to see your generator/analyzer running in a real project. The fastest iteration loop:

### 1. Pack locally

```bash
./build/pack.sh
```

This produces a package with a `dev.<N>` version suffix in `artifacts/packages/`.

### 2. Add a local NuGet source

In your consumer project's `NuGet.Config`, add a local source pointing to the packages directory:

```xml
<packageSources>
  <add key="local" value="/path/to/roslynkit/artifacts/packages" />
  <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
</packageSources>
```

### 3. Reference the dev package

```xml
<PackageReference Include="Deepstaging.RoslynKit" Version="1.0.0-dev.42" />
```

### 4. Iterate

After making changes, re-pack and restore:

```bash
./build/pack.sh && dotnet restore --no-cache && dotnet build
```

The `--no-cache` flag is critical — NuGet caches packages by version, so without it you'll keep getting the stale build.

> **Tip:** Bump the version suffix each iteration (`--version-suffix dev.43`) to avoid cache issues entirely.

## CI/CD

**Build & Test** runs on every push and PR automatically.

**Documentation** deploys to GitHub Pages when docs change on `main`. To enable, go to repo **Settings → Pages** and set **Source** to **GitHub Actions**.

**Dependabot** keeps NuGet packages, GitHub Actions, and docs dependencies up to date weekly. See `.github/dependabot.yml`.

```bash
# Preview docs locally (creates .venv automatically)
./build/docs.sh serve

# Build static site
./build/docs.sh build
```

**Publish to NuGet** is available via manual dispatch. To enable automated publishing:

1. Configure [Trusted Publishing](https://devblogs.microsoft.com/nuget/introducing-trusted-publishers/) at nuget.org
2. Add your repo under **Manage Packages → Manage GitHub Repositories**
3. Set `NUGET_USER` in GitHub repo **Settings → Variables → Actions**
4. Edit `.github/workflows/publish.yml` to trigger on tags or pushes

See the workflow file for detailed instructions.

## License

RPL-1.5 — This template uses [Deepstaging.Roslyn](https://github.com/deepstaging/roslyn) which is licensed under RPL-1.5.

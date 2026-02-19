# Packaging & Publishing

## Single Package

This project produces one NuGet package — `Deepstaging.RoslynKit` — that bundles everything:

- Attributes (`lib/netstandard2.0/`)
- Source generator (`analyzers/dotnet/cs/`)
- Analyzers (`analyzers/dotnet/cs/`)
- Code fixes (`analyzers/dotnet/cs/`)
- Projection and Deepstaging.Roslyn dependencies (`analyzers/dotnet/cs/`)

Consumers only need:

```xml
<PackageReference Include="Deepstaging.RoslynKit" Version="1.0.0" />
```

## Packing

```bash
# Dev build (version suffix from git commit count)
./build/pack.sh

# Release build (no suffix)
./build/pack.sh --no-version-suffix

# Custom suffix
./build/pack.sh --version-suffix beta.1
```

Output goes to `artifacts/packages/`.

## Local Development Loop

1. **Pack locally:** `./build/pack.sh`
2. **Add a local NuGet source** in your consumer project's `NuGet.Config`:
   ```xml
   <packageSources>
     <add key="local" value="/path/to/roslynkit/artifacts/packages" />
     <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
   </packageSources>
   ```
3. **Reference the dev package:**
   ```xml
   <PackageReference Include="Deepstaging.RoslynKit" Version="1.0.0-dev.42" />
   ```
4. **Iterate:** `./build/pack.sh && dotnet restore --no-cache && dotnet build`

!!! tip
    Bump the version suffix each iteration (`--version-suffix dev.43`) to avoid NuGet cache issues entirely.

## CI/CD

### Build & Test

Runs on every push and PR to `main`. See `.github/workflows/build.yml`.

### Publish to NuGet

Available via manual workflow dispatch. To enable:

1. Configure [Trusted Publishing](https://devblogs.microsoft.com/nuget/introducing-trusted-publishers/) at nuget.org
2. Add your repo under **Manage Packages → Manage GitHub Repositories**
3. Set `NUGET_USER` in GitHub repo **Settings → Variables → Actions**
4. Edit `.github/workflows/publish.yml` to trigger on tags or pushes

### Documentation Site

Deploys to GitHub Pages automatically when docs change on `main`. To enable:

1. Go to your repo **Settings → Pages**
2. Set **Source** to **GitHub Actions**
3. Push a change to `docs/` — the workflow handles the rest

The workflow is in `.github/workflows/docs.yml`.

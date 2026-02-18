# BuildPropertyExtensions

Extension methods on `AnalyzerConfigOptions` for reading MSBuild build properties. Handles the `build_property.` prefix automatically.

## Setup

MSBuild properties are exposed to analyzers and generators via `<CompilerVisibleProperty>` in a `.props` file:

```xml
<ItemGroup>
    <CompilerVisibleProperty Include="DeepstagingDataDirectory"/>
    <CompilerVisibleProperty Include="DeepstagingHasReadme"/>
</ItemGroup>
```

These appear in `AnalyzerConfigOptions.GlobalOptions` with a `build_property.` prefix. The extensions strip this prefix so you work with clean property names.

## API

### GetBuildProperty (string)

```csharp
var globalOptions = context.Options.AnalyzerConfigOptionsProvider.GlobalOptions;

// Returns ".config" if the property is set, "default" otherwise
var dataDir = globalOptions.GetBuildProperty("DeepstagingDataDirectory", "default");
```

Returns the fallback if the property is missing or empty.

### GetBuildProperty&lt;T&gt; (typed)

```csharp
// Parses as bool — returns false if missing, empty, or unparseable
var isDirty = globalOptions.GetBuildProperty("IsDirty", false);

// Parses as int
var count = globalOptions.GetBuildProperty("DirtyCount", 0);

// Also supports long and double
var bigNum = globalOptions.GetBuildProperty("BigNumber", 0L);
var ratio = globalOptions.GetBuildProperty("Ratio", 0.0);
```

Supported types: `bool`, `int`, `long`, `double`.

### TryGetBuildProperty

```csharp
if (globalOptions.TryGetBuildProperty("MyProperty", out var value))
{
    // value is non-null and non-empty
}
```

### DiscoverBuildProperties

Collects all properties matching a prefix into an `ImmutableDictionary<string, string?>`. Useful for forwarding to code fixes via `Diagnostic.Properties`:

```csharp
// All Deepstaging* properties (default prefix)
var props = globalOptions.DiscoverBuildProperties();

// Custom prefix
var gitProps = globalOptions.DiscoverBuildProperties("_DeepstagingGit");

// Forward to code fix
context.ReportDiagnostic(Diagnostic.Create(rule, location, props, args));
```

The dictionary keys have the `build_property.` prefix stripped. A property declared as `<CompilerVisibleProperty Include="DeepstagingDataDirectory"/>` appears with key `"DeepstagingDataDirectory"`.

## Method Reference

| Method | Returns | Description |
|--------|---------|-------------|
| `GetBuildProperty(name, fallback)` | `string` | String value with fallback |
| `GetBuildProperty<T>(name, fallback)` | `T` | Typed value (`bool`, `int`, `long`, `double`) |
| `TryGetBuildProperty(name, out value)` | `bool` | `false` if missing or empty |
| `DiscoverBuildProperties(prefix?)` | `ImmutableDictionary<string, string?>` | All matching properties (default: `"Deepstaging"`) |

## Usage Contexts

### In Analyzers

```csharp
protected override void Analyze(
    CompilationAnalysisContext context, ImmutableArray<RequiredEnvVar> items)
{
    var globalOptions = context.Options.AnalyzerConfigOptionsProvider.GlobalOptions;

    foreach (var required in items)
    {
        if (globalOptions.TryGetBuildProperty($"Env_{required.Name}", out _))
            continue;

        context.ReportDiagnostic(Diagnostic.Create(
            GetRule(0), required.Location, required.Name));
    }
}
```

### In Generators

```csharp
var gitState = context.AnalyzerConfigOptionsProvider
    .Select(static (provider, _) =>
    {
        var g = provider.GlobalOptions;
        return new GitState(
            Branch: g.GetBuildProperty("GitBranch", "unknown"),
            IsDirty: g.GetBuildProperty("GitIsDirty", false),
            DirtyCount: g.GetBuildProperty("GitDirtyCount", 0)
        );
    });
```

### In Code Fixes

Properties forwarded via `Diagnostic.Properties` arrive without the `build_property.` prefix:

```csharp
diagnostic.Properties.TryGetValue("DeepstagingDataDirectory", out var dataDir);
dataDir ??= ".config";
```

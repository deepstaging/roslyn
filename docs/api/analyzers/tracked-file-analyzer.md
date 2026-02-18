# TrackedFileTypeAnalyzer

`TrackedFileTypeAnalyzer` is the base class for analyzers that detect **missing or stale files** associated with type symbols. It reports paired "missing" and "stale" diagnostics sharing the same diagnostic ID, so a single code fix can handle either case.

## Quick Start

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[TracksFiles("SCHEMA001",
    MissingTitle = "Schema file should be generated",
    MissingMessage = "Type '{0}' is missing its schema file",
    StaleTitle = "Schema file is out of date",
    StaleMessage = "Schema file for '{0}' is stale",
    Category = "Schema")]
public sealed class SchemaFileAnalyzer : TrackedFileTypeAnalyzer
{
    protected override bool IsTrackedFile(string filePath) =>
        filePath.EndsWith(".schema.json");

    protected override bool IsRelevant(ValidSymbol<INamedTypeSymbol> symbol) =>
        symbol.HasAttribute<GenerateSchemaAttribute>();

    protected override string ComputeHash(ValidSymbol<INamedTypeSymbol> symbol) =>
        SchemaHasher.Compute(symbol);

    protected override string? ExtractHash(string fileContent) =>
        SchemaHasher.Extract(fileContent);

    protected override IEnumerable<string> GetExpectedFileNames(
        ValidSymbol<INamedTypeSymbol> symbol)
    {
        yield return $"{symbol.Name}.schema.json";
    }
}
```

## How It Works

1. On compilation start, scans `AdditionalTexts` for tracked files using `IsTrackedFile`
2. Collects all `Deepstaging*` build properties via `DiscoverBuildProperties()` for code fix forwarding
3. For each type symbol, checks if it `IsRelevant`
4. If **any** expected file is missing → reports the "missing" diagnostic
5. If all files are present but any has a **stale hash** → reports the "stale" diagnostic

Both diagnostics share the same diagnostic ID so a single code fix can handle either case.

## Abstract Members

| Member | Description |
|--------|-------------|
| `IsTrackedFile(filePath)` | Whether a file path from `AdditionalTexts` is a tracked file |
| `IsRelevant(symbol)` | Whether a type symbol should be analyzed |
| `ComputeHash(symbol)` | Compute the current hash for a symbol's model |
| `ExtractHash(fileContent)` | Extract an embedded hash from existing file content |
| `GetExpectedFileNames(symbol)` | File names that should exist for the symbol |

## Virtual Members

| Member | Default | Description |
|--------|---------|-------------|
| `GetMessageArgs(symbol)` | `[symbol.Name]` | Message format arguments |
| `GetLocation(symbol)` | `symbol.Location` | Diagnostic location |

## Properties

| Property | Type | Description |
|----------|------|-------------|
| `MissingRule` | `DiagnosticDescriptor` | The "missing files" descriptor |
| `StaleRule` | `DiagnosticDescriptor` | The "stale files" descriptor |

## TracksFilesAttribute

Declarative configuration for the paired diagnostic descriptors:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DiagnosticId` | `string` | *(required)* | ID shared by both rules |
| `MissingTitle` | `string` | `"Files should be generated"` | Title for missing diagnostic |
| `MissingMessage` | `string` | `"{0}"` | Message format (`{0}` = symbol name) |
| `MissingSeverity` | `DiagnosticSeverity` | `Info` | Severity for missing diagnostic |
| `MissingDescription` | `string` | `""` | Description for missing diagnostic |
| `StaleTitle` | `string` | `"Files are out of date"` | Title for stale diagnostic |
| `StaleMessage` | `string` | `"{0}"` | Message format (`{0}` = symbol name) |
| `StaleSeverity` | `DiagnosticSeverity` | `Warning` | Severity for stale diagnostic |
| `StaleDescription` | `string` | `""` | Description for stale diagnostic |
| `Category` | `string` | `"Usage"` | Diagnostic category |
| `IsEnabledByDefault` | `bool` | `true` | Whether both diagnostics are enabled |

## Build Property Forwarding

`TrackedFileTypeAnalyzer` automatically discovers all `Deepstaging*` build properties via [`DiscoverBuildProperties()`](build-property-extensions.md) and includes them in every reported diagnostic's `Properties` dictionary. Code fixes receive these properties without any additional wiring:

```csharp
// In your code fix — properties arrive automatically
diagnostic.Properties.TryGetValue("DeepstagingDataDirectory", out var dataDir);
```

To expose a new property, add it to your NuGet `.props` file:

```xml
<CompilerVisibleProperty Include="DeepstagingMyNewProperty"/>
```

## TrackedFiles

Helper class that discovers and indexes additional files with embedded hashes:

```csharp
var files = TrackedFiles.Discover(additionalTexts, isTracked, extractHash);

files.HasAny               // bool — any tracked files found?
files.HasFile("name.json") // bool — specific file exists?
files.GetHash("name.json") // string? — embedded hash value
```

| Method | Returns | Description |
|--------|---------|-------------|
| `Discover(texts, isTracked, extractHash)` | `TrackedFiles` | Scan additional texts |
| `HasAny` | `bool` | Whether any tracked files were found |
| `HasFile(fileName)` | `bool` | Whether a specific file exists |
| `GetHash(fileName)` | `string?` | Get embedded hash for a file |

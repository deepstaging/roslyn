# ReportsAttribute

`[Reports]` is the declarative attribute for configuring diagnostic descriptors on analyzer base classes. It eliminates the need for manual `DiagnosticDescriptor` field declarations.

## Usage

```csharp
[Reports("RK1002", "Type must be partial",
    Description = "Source generators require partial types.",
    Message = "Type '{0}' must be declared as partial",
    Category = "Usage",
    Severity = DiagnosticSeverity.Error)]
public sealed class MustBePartialAnalyzer : TypeAnalyzer
{
    // ...
}
```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DiagnosticId` | `string` | *(required)* | The diagnostic ID (e.g., `"DS0001"`) |
| `Title` | `string` | *(required)* | The diagnostic title |
| `Message` | `string` | `"{0}"` | Message format with placeholders (`{0}` = symbol name by default) |
| `Description` | `string` | `""` | Detailed description shown in IDE |
| `Category` | `string` | `"Usage"` | Diagnostic category |
| `Severity` | `DiagnosticSeverity` | `Error` | Default severity (`Error`, `Warning`, `Info`, `Hidden`) |
| `IsEnabledByDefault` | `bool` | `true` | Whether the diagnostic is enabled by default |

## AllowMultiple

`[Reports]` supports `AllowMultiple = true`, enabling multi-diagnostic analyzers:

```csharp
[Reports("DSDT001", "Required env var not set",
    Severity = DiagnosticSeverity.Warning)]
[Reports("DSDT002", "Optional env var not set",
    Severity = DiagnosticSeverity.Info)]
public sealed class EnvAnalyzer : AssemblyAttributeAnalyzer<EnvVar>
{
    // GetRule(0) → DSDT001
    // GetRule(1) → DSDT002
}
```

Single-`[Reports]` base classes (`SymbolAnalyzer`, `MultiDiagnosticSymbolAnalyzer`) use `GetCustomAttribute<ReportsAttribute>()` (singular), so they always pick the first one. Multi-diagnostic base classes (`AssemblyAttributeAnalyzer`) use `GetCustomAttributes<ReportsAttribute>()` (plural) to support multiple.

## Supported Base Classes

| Base Class | [Reports] Count | Access Pattern |
|------------|-----------------|----------------|
| `SymbolAnalyzer<T>` | Exactly 1 | `Rule` property |
| `MultiDiagnosticSymbolAnalyzer<S,I>` | Exactly 1 | `Rule` property |
| `AssemblyAttributeAnalyzer<T>` | 1 or more | `GetRule(index)` |

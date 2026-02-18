# AnalyzerTestContext

Test Roslyn analyzers with fluent diagnostic assertions.

## Creating

```csharp
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldReportDiagnostic("MY001");
```

## Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `WithAdditionalText(path, content)` | `AnalyzerTestContext` | Add an additional text file |
| `ShouldReportDiagnostic(id)` | `DiagnosticAssertion` | Assert specific diagnostic reported |
| `ShouldNotReportDiagnostic(id)` | `Task` | Assert diagnostic NOT reported |
| `ShouldHaveDiagnostics()` | `DiagnosticsAssertion` | Assert any diagnostics present |
| `ShouldHaveNoDiagnostics()` | `Task` | Assert no diagnostics at all |
| `GetDiagnosticsAsync()` | `Task<Diagnostic[]>` | Get raw diagnostics |

## DiagnosticAssertion

Fluent assertions for a specific diagnostic ID:

```csharp
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldReportDiagnostic("MY001")
    .WithSeverity(DiagnosticSeverity.Error)
    .WithMessage("*must be partial*");
```

| Method | Returns | Description |
|--------|---------|-------------|
| `WithSeverity(severity)` | `DiagnosticAssertion` | Assert severity level |
| `WithMessage(pattern)` | `DiagnosticAssertion` | Assert message matches pattern (supports `*` wildcard) |

## DiagnosticsAssertion

Fluent assertions without specifying a diagnostic ID upfront:

```csharp
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldHaveDiagnostics()
    .WithErrorCode("MY001")
    .WithSeverity(DiagnosticSeverity.Warning)
    .WithMessage("*deprecated*");
```

| Method | Returns | Description |
|--------|---------|-------------|
| `WithErrorCode(id)` | `DiagnosticsAssertion` | Filter to specific diagnostic ID |
| `WithSeverity(severity)` | `DiagnosticsAssertion` | Assert severity level |
| `WithMessage(pattern)` | `DiagnosticsAssertion` | Assert message matches pattern |

## Additional Text

For analyzers that inspect `AdditionalTexts` (e.g., `TrackedFileTypeAnalyzer`):

```csharp
await AnalyzeWith<MyFileAnalyzer>(source)
    .WithAdditionalText("Templates/MyTemplate.scriban-cs", templateContent)
    .ShouldHaveNoDiagnostics();
```

## Example

```csharp
[Test]
public async Task Reports_non_partial_type()
{
    const string source = """
        [AutoNotify]
        public class Customer { }
        """;

    await AnalyzeWith<MustBePartialAnalyzer>(source)
        .ShouldReportDiagnostic("RK1002")
        .WithSeverity(DiagnosticSeverity.Error)
        .WithMessage("*must be partial*");
}

[Test]
public async Task Does_not_report_partial_type()
{
    const string source = """
        [AutoNotify]
        public partial class Customer { }
        """;

    await AnalyzeWith<MustBePartialAnalyzer>(source)
        .ShouldHaveNoDiagnostics();
}
```

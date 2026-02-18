# RoslynTestBase

`RoslynTestBase` is the abstract base class for all Roslyn component tests. It provides unified entry points for testing symbols, analyzers, generators, code fixes, and templates.

## Entry Points

| Method | Returns | Description |
|--------|---------|-------------|
| `SymbolsFor(source)` | `SymbolTestContext` | Compile source and query symbols |
| `CompilationFor(source)` | `Compilation` | Get the raw compilation |
| `AnalyzeWith<T>(source)` | `AnalyzerTestContext` | Run analyzer against source |
| `GenerateWith<T>(source)` | `GeneratorTestContext` | Run generator against source |
| `FixWith<T>(source)` | `CodeFixTestContext` | Test code fix for compiler diagnostics |
| `AnalyzeAndFixWith<TA, TF>(source)` | `CodeFixTestContext` | Test analyzer + code fix together |
| `RenderTemplateFrom<T>(source)` | `TemplateTestContext` | Test Scriban template rendering |

## Properties

| Property | Type | Description |
|----------|------|-------------|
| `WellKnownSymbols` | `CommonCompilationSymbols.WellKnownSymbols` | Pre-resolved common types (Guid, String, etc.) |

## Usage

```csharp
public class MyAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task Reports_error_for_non_partial_type()
    {
        const string source = """
            [AutoNotify]
            public class Customer { }
            """;

        await AnalyzeWith<MustBePartialAnalyzer>(source)
            .ShouldReportDiagnostic("RK1002")
            .WithSeverity(DiagnosticSeverity.Error);
    }

    [Test]
    public async Task Generates_expected_output()
    {
        const string source = """
            [AutoNotify]
            public partial class Customer
            {
                [AutoNotify] private string _name;
            }
            """;

        await GenerateWith<AutoNotifyGenerator>(source)
            .ShouldGenerate()
            .WithFileNamed("Customer.g.cs")
            .WithNoDiagnostics()
            .VerifySnapshot();
    }
}
```

## Reference Configuration

If your tests reference types from your own assemblies, configure once via `ModuleInitializer`:

```csharp
internal static class TestInit
{
    [ModuleInitializer]
    public static void Init() =>
        ReferenceConfiguration.AddReferencesFromTypes(
            typeof(AutoNotifyAttribute));
}
```

See [ReferenceConfiguration](reference-configuration.md) for details.

## Detailed Context Documentation

Each entry point returns a context with its own fluent API:

- [SymbolTestContext](symbol-test-context.md) — Query symbols from compiled source
- [AnalyzerTestContext](analyzer-test-context.md) — Assert on analyzer diagnostics
- [GeneratorTestContext](generator-test-context.md) — Assert on generator output
- [CodeFixTestContext](codefix-test-context.md) — Assert on code fix transformations
- [TemplateTestContext](template-test-context.md) — Assert on template rendering

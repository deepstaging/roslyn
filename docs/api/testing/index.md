# Testing

Test utilities for Roslyn analyzers, generators, and code fixes.

## Installation

```bash
dotnet add package Deepstaging.Roslyn.Testing --prerelease
```

## Architecture

```
RoslynTestBase
  │
  ├── SymbolsFor(source)           → SymbolTestContext
  │     ├── RequireNamedType()       (ValidSymbol<INamedTypeSymbol>)
  │     ├── Type("Foo").Methods()    (fluent queries)
  │     └── GetProperty()            (OptionalSymbol<IPropertySymbol>)
  │
  ├── AnalyzeWith<T>(source)       → AnalyzerTestContext
  │     ├── ShouldReportDiagnostic()
  │     └── ShouldHaveNoDiagnostics()
  │
  ├── GenerateWith<T>(source)      → GeneratorTestContext
  │     ├── ShouldGenerate()
  │     ├── WithFileNamed()
  │     └── VerifySnapshot()
  │
  ├── FixWith<T>(source)           → CodeFixTestContext
  ├── AnalyzeAndFixWith<TA,TF>()   → CodeFixTestContext
  │     ├── ShouldProduce()
  │     └── ShouldAddAdditionalDocument()
  │
  └── RenderTemplateFrom<T>()     → TemplateTestContext
        ├── Render()
        └── VerifySnapshot()
```

## Quick Start

All tests inherit from `RoslynTestBase`:

```csharp
public class MyTests : RoslynTestBase
{
    [Test]
    public async Task Analyzer_reports_diagnostic()
    {
        await AnalyzeWith<MyAnalyzer>(source)
            .ShouldReportDiagnostic("MY001")
            .WithSeverity(DiagnosticSeverity.Error);
    }

    [Test]
    public async Task Generator_produces_output()
    {
        await GenerateWith<MyGenerator>(source)
            .ShouldGenerate()
            .WithFileNamed("Customer.g.cs")
            .WithNoDiagnostics()
            .VerifySnapshot();
    }

    [Test]
    public async Task CodeFix_transforms_source()
    {
        await AnalyzeAndFixWith<MyAnalyzer, MyCodeFix>(source)
            .ForDiagnostic("MY001")
            .ShouldProduce(expectedSource);
    }
}
```

### Reference Configuration

If tests need your own assemblies, configure once via `ModuleInitializer`:

```csharp
[ModuleInitializer]
public static void Init() =>
    ReferenceConfiguration.AddReferencesFromTypes(typeof(MyAttribute));
```

## API Reference

| Type | Description |
|------|-------------|
| [RoslynTestBase](roslyn-test-base.md) | Entry points for all test types |
| [SymbolTestContext](symbol-test-context.md) | Compile source and query symbols |
| [AnalyzerTestContext](analyzer-test-context.md) | Assert on analyzer diagnostics |
| [GeneratorTestContext](generator-test-context.md) | Assert on generator output with snapshots |
| [CodeFixTestContext](codefix-test-context.md) | Assert on code fix transformations |
| [TemplateTestContext](template-test-context.md) | Assert on template rendering |
| [Assertions](assertions.md) | TUnit assertion extensions for symbols, emit, and compilations |
| [ReferenceConfiguration](reference-configuration.md) | Configure assembly references for compilations |

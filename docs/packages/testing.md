# Deepstaging.Roslyn.Testing

Test utilities for Roslyn analyzers, generators, and code fixes.

## Installation

```bash
dotnet add package Deepstaging.Roslyn.Testing
```

## Quick Start

All tests inherit from `RoslynTestBase`:

```csharp
public class MyTests : RoslynTestBase
{
    [Test]
    public async Task TestSymbols()
    {
        var type = SymbolsFor("public class Foo { }").RequireNamedType("Foo");
        await Assert.That(type.Value.Name).IsEqualTo("Foo");
    }

    [Test]
    public async Task TestAnalyzer()
    {
        await AnalyzeWith<MyAnalyzer>(source)
            .ShouldReportDiagnostic("MY001");
    }

    [Test]
    public async Task TestGenerator()
    {
        await GenerateWith<MyGenerator>(source)
            .ShouldGenerate()
            .VerifySnapshot();
    }

    [Test]
    public async Task TestCodeFix()
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

## Entry Points

| Method | Description |
|--------|-------------|
| `SymbolsFor(source)` | Create compilation and query symbols |
| `CompilationFor(source)` | Get the raw compilation |
| `AnalyzeWith<T>(source)` | Run analyzer and assert diagnostics |
| `GenerateWith<T>(source)` | Run generator and assert output |
| `FixWith<T>(source)` | Test code fix for compiler diagnostics |
| `AnalyzeAndFixWith<TAnalyzer, TCodeFix>(source)` | Test code fix for analyzer diagnostics |
| `RenderTemplateFrom<T>(source)` | Test Scriban template rendering |

## Test Contexts

Each entry point returns a test context with fluent assertions:

- **[SymbolTestContext](../testing/symbol-test-context.md)** — Query symbols from compiled source
- **[AnalyzerTestContext](../testing/analyzer-test-context.md)** — Assert on analyzer diagnostics
- **[GeneratorTestContext](../testing/generator-test-context.md)** — Assert on generator output
- **[CodeFixTestContext](../testing/codefix-test-context.md)** — Assert on code fix transformations
- **[TemplateTestContext](../testing/template-test-context.md)** — Assert on template rendering

## Quick Examples

### Symbol Testing

```csharp
var ctx = SymbolsFor(source);

// Get types and members
var type = ctx.RequireNamedType("Customer");
var methods = ctx.Type("Customer").Methods().ThatArePublic().GetAll();
```

### Analyzer Testing

```csharp
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldReportDiagnostic("MY001")
    .WithSeverity(DiagnosticSeverity.Error)
    .WithMessage("*must be partial*");
```

### Generator Testing

```csharp
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithFileCount(2)
    .WithFileNamed("Customer.g.cs")
    .WithNoDiagnostics()
    .VerifySnapshot();
```

### Code Fix Testing

```csharp
await AnalyzeAndFixWith<MyAnalyzer, MyCodeFix>(source)
    .ForDiagnostic("MY001")
    .ShouldProduce(expectedSource);
```

### Template Testing

```csharp
await RenderTemplateFrom<MyGenerator>(source)
    .Render("MyTemplate.scriban-cs", ctx => new { Name = ctx.RequireNamedType("Foo").Value.Name })
    .ShouldRender()
    .VerifySnapshot();
```

## Detailed Documentation

- **[RoslynTestBase](../testing/roslyn-test-base.md)** — Full API reference for the base class
- **[ReferenceConfiguration](../testing/reference-configuration.md)** — Configure assembly references for tests

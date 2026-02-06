# Deepstaging.Roslyn.Testing

Test utilities for Roslyn analyzers, generators, and code fixes.

📚 **[Full Documentation](https://deepstaging.github.io/roslyn)**

> **See also:
** [RoslynTestBase](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn.Testing/Docs/RoslynTestBase.md) | [Reference Configuration](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn.Testing/Docs/ReferenceConfiguration.md) | [Roslyn Toolkit](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn/README.md)

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

---

## Entry Points

| Method                                           | Description                            |
|--------------------------------------------------|----------------------------------------|
| `SymbolsFor(source)`                             | Create compilation and query symbols   |
| `CompilationFor(source)`                         | Get the raw compilation                |
| `AnalyzeWith<T>(source)`                         | Run analyzer and assert diagnostics    |
| `GenerateWith<T>(source)`                        | Run generator and assert output        |
| `FixWith<T>(source)`                             | Test code fix for compiler diagnostics |
| `AnalyzeAndFixWith<TAnalyzer, TCodeFix>(source)` | Test code fix for analyzer diagnostics |
| `RenderTemplateFrom<T>(source)`                  | Test Scriban template rendering        |

---

## Documentation

### Test Contexts

Each entry point returns a test context with fluent assertions:

- *
  *[SymbolTestContext](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn.Testing/Docs/SymbolTestContext.md)
  ** — Query symbols from compiled source
- *
  *[AnalyzerTestContext](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn.Testing/Docs/AnalyzerTestContext.md)
  ** — Assert on analyzer diagnostics
- *
  *[GeneratorTestContext](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn.Testing/Docs/GeneratorTestContext.md)
  ** — Assert on generator output
- *
  *[CodeFixTestContext](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn.Testing/Docs/CodeFixTestContext.md)
  ** — Assert on code fix transformations
- *
  *[TemplateTestContext](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn.Testing/Docs/TemplateTestContext.md)
  ** — Assert on template rendering

### Guides

- *
  *[RoslynTestBase](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn.Testing/Docs/RoslynTestBase.md)
  ** — Full API reference for the base class
- *
  *[Reference Configuration](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn.Testing/Docs/ReferenceConfiguration.md)
  ** — Configure assembly references for tests

---

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

---

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service
or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we
think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](https://github.com/deepstaging/roslyn/blob/main/LICENSE) for the full legal text.

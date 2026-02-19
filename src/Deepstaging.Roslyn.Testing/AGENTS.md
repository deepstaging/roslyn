# Deepstaging.Roslyn.Testing — Agent Guide

Test infrastructure for Roslyn analyzers, generators, and code fixes. Built on TUnit with Verify snapshot testing.

Docs: https://deepstaging.github.io/roslyn/packages/testing/

## Setup

All test classes inherit from `RoslynTestBase`:

```csharp
public class MyTests : RoslynTestBase
{
    [Test]
    public async Task MyTest()
    {
        // ...
    }
}
```

If tests reference your own types (attributes, etc.), register them once via `ModuleInitializer`:

```csharp
[ModuleInitializer]
public static void Init() =>
    ReferenceConfiguration.AddReferencesFromTypes(typeof(MyAttribute));
```

## Entry Points

Every test starts by calling one method on `RoslynTestBase`:

| Method                              | Returns                | Use For                                     |
|-------------------------------------|------------------------|---------------------------------------------|
| `SymbolsFor(source)`                | `SymbolTestContext`    | Querying symbols from compiled source       |
| `CompilationFor(source)`            | `Compilation`          | Getting the raw Roslyn compilation          |
| `AnalyzeWith<T>(source)`            | `AnalyzerTestContext`  | Asserting analyzer diagnostics              |
| `GenerateWith<T>(source)`           | `GeneratorTestContext` | Asserting generator output                  |
| `FixWith<T>(source)`                | `CodeFixTestContext`   | Testing code fixes for compiler diagnostics |
| `AnalyzeAndFixWith<TA, TF>(source)` | `CodeFixTestContext`   | Testing code fixes for analyzer diagnostics |
| `RenderTemplateFrom<T>(source)`     | `TemplateTestContext`  | Testing Scriban template rendering          |

## Patterns

### Symbol Testing

```csharp
[Test]
public async Task FindPublicMethods()
{
    var ctx = SymbolsFor("public class Foo { public void Bar() {} }");
    var type = ctx.RequireNamedType("Foo");          // Throws if not found
    var methods = ctx.Type("Foo").Methods().ThatArePublic().GetAll();
    await Assert.That(methods).HasCount().EqualTo(1);
}
```

- `RequireNamedType("Name")` — returns `ValidSymbol<INamedTypeSymbol>`, throws if missing
- `Type("Name")` — returns `OptionalSymbol<INamedTypeSymbol>` for fluent chaining

### Analyzer Testing

```csharp
[Test]
public async Task ReportsError()
{
    await AnalyzeWith<MyAnalyzer>(source)
        .ShouldReportDiagnostic("MY001")
        .WithSeverity(DiagnosticSeverity.Error)
        .WithMessage("*must be partial*");    // Wildcard matching
}
```

### Generator Testing

```csharp
[Test]
public async Task GeneratesCode()
{
    await GenerateWith<MyGenerator>(source)
        .ShouldGenerate()
        .WithFileCount(2)
        .WithFileNamed("Customer.g.cs")
        .WithNoDiagnostics()
        .VerifySnapshot();                    // Verify snapshot comparison
}
```

### Code Fix Testing

```csharp
[Test]
public async Task FixesMissingPartial()
{
    await AnalyzeAndFixWith<MyAnalyzer, MyCodeFix>(source)
        .ForDiagnostic("MY001")
        .ShouldProduce(expectedSource);
}
```

### Template Testing

```csharp
[Test]
public async Task RendersTemplate()
{
    await RenderTemplateFrom<MyGenerator>(source)
        .Render("MyTemplate.scriban-cs", ctx =>
            new { Name = ctx.RequireNamedType("Foo").Value.Name })
        .ShouldRender()
        .VerifySnapshot();
}
```

## Key Conventions

- All test methods are `async Task` (TUnit requirement)
- Fluent assertion chains are awaited directly
- `VerifySnapshot()` creates/compares `.verified.txt` files (Verify library)
- Source code is passed as string literals — the framework compiles it against .NET 10.0 references
- `WithAdditionalText(path, content)` adds additional files for generators/analyzers that need them

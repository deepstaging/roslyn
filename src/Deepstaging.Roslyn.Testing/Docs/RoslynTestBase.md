# RoslynTestBase

Unified base class for testing Roslyn analyzers, generators, and code fixes.

> **See also:** [Testing README](../README.md) | [Reference Configuration](ReferenceConfiguration.md) | [Roslyn Toolkit](../../Deepstaging.Roslyn/README.md)

## Quick Start

All Roslyn tests inherit from `RoslynTestBase`:

```csharp
public class MyTests : RoslynTestBase
{
    [Test]
    public async Task QuerySymbols()
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
            .WithFileContaining("public partial class");
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

---

## Entry Points

### SymbolsFor(source)

Create a `SymbolTestContext` for querying symbols from compiled source:

```csharp
var ctx = SymbolsFor(@"
    public class Customer 
    {
        public string Name { get; set; }
    }");

// Get/Require patterns - Require throws if not found
var type = ctx.RequireNamedType("Customer");
var prop = ctx.RequireProperty("Name");

// Optional patterns - returns OptionalSymbol
var maybeType = ctx.GetNamedType("Customer");
if (maybeType.IsNotValid(out var valid))
    return;
```

### CompilationFor(source)

Get the raw `Compilation` for source code:

```csharp
var compilation = CompilationFor(source);
```

### AnalyzeWith\<TAnalyzer\>(source)

Run an analyzer and make assertions about diagnostics:

```csharp
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldReportDiagnostic("MY001")
    .WithSeverity(DiagnosticSeverity.Error)
    .WithMessage("*must be partial*");
```

### GenerateWith\<TGenerator\>(source)

Run a source generator and make assertions about output:

```csharp
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithFileCount(2)
    .WithFileNamed("Customer.g.cs")
    .WithFileContaining("public partial class")
    .WithNoDiagnostics();
```

### FixWith\<TCodeFix\>(source)

Test a code fix for compiler diagnostics:

```csharp
await FixWith<MyCodeFix>(source)
    .ForDiagnostic("CS0246")
    .ShouldProduce(expectedSource);
```

### AnalyzeAndFixWith\<TAnalyzer, TCodeFix\>(source)

Test a code fix for analyzer diagnostics:

```csharp
await AnalyzeAndFixWith<MyAnalyzer, MyCodeFix>(source)
    .ForDiagnostic("MY001")
    .ShouldProduce(expectedSource);
```

### RenderTemplateFrom\<TGenerator\>(source)

Test Scriban template rendering with symbol context:

```csharp
await RenderTemplateFrom<MyGenerator>(source)
    .Render("MyTemplate.scriban-cs", ctx => new { Name = ctx.RequireNamedType("Foo").Value.Name })
    .ShouldRender()
    .VerifySnapshot();
```

---

## SymbolTestContext

Query symbols from a compilation with fluent APIs.

### Direct Symbol Access

```csharp
var ctx = SymbolsFor(source);

// Types
OptionalSymbol<INamedTypeSymbol> type = ctx.GetNamedType("Customer");
ValidSymbol<INamedTypeSymbol> type = ctx.RequireNamedType("Customer");

// Members (searches all types)
ValidSymbol<IMethodSymbol> method = ctx.RequireMethod("ProcessAsync");
ValidSymbol<IPropertySymbol> prop = ctx.RequireProperty("Name");
ValidSymbol<IFieldSymbol> field = ctx.RequireField("_logger");
ValidSymbol<IParameterSymbol> param = ctx.RequireParameter("id");

// Namespaces
ValidSymbol<INamespaceSymbol> ns = ctx.RequireNamespace("MyApp.Services");
```

### Fluent Type Queries

```csharp
// Query members on a specific type
var publicMethods = ctx.Type("Customer").Methods().ThatArePublic().GetAll();
var requiredProps = ctx.Type("Customer").Properties().ThatAreRequired().GetAll();
var constructors = ctx.Type("Customer").Constructors().GetAll();

// Query types in source (excludes referenced assemblies)
var allClasses = ctx.Types().ThatAreClasses().ThatArePublic().GetAll();

// Query all types including references
var withAttribute = ctx.AllTypesInCompilation()
    .ThatAreClasses()
    .WithAttribute("MyAttribute")
    .Query()
    .GetAll();
```

### Projections

```csharp
// Map a type to a custom projection
var model = ctx.Map("Customer", type => new CustomerModel(type));

// Query with compilation context
var info = ctx.Query(
    s => s.RequireNamedType("Runtime"),
    (symbol, compilation) => symbol.QuerySystemInfo(compilation));
```

---

## Analyzer Assertions

```csharp
// Assert specific diagnostic
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldReportDiagnostic("MY001");

// With additional constraints
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldReportDiagnostic("MY001")
    .WithSeverity(DiagnosticSeverity.Error)
    .WithMessage("*partial*");  // Wildcard matching

// Assert no specific diagnostic
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldNotReportDiagnostic("MY001");

// Assert diagnostics exist (without specific ID)
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldHaveDiagnostics()
    .WithErrorCode("MY001");

// Assert no diagnostics at all
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldHaveNoDiagnostics();
```

---

## Generator Assertions

```csharp
// Basic generation check
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate();

// Assert no generation
await GenerateWith<MyGenerator>(source)
    .ShouldNotGenerate();

// Detailed assertions
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithFileCount(2)
    .WithFileNamed("Customer.g.cs")
    .WithFileContaining("public partial class")
    .WithoutFileContaining("// TODO");

// Diagnostic assertions
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithNoDiagnostics();

await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithNoErrors();

await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithNoDiagnostics(filter => filter
        .WithSeverity(DiagnosticSeverity.Error)
        .WithId("DS0001"));

// Snapshot testing
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .VerifySnapshot();
```

---

## Code Fix Assertions

```csharp
// Test code fix for analyzer diagnostic
await AnalyzeAndFixWith<MyAnalyzer, MyCodeFix>(source)
    .ForDiagnostic("MY001")
    .ShouldProduce(expectedSource);

// Test code fix for compiler diagnostic
await FixWith<MyCodeFix>(source)
    .ForDiagnostic("CS0246")
    .ShouldProduce(expectedSource);

// Add analyzer dynamically
await FixWith<MyCodeFix>(source)
    .WithAnalyzer<MyAnalyzer>()
    .ForDiagnostic("MY001")
    .ShouldProduce(expectedSource);
```

---

## Template Assertions

```csharp
// Test template rendering with symbol queries
await RenderTemplateFrom<MyGenerator>(source)
    .Render("MyTemplate.scriban-cs", ctx => new 
    { 
        Name = ctx.RequireNamedType("Foo").Value.Name,
        Properties = ctx.Type("Foo").Properties().GetAll()
    })
    .ShouldRender()
    .WithContent("public class Foo");

// Direct context object
await RenderTemplateFrom<MyGenerator>(source)
    .Render("MyTemplate.scriban-cs", new { Name = "Test" })
    .ShouldRender()
    .VerifySnapshot();

// Assert template failure
await RenderTemplateFrom<MyGenerator>(source)
    .Render("Invalid.scriban-cs", new { })
    .ShouldFail();
```

---

## Reference Configuration

If your tests need custom assembly references (e.g., for attributes), configure them once via `ModuleInitializer`:

```csharp
[ModuleInitializer]
public static void Init() =>
    ReferenceConfiguration.AddReferencesFromTypes(typeof(MyAttribute));
```

See [Reference Configuration](ReferenceConfiguration.md) for details.

---

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](../../../LICENSE) for the full legal text.

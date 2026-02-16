# Roslyn Toolkit Project — Agent Guide

This is a Roslyn source generator, analyzer, and code fix project built with [Deepstaging.Roslyn](https://deepstaging.github.io/roslyn).

## Project Structure

```
src/
├── *.RoslynKit/              # Attributes — consumed by user code
├── *.RoslynKit.Projection/   # Queries and models — shared extraction logic
├── *.RoslynKit.Generators/   # Source generators — emit code from models
├── *.RoslynKit.Analyzers/    # Analyzers — validate attribute usage
├── *.RoslynKit.CodeFixes/    # Code fixes — auto-fix analyzer diagnostics
└── *.RoslynKit.Tests/        # Tests — TUnit + Deepstaging.Roslyn.Testing
```

**Data flows one way:** Attributes → Projection (queries + models) → Generators / Analyzers / CodeFixes.

The Projection layer is the shared core. Generators, analyzers, and code fixes all consume the same query methods and models — they never interpret attributes independently.

## Build & Test

```bash
dotnet build                    # Build all projects (also the lint step — warnings are errors)
dotnet test                     # Run all tests
./build/pack.sh                 # Create NuGet packages
```

## Target Frameworks

- **Library projects** (attributes, projection, generators, analyzers, code fixes): `netstandard2.0` — required for Roslyn analyzer/generator host compatibility
- **Test project**: modern TFM (net10.0+)

## Deepstaging.Roslyn Patterns

This project uses the Deepstaging.Roslyn fluent API. Key patterns:

### IsNotValid Early Exit

The primary convention for handling nullable Roslyn data:

```csharp
if (symbol.GetAttribute("MyAttribute").IsNotValid(out var attr))
    return;

// attr is guaranteed valid from here
var name = attr.NamedArg<string>("Name").OrDefault("default");
```

### Queries

Find symbols with chainable filters. Returns real `ISymbol` instances:

```csharp
TypeQuery.From(compilation)
    .ThatArePublic()
    .ThatAreClasses()
    .WithAttribute("MyAttribute")
    .GetAll();
```

### Emit

Fluent builders that produce compilable C#:

```csharp
TypeBuilder.Class("Generated")
    .InNamespace("MyApp")
    .AsPartial()
    .AddProperty("Name", "string")
    .Emit();
```

### Projections

Safe wrappers over nullable Roslyn symbols. `Optional` → validate → `Valid`:

```csharp
var value = symbol
    .GetAttribute("ConfigAttribute")
    .ConstructorArg<int>(0)
    .OrDefault(0);
```

### Scriban Templates

Source generators can use `.scriban-cs` templates instead of emit builders:

- Templates live alongside the generator that uses them
- Use `TemplateRenderer` to render with a model
- Test with `RenderTemplateFrom<T>(source)` in test classes

## Testing Patterns

Tests inherit from `RoslynTestBase`:

```csharp
public class MyTests : RoslynTestBase
{
    [Test]
    public async Task AnalyzerReportsError()
    {
        await AnalyzeWith<MyAnalyzer>(source)
            .ShouldReportDiagnostic("RK1001");
    }

    [Test]
    public async Task GeneratorEmitsCode()
    {
        await GenerateWith<MyGenerator>(source)
            .ShouldGenerate()
            .VerifySnapshot();
    }

    [Test]
    public async Task CodeFixWorks()
    {
        await AnalyzeAndFixWith<MyAnalyzer, MyCodeFix>(source)
            .ForDiagnostic("RK1001")
            .ShouldProduce(expectedSource);
    }
}
```

Register custom type references once via `ModuleInitializer`:

```csharp
[ModuleInitializer]
public static void Init() =>
    ReferenceConfiguration.AddReferencesFromTypes(typeof(MyAttribute));
```

## Adding New Features

1. **Define attribute** in the RoslynKit project
2. **Add queries and model** in the Projection project — extract attribute data into a strongly-typed model
3. **Implement generator** (or analyzer/code fix) consuming the projection model
4. **Write tests** using the appropriate `RoslynTestBase` entry point

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
<!--#if (includeRuntime) -->
└── *.RoslynKit.Runtime/      # Runtime base classes (ObservableObject)
<!--#endif -->
test/
└── *.RoslynKit.Tests/        # Tests — TUnit + Deepstaging.Roslyn.Testing
```

**Data flows one way:** Attributes → Projection (queries + models) → Generators / Analyzers / CodeFixes.

The Projection layer is the shared core. Generators, analyzers, and code fixes all consume the same query methods and models — they never interpret attributes independently.

## Build, Test, Lint

```bash
dotnet build                                                          # Build all (warnings are errors — this is the lint step)
dotnet run --project test/Deepstaging.RoslynKit.Tests -c Release      # Run all tests
dotnet run --project test/Deepstaging.RoslynKit.Tests -c Release \
  --treenode-filter /*/*/AutoNotifyGeneratorTests/*                    # Run one test class
./build/pack.sh                                                       # Create NuGet package
./build/docs.sh serve                                                 # Preview docs locally
```

The `--treenode-filter` syntax is `/<Assembly>/<Namespace>/<Class>/<Test>` with `*` wildcards.

## Target Frameworks

- **Library projects** (attributes, projection, generators, analyzers, code fixes): `netstandard2.0` — required for Roslyn analyzer/generator host compatibility
<!--#if (includeRuntime) -->
- **Runtime project**: `net10.0` — ships as a runtime dependency in the NuGet package
<!--#endif -->
- **Test project**: `net10.0`

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
symbol.QueryFields()
    .ThatArePrivate()
    .ThatAreInstance()
    .Where(f => f.Name.StartsWith("_"));
```

### Emit

Fluent builders that produce compilable C#:

```csharp
TypeBuilder
    .Parse("public partial class Generated")
    .InNamespace("MyApp")
    .ImplementsINotifyPropertyChanged()
    .AddProperty("Name", "string", p => p
        .WithGetter(b => b.AddStatement("return _name"))
        .WithSetter(b => b.AddStatement("_name = value")))
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

## Testing Patterns

Tests inherit from `RoslynTestBase`:

```csharp
public class MyTests : RoslynTestBase
{
    [Test]
    public async Task AnalyzerReportsError() =>
        await AnalyzeWith<MyAnalyzer>(source)
            .ShouldReportDiagnostic("RK001");

    [Test]
    public async Task GeneratorEmitsCode() =>
        await GenerateWith<MyGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("public string Name")
            .CompilesSuccessfully();

    [Test]
    public async Task SnapshotTest() =>
        await GenerateWith<MyGenerator>(source)
            .ShouldGenerate()
            .VerifySnapshot();

    [Test]
    public async Task CodeFixWorks() =>
        await AnalyzeAndFixWith<MyAnalyzer, MyCodeFix>(source)
            .ForDiagnostic("RK001")
            .ShouldProduce(expectedSource);
}
```

Register custom type references once via `ModuleInitializer` in `ModuleInit.cs`:

```csharp
[ModuleInitializer]
public static void Init() =>
    ReferenceConfiguration.AddReferencesFromTypes(typeof(MyAttribute));
```

## Packaging

This project uses single-package bundling. The root csproj packs everything into one NuGet package:

- Attributes → `lib/netstandard2.0/`
- Generator + Projection → `analyzers/dotnet/cs/`
- Analyzers + CodeFixes → `analyzers/dotnet/cs/`
<!--#if (includeRuntime) -->
- Runtime → `lib/net10.0/`
<!--#endif -->

## Adding New Features

1. **Define attribute** in the RoslynKit project
2. **Add queries and model** in the Projection project — extract attribute data into a `[PipelineModel]` record
3. **Implement generator** (or analyzer/code fix) consuming the projection model
4. **Write tests** using the appropriate `RoslynTestBase` entry point
5. **Update snapshot** files if generator output changes

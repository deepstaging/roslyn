# Getting Started

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- An IDE with Roslyn support (Rider, Visual Studio, VS Code with C# Dev Kit)

## Build & Test

```bash
dotnet build
dotnet run --project test/Deepstaging.RoslynKit.Tests -c Release
```

## Project Structure

```
src/
├── RoslynKit/              Attributes — [AutoNotify], [AlsoNotify]
├── RoslynKit.Projection/   Queries + Models — extract data from symbols
├── RoslynKit.Generators/   Source generator — emit code from models
├── RoslynKit.Analyzers/    Analyzers — validate symbols, report diagnostics
├── RoslynKit.CodeFixes/    Code fixes — auto-fix analyzer diagnostics
└── RoslynKit.Runtime/      Base classes for generated code (optional)
test/
└── RoslynKit.Tests/        Tests for all of the above
docs/                       This documentation site
```

!!! tip "Runtime project"
    If you scaffolded with `--includeRuntime`, you'll also have a **Runtime** project containing `ObservableObject` — a base class with `SetField<T>` and `OnPropertyChanged`. This ships as a `net10.0` library inside the NuGet package.

## How It Works

### 1. User writes code with attributes

```csharp
[AutoNotify]
public partial class PersonViewModel
{
    [AlsoNotify("FullName")]
    private string _firstName = "";
    private int _age;
}
```

### 2. Projection extracts a model

The `QueryAutoNotify()` extension method walks the Roslyn symbol tree and produces a strongly-typed `AutoNotifyModel`:

```csharp
// AutoNotifyProjection.cs
public static AutoNotifyModel? QueryAutoNotify(this ValidSymbol<INamedTypeSymbol> symbol)
```

This model contains the namespace, type name, accessibility, and a list of `AutoNotifyFieldModel` — each carrying the field name, property name, type, and any `[AlsoNotify]` targets.

### 3. Generator emits code

The `AutoNotifyWriter` takes the model and uses the Emit API to build a partial class:

```csharp
TypeBuilder
    .Parse($"{model.Accessibility} partial class {model.TypeName}")
    .InNamespace(model.Namespace)
    .ImplementsINotifyPropertyChanged()
    .WithEach(model.Fields, (type, field) => type
        .AddProperty(field.PropertyName, field.TypeName, p => p
            .WithGetter(...)
            .WithSetter(...)))
    .Emit();
```

### 4. Analyzers validate

`MustBePartialAnalyzer` reports **RK001** if the class isn't `partial`. `FieldMustBePrivateAnalyzer` reports **RK002** if a backing field isn't `private`. Both use the same Projection layer.

### 5. Code fixes offer one-click repairs

`MakePartialCodeFix` adds the `partial` modifier. `MakePrivateCodeFix` changes the field to `private`.

## Extending

To add a new attribute and generator:

1. **Define the attribute** in `src/RoslynKit/`
2. **Create the projection** — add a model in `Projection/Models/` and a query extension in `Projection/`
3. **Write the generator** — add a writer in `Generators/Writers/` and register it in a new `IIncrementalGenerator`
4. **Add analyzers** if the attribute has constraints (must be partial, must be on a class, etc.)
5. **Write tests** — generator, analyzer, and code fix tests in `test/`

Each piece is independently testable. The Projection layer is the glue.

## Documentation Site

This project includes a [MkDocs Material](https://squidfunk.github.io/mkdocs-material/) site for documentation.

```bash
# Preview locally (creates .venv automatically)
./build/docs.sh serve

# Build static site
./build/docs.sh build
```

The site deploys to GitHub Pages automatically on push to `main` when docs change. See [Packaging & Publishing](guides/packaging.md) for setup.

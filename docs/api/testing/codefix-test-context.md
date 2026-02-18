# CodeFixTestContext

Test Roslyn code fix providers with before/after source assertions.

## Creating

### For Analyzer Diagnostics

```csharp
await AnalyzeAndFixWith<MyAnalyzer, MyCodeFix>(source)
    .ForDiagnostic("MY001")
    .ShouldProduce(expectedSource);
```

### For Compiler Diagnostics

```csharp
await FixWith<MyCodeFix>(source)
    .ForDiagnostic("CS0246")
    .ShouldProduce(expectedSource);
```

## Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `WithAnalyzer<T>()` | `CodeFixTestContext` | Specify analyzer (alternative to `AnalyzeAndFixWith`) |
| `WithAdditionalText(path, content)` | `CodeFixTestContext` | Add an additional text file |
| `ForDiagnostic(id)` | `CodeFixAssertion` | Specify which diagnostic to fix |

## CodeFixAssertion

Assertions for code fix results:

| Method | Returns | Description |
|--------|---------|-------------|
| `ShouldProduce(expectedSource)` | `Task` | Assert the fixed source matches expected |
| `ShouldAddAdditionalDocument()` | `AdditionalDocumentAssertion` | Assert an additional document was created |
| `ShouldAddSourceDocument()` | `SourceDocumentAssertion` | Assert a source document was created |
| `ShouldOfferFix(title?)` | `Task` | Assert a code fix was offered (optionally with specific title) |
| `ShouldNotOfferFix()` | `Task` | Assert no code fix was offered |

## Document Assertions

For code fixes that create additional or source documents:

### AdditionalDocumentAssertion

```csharp
await AnalyzeAndFixWith<MyAnalyzer, MyCodeFix>(source)
    .ForDiagnostic("MY001")
    .ShouldAddAdditionalDocument()
    .WithPathContaining("Templates/")
    .WithContentContaining("{{ model.name }}")
    .WithoutContentContaining("sealed");
```

### SourceDocumentAssertion

```csharp
await AnalyzeAndFixWith<MyAnalyzer, MyCodeFix>(source)
    .ForDiagnostic("MY001")
    .ShouldAddSourceDocument()
    .WithPathContaining("Generated/")
    .WithContentContaining("partial class");
```

Both assertion types support:

| Method | Returns | Description |
|--------|---------|-------------|
| `WithPathContaining(fragment)` | self | Assert path contains substring |
| `WithContentContaining(text)` | self | Assert content contains text |
| `WithoutContentContaining(text)` | self | Assert content excludes text |

## Example

```csharp
[Test]
public async Task Adds_partial_modifier()
{
    const string source = """
        [AutoNotify]
        public class Customer { }
        """;

    const string expected = """
        [AutoNotify]
        public partial class Customer { }
        """;

    await AnalyzeAndFixWith<MustBePartialAnalyzer, AddPartialCodeFix>(source)
        .ForDiagnostic("RK1002")
        .ShouldProduce(expected);
}

[Test]
public async Task Creates_template_file()
{
    await AnalyzeAndFixWith<ScaffoldAnalyzer, ScaffoldCodeFix>(source)
        .ForDiagnostic("DSRK005")
        .ShouldAddAdditionalDocument()
        .WithPathContaining("Templates/")
        .WithContentContaining("{{ model.name }}");
}
```

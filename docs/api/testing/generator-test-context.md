# GeneratorTestContext

Test Roslyn source generators with fluent output assertions and snapshot testing.

## Creating

```csharp
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .VerifySnapshot();
```

## Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `WithAdditionalText(path, content)` | `GeneratorTestContext` | Add an additional text file |
| `ShouldGenerate()` | `GeneratorAssertions` | Assert generation produces output |
| `ShouldNotGenerate()` | `Task` | Assert no output generated |
| `GetResultAsync()` | `Task<GeneratorDriverRunResult>` | Get raw generator result |

## GeneratorAssertions

Chainable assertions for generator output:

```csharp
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithFileCount(2)
    .WithFileNamed("Customer.g.cs")
    .WithFileContaining("partial class Customer")
    .WithoutFileContaining("sealed")
    .WithNoDiagnostics()
    .CompilesSuccessfully()
    .VerifySnapshot();
```

| Method | Returns | Description |
|--------|---------|-------------|
| `WithFileCount(count)` | `GeneratorAssertions` | Assert number of generated files |
| `WithFileNamed(name)` | `GeneratorAssertions` | Assert a file with this name exists |
| `WithFileContaining(content)` | `GeneratorAssertions` | Assert any file contains text |
| `WithoutFileContaining(content)` | `GeneratorAssertions` | Assert no file contains text |
| `WithNoDiagnostics()` | `GeneratorAssertions` | Assert zero diagnostics |
| `WithNoDiagnostics(filter)` | `GeneratorAssertions` | Assert no diagnostics matching filter |
| `WithNoErrors()` | `GeneratorAssertions` | Assert no error-level diagnostics |
| `WithNoWarnings()` | `GeneratorAssertions` | Assert no warning-level diagnostics |
| `CompilesSuccessfully()` | `GeneratorAssertions` | Assert generated code compiles |
| `VerifySnapshot(sourceFile)` | `Task` | Snapshot test with Verify |

## DiagnosticFilter

Configure which diagnostics to check with `WithNoDiagnostics`:

```csharp
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithNoDiagnostics(f => f
        .WithSeverity(DiagnosticSeverity.Error)
        .WithIds("CS0001", "CS0002"));
```

| Method | Returns | Description |
|--------|---------|-------------|
| `WithSeverity(severity)` | `DiagnosticFilter` | Filter by severity level |
| `WithId(id)` | `DiagnosticFilter` | Filter by single diagnostic ID |
| `WithIds(ids)` | `DiagnosticFilter` | Filter by multiple IDs |
| `WithMessageContaining(text)` | `DiagnosticFilter` | Filter by message content |

## Snapshot Testing

`VerifySnapshot()` uses [Verify](https://github.com/VerifyTests/Verify) to snapshot generated output. On first run it creates `.verified.txt` files; subsequent runs compare against them.

```csharp
[Test]
public async Task Generates_customer_code()
{
    await GenerateWith<MyGenerator>(source)
        .ShouldGenerate()
        .VerifySnapshot();
}
```

## Additional Text

For generators that use `AdditionalTexts` (e.g., user templates):

```csharp
await GenerateWith<MyGenerator>(source)
    .WithAdditionalText(
        "Templates/MyProject/Customer.scriban-cs",
        customTemplateContent)
    .ShouldGenerate()
    .WithFileNamed("Customer.g.cs");
```

# Testing

Tests use [TUnit](https://github.com/thomhurst/TUnit) with [Verify](https://github.com/VerifyTests/Verify) for snapshot testing, built on `RoslynTestBase` from `Deepstaging.Roslyn.Testing`.

## Running Tests

```bash
# All tests
dotnet run --project test/Deepstaging.RoslynKit.Tests -c Release

# By class
dotnet run --project test/Deepstaging.RoslynKit.Tests -c Release \
  --treenode-filter /*/*/AutoNotifyGeneratorTests/*

# By test name
dotnet run --project test/Deepstaging.RoslynKit.Tests -c Release \
  --treenode-filter /*/*/*/Generates_property_for_private_field
```

## Test Patterns

### Generator Tests

```csharp
[Test]
public async Task Generates_property_for_private_field() =>
    await GenerateWith<AutoNotifyGenerator>(source)
        .ShouldGenerate()
        .WithFileContaining("public string Name")
        .CompilesSuccessfully();
```

- `ShouldGenerate()` — asserts at least one file was generated
- `WithFileContaining("...")` — asserts generated output contains the string
- `WithoutFileContaining("...")` — asserts it does NOT contain the string
- `CompilesSuccessfully()` — compiles the generated code and asserts no errors
- `VerifySnapshot()` — snapshot test with Verify (creates `.verified.txt`)

### Analyzer Tests

```csharp
[Test]
public async Task Reports_RK001_when_not_partial() =>
    await AnalyzeWith<MustBePartialAnalyzer>(source)
        .ShouldReportDiagnostic("RK001")
        .WithSeverity(DiagnosticSeverity.Error);

[Test]
public async Task No_diagnostic_when_partial() =>
    await AnalyzeWith<MustBePartialAnalyzer>(source)
        .ShouldNotReportDiagnostic("RK001");
```

### Code Fix Tests

```csharp
[Test]
public async Task MakePartial_adds_partial_modifier() =>
    await AnalyzeAndFixWith<MustBePartialAnalyzer, MakePartialCodeFix>(source)
        .ForDiagnostic("RK001")
        .ShouldProduce(expected);

[Test]
public async Task MakePartial_offers_fix() =>
    await AnalyzeAndFixWith<MustBePartialAnalyzer, MakePartialCodeFix>(source)
        .ForDiagnostic("RK001")
        .ShouldOfferFix("Make type partial");
```

## ModuleInit

`test/Deepstaging.RoslynKit.Tests/ModuleInit.cs` registers assembly references needed by test compilations:

```csharp
[assembly: AssemblyFixture(typeof(ModuleInit))]
public class ModuleInit
{
    public static void Initialize()
    {
        RoslynTestBase.AddReferences(
            typeof(AutoNotifyAttribute),
            typeof(AlsoNotifyAttribute),
            typeof(INotifyPropertyChanged));
    }
}
```

Without this, `CompilesSuccessfully()` would fail because the test compilation wouldn't have access to the attribute types or `System.ObjectModel`.

## Snapshot Testing

The `VerifySnapshot()` method creates a `.received.txt` file on first run. To accept it:

```bash
cp *.received.txt *.verified.txt
```

On subsequent runs, Verify diffs the output against `.verified.txt` and fails if they differ. This catches unintended changes to generated output.

# ReferenceConfiguration

Configure metadata references for test compilations. Call once via `[ModuleInitializer]` to make your project's types available in test source code.

## Setup

```csharp
internal static class TestInit
{
    [ModuleInitializer]
    public static void Init() =>
        ReferenceConfiguration.AddReferencesFromTypes(
            typeof(AutoNotifyAttribute),
            typeof(MyModel));
}
```

This ensures that when `SymbolsFor(source)`, `AnalyzeWith<T>(source)`, etc. compile the source string, they can resolve your attribute types and other dependencies.

## Methods

| Method | Description |
|--------|-------------|
| `AddReferencesFromTypes(params Type[] types)` | Add references from types' containing assemblies |
| `AddReferences(params Assembly[] assemblies)` | Add references from assemblies directly |
| `AddReferences(params MetadataReference[] references)` | Add pre-built metadata references |
| `AddReferencesFromPaths(params string[] assemblyPaths)` | Add references from file paths |
| `Clear()` | Remove all configured references |

## When to Use

You need `ReferenceConfiguration` when your test source code references types from your own packages:

```csharp
// This source references [AutoNotify] — needs the assembly reference
const string source = """
    [AutoNotify]
    public partial class Customer { }
    """;

await AnalyzeWith<MustBePartialAnalyzer>(source)
    .ShouldReportDiagnostic("RK1002");
```

Without configuration, the compilation would fail to resolve `AutoNotifyAttribute`.

## What's Included by Default

`RoslynTestBase` automatically includes references to:

- .NET base class libraries (`System`, `System.Collections`, etc.)
- `Microsoft.CodeAnalysis` assemblies
- `Deepstaging.Roslyn` core library

You only need to add references for **your own** assemblies.

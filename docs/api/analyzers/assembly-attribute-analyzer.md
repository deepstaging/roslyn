# AssemblyAttributeAnalyzer

`AssemblyAttributeAnalyzer<TItem>` is the base class for analyzers that scan **assembly-level** attributes (`[assembly: ...]`) and report diagnostics based on the extracted data.

## Quick Start

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports("DSDT001", "Required environment variable not set",
    Message = "Environment variable '{0}' is not set",
    Category = "DevTools",
    Severity = DiagnosticSeverity.Warning)]
[Reports("DSDT002", "Optional environment variable not set",
    Message = "Optional environment variable '{0}' is not set",
    Category = "DevTools",
    Severity = DiagnosticSeverity.Info)]
public sealed class RequiresEnvAnalyzer : AssemblyAttributeAnalyzer<RequiredEnvVar>
{
    protected override string AttributeFullyQualifiedName =>
        "MyApp.RequiresEnvAttribute";

    protected override bool TryExtractItem(
        ValidAttribute attribute, Location location, out RequiredEnvVar item)
    {
        var name = attribute.ConstructorArg<string>(0).OrNull();
        if (name == null) { item = default; return false; }

        var optional = attribute.NamedArg<bool>("Optional").OrDefault(false);
        item = new RequiredEnvVar(name, optional, location);
        return true;
    }

    protected override void Analyze(
        CompilationAnalysisContext context, ImmutableArray<RequiredEnvVar> items)
    {
        foreach (var required in items)
        {
            var rule = required.IsOptional ? GetRule(1) : GetRule(0);
            context.ReportDiagnostic(Diagnostic.Create(
                rule, required.Location, required.Name));
        }
    }
}
```

## How It Works

1. Annotate with one or more `[Reports]` attributes (multi-diagnostic support)
2. The base class handles `Initialize`, `ConfigureGeneratedCodeAnalysis`, `EnableConcurrentExecution`, and `SupportedDiagnostics`
3. On compilation, scans assembly attributes matching `AttributeFullyQualifiedName`
4. Projects each match through `ValidAttribute` and calls `TryExtractItem`
5. Passes all successfully extracted items to `Analyze` in a single batch

## Abstract Members

| Member | Description |
|--------|-------------|
| `AttributeFullyQualifiedName` | Fully qualified metadata name of the attribute to scan (e.g., `"MyApp.RequiresToolAttribute"`) |
| `TryExtractItem(attribute, location, out item)` | Extract a model from each matching attribute. Return `false` to skip. |
| `Analyze(context, items)` | Evaluate all extracted items and report diagnostics. |

## Properties & Methods

| Member | Type | Description |
|--------|------|-------------|
| `SupportedDiagnostics` | `ImmutableArray<DiagnosticDescriptor>` | All descriptors from `[Reports]` attributes |
| `GetRule(index)` | `DiagnosticDescriptor` | Get descriptor by declaration order (0-based) |

## Multiple [Reports] Attributes

Each `[Reports]` attribute declares a diagnostic descriptor. Access them by index matching the declaration order on the class:

```csharp
[Reports("DSDT020", "Tool not installed", ...)]     // GetRule(0)
[Reports("DSDT021", "Tool version mismatch", ...)]  // GetRule(1)
[Reports("DSDT022", "Tool manifest missing", ...)]  // GetRule(2)
public sealed class RequiresToolAnalyzer : AssemblyAttributeAnalyzer<RequiredTool>
{
    protected override void Analyze(
        CompilationAnalysisContext context, ImmutableArray<RequiredTool> items)
    {
        // ...
        context.ReportDiagnostic(Diagnostic.Create(
            GetRule(0), location, toolName));  // DSDT020
    }
}
```

## Comparison with SymbolAnalyzer

| | `SymbolAnalyzer<TSymbol>` | `AssemblyAttributeAnalyzer<TItem>` |
|---|---|---|
| **Scope** | Per-symbol (`RegisterSymbolAction`) | Compilation-level (`RegisterCompilationAction`) |
| **Trigger** | Individual symbols | Assembly-level attributes |
| **[Reports]** | Single | One or more (`AllowMultiple = true`) |
| **Item extraction** | Automatic (attribute match) | Manual via `TryExtractItem` |
| **Analysis** | `ShouldReport` returns bool | `Analyze` has full control |

## When to Use

Use `AssemblyAttributeAnalyzer` when your diagnostics are driven by assembly-level attributes like:

```csharp
[assembly: RequiresTool("dotnet-ef", Version = "9.*")]
[assembly: RequiresEnv("DATABASE_URL")]
[assembly: Convention(RequireReadme = true)]
```

For per-symbol analysis (classes, methods, fields with attributes), use [SymbolAnalyzer](symbol-analyzer.md) instead.

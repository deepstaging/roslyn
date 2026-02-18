# Analyzer Pattern

Use the base classes from Deepstaging.Roslyn for common analyzer patterns.

## Basic Structure

```csharp
[Reports(DiagnosticId, "Type with [AutoNotify] must be partial",
    Description = "Source generators require partial types.",
    Category = "Usage",
    Severity = DiagnosticSeverity.Error)]
public sealed class AutoNotifyMustBePartialAnalyzer : TypeAnalyzer<AutoNotifyAttribute>
{
    public const string DiagnosticId = "RK1002";

    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) 
        => !type.IsPartial;
}
```

## Available Base Classes

All base classes inherit from `SymbolAnalyzer<TSymbol>` and provide specialized handling for different symbol types:

| Base Class | Symbol Type | Use Case |
|------------|-------------|----------|
| `TypeAnalyzer<TAttribute>` | `INamedTypeSymbol` | Validate types (classes, structs, interfaces) with attribute |
| `MethodAnalyzer<TAttribute>` | `IMethodSymbol` | Validate methods with attribute |
| `PropertyAnalyzer<TAttribute>` | `IPropertySymbol` | Validate properties with attribute |
| `FieldAnalyzer<TAttribute>` | `IFieldSymbol` | Validate fields with attribute |
| `EventAnalyzer<TAttribute>` | `IEventSymbol` | Validate events with attribute |
| `ParameterAnalyzer<TAttribute>` | `IParameterSymbol` | Validate parameters with attribute |
| `NamespaceAnalyzer<TAttribute>` | `INamespaceSymbol` | Validate namespaces with attribute |
| `TypeParameterAnalyzer<TAttribute>` | `ITypeParameterSymbol` | Validate generic type parameters with attribute |

## Type Analyzer Examples

### Must Be Partial

```csharp
[Reports(DiagnosticId, "Type with [AutoNotify] must be partial",
    Description = "Source generators require partial types.",
    Category = "Usage",
    Severity = DiagnosticSeverity.Error)]
public sealed class AutoNotifyMustBePartialAnalyzer : TypeAnalyzer<AutoNotifyAttribute>
{
    public const string DiagnosticId = "RK1002";

    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) 
        => !type.IsPartial;
}
```

### Must Be Sealed

```csharp
[Reports(DiagnosticId, "Effects module should be sealed",
    Description = "Effects modules should be sealed for performance.",
    Category = "Design",
    Severity = DiagnosticSeverity.Warning)]
public sealed class EffectsModuleShouldBeSealedAnalyzer : TypeAnalyzer<EffectsModuleAttribute>
{
    public const string DiagnosticId = "EFF001";

    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) 
        => type.IsClass && !type.IsSealed && !type.IsAbstract;
}
```

### Must Implement Interface

```csharp
[Reports(DiagnosticId, "Type must implement IDisposable",
    Category = "Usage",
    Severity = DiagnosticSeverity.Warning)]
public sealed class MustImplementDisposableAnalyzer : TypeAnalyzer<ManagedResourceAttribute>
{
    public const string DiagnosticId = "MR001";

    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) 
        => !type.AllInterfaces.Any(i => i.Name == "IDisposable");
}
```

### Struct Must Be Readonly

```csharp
[Reports(DiagnosticId, "Strong ID should be readonly struct",
    Description = "Value types used as IDs should be readonly for performance.",
    Category = "Performance",
    Severity = DiagnosticSeverity.Warning)]
public sealed class StrongIdShouldBeReadonlyAnalyzer : TypeAnalyzer<StrongIdAttribute>
{
    public const string DiagnosticId = "SID002";

    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) 
        => type.IsValueType && !type.IsReadOnly;
}
```

### Must Have Parameterless Constructor

```csharp
[Reports(DiagnosticId, "Serializable type must have parameterless constructor",
    Category = "Serialization",
    Severity = DiagnosticSeverity.Error)]
public sealed class RequiresParameterlessConstructorAnalyzer : TypeAnalyzer<SerializableAttribute>
{
    public const string DiagnosticId = "SER001";

    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) 
        => !type.Constructors.Any(c => c.Parameters.IsEmpty && !c.IsStatic);
}
```

## Method Analyzer Examples

### Async Method Naming

```csharp
[Reports(DiagnosticId, "Async method should have 'Async' suffix",
    Category = "Naming",
    Severity = DiagnosticSeverity.Warning)]
public sealed class AsyncMethodNamingAnalyzer : MethodAnalyzer<AsyncAttribute>
{
    public const string DiagnosticId = "ASYNC001";

    protected override bool ShouldReport(ValidSymbol<IMethodSymbol> method) 
        => method.IsAsync && !method.Name.EndsWith("Async");
}
```

### Return Type Validation

```csharp
[Reports(DiagnosticId, "Command handler must return Task",
    Category = "Usage",
    Severity = DiagnosticSeverity.Error)]
public sealed class CommandHandlerReturnTypeAnalyzer : MethodAnalyzer<CommandHandlerAttribute>
{
    public const string DiagnosticId = "CMD001";

    protected override bool ShouldReport(ValidSymbol<IMethodSymbol> method)
    {
        var returnType = method.ReturnType.Name;
        return returnType != "Task" && returnType != "ValueTask";
    }
}
```

### Parameter Count Validation

```csharp
[Reports(DiagnosticId, "Factory method must have no parameters",
    Category = "Design",
    Severity = DiagnosticSeverity.Error)]
public sealed class FactoryMethodParameterAnalyzer : MethodAnalyzer<FactoryAttribute>
{
    public const string DiagnosticId = "FAC001";

    protected override bool ShouldReport(ValidSymbol<IMethodSymbol> method) 
        => method.Parameters.Length > 0;
}
```

## Property Analyzer Examples

### Must Have Setter

```csharp
[Reports(DiagnosticId, "Bindable property must have setter",
    Category = "Usage",
    Severity = DiagnosticSeverity.Error)]
public sealed class BindablePropertyMustHaveSetterAnalyzer : PropertyAnalyzer<BindableAttribute>
{
    public const string DiagnosticId = "BIND001";

    protected override bool ShouldReport(ValidSymbol<IPropertySymbol> property) 
        => property.SetMethod is null;
}
```

### Must Be Virtual

```csharp
[Reports(DiagnosticId, "Lazy property should be virtual for proxy support",
    Category = "Design",
    Severity = DiagnosticSeverity.Warning)]
public sealed class LazyPropertyShouldBeVirtualAnalyzer : PropertyAnalyzer<LazyLoadAttribute>
{
    public const string DiagnosticId = "LAZY001";

    protected override bool ShouldReport(ValidSymbol<IPropertySymbol> property) 
        => !property.IsVirtual && !property.IsOverride;
}
```

## Field Analyzer Examples

### Must Be Private

```csharp
[Reports(DiagnosticId, "Backing field must be private",
    Description = "Fields marked with [Notify] must be private.",
    Category = "Encapsulation",
    Severity = DiagnosticSeverity.Error)]
public sealed class NotifyFieldMustBePrivateAnalyzer : FieldAnalyzer<NotifyAttribute>
{
    public const string DiagnosticId = "RK1003";

    protected override bool ShouldReport(ValidSymbol<IFieldSymbol> field) 
        => field.DeclaredAccessibility != Accessibility.Private;
}
```

### Naming Convention

```csharp
[Reports(DiagnosticId, "Backing field should start with underscore",
    Category = "Naming",
    Severity = DiagnosticSeverity.Info)]
public sealed class FieldNamingAnalyzer : FieldAnalyzer<NotifyAttribute>
{
    public const string DiagnosticId = "RK1004";

    protected override bool ShouldReport(ValidSymbol<IFieldSymbol> field) 
        => !field.Name.StartsWith("_");
}
```

## Parameter Analyzer Examples

### Must Not Be Nullable

```csharp
[Reports(DiagnosticId, "Required parameter cannot be nullable",
    Category = "Nullability",
    Severity = DiagnosticSeverity.Error)]
public sealed class RequiredParameterNotNullableAnalyzer : ParameterAnalyzer<RequiredAttribute>
{
    public const string DiagnosticId = "REQ001";

    protected override bool ShouldReport(ValidSymbol<IParameterSymbol> parameter) 
        => parameter.NullableAnnotation == NullableAnnotation.Annotated;
}
```

## Event Analyzer Examples

### Event Naming

```csharp
[Reports(DiagnosticId, "Domain event should end with 'Event'",
    Category = "Naming",
    Severity = DiagnosticSeverity.Warning)]
public sealed class DomainEventNamingAnalyzer : EventAnalyzer<DomainEventAttribute>
{
    public const string DiagnosticId = "DOM001";

    protected override bool ShouldReport(ValidSymbol<IEventSymbol> evt) 
        => !evt.Name.EndsWith("Event");
}
```

## Namespace Analyzer Examples

### Naming Convention

```csharp
[Reports(DiagnosticId, "Namespace must match folder structure",
    Category = "Naming",
    Severity = DiagnosticSeverity.Warning)]
public sealed class NamespaceMatchesFolderAnalyzer : NamespaceAnalyzer<ValidateNamespaceAttribute>
{
    public const string DiagnosticId = "NS001";

    protected override bool ShouldReport(ValidSymbol<INamespaceSymbol> ns) 
        => !ns.Name.EndsWith(ExpectedSuffix(ns));
}
```

## Type Parameter Analyzer Examples

### Constraint Validation

```csharp
[Reports(DiagnosticId, "Type parameter must have 'new()' constraint",
    Category = "Generics",
    Severity = DiagnosticSeverity.Error)]
public sealed class RequiresNewConstraintAnalyzer : TypeParameterAnalyzer<FactoryTypeAttribute>
{
    public const string DiagnosticId = "GEN001";

    protected override bool ShouldReport(ValidSymbol<ITypeParameterSymbol> typeParam) 
        => !typeParam.HasConstructorConstraint;
}
```

### Reference Type Constraint

```csharp
[Reports(DiagnosticId, "Cache key must be reference type",
    Description = "Cache keys should be reference types for proper equality semantics.",
    Category = "Usage",
    Severity = DiagnosticSeverity.Error)]
public sealed class CacheKeyMustBeReferenceTypeAnalyzer : TypeParameterAnalyzer<CacheKeyAttribute>
{
    public const string DiagnosticId = "CACHE001";

    protected override bool ShouldReport(ValidSymbol<ITypeParameterSymbol> typeParam) 
        => !typeParam.HasReferenceTypeConstraint;
}
```

## Using Projection Layer

For complex validation, use the projection layer:

```csharp
[Reports(DiagnosticId, "AutoNotify type must have at least one notifiable field",
    Category = "Usage",
    Severity = DiagnosticSeverity.Warning)]
public sealed class AutoNotifyMustHaveFieldsAnalyzer : TypeAnalyzer<AutoNotifyAttribute>
{
    public const string DiagnosticId = "RK1005";

    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type)
    {
        // Use projection layer for complex queries
        var model = type.QueryAutoNotify();
        return model is null || model.Properties.IsEmpty;
    }
}
```

## Custom SymbolAnalyzer

For symbols not covered by the specialized base classes, extend `SymbolAnalyzer<TSymbol>` directly:

```csharp
public abstract class LocalVariableAnalyzer<TAttribute> : SymbolAnalyzer<ILocalSymbol>
    where TAttribute : Attribute
{
    // Custom implementation for local variable analysis
    protected abstract bool ShouldReport(ValidSymbol<ILocalSymbol> local);
}
```

## Multi-Diagnostic Analyzers

When a single type can produce multiple diagnostics (e.g., one per invalid property), use `MultiDiagnosticTypeAnalyzer<TItem>`:

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(DiagnosticId, "Pipeline model property uses ImmutableArray<T>",
    Message = "Property '{0}' on pipeline model '{1}' uses ImmutableArray<T> — use EquatableArray<T> instead",
    Category = "PipelineModel")]
public sealed class PipelineModelImmutableArrayAnalyzer : MultiDiagnosticTypeAnalyzer<ValidSymbol<IPropertySymbol>>
{
    public const string DiagnosticId = "DSRK001";

    protected override IEnumerable<ValidSymbol<IPropertySymbol>> GetDiagnosticItems(
        ValidSymbol<INamedTypeSymbol> type)
    {
        if (type.LacksAttribute<PipelineModelAttribute>())
            yield break;

        var properties = type.QueryProperties()
            .ThatAreInstance()
            .Where(x => x.Type.IsImmutableArrayType());

        foreach (var property in properties.GetAll())
            yield return property;
    }

    protected override object[] GetMessageArgs(
        ValidSymbol<INamedTypeSymbol> symbol, ValidSymbol<IPropertySymbol> item)
        => [item.Name, symbol.Name];

    protected override Location GetLocation(
        ValidSymbol<INamedTypeSymbol> symbol, ValidSymbol<IPropertySymbol> item)
        => item.Location;
}
```

### Key Differences from TypeAnalyzer

| | `TypeAnalyzer<TAttribute>` | `MultiDiagnosticTypeAnalyzer<TItem>` |
|---|---|---|
| **Diagnostics per type** | One | Zero or more |
| **Override** | `ShouldReport(type) → bool` | `GetDiagnosticItems(type) → IEnumerable<TItem>` |
| **Location** | Type declaration | Custom via `GetLocation()` |
| **Message args** | Type name only | Custom via `GetMessageArgs()` |
| **Attribute filter** | Generic `TAttribute` parameter | Manual in `GetDiagnosticItems` |

See the [PipelineModel analyzers](../api/projections/pipeline-model.md) for a real-world example of four `MultiDiagnosticTypeAnalyzer` implementations.

## Assembly Attribute Analyzers

When you need to analyze **assembly-level** attributes (e.g., `[assembly: RequiresTool("dotnet-ef")]`) rather than per-symbol attributes, use `AssemblyAttributeAnalyzer<TItem>`:

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
        var globalOptions = context.Options.AnalyzerConfigOptionsProvider.GlobalOptions;

        foreach (var required in items)
        {
            if (globalOptions.TryGetBuildProperty($"Env_{required.Name}", out _))
                continue;

            var rule = required.IsOptional ? GetRule(1) : GetRule(0);
            context.ReportDiagnostic(Diagnostic.Create(rule, required.Location, required.Name));
        }
    }
}
```

### How It Works

1. Annotate with one or more `[Reports]` attributes — multiple diagnostics supported
2. The base class handles `Initialize`, `ConfigureGeneratedCodeAnalysis`, `EnableConcurrentExecution`, and `SupportedDiagnostics`
3. On compilation, scans assembly attributes matching `AttributeFullyQualifiedName`
4. Projects each match through `ValidAttribute` and calls `TryExtractItem`
5. Passes all extracted items to `Analyze` in a single batch

### Key Differences from SymbolAnalyzer

| | `SymbolAnalyzer<TSymbol>` | `AssemblyAttributeAnalyzer<TItem>` |
|---|---|---|
| **Scope** | Per-symbol (`RegisterSymbolAction`) | Compilation-level (`RegisterCompilationAction`) |
| **Trigger** | Symbol with specific attribute | Assembly-level attributes |
| **[Reports]** | Single | One or more (via `AllowMultiple`) |
| **Access rules** | `SupportedDiagnostics[0]` | `GetRule(index)` by declaration order |
| **Item extraction** | Automatic (attribute match) | Manual via `TryExtractItem` |

### Multiple [Reports] Attributes

Each `[Reports]` attribute declares a diagnostic descriptor. Access them by index matching declaration order:

```csharp
GetRule(0) // First [Reports] — e.g., "Required env var not set" (Warning)
GetRule(1) // Second [Reports] — e.g., "Optional env var not set" (Info)
```

## Build Properties

MSBuild properties exposed via `<CompilerVisibleProperty>` appear in `AnalyzerConfigOptions.GlobalOptions` with a `build_property.` prefix. The `BuildPropertyExtensions` handle this prefix automatically.

### Reading Properties

```csharp
using Deepstaging.Roslyn.Analyzers;

var globalOptions = context.Options.AnalyzerConfigOptionsProvider.GlobalOptions;

// String access with fallback
var dataDir = globalOptions.GetBuildProperty("DeepstagingDataDirectory", ".config");

// Typed access (bool, int, long, double)
var isDirty = globalOptions.GetBuildProperty("IsDirty", false);
var count = globalOptions.GetBuildProperty("DirtyCount", 0);

// Try-pattern
if (globalOptions.TryGetBuildProperty("MyProperty", out var value))
{
    // value is non-null and non-empty
}
```

### Discovering Properties

Collect all properties matching a prefix into an `ImmutableDictionary` — useful for forwarding to code fixes via `Diagnostic.Properties`:

```csharp
// All Deepstaging* properties (default prefix)
var props = globalOptions.DiscoverBuildProperties();

// Custom prefix
var gitProps = globalOptions.DiscoverBuildProperties("_DeepstagingGit");

// Forward to code fix via Diagnostic.Properties
context.ReportDiagnostic(Diagnostic.Create(rule, location, props, args));
```

The dictionary keys have the `build_property.` prefix stripped, so a property declared as:

```xml
<CompilerVisibleProperty Include="DeepstagingDataDirectory"/>
```

appears with the key `"DeepstagingDataDirectory"` in the returned dictionary.

### Using in Generators

The extensions work with `AnalyzerConfigOptions` from any context — generators, analyzers, or compilation actions:

```csharp
var gitState = context.AnalyzerConfigOptionsProvider
    .Select(static (provider, _) =>
    {
        var g = provider.GlobalOptions;
        return new GitState(
            Branch: g.GetBuildProperty("GitBranch", "unknown"),
            IsDirty: g.GetBuildProperty("GitIsDirty", false),
            DirtyCount: g.GetBuildProperty("GitDirtyCount", 0)
        );
    });
```

### Using in Code Fixes

Properties forwarded via `Diagnostic.Properties` arrive in code fixes without the `build_property.` prefix:

```csharp
protected override CodeAction CreateFix(
    Project project, ValidSymbol<INamedTypeSymbol> symbol, Diagnostic diagnostic)
{
    diagnostic.Properties.TryGetValue("DeepstagingDataDirectory", out var dataDir);
    dataDir ??= ".config";

    return project.ModifyPropsFileAction<MyProps>("Fix it", dataDir, doc => { ... });
}
```

### Available Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `GetBuildProperty(name, fallback)` | `string` | String value with fallback |
| `GetBuildProperty<T>(name, fallback)` | `T` | Typed value (`bool`, `int`, `long`, `double`) |
| `TryGetBuildProperty(name, out value)` | `bool` | Try-pattern, `false` if missing or empty |
| `DiscoverBuildProperties(prefix?)` | `ImmutableDictionary<string, string?>` | All properties matching prefix (default: `"Deepstaging"`) |

## Tracked File Analyzers

When a source generator produces additional files (e.g., JSON schemas) and you need to detect when they're missing or stale, use `TrackedFileTypeAnalyzer`:

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[TracksFiles("SCHEMA001",
    MissingTitle = "Schema file should be generated",
    MissingMessage = "Type '{0}' is missing its schema file",
    StaleTitle = "Schema file is out of date",
    StaleMessage = "Schema file for '{0}' is stale",
    Category = "Schema")]
public sealed class SchemaFileAnalyzer : TrackedFileTypeAnalyzer
{
    protected override bool IsTrackedFile(string filePath) =>
        filePath.EndsWith(".schema.json");

    protected override bool IsRelevant(ValidSymbol<INamedTypeSymbol> symbol) =>
        symbol.HasAttribute<GenerateSchemaAttribute>();

    protected override string ComputeHash(ValidSymbol<INamedTypeSymbol> symbol) =>
        SchemaHasher.Compute(symbol);

    protected override string? ExtractHash(string fileContent) =>
        SchemaHasher.Extract(fileContent);

    protected override IEnumerable<string> GetExpectedFileNames(ValidSymbol<INamedTypeSymbol> symbol)
    {
        yield return $"{symbol.Name}.schema.json";
    }
}
```

### How It Works

1. On compilation start, scans `AdditionalTexts` for tracked files using `IsTrackedFile`
2. For each type symbol, `IsRelevant` determines if it should be analyzed
3. If **any** expected file is missing → reports the "missing" diagnostic
4. If all files are present but any has a **stale hash** → reports the "stale" diagnostic

Both diagnostics share the same diagnostic ID so a single code fix can handle either case.

### TracksFilesAttribute

Declarative configuration for the missing/stale diagnostic descriptors:

| Property | Default | Description |
|----------|---------|-------------|
| `MissingTitle` | "Files should be generated" | Title for missing diagnostic |
| `MissingMessage` | "{0}" | Message format (`{0}` = symbol name) |
| `MissingSeverity` | Info | Severity for missing diagnostic |
| `StaleTitle` | "Files are out of date" | Title for stale diagnostic |
| `StaleMessage` | "{0}" | Message format (`{0}` = symbol name) |
| `StaleSeverity` | Warning | Severity for stale diagnostic |
| `Category` | "Usage" | Diagnostic category |

### TrackedFiles

Helper class that discovers and indexes additional files with embedded hashes:

```csharp
var files = TrackedFiles.Discover(additionalTexts, isTracked, extractHash);

files.HasAny              // bool — any tracked files found?
files.HasFile("name.json") // bool — specific file exists?
files.GetHash("name.json") // string? — embedded hash value
```

### Build Property Forwarding

`TrackedFileTypeAnalyzer` automatically discovers all `Deepstaging*` build properties via `DiscoverBuildProperties()` and includes them in every reported diagnostic's `Properties` dictionary. Code fixes receive these properties without any additional wiring:

```csharp
// In your code fix — properties arrive automatically
diagnostic.Properties.TryGetValue("DeepstagingDataDirectory", out var dataDir);
```

To expose a new property to the analyzer, add it to your NuGet `.props` file:

```xml
<CompilerVisibleProperty Include="DeepstagingMyNewProperty"/>
```

### Virtual Overrides

| Method | Default | Description |
|--------|---------|-------------|
| `GetMessageArgs(symbol)` | `[symbol.Name]` | Custom message format arguments |
| `GetLocation(symbol)` | `symbol.Location` | Custom diagnostic location |

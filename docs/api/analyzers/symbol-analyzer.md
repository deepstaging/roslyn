# SymbolAnalyzer

`SymbolAnalyzer<TSymbol>` is the primary base class for analyzers that check a single condition on a symbol and report one diagnostic.

## Quick Start

```csharp
[Reports("RK1002", "Type with [AutoNotify] must be partial",
    Description = "Source generators require partial types.",
    Category = "Usage",
    Severity = DiagnosticSeverity.Error)]
public sealed class MustBePartialAnalyzer : TypeAnalyzer
{
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type)
        => type.HasAttribute<AutoNotifyAttribute>() && !type.IsPartial;
}
```

The base class handles `Initialize`, `ConfigureGeneratedCodeAnalysis`, `EnableConcurrentExecution`, `SupportedDiagnostics`, and `RegisterSymbolAction`. You only implement `ShouldReport`.

## How It Works

1. Declare the diagnostic via `[Reports]` on the class
2. The base class infers the `SymbolKind` from the `TSymbol` type parameter
3. On each symbol, wraps it in `ValidSymbol<TSymbol>` and calls `ShouldReport`
4. If `true`, reports the diagnostic at the symbol's location

## Specialized Aliases

These non-generic aliases save you from specifying the symbol type:

| Alias | Equivalent To | Symbol Type |
|-------|---------------|-------------|
| `TypeAnalyzer` | `SymbolAnalyzer<INamedTypeSymbol>` | Classes, structs, interfaces, enums |
| `MethodAnalyzer` | `SymbolAnalyzer<IMethodSymbol>` | Methods |
| `PropertyAnalyzer` | `SymbolAnalyzer<IPropertySymbol>` | Properties |
| `FieldAnalyzer` | `SymbolAnalyzer<IFieldSymbol>` | Fields |
| `EventAnalyzer` | `SymbolAnalyzer<IEventSymbol>` | Events |
| `ParameterAnalyzer` | `SymbolAnalyzer<IParameterSymbol>` | Parameters |
| `NamespaceAnalyzer` | `SymbolAnalyzer<INamespaceSymbol>` | Namespaces |
| `TypeParameterAnalyzer` | `SymbolAnalyzer<ITypeParameterSymbol>` | Generic type parameters |

## Abstract & Virtual Members

| Member | Kind | Default | Description |
|--------|------|---------|-------------|
| `ShouldReport(symbol)` | abstract | — | Return `true` to report the diagnostic |
| `GetMessageArgs(symbol)` | virtual | `[symbol.Name]` | Message format arguments |
| `GetLocation(symbol)` | virtual | `symbol.Location` | Diagnostic location |

## Properties

| Property | Type | Description |
|----------|------|-------------|
| `Rule` | `DiagnosticDescriptor` | The diagnostic descriptor created from `[Reports]` |

## Examples

### Must Be Sealed

```csharp
[Reports("EFF001", "Effects module should be sealed",
    Category = "Design",
    Severity = DiagnosticSeverity.Warning)]
public sealed class ModuleShouldBeSealedAnalyzer : TypeAnalyzer
{
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type)
        => type.HasAttribute<EffectsModuleAttribute>()
           && type.IsClass && !type.IsSealed && !type.IsAbstract;
}
```

### Async Method Naming

```csharp
[Reports("ASYNC001", "Async method should have 'Async' suffix",
    Category = "Naming",
    Severity = DiagnosticSeverity.Warning)]
public sealed class AsyncNamingAnalyzer : MethodAnalyzer
{
    protected override bool ShouldReport(ValidSymbol<IMethodSymbol> method)
        => method.HasAttribute<AsyncAttribute>()
           && method.IsAsync && !method.Name.EndsWith("Async");
}
```

### Backing Field Must Be Private

```csharp
[Reports("RK1003", "Backing field must be private",
    Category = "Encapsulation",
    Severity = DiagnosticSeverity.Error)]
public sealed class FieldMustBePrivateAnalyzer : FieldAnalyzer
{
    protected override bool ShouldReport(ValidSymbol<IFieldSymbol> field)
        => field.HasAttribute<NotifyAttribute>()
           && field.DeclaredAccessibility != Accessibility.Private;
}
```

### Custom Message Args

```csharp
[Reports("SER001", "Serializable type must have parameterless constructor",
    Message = "Type '{0}' has no parameterless constructor",
    Category = "Serialization",
    Severity = DiagnosticSeverity.Error)]
public sealed class NeedsCtorAnalyzer : TypeAnalyzer
{
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type)
        => type.HasAttribute<SerializableAttribute>()
           && !type.Constructors.Any(c => c.Parameters.IsEmpty && !c.IsStatic);

    protected override object[] GetMessageArgs(ValidSymbol<INamedTypeSymbol> type)
        => [type.ToDisplayString()];
}
```

### Using Projection Layer

```csharp
[Reports("RK1005", "AutoNotify type must have at least one notifiable field",
    Category = "Usage",
    Severity = DiagnosticSeverity.Warning)]
public sealed class MustHaveFieldsAnalyzer : TypeAnalyzer
{
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type)
    {
        var model = type.QueryAutoNotify();
        return model is null || model.Properties.IsEmpty;
    }
}
```

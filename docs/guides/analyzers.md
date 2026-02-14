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

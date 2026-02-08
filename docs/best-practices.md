# Best Practices

This guide covers recommended patterns for organizing Roslyn projects using Deepstaging.Roslyn.

## Learning Resources

| Resource | Purpose |
|----------|---------|
| [RoslynKit Template](https://github.com/deepstaging/templates) | Starting point for your own Roslyn projects. Use `dotnet new roslynkit` to scaffold a new solution with the recommended structure. |
| [Deepstaging.Ids](https://github.com/deepstaging/ids) | Reference implementation of a complete Roslyn toolkit. A strongly-typed ID generator inspired by [Andrew Lock's StronglyTypedId](https://github.com/andrewlock/StronglyTypedId), demonstrating the Projection pattern, Emit API extensions, and testing practices. |

## Project Organization

A well-structured Roslyn toolkit separates concerns across multiple projects:

```
MyProject/
├── src/
│   ├── MyProject.RoslynKit/           # Attributes only
│   ├── MyProject.RoslynKit.Projection/ # Queries and Models
│   ├── MyProject.RoslynKit.Generators/ # Source generators
│   ├── MyProject.RoslynKit.Analyzers/  # Diagnostic analyzers
│   ├── MyProject.RoslynKit.CodeFixes/  # Code fix providers
│   └── MyProject.RoslynKit.Tests/      # All tests
```

### Project Responsibilities

| Project | Purpose | Dependencies |
|---------|---------|--------------|
| **RoslynKit** | Attribute definitions only | None (users reference this) |
| **Projection** | Query + Model layer | RoslynKit, Deepstaging.Roslyn |
| **Generators** | Source generation | Projection |
| **Analyzers** | Diagnostics | Projection |
| **CodeFixes** | Quick fixes | Analyzers (for DiagnosticId) |

!!! tip "Why separate Projection?"
    The Projection layer is the **single source of truth** for interpreting your attributes. Both generators and analyzers consume the same queries and models, ensuring consistent behavior.

## The Projection Pattern

The Projection layer converts Roslyn symbols into strongly-typed models through three components:

### 1. AttributeQuery Types

Wrap attribute access with typed properties and defaults:

```csharp
// Attributes/AutoNotifyAttributeQuery.cs
public sealed record AutoNotifyAttributeQuery(AttributeData AttributeData)
    : AttributeQuery(AttributeData)
{
    public bool GenerateBaseImplementation => 
        NamedArg<bool>("GenerateBaseImplementation").OrDefault(true);
}
```

### 2. Models

Simple records capturing data needed for generation:

```csharp
// Models/AutoNotifyModel.cs
public sealed record AutoNotifyModel
{
    public required string Namespace { get; init; }
    public required string TypeName { get; init; }
    public required Accessibility Accessibility { get; init; }
    public required ImmutableArray<NotifyPropertyModel> Properties { get; init; }
}
```

### 3. Query Extensions

Extension methods on `ValidSymbol<T>` that build models:

```csharp
// AutoNotify.cs
extension(ValidSymbol<INamedTypeSymbol> symbol)
{
    public AutoNotifyModel? QueryAutoNotify()
    {
        var properties = symbol.QueryNotifyProperties();
        if (properties.IsEmpty)
            return null;

        return new AutoNotifyModel
        {
            Namespace = symbol.Namespace ?? "",
            TypeName = symbol.Name,
            Accessibility = symbol.Accessibility,
            Properties = properties
        };
    }
}
```

## Generator Pattern

Generators should be thin—just wiring between the projection and writer layers:

```csharp
[Generator]
public sealed class AutoNotifyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var models = context.ForAttribute<AutoNotifyAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol
                .AsValidNamedType()
                .QueryAutoNotify());  // Projection layer

        context.RegisterSourceOutput(models, static (ctx, model) => model
            .WriteAutoNotifyClass()   // Writer layer
            .AddSourceTo(ctx, HintName.From(model.Namespace, model.TypeName)));
    }
}
```

### Writer Classes

Keep code generation logic in dedicated Writer classes:

```csharp
// Writers/AutoNotifyWriter.cs
extension(AutoNotifyModel model)
{
    public OptionalEmit WriteAutoNotifyClass() => TypeBuilder
        .Class(model.TypeName)
        .AsPartial()
        .InNamespace(model.Namespace)
        .WithAccessibility(model.Accessibility)
        // ... build the type
        .Emit();
}
```

!!! note "Why OptionalEmit?"
    `Emit()` returns `OptionalEmit` which safely handles null models. The `AddSourceTo` extension only emits when the result is valid.

## Analyzer Pattern

Use the base classes from Deepstaging.Roslyn for common patterns:

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

### Available Base Classes

| Base Class | Symbol Type | Use Case |
|------------|-------------|----------|
| `TypeAnalyzer<TAttribute>` | `INamedTypeSymbol` | Validate types with attribute |
| `MethodAnalyzer<TAttribute>` | `IMethodSymbol` | Validate methods with attribute |
| `FieldAnalyzer<TAttribute>` | `IFieldSymbol` | Validate fields (checks containing type) |

## CodeFix Pattern

Reference the analyzer's DiagnosticId and use provided helpers:

```csharp
[CodeFix(AutoNotifyMustBePartialAnalyzer.DiagnosticId)]
public sealed class MakePartialClassCodeFixProvider : ClassCodeFix
{
    protected override CodeAction CreateFix(
        Document document, 
        ValidSyntax<ClassDeclarationSyntax> syntax)
    {
        return document.AddPartialModifierAction(syntax);
    }
}
```

### Available Base Classes

| Base Class | Syntax Type | Helpers |
|------------|-------------|---------|
| `ClassCodeFix` | `ClassDeclarationSyntax` | `AddPartialModifierAction` |
| `StructCodeFix` | `StructDeclarationSyntax` | `AddPartialModifierAction` |
| `FieldCodeFix` | `FieldDeclarationSyntax` | `MakePrivateAction` |

## Query Best Practices

### Use Fluent Chains

Compose filters naturally:

```csharp
// Good: fluent chain
var methods = type.QueryMethods()
    .ThatArePublic()
    .ThatAreAsync()
    .WithReturnType("Task")
    .GetAll();

// Avoid: manual filtering
var methods = type.GetMembers()
    .OfType<IMethodSymbol>()
    .Where(m => m.DeclaredAccessibility == Accessibility.Public)
    .Where(m => m.IsAsync)
    .Where(m => m.ReturnType.Name == "Task");
```

### Early Exit with IsNotValid

Use the projection pattern for null-safety:

```csharp
var attr = symbol.GetAttribute("MyAttribute");

if (attr.IsNotValid(out var valid))
    return null;  // Early exit

// valid is guaranteed non-null
var name = valid.NamedArg("Name").OrDefault("Default");
```

### Use OrDefault for Optional Values

```csharp
var maxRetries = attr.NamedArg("MaxRetries").OrDefault(3);
var prefix = attr.NamedArg("Prefix").OrDefault("");
```

## Emit Best Practices

### Use WithEach for Collections

```csharp
TypeBuilder.Class("Generated")
    .WithEach(model.Properties, (builder, prop) => builder
        .AddProperty(prop.Name, prop.Type))
    .Emit();
```

### Use If for Conditional Generation

```csharp
TypeBuilder.Class("Generated")
    .If(model.ImplementsInterface, b => b
        .Implements("INotifyPropertyChanged")
        .AddEvent("PropertyChanged", "PropertyChangedEventHandler?"))
    .Emit();
```

### Parse for Complex Signatures

```csharp
// Instead of building piece by piece:
MethodBuilder.Parse("protected virtual void OnPropertyChanged(string? name = null)")
    .WithBody(b => b.AddStatement("PropertyChanged?.Invoke(this, new(name))"));
```

## Testing Best Practices

### Inherit from RoslynTestBase

```csharp
public class MyGeneratorTests : RoslynTestBase
{
    [Test]
    public async Task Generates_Properties()
    {
        const string source = """
            [AutoNotify]
            public partial class Person
            {
                private string _name;
            }
            """;

        await GenerateWith<AutoNotifyGenerator>(source)
            .ShouldGenerate()
            .WithFileNamed("Person.g.cs")
            .VerifySnapshot();
    }
}
```

### Use ModuleInitializer for References

```csharp
[ModuleInitializer]
public static void Init() => 
    ReferenceConfiguration.AddReferencesFromTypes(
        typeof(AutoNotifyAttribute));
```

### Test Analyzers and Fixes Together

```csharp
[Test]
public async Task Reports_NonPartial_Class()
{
    await AnalyzeWith<AutoNotifyMustBePartialAnalyzer>(source)
        .ShouldReportDiagnostic("RK1002")
        .WithSeverity(DiagnosticSeverity.Error);
}

[Test]
public async Task Fixes_NonPartial_Class()
{
    await AnalyzeAndFixWith<AutoNotifyMustBePartialAnalyzer, MakePartialCodeFix>(source)
        .ForDiagnostic("RK1002")
        .ShouldProduce(expectedSource);
}
```

## File Organization

### Projection Layer

```
Projection/
├── Attributes/           # AttributeQuery wrapper types
│   ├── AutoNotifyAttributeQuery.cs
│   └── AlsoNotifyAttributeQuery.cs
├── Models/               # Data models
│   ├── AutoNotifyModel.cs
│   └── NotifyPropertyModel.cs
├── Queries.cs            # ValidAttribute → AttributeQuery conversions
├── AutoNotify.cs         # ValidSymbol extensions for AutoNotify
└── GlobalUsings.cs
```

### Generators Layer

```
Generators/
├── Writers/              # Code generation logic
│   └── AutoNotifyWriter.cs
├── AutoNotifyGenerator.cs
└── GlobalUsings.cs
```

### Analyzers Layer

```
Analyzers/
├── AutoNotifyMustBePartialAnalyzer.cs
├── AutoNotifyFieldMustBePrivateAnalyzer.cs
└── README.md             # Document all diagnostic IDs
```

## Summary

1. **Separate concerns** into distinct projects (Attributes, Projection, Generators, Analyzers, CodeFixes)
2. **Use the Projection pattern** as the single source of truth for attribute interpretation
3. **Keep generators thin**—delegate to projection queries and writer extensions
4. **Leverage base classes** for analyzers and code fixes
5. **Use fluent APIs** for queries and emit—they're more readable and composable
6. **Test thoroughly** with RoslynTestBase and snapshot verification

# Generator Pattern

Generators should be thin—just wiring between the projection and writer layers.

## Basic Structure

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

### More Generator Examples

```csharp
// Generator with multiple output files
[Generator]
public sealed class StrongIdGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var models = context.ForAttribute<StrongIdAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol
                .AsValidNamedType()
                .QueryStrongId());

        // Main type generation
        context.RegisterSourceOutput(models, static (ctx, model) => model
            .WriteStrongIdStruct()
            .AddSourceTo(ctx, HintName.From(model.Namespace, model.TypeName)));

        // Optional JSON converter
        context.RegisterSourceOutput(models, static (ctx, model) =>
        {
            if (model?.GenerateJsonConverter != true) return;
            model.WriteJsonConverter()
                .AddSourceTo(ctx, HintName.From(model.Namespace, $"{model.TypeName}JsonConverter"));
        });
    }
}

// Generator combining multiple sources
[Generator]
public sealed class EffectsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var modules = context.ForAttribute<EffectsModuleAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol
                .AsValidNamedType()
                .QueryEffectsModule());

        var compilation = context.CompilationProvider;

        // Combine with compilation for additional context
        context.RegisterSourceOutput(
            modules.Combine(compilation),
            static (ctx, tuple) =>
            {
                var (model, compilation) = tuple;
                model.WriteEffectsModule(compilation)
                    .AddSourceTo(ctx, HintName.From(model.Namespace, model.TypeName));
            });
    }
}

// Generator with post-initialization (static files)
[Generator]
public sealed class FrameworkGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Emit static helper types once
        context.RegisterPostInitializationOutput(static ctx =>
        {
            ctx.AddSource("FrameworkAttributes.g.cs", """
                namespace MyFramework;
                
                [AttributeUsage(AttributeTargets.Class)]
                internal sealed class GeneratedAttribute : Attribute { }
                """);
        });

        // Then emit per-type code
        var models = context.ForAttribute<ServiceAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryService());

        context.RegisterSourceOutput(models, static (ctx, model) => model
            .WriteServiceProxy()
            .AddSourceTo(ctx, HintName.From(model.Namespace, model.TypeName)));
    }
}
```

## Writer Classes

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

### More Writer Examples

```csharp
// Writer generating properties from fields
extension(AutoNotifyModel model)
{
    public OptionalEmit WriteAutoNotifyClass() => TypeBuilder
        .Class(model.TypeName)
        .AsPartial()
        .InNamespace(model.Namespace)
        .WithAccessibility(model.Accessibility)
        .Implements("INotifyPropertyChanged")
        .AddEvent("PropertyChanged", "PropertyChangedEventHandler?")
        .WithEach(model.Properties, WriteProperty)
        .AddMethod(WriteOnPropertyChanged())
        .Emit();

    static TypeBuilder WriteProperty(TypeBuilder builder, NotifyPropertyModel prop) => builder
        .AddProperty(prop.PropertyName, prop.TypeName, p => p
            .WithGetter(b => b.AddStatement($"return {prop.FieldName};"))
            .WithSetter(b => b
                .AddStatement($"if ({prop.FieldName} == value) return;")
                .AddStatement($"{prop.FieldName} = value;")
                .AddStatement($"OnPropertyChanged(nameof({prop.PropertyName}));")
                .WithEach(prop.AlsoNotify, (bb, name) => bb
                    .AddStatement($"OnPropertyChanged(nameof({name});"))));

    static MethodBuilder WriteOnPropertyChanged() => MethodBuilder
        .Parse("protected virtual void OnPropertyChanged(string? propertyName = null)")
        .WithBody(b => b
            .AddStatement("PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));"));
}

// Writer generating a complete class with constructor
extension(StrongIdModel model)
{
    public OptionalEmit WriteStrongIdStruct() => TypeBuilder
        .Struct(model.TypeName)
        .AsPartial()
        .AsReadonly()
        .InNamespace(model.Namespace)
        .Implements("IEquatable<" + model.TypeName + ">")
        .AddField("_value", model.BackingType, f => f.AsPrivate().AsReadonly())
        .AddConstructor(c => c
            .AddParameter("value", model.BackingType)
            .WithBody(b => b.AddStatement("_value = value;")))
        .AddProperty("Value", model.BackingType, p => p
            .WithGetter(b => b.AddStatement("return _value;")))
        .AddMethod(WriteEqualsMethod(model))
        .AddMethod(WriteGetHashCodeMethod())
        .AddMethod(WriteToStringMethod())
        .Emit();
}

// Writer with conditional sections
extension(EffectsModuleModel model)
{
    public OptionalEmit WriteEffectsModule() => TypeBuilder
        .Class(model.TypeName + "Effects")
        .AsPartial()
        .AsSealed()
        .InNamespace(model.Namespace)
        .WithEach(model.Effects, WriteEffectMethod)
        .If(model.HasAsyncEffects, b => b
            .AddUsing("System.Threading.Tasks"))
        .If(model.RequiresDependencyInjection, b => b
            .AddConstructor(c => c
                .AddParameter("provider", "IServiceProvider")
                .WithBody(bb => bb.AddStatement("_provider = provider;")))
            .AddField("_provider", "IServiceProvider", f => f.AsPrivate().AsReadonly()))
        .Emit();
}
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

### More Emit Patterns

```csharp
// Chained conditionals
TypeBuilder.Class("Repository")
    .If(model.UseCaching, b => b.AddField("_cache", "ICache"))
    .If(model.UseLogging, b => b.AddField("_logger", "ILogger"))
    .If(model.UseMetrics, b => b.AddField("_metrics", "IMetrics"))
    .Emit();

// Nested builders
TypeBuilder.Class("Outer")
    .AddNestedType(TypeBuilder
        .Class("Inner")
        .AsPrivate()
        .AddProperty("Value", "int"))
    .Emit();

// XML documentation
TypeBuilder.Class("Documented")
    .WithXmlDoc(d => d
        .Summary("A well-documented class.")
        .Remarks("Use this for important things."))
    .AddMethod(m => m
        .WithName("Calculate")
        .WithReturnType("int")
        .AddParameter("input", "string")
        .WithXmlDoc(d => d
            .Summary("Calculates something.")
            .Param("input", "The input value.")
            .Returns("The calculated result.")))
    .Emit();

// Attributes on generated code
TypeBuilder.Class("Generated")
    .WithAttribute("GeneratedCode", a => a
        .AddArgument("\"MyGenerator\"")
        .AddArgument("\"1.0.0\""))
    .WithAttribute("ExcludeFromCodeCoverage")
    .Emit();
```

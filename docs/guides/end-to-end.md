# End-to-End Walkthrough

This guide traces one feature — **StrongId** — across all five layers of a Roslyn toolkit, showing how attributes, projections, generators, analyzers, and code fixes compose into a complete developer experience.

The source code is from [Deepstaging](https://github.com/deepstaging/deepstaging) and the [Samples](https://github.com/deepstaging/samples) repository.

## What the User Writes

A developer declares a strongly-typed ID by applying `[StrongId]` to a partial struct:

```csharp
[StrongId(Converters = IdConverters.EfCoreValueConverter)]
public readonly partial struct WorkshopId;

[StrongId(Converters = IdConverters.EfCoreValueConverter)]
public readonly partial struct SessionId;

[StrongId(Converters = IdConverters.EfCoreValueConverter)]
public readonly partial struct AttendeeId;
```

That's all the user writes. The generator produces the full implementation: constructor, `Value` property, `IEquatable<T>`, `IComparable<T>`, `IParsable<T>`, `ToString()`, factory methods, and an EF Core `ValueConverter`.

## Layer 1: Attributes

The user-facing package contains only the attribute and supporting enums. No Roslyn dependencies.

<a href="https://github.com/deepstaging/deepstaging/blob/main/src/Deepstaging/Ids/StrongIdAttribute.cs" target="_blank">`StrongIdAttribute.cs`</a>

```csharp
[AttributeUsage(AttributeTargets.Struct)]
public sealed class StrongIdAttribute : Attribute
{
    public BackingType BackingType { get; set; } = BackingType.Guid;
    public IdConverters Converters { get; set; } = IdConverters.None;
}
```

<a href="https://github.com/deepstaging/deepstaging/blob/main/src/Deepstaging/Ids/BackingType.cs" target="_blank">`BackingType.cs`</a>

```csharp
public enum BackingType { Guid, Int, Long, String }
```

<a href="https://github.com/deepstaging/deepstaging/blob/main/src/Deepstaging/Ids/IdConverters.cs" target="_blank">`IdConverters.cs`</a>

```csharp
[Flags]
public enum IdConverters
{
    None = 0,
    SystemTextJson = 1,
    NewtonsoftJson = 2,
    EfCoreValueConverter = 4,
    DapperTypeHandler = 8,
    TypeConverter = 16,
}
```

## Layer 2: Projection

The Projection layer extracts attribute data and builds a pipeline-safe model.

### AttributeQuery

<a href="https://github.com/deepstaging/deepstaging/blob/main/src/Deepstaging.Projection/Ids/Attributes/StrongIdAttributeQuery.cs" target="_blank">`StrongIdAttributeQuery.cs`</a>

```csharp
public sealed record StrongIdAttributeQuery(AttributeData AttributeData)
    : AttributeQuery(AttributeData)
{
    public BackingType BackingType =>
        NamedArg<int>(nameof(StrongIdAttribute.BackingType))
            .ToEnum<BackingType>()
            .OrDefault(BackingType.Guid);

    public ValidSymbol<INamedTypeSymbol> BackingTypeSymbol(SemanticModel model) =>
        BackingType switch
        {
            BackingType.Guid => model.WellKnownSymbols.Guid,
            BackingType.Int => model.WellKnownSymbols.Int32,
            BackingType.Long => model.WellKnownSymbols.Int64,
            BackingType.String => model.WellKnownSymbols.String,
            _ => throw new ArgumentOutOfRangeException(nameof(BackingType))
        };

    public IdConverters Converters =>
        NamedArg<int>(nameof(StrongIdAttribute.Converters))
            .ToEnum<IdConverters>()
            .OrDefault(IdConverters.None);
}
```

### Model

<a href="https://github.com/deepstaging/deepstaging/blob/main/src/Deepstaging.Projection/Ids/Models/StrongIdModel.cs" target="_blank">`StrongIdModel.cs`</a>

```csharp
[PipelineModel]
public sealed record StrongIdModel
{
    public required string Namespace { get; init; }
    public required string TypeName { get; init; }
    public required string Accessibility { get; init; }
    public required BackingType BackingType { get; init; }
    public required IdConverters Converters { get; init; }
    public required TypeSnapshot BackingTypeSnapshot { get; init; }
}
```

### Query

<a href="https://github.com/deepstaging/deepstaging/blob/main/src/Deepstaging.Projection/Ids/Queries.cs" target="_blank">`Queries.cs`</a>

```csharp
extension(ValidSymbol<INamedTypeSymbol> symbol)
{
    public StrongIdModel ToStrongIdModel(SemanticModel model) =>
        symbol.GetAttribute<StrongIdAttribute>()
            .Map(attr => attr.AsQuery<StrongIdAttributeQuery>())
            .Map(attr => new StrongIdModel
            {
                Namespace = symbol.Namespace ?? "",
                TypeName = symbol.Name,
                Accessibility = symbol.AccessibilityString,
                BackingType = attr.BackingType,
                BackingTypeSnapshot = attr.BackingTypeSymbol(model).ToSnapshot(),
                Converters = attr.Converters
            })
            .OrThrow($"Expected '{symbol.FullyQualifiedName}' to have StrongIdAttribute.");
}
```

## Layer 3: Generator + Writer

The generator is thin — 14 lines of wiring:

<a href="https://github.com/deepstaging/deepstaging/blob/main/src/Deepstaging.Generators/StrongIdGenerator.cs" target="_blank">`StrongIdGenerator.cs`</a>

```csharp
[Generator]
public sealed class StrongIdGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var models = context.ForAttribute<StrongIdAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol
                .AsValidNamedType()
                .ToStrongIdModel(ctx.SemanticModel));

        context.RegisterSourceOutput(models, static (ctx, model) =>
        {
            model.WriteStrongId()
                .AddSourceTo(ctx, HintName.From(model.Namespace, model.TypeName));
        });
    }
}
```

The writer does the heavy lifting:

<a href="https://github.com/deepstaging/deepstaging/blob/main/src/Deepstaging.Generators/Writers/Ids/StrongIdWriter.cs" target="_blank">`StrongIdWriter.cs`</a>

```csharp
extension(StrongIdModel model)
{
    public OptionalEmit WriteStrongId()
    {
        var backingType = model.BackingTypeSnapshot;
        var valueProperty = PropertyBuilder
            .Parse($"public {backingType.FullyQualifiedName} Value {{ get; }}");

        return TypeBuilder
            .Parse($"{model.Accessibility} partial struct {model.TypeName}")
            .InNamespace(model.Namespace)
            .AddProperty(valueProperty)
            .AddConstructor(model)
            .ImplementsIEquatable(backingType, valueProperty)
            .ImplementsIComparable(backingType, valueProperty)
            .ImplementsIParsable(backingType)
            .OverridesToString(
                model.BackingType == BackingType.String
                    ? $"{valueProperty.Name} ?? \"\""
                    : $"{valueProperty.Name}.ToString()",
                true)
            .AddFactoryMethods(model)
            .AddConverters(model, valueProperty)
            .Emit();
    }
}
```

## Layer 4: Analyzers

Analyzers enforce correctness at compile time. StrongId has two:

```csharp
// The struct must be partial (so the generator can extend it)
[Reports(Diagnostics.StrongIdMustBePartial)]
public sealed class StrongIdMustBePartialAnalyzer : TypeAnalyzer<StrongIdAttribute>
{
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> symbol) =>
        !symbol.IsPartial();
}

// The struct should be readonly (value semantics)
[Reports(Diagnostics.StrongIdShouldBeReadonly)]
public sealed class StrongIdShouldBeReadonlyAnalyzer : TypeAnalyzer<StrongIdAttribute>
{
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> symbol) =>
        !symbol.IsReadonly();
}
```

Each analyzer is a single class, a single override, returning a boolean. The base class handles all Roslyn registration boilerplate.

## Layer 5: Code Fixes

Code fixes pair with analyzers to offer one-click repairs:

```csharp
[CodeFix(Diagnostics.StrongIdMustBePartial)]
[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class StructMustBePartialCodeFix : StructCodeFix
{
    protected override CodeAction CreateFix(
        Document document,
        ValidSyntax<StructDeclarationSyntax> syntax) =>
        document.AddPartialModifierAction(syntax);
}

[CodeFix(Diagnostics.StrongIdShouldBeReadonly)]
[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class StructShouldBeReadonlyCodeFix : StructCodeFix
{
    protected override CodeAction CreateFix(
        Document document,
        ValidSyntax<StructDeclarationSyntax> syntax) =>
        document.AddModifierAction(syntax, SyntaxKind.ReadOnlyKeyword, "Add 'readonly' modifier");
}
```

## The Result

From the user's perspective, they write:

```csharp
[StrongId(Converters = IdConverters.EfCoreValueConverter)]
public readonly partial struct WorkshopId;
```

And get:

- ✅ A complete struct with `Value` property and constructor
- ✅ `IEquatable<WorkshopId>`, `IComparable<WorkshopId>`, `IParsable<WorkshopId>`
- ✅ `ToString()`, `Parse()`, `TryParse()`, factory methods
- ✅ An EF Core `ValueConverter<WorkshopId, Guid>` for database mapping
- ✅ Compile-time errors if `partial` or `readonly` is missing — with one-click fixes

## Key Takeaways

1. **Attributes are minimal** — just data containers, no Roslyn dependency
2. **Projection is the single source of truth** — both generators and analyzers consume the same queries and models
3. **Generators are thin** — they wire pipelines, writers do the work
4. **Analyzers are single-purpose** — one class, one rule, one boolean
5. **Code fixes pair with analyzers** — same diagnostic ID, one-click resolution
6. **The user experience is seamless** — declare intent with an attribute, get a complete implementation with compile-time safety

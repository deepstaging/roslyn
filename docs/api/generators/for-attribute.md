# ForAttribute / AttributeMapper

`ForAttribute` is a fluent extension on `IncrementalGeneratorInitializationContext` that wraps Roslyn's `ForAttributeWithMetadataName` with a cleaner API.

## Usage

### By Generic Type

```csharp
var models = context.ForAttribute<StrongIdAttribute>()
    .Map(static (ctx, _) => ExtractModel(ctx));
```

The fully qualified name is resolved automatically from the type.

### By Type Object

Supports open generic types without backtick-arity syntax:

```csharp
var models = context.ForAttribute(typeof(MyAttribute<>))
    .Map(static (ctx, _) => ExtractModel(ctx));
```

### By String Name

```csharp
var models = context.ForAttribute("MyApp.MyAttribute")
    .Map(static (ctx, _) => ExtractModel(ctx));
```

## AttributeMapper Methods

| Method | Description |
|--------|-------------|
| `Map<TModel>(builder)` | Map each attributed symbol to a model. Model type is inferred. |
| `Where<TModel>(syntaxPredicate, builder)` | Map with a syntax node filter applied first. |

### Map

```csharp
context.ForAttribute<RepoAttribute>()
    .Map(static (ctx, _) => new RepoModel(
        ctx.TargetSymbol.ContainingNamespace?.ToDisplayString(),
        ctx.TargetSymbol.Name));
```

Null returns are automatically filtered out — return `null` to skip a symbol.

### Where (with Syntax Predicate)

```csharp
context.ForAttribute<RepoAttribute>().Where(
    static (node, _) => node is ClassDeclarationSyntax,
    static (ctx, _) => ExtractModel(ctx)
);
```

The syntax predicate runs before the semantic model is loaded, providing a fast filter.

## Null Handling

Both `Map` and `Where` automatically filter nulls from the output. If your builder returns `null`, the symbol is silently skipped:

```csharp
.Map(static (ctx, _) =>
{
    if (ctx.TargetSymbol is not INamedTypeSymbol type)
        return null;  // Skipped — no output for this symbol

    return new MyModel(type.Name);
})
```

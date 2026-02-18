# Customizable Templates & Scaffolding

!!! warning "Experimental"
    This API is experimental and has no consumer usage yet. The design may change significantly in future releases. Do not build critical workflows on it.

The customizable template system lets consumers override generated code by providing their own Scriban templates, and auto-generates starter templates to make that easy.

## Architecture

```
Generator
  │
  ├── Build default emit (TypeBuilder, etc.)
  │         │
  │         ├── TemplateMap<TModel>.Bind(value, m => m.Property)
  │         │     Records which model properties map to which output
  │         │
  │         └── CustomizableEmit
  │               │
  │               ├── Has: default emit + template name + model + bindings
  │               │
  │               ├── ResolveFrom(UserTemplates)
  │               │     ├── User template exists → render it with model
  │               │     └── No user template → use default emit
  │               │
  │               └── ScaffoldEmitter.EmitScaffold(ctx, customizable, trigger)
  │                     ├── TemplateScaffold.Generate(customizable)
  │                     │     Replaces bound values with {{ model.property }}
  │                     └── Emits [assembly: AssemblyMetadata(...)]
  │                           ├── ScaffoldAvailableAnalyzer → DSRK005 info
  │                           └── ScaffoldTemplateCodeFix → creates file
  │
  └── Output
```

---

## TemplateMap&lt;TModel&gt;

Records which model properties were used during emit construction. This enables scaffold generation — the system knows which values to replace with `{{ model.property_name }}` placeholders.

```csharp
var map = new TemplateMap<MyModel>();

// Bind records the mapping AND returns the value unchanged (pass-through)
var builder = TypeBuilder.ForClass(map.Bind(model.Name, m => m.Name))
    .InNamespace(map.Bind(model.Namespace, m => m.Namespace))
    .AddField("MaxRetries", map.Bind(model.MaxRetries.ToString(), m => m.MaxRetries));
```

| Member | Returns | Description |
|--------|---------|-------------|
| `Bind<T>(T value, Expression<Func<TModel, T>> selector)` | `T` | Records mapping, returns value unchanged |
| `Bindings` | `IReadOnlyList<TemplateBinding>` | All recorded bindings |

**TemplateBinding** — `readonly struct` with `PropertyPath` (dot-separated, e.g., `Type.CodeName`) and `Value` (string representation). Only non-null, non-empty values are recorded.

---

## CustomizableEmit

Bridges `OptionalEmit` (the default code) with user-overridable Scriban templates.

### Creating

```csharp
OptionalEmit defaultEmit = builder.Emit();

CustomizableEmit customizable = defaultEmit
    .WithUserTemplate("MyProject.MyType", model, map);
```

### Resolving

```csharp
// Option 1: Resolve manually
OptionalEmit resolved = customizable.ResolveFrom(userTemplates);

// Option 2: Resolve and add to output in one step
customizable.AddSourceTo(context, "MyType.g.cs", userTemplates);
```

Resolution logic:

1. If a user template exists → render it with the model
2. Validate rendered output is valid C# (report `DSRK006` if not)
3. If no user template → use the default emit

| Property | Type | Description |
|----------|------|-------------|
| `DefaultEmit` | `OptionalEmit` | The default generated code |
| `TemplateName` | `string` | Namespaced template identifier |
| `Model` | `object?` | Model for template rendering |
| `Bindings` | `IReadOnlyList<TemplateBinding>` | Property bindings from TemplateMap |

---

## UserTemplates

Provides access to user-defined Scriban templates discovered from `AdditionalTexts`.

### Pipeline Setup

```csharp
public void Initialize(IncrementalGeneratorInitializationContext context)
{
    var userTemplates = context.UserTemplatesProvider();

    var combined = models.Combine(userTemplates);

    context.RegisterSourceOutput(combined, (ctx, pair) =>
    {
        var (model, templates) = pair;
        customizable.AddSourceTo(ctx, filename, templates);
    });
}
```

| Member | Returns | Description |
|--------|---------|-------------|
| `From(ImmutableArray<AdditionalText> additionalTexts)` | `UserTemplates` | Load from additional texts |
| `Empty` | `UserTemplates` | Empty instance (no user templates) |
| `TryRender(string templateName, object? model)` | `RenderResult?` | Render if exists, null otherwise |
| `HasTemplate(string templateName)` | `bool` | Check existence |
| `GetFilePath(string templateName)` | `string?` | Get file path for diagnostics |

**Convention:** User templates are discovered at `Templates/{ProjectPrefix}/{Name}.scriban-cs`. For example, `Templates/MyProject/MyType.scriban-cs` matches template name `MyProject.MyType`.

---

## Scaffolding

The scaffold system auto-generates starter templates from default emit output by replacing bound values with `{{ model.property }}` placeholders.

### ScaffoldEmitter

Emits `[assembly: AssemblyMetadata]` attributes to advertise customizable templates:

```csharp
ScaffoldEmitter.EmitScaffold(ctx, customizable, "MyNamespace.MyAttribute");
```

| Key Pattern | Value |
|-------------|-------|
| `Deepstaging.Scaffold:{Name}` | Trigger attribute fully qualified name |
| `Deepstaging.Scaffold:{Name}:Content` | Scaffold template content |

### TemplateScaffold

Generates a scaffold by replacing bound values with Scriban placeholders:

```csharp
string? scaffold = TemplateScaffold.Generate(customizable);
```

Given default emit `namespace MyApp; public partial class Customer { }` with bindings `Namespace` → `"MyApp"`, `Name` → `"Customer"`:

```scriban
namespace {{ model.namespace }};
public partial class {{ model.name }} { }
```

Returns `null` if the default emit is invalid. Replacements are sorted by value length (descending) to prevent partial matches.

### ScaffoldMetadata / ScaffoldInfo

Read scaffold info from compilation assembly attributes:

```csharp
ImmutableArray<ScaffoldInfo> scaffolds = ScaffoldMetadata.ReadFrom(compilation);
```

`ScaffoldInfo` is a `readonly record struct` with `TemplateName`, `TriggerAttributeName`, and `Scaffold` (nullable content).

### Bundled Analyzer & Code Fix

**ScaffoldAvailableAnalyzer** — Reports info diagnostic when a trigger attribute has a customizable template but no user template file exists.

| ID | Severity | Message |
|----|----------|---------|
| `DSRK005` | Info | Customizable template available for `{type}` |

**ScaffoldTemplateCodeFix** — Creates `Templates/{templateName}.scriban-cs` with scaffold content when the user invokes the DSRK005 code fix.

---

## Diagnostics

| ID | Severity | Description |
|----|----------|-------------|
| `DSRK005` | Info | Customizable template available (code fix creates starter template) |
| `DSRK006` | Error | User template rendered invalid C# |
| `DSRK007` | Error | User template has Scriban parse or render errors |

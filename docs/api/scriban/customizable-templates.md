# Customizable Templates

!!! warning "Experimental"
    This API is experimental and has no consumer usage yet. The design may change significantly in future releases. Do not build critical workflows on it.

The customizable template system lets consumers override generated code by providing their own Scriban templates. This is built from three types working together: `TemplateMap<TModel>`, `CustomizableEmit`, and `UserTemplates`.

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
  │               └── ResolveFrom(UserTemplates)
  │                     │
  │                     ├── User template exists → render it with model
  │                     └── No user template → use default emit
  │
  └── Output
```

## TemplateMap&lt;TModel&gt;

Records which model properties were used during emit construction. This enables scaffold generation — the system knows which values to replace with `{{ model.property_name }}` placeholders.

### Usage

```csharp
var map = new TemplateMap<MyModel>();

// Bind records the mapping AND returns the value unchanged (pass-through)
var builder = TypeBuilder.ForClass(map.Bind(model.Name, m => m.Name))
    .InNamespace(map.Bind(model.Namespace, m => m.Namespace))
    .AddField("MaxRetries", map.Bind(model.MaxRetries.ToString(), m => m.MaxRetries));
```

### Members

| Member | Returns | Description |
|--------|---------|-------------|
| `Bind<T>(T value, Expression<Func<TModel, T>> selector)` | `T` | Records mapping, returns value unchanged |
| `Bindings` | `IReadOnlyList<TemplateBinding>` | All recorded bindings |

### TemplateBinding

| Property | Type | Description |
|----------|------|-------------|
| `PropertyPath` | `string` | Dot-separated path (e.g., `Type.CodeName`) |
| `Value` | `string` | String representation of the bound value |

!!! note
    Only non-null, non-empty string values are recorded as bindings.

---

## CustomizableEmit

Bridges `OptionalEmit` (the default code) with user-overridable Scriban templates.

### Creating

Use the extension methods on `OptionalEmit`:

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

### Properties

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

    // Combine with your model pipeline
    var combined = models.Combine(userTemplates);

    context.RegisterSourceOutput(combined, (ctx, pair) =>
    {
        var (model, templates) = pair;
        customizable.AddSourceTo(ctx, filename, templates);
    });
}
```

### Members

| Member | Returns | Description |
|--------|---------|-------------|
| `From(ImmutableArray<AdditionalText> additionalTexts)` | `UserTemplates` | Load from additional texts |
| `Empty` | `UserTemplates` | Empty instance (no user templates) |
| `TryRender(string templateName, object? model)` | `RenderResult?` | Render if exists, null otherwise |
| `HasTemplate(string templateName)` | `bool` | Check existence |
| `GetFilePath(string templateName)` | `string?` | Get file path for diagnostics |

### Convention

User templates are discovered by file name convention:

```
Templates/{ProjectPrefix}/{Name}.scriban-cs
```

For example, `Templates/MyProject/MyType.scriban-cs` matches template name `MyProject.MyType`.

## Diagnostics

| ID | Severity | Description |
|----|----------|-------------|
| `DSRK006` | Error | User template rendered invalid C# — includes template name and syntax errors |
| `DSRK007` | Error | User template has Scriban parse or render errors — includes template name and details |

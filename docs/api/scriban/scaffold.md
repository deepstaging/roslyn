# Template Scaffolding

The scaffold system auto-generates starter templates from default emit output, making it easy for consumers to begin customizing generated code.

## How It Works

```
Generator emits code
  │
  ├── CustomizableEmit has: default code + TemplateMap bindings
  │
  └── ScaffoldEmitter.EmitScaffold(ctx, customizable, triggerAttribute)
        │
        ├── TemplateScaffold.Generate(customizable)
        │     Replaces bound values with {{ model.property }} placeholders
        │
        └── Emits [assembly: AssemblyMetadata("Deepstaging.Scaffold:Name", ...)]
              │
              ├── ScaffoldAvailableAnalyzer reads metadata
              │     Reports DSRK005 info diagnostic if no user template exists
              │
              └── ScaffoldTemplateCodeFix offers "Create template" action
                    Creates Templates/{name}.scriban-cs with scaffold content
```

## ScaffoldEmitter

Emits `[assembly: AssemblyMetadata]` attributes to advertise customizable templates to downstream analyzers.

```csharp
ScaffoldEmitter.EmitScaffold(
    ctx,                      // SourceProductionContext
    customizable,             // CustomizableEmit with bindings
    "MyNamespace.MyAttribute" // Trigger attribute FQN
);
```

### Metadata Keys

| Key Pattern | Value |
|-------------|-------|
| `Deepstaging.Scaffold:{Name}` | Trigger attribute fully qualified name |
| `Deepstaging.Scaffold:{Name}:Content` | Scaffold template content |

## TemplateScaffold

Generates a Scriban template scaffold by replacing bound values in the default emit with `{{ model.property }}` placeholders.

```csharp
string? scaffold = TemplateScaffold.Generate(customizable);
```

Returns `null` if the default emit is invalid. Replacements are sorted by value length (descending) to prevent partial matches.

### Example

Given default emit:

```csharp
namespace MyApp;
public partial class Customer { }
```

With bindings: `Namespace` → `"MyApp"`, `Name` → `"Customer"`

Scaffold output:

```scriban
namespace {{ model.namespace }};
public partial class {{ model.name }} { }
```

## ScaffoldInfo

`readonly record struct` containing discovered scaffold metadata.

| Property | Type | Description |
|----------|------|-------------|
| `TemplateName` | `string` | Namespaced template name |
| `TriggerAttributeName` | `string` | Trigger attribute fully qualified name |
| `Scaffold` | `string?` | Scaffold content (null if not provided) |

## ScaffoldMetadata

Reads scaffold info from compilation assembly attributes:

```csharp
ImmutableArray<ScaffoldInfo> scaffolds = ScaffoldMetadata.ReadFrom(compilation);
```

## Bundled Analyzer & Code Fix

### ScaffoldAvailableAnalyzer

Reports an info diagnostic when a type has a trigger attribute with a customizable template but no user template file exists yet.

| ID | Severity | Message |
|----|----------|---------|
| `DSRK005` | Info | Customizable template available for `{type}` |

The analyzer reads `AssemblyMetadata` from the compilation and checks `AdditionalFiles` for matching templates.

### ScaffoldTemplateCodeFix

Extends `AdditionalDocumentCodeFix` to create a starter template file from scaffold content when the user invokes the DSRK005 code fix.

Creates: `Templates/{templateName}.scriban-cs`

# Context Extensions

Extension methods for adding Scriban template output to generator contexts.

## SourceProductionContext

The primary extension for the main generation phase:

```csharp
context.AddFromTemplate(
    Named("MyTemplate.scriban-cs"),
    hintName: "MyType.g.cs",
    context: model,
    diagnostics: optionalDiagnosticList,
    format: true);
```

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `template` | `TemplateName` | required | Template to render |
| `hintName` | `string` | required | Output file name |
| `context` | `object?` | `null` | Model for rendering |
| `diagnostics` | `IReadOnlyList<Diagnostic>?` | `null` | Additional diagnostics to report |
| `format` | `bool` | `false` | Format output with Roslyn formatter |

If rendering fails, the error diagnostic is reported and no source is added.

## IncrementalGeneratorPostInitializationContext

For static output that doesn't depend on user source code:

```csharp
context.AddFromTemplate(
    Named("StaticTypes.scriban-cs"),
    hintName: "StaticTypes.g.cs",
    context: model,
    format: false);
```

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `template` | `TemplateName` | required | Template to render |
| `hintName` | `string` | required | Output file name |
| `context` | `object?` | `null` | Model for rendering |
| `format` | `bool` | `false` | Format output with Roslyn formatter |

## UserTemplatesProvider

Pipeline extension for discovering user templates from `AdditionalTexts`:

```csharp
public void Initialize(IncrementalGeneratorInitializationContext context)
{
    IncrementalValueProvider<UserTemplates> userTemplates =
        context.UserTemplatesProvider();

    // Combine with model pipeline
    var combined = models.Combine(userTemplates);
}
```

Filters `AdditionalTexts` to files with known Scriban extensions (`.scriban-cs`, `.scriban`, `.scriban-html`, `.scriban-txt`, `.liquid`).

## ScribanExtension

`readonly struct` representing a validated Scriban file extension.

| Static Instance | Value |
|-----------------|-------|
| `ScribanExtension.CSharp` | `.scriban-cs` |
| `ScribanExtension.Scriban` | `.scriban` |
| `ScribanExtension.Html` | `.scriban-html` |
| `ScribanExtension.Text` | `.scriban-txt` |
| `ScribanExtension.Liquid` | `.liquid` |

| Method | Description |
|--------|-------------|
| `From(string extension)` | Create with validation (throws `ArgumentException`) |
| `IsKnown(string extension)` | Check if extension is recognized |
| `IsKnownSuffix(string name)` | Check if file name ends with known extension |

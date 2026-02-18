# Template

`Template` is a `sealed record` that loads, parses, and renders Scriban templates from embedded resources with automatic two-level caching.

## Rendering

### From Embedded Resource

```csharp
private static readonly Func<string, TemplateName> Named =
    TemplateName.ForGenerator<MyGenerator>();

var result = Template.RenderTemplate(Named("MyTemplate.scriban-cs"), model);
```

### From Raw Text

```csharp
var result = Template.RenderFromText(templateText, "MyTemplate", model);
```

Both methods return a [`RenderResult`](#renderresult).

## Properties

| Property | Type | Description |
|----------|------|-------------|
| `Name` | `TemplateName` | Template identifier |
| `Context` | `object?` | Render context (model) |
| `Text` | `string` | Template text loaded from embedded resource |

## Caching

Templates are cached at two levels for IDE responsiveness:

1. **Text cache** — Embedded resource text is cached by `(Assembly, ResourceName)`. Avoids repeated I/O on each keystroke.
2. **Parse cache** — Parsed Scriban templates are cached by template text content. Saves 5–10ms per parse.

Both caches use `ConcurrentDictionary` for thread safety during concurrent execution.

## Snake Case Access

Model properties are automatically accessible in snake_case within templates:

```csharp
record MyModel(string FullName, int MaxRetries);
```

```scriban
{{ model.full_name }}     # Accesses FullName
{{ model.max_retries }}   # Accesses MaxRetries
```

This is handled by `ScriptObject.ImportDeep` with reflection caching — first import ~1–2ms, subsequent ~0.01ms.

## Error Handling

If a template is not found, `TemplateNotFoundException` is thrown with a list of available templates in the assembly.

---

## RenderResult

`RenderResult` is an `abstract record` representing the outcome of a template render.

### Success

```csharp
if (result is RenderResult.Success success)
{
    string code = success.Text;
    object? model = success.Context;
}
```

| Property | Type | Description |
|----------|------|-------------|
| `Text` | `string` | Rendered output |
| `Context` | `object?` | Model used for rendering |
| `TemplateName` | `string` | Template that was rendered |

### Failure

```csharp
if (result is RenderResult.Failure failure)
{
    Diagnostic error = failure.Diagnostic;
}
```

| Property | Type | Description |
|----------|------|-------------|
| `Diagnostic` | `Diagnostic` | Roslyn diagnostic with error details |
| `TemplateName` | `string` | Template that failed |

### Diagnostics

| ID | Severity | Scenario |
|----|----------|----------|
| `DEEPCORE001` | Error | Parse error — `ParsingError occurred while rendering a template: {errors}` |
| `DEEPCORE001` | Error | Render error — `{ExceptionType} occurred while rendering a template: {message}` |

Errors are reported via `SourceProductionContext.ReportDiagnostic()` so they appear in the IDE.

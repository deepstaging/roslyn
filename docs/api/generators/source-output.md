# Source Output Extensions

Extensions for safely adding generated source to the compilation.

## OptionalEmit.AddSourceTo

Adds an `OptionalEmit` result as generated source, reporting any diagnostics:

```csharp
context.RegisterSourceOutput(models, static (ctx, model) =>
{
    model.WriteStrongId()
        .AddSourceTo(ctx, HintName.From(model.Namespace, model.TypeName));
});
```

If the emit is invalid (e.g., model was null), diagnostics are reported and no source is added.

### Behavior

| Emit State | Action |
|------------|--------|
| Valid | Adds source with `ctx.AddSource(filename, code)` |
| Invalid with diagnostics | Reports each diagnostic via `ctx.ReportDiagnostic` |
| Invalid without diagnostics | No action (silent skip) |

## SourceProductionContext.AddSourceFrom

The reverse calling convention — same behavior:

```csharp
ctx.AddSourceFrom(emitResult, hintName);
```

## ReportDefaultError

Reports a generic code generation error:

```csharp
try
{
    // generation logic
}
catch (Exception ex)
{
    context.ReportDefaultError(model, ex);
    // Reports DEEPGEN001: "Error generating code for {ModelType}: {message}"
}
```

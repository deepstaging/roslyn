# DiagnosticsRefs

OpenTelemetry Activity API, debug/trace types, and well-known API calls for `System.Diagnostics`.

> **See also:** [Refs Overview](index.md) | [TypeRef & Primitives](../type-ref.md)

---

## Activity / OpenTelemetry Types

| Member | Returns | Produces |
|--------|---------|----------|
| `Activity` | `TypeRef` | `Activity` |
| `ActivitySource` | `TypeRef` | `ActivitySource` |
| `ActivityKind` | `TypeRef` | `ActivityKind` |
| `ActivityStatusCode` | `TypeRef` | `ActivityStatusCode` |
| `DiagnosticSource` | `TypeRef` | `DiagnosticSource` |

## Debug / Trace Types

| Member | Returns | Produces |
|--------|---------|----------|
| `Debug` | `TypeRef` | `Debug` |
| `Trace` | `TypeRef` | `Trace` |
| `Debugger` | `TypeRef` | `Debugger` |
| `Stopwatch` | `TypeRef` | `Stopwatch` |
| `Process` | `TypeRef` | `Process` |

## API Call Helpers

| Method | Returns | Produces |
|--------|---------|----------|
| `StartActivity(source, name)` | `ExpressionRef` | `source.StartActivity(name)` |
| `StartActivity(source, name, kind)` | `ExpressionRef` | `source.StartActivity(name, kind)` |
| `SetTag(activity, key, value)` | `ExpressionRef` | `activity.SetTag(key, value)` |
| `SetStatus(activity, statusCode)` | `ExpressionRef` | `activity.SetStatus(statusCode)` |
| `StartNew()` | `ExpressionRef` | `Stopwatch.StartNew()` |
| `Assert(condition)` | `ExpressionRef` | `Debug.Assert(condition)` |
| `Assert(condition, message)` | `ExpressionRef` | `Debug.Assert(condition, message)` |

---

## Examples

### OpenTelemetry Tracing

```csharp
// ActivitySource field
builder.AddField("_source", DiagnosticsRefs.ActivitySource, f => f
    .AsStatic()
    .AsReadonly()
    .WithInitializer($"new {DiagnosticsRefs.ActivitySource}(\"MyService\")"))

// Start an activity
body.AddStatement(
    $"using var activity = {DiagnosticsRefs.StartActivity("_source", "\"ProcessOrder\"")};")
// → using var activity = _source.StartActivity("ProcessOrder");

// Start with kind
body.AddStatement(
    $"using var activity = {DiagnosticsRefs.StartActivity("_source", "\"HandleRequest\"", "ActivityKind.Server")};")

// Tag the activity
body.AddStatement(
    $"{DiagnosticsRefs.SetTag("activity", "\"order.id\"", "orderId")};")
// → activity.SetTag("order.id", orderId);

// Set status
body.AddStatement(
    $"{DiagnosticsRefs.SetStatus("activity", $"{DiagnosticsRefs.ActivityStatusCode}.Member(\"Ok\")")};")
```

### Stopwatch

```csharp
// Start timing
body.AddStatement($"var sw = {DiagnosticsRefs.StartNew()};")
// → var sw = global::System.Diagnostics.Stopwatch.StartNew();
```

### Debug Assertions

```csharp
// Simple assertion
body.AddStatement($"{DiagnosticsRefs.Assert("value != null")};")
// → global::System.Diagnostics.Debug.Assert(value != null);

// Assertion with message
body.AddStatement(
    $"{DiagnosticsRefs.Assert("count >= 0", "\"Count must be non-negative\"")};")
// → global::System.Diagnostics.Debug.Assert(count >= 0, "Count must be non-negative");
```

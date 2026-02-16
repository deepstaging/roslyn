# SystemRefs

Common `System` value types, utility types, and well-known API calls.

> **See also:** [Refs Overview](index.md) | [ExceptionRefs](exceptions.md) | [DelegateRefs](linq.md#delegaterefs)

---

## Value Types

| Member | Returns | Produces |
|--------|---------|----------|
| `Guid` | `TypeRef` | `Guid` |
| `DateTime` | `TypeRef` | `DateTime` |
| `DateTimeOffset` | `TypeRef` | `DateTimeOffset` |
| `TimeSpan` | `TypeRef` | `TimeSpan` |
| `DateOnly` | `TypeRef` | `DateOnly` |
| `TimeOnly` | `TypeRef` | `TimeOnly` |

## Utility Types

| Member | Returns | Produces |
|--------|---------|----------|
| `Uri` | `TypeRef` | `Uri` |
| `Version` | `TypeRef` | `Version` |
| `Convert` | `TypeRef` | `Convert` |
| `Lazy(valueType)` | `TypeRef` | `Lazy<T>` |
| `Nullable(valueType)` | `TypeRef` | `Nullable<T>` |

## API Call Helpers

| Method | Returns | Produces |
|--------|---------|----------|
| `NewGuid()` | `ExpressionRef` | `Guid.NewGuid()` |
| `GuidParse(value)` | `ExpressionRef` | `Guid.Parse(value)` |
| `GuidEmpty` | `ExpressionRef` | `Guid.Empty` |
| `UtcNow` | `ExpressionRef` | `DateTime.UtcNow` |
| `Now` | `ExpressionRef` | `DateTime.Now` |
| `DateTimeOffsetUtcNow` | `ExpressionRef` | `DateTimeOffset.UtcNow` |

---

## Examples

### Type Positions

```csharp
// Properties
builder.AddProperty("Id", SystemRefs.Guid)
builder.AddProperty("CreatedAt", SystemRefs.DateTimeOffset)
builder.AddProperty("Duration", SystemRefs.TimeSpan)

// Nullable value types
builder.AddProperty("ExpiresAt", SystemRefs.DateTime.Nullable())
// → global::System.DateTime?

// Lazy<T> field
builder.AddField("_config", SystemRefs.Lazy(ConfigurationRefs.IConfiguration))
// → global::System.Lazy<global::...IConfiguration>

// Nullable<T> (explicit generic form)
method.AddParameter("count", SystemRefs.Nullable("int"))
// → global::System.Nullable<int>
```

### API Call Helpers

```csharp
// Generate a new GUID
body.AddStatement($"var id = {SystemRefs.NewGuid()};")
// → var id = global::System.Guid.NewGuid();

// Parse a GUID
body.AddStatement($"var id = {SystemRefs.GuidParse("input")};")
// → var id = global::System.Guid.Parse(input);

// Default GUID
body.AddStatement($"var empty = {SystemRefs.GuidEmpty};")
// → var empty = global::System.Guid.Empty;

// Current timestamp
body.AddStatement($"var now = {SystemRefs.UtcNow};")
// → var now = global::System.DateTime.UtcNow;

body.AddStatement($"var timestamp = {SystemRefs.DateTimeOffsetUtcNow};")
// → var timestamp = global::System.DateTimeOffset.UtcNow;
```

!!! tip "Related Classes"
    Exception types (`ArgumentNullException`, etc.) are in [ExceptionRefs](exceptions.md).
    Delegate types (`Func<>`, `Action<>`) are in [DelegateRefs](linq.md#delegaterefs).

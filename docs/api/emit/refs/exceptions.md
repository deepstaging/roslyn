# ExceptionRefs

Common exception types and guard-clause helpers from `System`.

> **See also:** [Refs Overview](index.md) | [TypeRef & Primitives](../type-ref.md)

---

## Types

| Member | Returns | Produces |
|--------|---------|----------|
| `ArgumentNull` | `TypeRef` | `ArgumentNullException` |
| `Argument` | `TypeRef` | `ArgumentException` |
| `ArgumentOutOfRange` | `TypeRef` | `ArgumentOutOfRangeException` |
| `InvalidOperation` | `TypeRef` | `InvalidOperationException` |
| `InvalidCast` | `TypeRef` | `InvalidCastException` |
| `Format` | `TypeRef` | `FormatException` |
| `NotSupported` | `TypeRef` | `NotSupportedException` |
| `NotImplemented` | `TypeRef` | `NotImplementedException` |

## API Call Helpers

Guard clauses that throw on invalid input — the .NET 6+ `ThrowIf*` pattern:

| Method | Returns | Produces |
|--------|---------|----------|
| `ThrowIfNull(value)` | `ExpressionRef` | `ArgumentNullException.ThrowIfNull(value)` |
| `ThrowIfNullOrEmpty(value)` | `ExpressionRef` | `ArgumentException.ThrowIfNullOrEmpty(value)` |
| `ThrowIfNullOrWhiteSpace(value)` | `ExpressionRef` | `ArgumentException.ThrowIfNullOrWhiteSpace(value)` |
| `ThrowIfNegative(value)` | `ExpressionRef` | `ArgumentOutOfRangeException.ThrowIfNegative(value)` |
| `ThrowIfZero(value)` | `ExpressionRef` | `ArgumentOutOfRangeException.ThrowIfZero(value)` |

---

## Examples

### Throw Statements

```csharp
// throw new ArgumentNullException(nameof(value))
body.AddStatement($"throw new {ExceptionRefs.ArgumentNull}(nameof(value));")

// throw new InvalidOperationException("message")
body.AddStatement($"throw new {ExceptionRefs.InvalidOperation}(\"Already initialized\");")

// throw new NotImplementedException()
body.AddStatement($"throw new {ExceptionRefs.NotImplemented}();")
```

### Guard Clauses

```csharp
// ArgumentNullException.ThrowIfNull(value)
body.AddStatement($"{ExceptionRefs.ThrowIfNull("value")};")
// → global::System.ArgumentNullException.ThrowIfNull(value);

// ArgumentException.ThrowIfNullOrEmpty(name)
body.AddStatement($"{ExceptionRefs.ThrowIfNullOrEmpty("name")};")

// ArgumentOutOfRangeException.ThrowIfNegative(count)
body.AddStatement($"{ExceptionRefs.ThrowIfNegative("count")};")
```

### Common Pattern: Constructor Guards

```csharp
builder.AddConstructor(ctor => ctor
    .AddParameter("name", "string")
    .AddParameter("count", "int")
    .WithBody(b => b
        .AddStatement($"{ExceptionRefs.ThrowIfNullOrWhiteSpace("name")};")
        .AddStatement($"{ExceptionRefs.ThrowIfNegative("count")};")
        .AddStatement("_name = name;")
        .AddStatement("_count = count;")))
```

### Catch Blocks

```csharp
// Type in catch clause
body.AddStatement($"catch ({ExceptionRefs.InvalidOperation} ex)")
```

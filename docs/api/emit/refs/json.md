# JsonRefs & EncodingRefs

Serialization type references and API call helpers.

> **See also:** [Refs Overview](index.md) | [TypeRef & Primitives](../type-ref.md)

---

## JsonRefs

`System.Text.Json` and `System.Text.Json.Serialization` types.

### Types

| Member | Returns | Produces |
|--------|---------|----------|
| `Serializer` | `TypeRef` | `JsonSerializer` |
| `SerializerOptions` | `TypeRef` | `JsonSerializerOptions` |
| `Reader` | `TypeRef` | `Utf8JsonReader` |
| `Writer` | `TypeRef` | `Utf8JsonWriter` |
| `ConverterOf(valueType)` | `TypeRef` | `JsonConverter<T>` |

### Attributes

| Member | Returns | Produces |
|--------|---------|----------|
| `Converter` | `AttributeRef` | `[JsonConverter]` |

### API Call Helpers

| Method | Returns | Produces |
|--------|---------|----------|
| `Serialize(value)` | `ExpressionRef` | `JsonSerializer.Serialize(value)` |
| `Serialize(value, options)` | `ExpressionRef` | `JsonSerializer.Serialize(value, options)` |
| `Deserialize(type, json)` | `ExpressionRef` | `JsonSerializer.Deserialize<T>(json)` |
| `Deserialize(type, json, options)` | `ExpressionRef` | `JsonSerializer.Deserialize<T>(json, options)` |

### Examples

```csharp
// Custom JsonConverter base class
builder.Extends(JsonRefs.ConverterOf("OrderId"))
// → : global::System.Text.Json.Serialization.JsonConverter<OrderId>

// [JsonConverter] attribute
property.WithAttribute(JsonRefs.Converter)

// [JsonConverter(typeof(MyConverter))]
property.WithAttribute(JsonRefs.Converter.WithArgument($"typeof({converterType})"))

// Serialize
body.AddStatement($"var json = {JsonRefs.Serialize("model")};")
// → var json = global::System.Text.Json.JsonSerializer.Serialize(model);

// Serialize with options
body.AddStatement($"var json = {JsonRefs.Serialize("model", "_options")};")

// Deserialize
body.AddReturn(JsonRefs.Deserialize("OrderDto", "json"))
// → return global::System.Text.Json.JsonSerializer.Deserialize<OrderDto>(json)

// Deserialize with options
body.AddReturn(JsonRefs.Deserialize("Response", "content", "options"))
```

### Reader/Writer Parameters

```csharp
// JsonConverter override
method
    .AddParameter("reader", JsonRefs.Reader, p => p.AsRef())
    .AddParameter("typeToConvert", "Type")
    .AddParameter("options", JsonRefs.SerializerOptions)
```

---

## EncodingRefs

`System.Text.Encoding` well-known instances.

### Expressions

| Member | Returns | Produces |
|--------|---------|----------|
| `UTF8` | `ExpressionRef` | `Encoding.UTF8` |
| `ASCII` | `ExpressionRef` | `Encoding.ASCII` |
| `Unicode` | `ExpressionRef` | `Encoding.Unicode` |

!!! note "These are ExpressionRef, not TypeRef"
    `Encoding.UTF8` is a singleton instance, not a type. Use it in expression positions:

    ```csharp
    body.AddStatement($"var bytes = {EncodingRefs.UTF8}.GetBytes(text);")
    // → var bytes = global::System.Text.Encoding.UTF8.GetBytes(text);
    ```

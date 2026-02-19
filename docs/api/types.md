# Types

Type-safe wrappers for common .NET generic types. Each wrapper is a `readonly record struct` that carries constituent type information and produces globally-qualified output.

> **See also:** [TypeRef & Primitives](emit/type-ref.md) | [Expressions](expressions.md) | [LanguageExt Types](languageext/types.md)

## Overview

When you write `TypeRef.Global("System.Collections.Generic.List<string>")`, the element type `string` is baked into the string — lost to downstream code. Typed wrappers preserve this structure:

```csharp
using Deepstaging.Roslyn.Types;

// ❌ Flat string — can't recover the element type
TypeRef list = TypeRef.Global("System.Collections.Generic.List<string>");

// ✅ Typed wrapper — element type is accessible
var list = new ListTypeRef("string");
list.ElementType  // → "string"
list.ToString()   // → "global::System.Collections.Generic.List<string>"
```

Every wrapper has:

- **Constituent type properties** — access the inner types that built the wrapper
- **Implicit conversion to `TypeRef`** — use anywhere a `TypeRef` is expected
- **Implicit conversion to `string`** — use in string interpolation
- **Globally-qualified `ToString()`** — always produces `global::*` output

---

## Async

### TaskTypeRef

Represents `Task<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ResultType` | `TypeRef` | The result type |

```csharp
var task = new TaskTypeRef("string");

task.ResultType   // → "string"
task.ToString()   // → "global::System.Threading.Tasks.Task<string>"

method.WithReturnType(task);  // implicit TypeRef conversion
```

### ValueTaskTypeRef

Represents `ValueTask<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ResultType` | `TypeRef` | The result type |

```csharp
var vtask = new ValueTaskTypeRef("int");
vtask.ToString()  // → "global::System.Threading.Tasks.ValueTask<int>"
```

---

## Collections

### ListTypeRef

Represents `List<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var list = new ListTypeRef("Order");
list.ElementType  // → "Order"
list.ToString()   // → "global::System.Collections.Generic.List<Order>"
```

### DictionaryTypeRef

Represents `Dictionary<TKey, TValue>`.

| Property | Type | Description |
|----------|------|-------------|
| `KeyType` | `TypeRef` | The key type |
| `ValueType` | `TypeRef` | The value type |

```csharp
var dict = new DictionaryTypeRef("string", "int");
dict.KeyType    // → "string"
dict.ValueType  // → "int"
dict.ToString() // → "global::System.Collections.Generic.Dictionary<string, int>"
```

### HashSetTypeRef

Represents `HashSet<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var set = new HashSetTypeRef("string");
set.ToString()  // → "global::System.Collections.Generic.HashSet<string>"
```

---

## Collection Interfaces

### EnumerableTypeRef

Represents `IEnumerable<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var items = new EnumerableTypeRef("Order");
items.ToString()  // → "global::System.Collections.Generic.IEnumerable<Order>"
```

### CollectionInterfaceTypeRef

Represents `ICollection<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var col = new CollectionInterfaceTypeRef("string");
col.ToString()  // → "global::System.Collections.Generic.ICollection<string>"
```

### ListInterfaceTypeRef

Represents `IList<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var list = new ListInterfaceTypeRef("Customer");
list.ToString()  // → "global::System.Collections.Generic.IList<Customer>"
```

### SetInterfaceTypeRef

Represents `ISet<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var tags = new SetInterfaceTypeRef("string");
tags.ToString()  // → "global::System.Collections.Generic.ISet<string>"
```

### ReadOnlyListTypeRef

Represents `IReadOnlyList<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var items = new ReadOnlyListTypeRef("Order");
items.ToString()  // → "global::System.Collections.Generic.IReadOnlyList<Order>"
```

### ReadOnlyCollectionTypeRef

Represents `IReadOnlyCollection<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var items = new ReadOnlyCollectionTypeRef("int");
items.ToString()  // → "global::System.Collections.Generic.IReadOnlyCollection<int>"
```

### DictionaryInterfaceTypeRef

Represents `IDictionary<TKey, TValue>`.

| Property | Type | Description |
|----------|------|-------------|
| `KeyType` | `TypeRef` | The key type |
| `ValueType` | `TypeRef` | The value type |

```csharp
var dict = new DictionaryInterfaceTypeRef("string", "int");
dict.ToString()  // → "global::System.Collections.Generic.IDictionary<string, int>"
```

### ReadOnlyDictionaryTypeRef

Represents `IReadOnlyDictionary<TKey, TValue>`.

| Property | Type | Description |
|----------|------|-------------|
| `KeyType` | `TypeRef` | The key type |
| `ValueType` | `TypeRef` | The value type |

```csharp
var config = new ReadOnlyDictionaryTypeRef("string", "object");
config.ToString()  // → "global::System.Collections.Generic.IReadOnlyDictionary<string, object>"
```

### KeyValuePairTypeRef

Represents `KeyValuePair<TKey, TValue>`.

| Property | Type | Description |
|----------|------|-------------|
| `KeyType` | `TypeRef` | The key type |
| `ValueType` | `TypeRef` | The value type |

```csharp
var kvp = new KeyValuePairTypeRef("string", "int");
kvp.ToString()  // → "global::System.Collections.Generic.KeyValuePair<string, int>"
```

---

## Nullable

### NullableTypeRef

Represents `Nullable<T>` (value types only).

| Property | Type | Description |
|----------|------|-------------|
| `InnerType` | `TypeRef` | The underlying value type |

```csharp
var nullable = new NullableTypeRef("int");
nullable.InnerType  // → "int"
nullable.ToString() // → "global::System.Nullable<int>"
```

---

## Lazy

### LazyTypeRef

Represents `Lazy<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ValueType` | `TypeRef` | The lazily-initialized type |

```csharp
var lazy = new LazyTypeRef("ExpensiveService");
lazy.ValueType  // → "ExpensiveService"
lazy.ToString() // → "global::System.Lazy<ExpensiveService>"
```

---

## Comparison

### EqualityComparerTypeRef

Represents `EqualityComparer<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ComparedType` | `TypeRef` | The type being compared |

```csharp
var eq = new EqualityComparerTypeRef("string");
eq.ComparedType  // → "string"
eq.ToString()    // → "global::System.Collections.Generic.EqualityComparer<string>"
```

### ComparerTypeRef

Represents `Comparer<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ComparedType` | `TypeRef` | The type being compared |

```csharp
var cmp = new ComparerTypeRef("int");
cmp.ToString()  // → "global::System.Collections.Generic.Comparer<int>"
```

---

## Delegates

### FuncTypeRef

Represents `Func<T1, ..., TResult>`.

| Property | Type | Description |
|----------|------|-------------|
| `ParameterTypes` | `ImmutableArray<TypeRef>` | The parameter types |
| `ReturnType` | `TypeRef` | The return type |

```csharp
// Func<string, bool>
var func = new FuncTypeRef([TypeRef.From("string")], TypeRef.From("bool"));
func.ToString()  // → "global::System.Func<string, bool>"

// Func<int> (no parameters)
var producer = new FuncTypeRef(TypeRef.From("int"));
producer.ToString()  // → "global::System.Func<int>"
```

### ActionTypeRef

Represents `Action` or `Action<T1, ...>`.

| Property | Type | Description |
|----------|------|-------------|
| `ParameterTypes` | `ImmutableArray<TypeRef>` | The parameter types |

```csharp
// Action<string>
var action = new ActionTypeRef(TypeRef.From("string"));
action.ToString()  // → "global::System.Action<string>"

// Action (no parameters)
var noParam = new ActionTypeRef(ImmutableArray<TypeRef>.Empty);
noParam.ToString()  // → "global::System.Action"
```

---

## Immutable Collections

### ImmutableArrayTypeRef

Represents `ImmutableArray<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var arr = new ImmutableArrayTypeRef("string");
arr.ToString()  // → "global::System.Collections.Immutable.ImmutableArray<string>"
```

### ImmutableListTypeRef

Represents `ImmutableList<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var list = new ImmutableListTypeRef("Order");
list.ToString()  // → "global::System.Collections.Immutable.ImmutableList<Order>"
```

### ImmutableDictionaryTypeRef

Represents `ImmutableDictionary<TKey, TValue>`.

| Property | Type | Description |
|----------|------|-------------|
| `KeyType` | `TypeRef` | The key type |
| `ValueType` | `TypeRef` | The value type |

```csharp
var idict = new ImmutableDictionaryTypeRef("string", "int");
idict.ToString()  // → "global::System.Collections.Immutable.ImmutableDictionary<string, int>"
```

---

## LINQ

### QueryableTypeRef

Represents `IQueryable<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var query = new QueryableTypeRef("Customer");
query.ToString()  // → "global::System.Linq.IQueryable<Customer>"
```

### OrderedQueryableTypeRef

Represents `IOrderedQueryable<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var ordered = new OrderedQueryableTypeRef("Product");
ordered.ToString()  // → "global::System.Linq.IOrderedQueryable<Product>"
```

### LinqExpressionTypeRef

Represents `Expression<TDelegate>` from `System.Linq.Expressions`.

| Property | Type | Description |
|----------|------|-------------|
| `DelegateType` | `TypeRef` | The delegate type |

```csharp
// With a string delegate type
var expr = new LinqExpressionTypeRef("Func<Customer, bool>");
expr.ToString()  // → "global::System.Linq.Expressions.Expression<Func<Customer, bool>>"

// Composing with FuncTypeRef
var func = new FuncTypeRef([TypeRef.From("Customer")], TypeRef.From("bool"));
var expr = new LinqExpressionTypeRef(func);
expr.ToString()  // → "global::System.Linq.Expressions.Expression<global::System.Func<Customer, bool>>"
```

---

## Span & Memory

### SpanTypeRef

Represents `Span<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var span = new SpanTypeRef("byte");
span.ToString()  // → "global::System.Span<byte>"
```

### ReadOnlySpanTypeRef

Represents `ReadOnlySpan<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var span = new ReadOnlySpanTypeRef("char");
span.ToString()  // → "global::System.ReadOnlySpan<char>"
```

### MemoryTypeRef

Represents `Memory<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var mem = new MemoryTypeRef("byte");
mem.ToString()  // → "global::System.Memory<byte>"
```

### ReadOnlyMemoryTypeRef

Represents `ReadOnlyMemory<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `ElementType` | `TypeRef` | The element type |

```csharp
var mem = new ReadOnlyMemoryTypeRef("byte");
mem.ToString()  // → "global::System.ReadOnlyMemory<byte>"
```

---

## JSON

### JsonConverterTypeRef

Represents `JsonConverter<T>` from `System.Text.Json.Serialization`.

| Property | Type | Description |
|----------|------|-------------|
| `ValueType` | `TypeRef` | The type being converted |

```csharp
var converter = new JsonConverterTypeRef("MyEnum");
converter.ToString()  // → "global::System.Text.Json.Serialization.JsonConverter<MyEnum>"
```

### JsonTypes

Static `TypeRef` constants for `System.Text.Json`.

| Property | Produces |
|----------|----------|
| `Namespace` | `NamespaceRef` for `System.Text.Json` |
| `SerializationNamespace` | `NamespaceRef` for `System.Text.Json.Serialization` |
| `Serializer` | `global::System.Text.Json.JsonSerializer` |
| `SerializerOptions` | `global::System.Text.Json.JsonSerializerOptions` |
| `Reader` | `global::System.Text.Json.Utf8JsonReader` |
| `Writer` | `global::System.Text.Json.Utf8JsonWriter` |

### JsonAttributes

| Property | Produces |
|----------|----------|
| `Converter` | `[global::System.Text.Json.Serialization.JsonConverter]` |

---

## HTTP

### HttpTypes

Static `TypeRef` constants for `System.Net.Http`.

| Property | Produces |
|----------|----------|
| `Namespace` | `NamespaceRef` for `System.Net.Http` |
| `Client` | `global::System.Net.Http.HttpClient` |
| `RequestMessage` | `global::System.Net.Http.HttpRequestMessage` |
| `ResponseMessage` | `global::System.Net.Http.HttpResponseMessage` |
| `Method` | `global::System.Net.Http.HttpMethod` |
| `Content` | `global::System.Net.Http.HttpContent` |
| `StringContent` | `global::System.Net.Http.StringContent` |
| `ByteArrayContent` | `global::System.Net.Http.ByteArrayContent` |
| `StreamContent` | `global::System.Net.Http.StreamContent` |

---

## Entity Framework

### DbSetTypeRef

Represents `DbSet<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `EntityType` | `TypeRef` | The entity type |

```csharp
var dbSet = new DbSetTypeRef("Customer");
dbSet.ToString()  // → "global::Microsoft.EntityFrameworkCore.DbSet<Customer>"
```

### EntityTypeBuilderTypeRef

Represents `EntityTypeBuilder<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `EntityType` | `TypeRef` | The entity type |

```csharp
var builder = new EntityTypeBuilderTypeRef("Customer");
builder.ToString()  // → "global::Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Customer>"
```

### EntityTypeConfigurationTypeRef

Represents `IEntityTypeConfiguration<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `EntityType` | `TypeRef` | The entity type |

```csharp
var config = new EntityTypeConfigurationTypeRef("Customer");
config.ToString()  // → "global::Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<Customer>"
```

### EntityFrameworkTypes

| Property | Produces |
|----------|----------|
| `Namespace` | `NamespaceRef` for `Microsoft.EntityFrameworkCore` |
| `BuildersNamespace` | `NamespaceRef` for `Microsoft.EntityFrameworkCore.Metadata.Builders` |
| `DbContext` | `global::Microsoft.EntityFrameworkCore.DbContext` |
| `ModelBuilder` | `global::Microsoft.EntityFrameworkCore.ModelBuilder` |

### EntityFrameworkAttributes

Data annotation `AttributeRef` constants for EF models.

| Property | Attribute |
|----------|-----------|
| `Key` | `[Key]` |
| `Required` | `[Required]` |
| `MaxLength` | `[MaxLength]` |
| `StringLength` | `[StringLength]` |
| `Range` | `[Range]` |
| `Table` | `[Table]` |
| `Column` | `[Column]` |
| `ForeignKey` | `[ForeignKey]` |
| `NotMapped` | `[NotMapped]` |
| `DatabaseGenerated` | `[DatabaseGenerated]` |

---

## Hosting

### HostingTypes

Static `TypeRef` constants for `Microsoft.Extensions.Hosting`.

| Property | Produces |
|----------|----------|
| `Namespace` | `NamespaceRef` for `Microsoft.Extensions.Hosting` |
| `BackgroundService` | `global::Microsoft.Extensions.Hosting.BackgroundService` |
| `IHostedService` | `global::Microsoft.Extensions.Hosting.IHostedService` |
| `IHost` | `global::Microsoft.Extensions.Hosting.IHost` |
| `IHostApplicationLifetime` | `global::Microsoft.Extensions.Hosting.IHostApplicationLifetime` |
| `IHostEnvironment` | `global::Microsoft.Extensions.Hosting.IHostEnvironment` |

### ChannelTypes

Static factory/constant for `System.Threading.Channels`.

| Member | Produces |
|--------|----------|
| `Namespace` | `NamespaceRef` for `System.Threading.Channels` |
| `Channel(itemType)` | `global::System.Threading.Channels.Channel<T>` |
| `BoundedChannelOptions` | `global::System.Threading.Channels.BoundedChannelOptions` |
| `UnboundedChannelOptions` | `global::System.Threading.Channels.UnboundedChannelOptions` |
| `BoundedChannelFullMode` | `global::System.Threading.Channels.BoundedChannelFullMode` |

```csharp
HostingTypes.BackgroundService.ToString()
// → "global::Microsoft.Extensions.Hosting.BackgroundService"

ChannelTypes.Channel(TypeRef.From("OrderEvent")).ToString()
// → "global::System.Threading.Channels.Channel<OrderEvent>"
```

---

## Dependency Injection

### DependencyInjectionTypes

Static `TypeRef` constants for `Microsoft.Extensions.DependencyInjection`.

| Property | Produces |
|----------|----------|
| `Namespace` | `NamespaceRef` for `Microsoft.Extensions.DependencyInjection` |
| `IServiceCollection` | `global::Microsoft.Extensions.DependencyInjection.IServiceCollection` |
| `IServiceProvider` | `global::System.IServiceProvider` |
| `IServiceScopeFactory` | `global::Microsoft.Extensions.DependencyInjection.IServiceScopeFactory` |
| `IServiceScope` | `global::Microsoft.Extensions.DependencyInjection.IServiceScope` |
| `ServiceDescriptor` | `global::Microsoft.Extensions.DependencyInjection.ServiceDescriptor` |

---

## Logging

### LoggerTypeRef

Represents `ILogger<T>`.

| Property | Type | Description |
|----------|------|-------------|
| `CategoryType` | `TypeRef` | The logger category type |

```csharp
var logger = new LoggerTypeRef("MyService");
logger.ToString()  // → "global::Microsoft.Extensions.Logging.ILogger<MyService>"
```

### LoggingTypes

| Property | Produces |
|----------|----------|
| `Namespace` | `NamespaceRef` for `Microsoft.Extensions.Logging` |
| `ILogger` | `global::Microsoft.Extensions.Logging.ILogger` |
| `ILoggerFactory` | `global::Microsoft.Extensions.Logging.ILoggerFactory` |
| `LogLevel` | `global::Microsoft.Extensions.Logging.LogLevel` |

---

## Configuration

### ConfigurationTypes

Static `TypeRef` constants for `Microsoft.Extensions.Configuration`.

| Property | Produces |
|----------|----------|
| `Namespace` | `NamespaceRef` for `Microsoft.Extensions.Configuration` |
| `IConfiguration` | `global::Microsoft.Extensions.Configuration.IConfiguration` |
| `IConfigurationSection` | `global::Microsoft.Extensions.Configuration.IConfigurationSection` |
| `IConfigurationRoot` | `global::Microsoft.Extensions.Configuration.IConfigurationRoot` |
| `IConfigurationBuilder` | `global::Microsoft.Extensions.Configuration.IConfigurationBuilder` |

---

## Diagnostics

### DiagnosticsTypes

Static `TypeRef` constants for `System.Diagnostics`.

| Property | Produces |
|----------|----------|
| `Namespace` | `NamespaceRef` for `System.Diagnostics` |
| `Activity` | `global::System.Diagnostics.Activity` |
| `ActivitySource` | `global::System.Diagnostics.ActivitySource` |
| `ActivityKind` | `global::System.Diagnostics.ActivityKind` |
| `ActivityStatusCode` | `global::System.Diagnostics.ActivityStatusCode` |
| `DiagnosticSource` | `global::System.Diagnostics.DiagnosticSource` |
| `Stopwatch` | `global::System.Diagnostics.Stopwatch` |
| `Process` | `global::System.Diagnostics.Process` |
| `Debug` | `global::System.Diagnostics.Debug` |
| `Trace` | `global::System.Diagnostics.Trace` |
| `Debugger` | `global::System.Diagnostics.Debugger` |

---

## Composability

Typed wrappers compose naturally because they implicitly convert to `TypeRef`:

```csharp
// Nested generics
var nested = new EqualityComparerTypeRef((TypeRef)new ListTypeRef("int"));
nested.ToString()
// → "global::System.Collections.Generic.EqualityComparer<global::System.Collections.Generic.List<int>>"

// Use in builders
var task = new TaskTypeRef((TypeRef)new ListTypeRef("Order"));
method.WithReturnType(task);  // Task<List<Order>>

// Use in expression factories
var eq = EqualityComparerExpression.DefaultEquals("string", "_name", "value");
// → "global::System.Collections.Generic.EqualityComparer<string>.Default.Equals(_name, value)"
```

# TypeRef & Refs

Globally-qualified type and namespace references for generated code.

> **See also:** [Emit Overview](index.md) | [TypeBuilder](type-builder.md) | [MethodBuilder](method-builder.md)

## Overview

`TypeRef` and `NamespaceRef` live in `Deepstaging.Roslyn.Emit.Refs`. Type references are fully-qualified with `global::` so generated code never conflicts with user-defined types.

```csharp
TaskRefs.Task("string")                   // global::System.Threading.Tasks.Task<string>
CollectionRefs.List("int")                // global::System.Collections.Generic.List<int>
ExceptionRefs.ArgumentNull                // global::System.ArgumentNullException
```

Each domain has a standalone static class with a `Namespace` property (`NamespaceRef`) and type factories. This pattern is extensible — define your own `*Refs` class for any namespace.

---

## NamespaceRef

A lightweight primitive representing a .NET namespace.

```csharp
var ns = NamespaceRef.From("MyCompany.Domain.Events");
TypeRef eventType = ns.Type("OrderCreated");  // global::MyCompany.Domain.Events.OrderCreated
```

| Member | Description |
|--------|-------------|
| `From(string)` | Create from a dotted namespace string |
| `Type(string)` | Create a globally-qualified TypeRef in this namespace |
| `Value` | The raw namespace string |
| implicit `string` | Converts to string |

---

## TypeRef

### Factory Methods

| Method | Description |
|--------|-------------|
| `From(string)` | Create from a type name |
| `From(ValidSymbol<T>)` | Create from a validated symbol |
| `From(SymbolSnapshot)` | Create from a snapshot |
| `Global(string)` | Create with `global::` prefix |

```csharp
TypeRef.From("MyApp.Models.Customer")
TypeRef.From(validSymbol)
TypeRef.From(snapshot)
TypeRef.Global("System.Text.Json.JsonSerializer")
```

### Instance Methods

| Method | Description |
|--------|-------------|
| `Of(params TypeRef[])` | Add generic type arguments |
| `Nullable()` | Append `?` |
| `Array()` | Single-dimensional array |
| `Array(int rank)` | Multi-dimensional array |
| `As(TypeRef)` | Safe cast (`as`) expression |
| `Cast(TypeRef)` | Direct cast expression |
| `OrDefault(TypeRef)` | Null-coalescing (`??`) expression |
| `Invoke(params TypeRef[])` | Null-conditional delegate invocation |

```csharp
TypeRef.From("MyService").Nullable()      // MyService?
TypeRef.From("byte").Array()              // byte[]
TypeRef.Global("MyApp.IHandler").Of("string", "int")  // global::MyApp.IHandler<string, int>
```

### Tuples

Named tuples with arbitrary arity:

```csharp
TypeRef.Tuple(
    (TypeRef.From("string"), "Name"),
    (TypeRef.From("int"), "Age"))
// (string Name, int Age)
```

### Implicit Conversions

`TypeRef` converts implicitly to and from `string`:

```csharp
TypeRef typeRef = "string";                        // from string
string code = TaskRefs.Task("string");             // to string
```

---

## Ref Types

### ExceptionRefs

Common exception types from `System`.

| Member | Resolves To |
|--------|-------------|
| `ExceptionRefs.ArgumentNull` | `System.ArgumentNullException` |
| `ExceptionRefs.Argument` | `System.ArgumentException` |
| `ExceptionRefs.ArgumentOutOfRange` | `System.ArgumentOutOfRangeException` |
| `ExceptionRefs.InvalidOperation` | `System.InvalidOperationException` |
| `ExceptionRefs.InvalidCast` | `System.InvalidCastException` |
| `ExceptionRefs.Format` | `System.FormatException` |
| `ExceptionRefs.NotSupported` | `System.NotSupportedException` |
| `ExceptionRefs.NotImplemented` | `System.NotImplementedException` |

```csharp
body.AddStatement($"throw new {ExceptionRefs.ArgumentNull}(nameof(value));")
```

### CollectionRefs

Generic collection types from `System.Collections.Generic`.

| Method | Resolves To |
|--------|-------------|
| `CollectionRefs.List(element)` | `List<T>` |
| `CollectionRefs.Dictionary(key, value)` | `Dictionary<TKey, TValue>` |
| `CollectionRefs.HashSet(element)` | `HashSet<T>` |
| `CollectionRefs.KeyValuePair(key, value)` | `KeyValuePair<TKey, TValue>` |
| `CollectionRefs.IEnumerable(element)` | `IEnumerable<T>` |
| `CollectionRefs.ICollection(element)` | `ICollection<T>` |
| `CollectionRefs.IList(element)` | `IList<T>` |
| `CollectionRefs.IDictionary(key, value)` | `IDictionary<TKey, TValue>` |
| `CollectionRefs.ISet(element)` | `ISet<T>` |
| `CollectionRefs.IReadOnlyList(element)` | `IReadOnlyList<T>` |
| `CollectionRefs.IReadOnlyCollection(element)` | `IReadOnlyCollection<T>` |
| `CollectionRefs.IReadOnlyDictionary(key, value)` | `IReadOnlyDictionary<TKey, TValue>` |

```csharp
method.WithReturnType(CollectionRefs.IReadOnlyList("string"))
```

### ImmutableCollectionRefs

Immutable collection types from `System.Collections.Immutable`.

| Method | Resolves To |
|--------|-------------|
| `ImmutableCollectionRefs.ImmutableArray(element)` | `ImmutableArray<T>` |
| `ImmutableCollectionRefs.ImmutableList(element)` | `ImmutableList<T>` |
| `ImmutableCollectionRefs.ImmutableDictionary(key, value)` | `ImmutableDictionary<TKey, TValue>` |

### TaskRefs

Async types from `System.Threading.Tasks` and `System.Threading`.

| Member | Resolves To |
|--------|-------------|
| `TaskRefs.Task()` | `Task` |
| `TaskRefs.Task(result)` | `Task<T>` |
| `TaskRefs.ValueTask()` | `ValueTask` |
| `TaskRefs.ValueTask(result)` | `ValueTask<T>` |
| `TaskRefs.CompletedTask` | `Task.CompletedTask` |
| `TaskRefs.CompletedValueTask` | `ValueTask.CompletedTask` |
| `TaskRefs.CancellationToken` | `CancellationToken` |

```csharp
method.WithReturnType(TaskRefs.Task(CollectionRefs.IReadOnlyList("Order")))
method.AddParameter("ct", TaskRefs.CancellationToken)
```

### JsonRefs

`System.Text.Json` types.

| Member | Resolves To |
|--------|-------------|
| `JsonRefs.Serializer` | `JsonSerializer` |
| `JsonRefs.SerializerOptions` | `JsonSerializerOptions` |
| `JsonRefs.Reader` | `Utf8JsonReader` |
| `JsonRefs.Writer` | `Utf8JsonWriter` |
| `JsonRefs.Converter(valueType)` | `JsonConverter<T>` |
| `JsonRefs.ConverterAttribute` | `JsonConverter` (attribute) |

### EncodingRefs

`System.Text.Encoding` instances.

| Member | Resolves To |
|--------|-------------|
| `EncodingRefs.UTF8` | `Encoding.UTF8` |
| `EncodingRefs.ASCII` | `Encoding.ASCII` |
| `EncodingRefs.Unicode` | `Encoding.Unicode` |

### HttpRefs

`System.Net.Http` types and HTTP method constants.

| Member | Resolves To |
|--------|-------------|
| `HttpRefs.Client` | `HttpClient` |
| `HttpRefs.RequestMessage` | `HttpRequestMessage` |
| `HttpRefs.ResponseMessage` | `HttpResponseMessage` |
| `HttpRefs.Method` | `HttpMethod` |
| `HttpRefs.Get` | `HttpMethod.Get` |
| `HttpRefs.Post` | `HttpMethod.Post` |
| `HttpRefs.Put` | `HttpMethod.Put` |
| `HttpRefs.Patch` | `HttpMethod.Patch` |
| `HttpRefs.Delete` | `HttpMethod.Delete` |
| `HttpRefs.Verb(string)` | `HttpMethod.{verb}` |
| `HttpRefs.Content` | `HttpContent` |
| `HttpRefs.StringContent` | `StringContent` |
| `HttpRefs.ByteArrayContent` | `ByteArrayContent` |
| `HttpRefs.StreamContent` | `StreamContent` |

### ConfigurationRefs

`Microsoft.Extensions.Configuration` types.

| Member | Resolves To |
|--------|-------------|
| `ConfigurationRefs.IConfiguration` | `IConfiguration` |
| `ConfigurationRefs.IConfigurationSection` | `IConfigurationSection` |
| `ConfigurationRefs.IConfigurationRoot` | `IConfigurationRoot` |
| `ConfigurationRefs.IConfigurationBuilder` | `IConfigurationBuilder` |

```csharp
method.AddParameter("configuration", ConfigurationRefs.IConfiguration)
method.AddParameter("section", ConfigurationRefs.IConfigurationSection)
```

### DependencyInjectionRefs

`Microsoft.Extensions.DependencyInjection` types.

| Member | Resolves To |
|--------|-------------|
| `DependencyInjectionRefs.IServiceCollection` | `IServiceCollection` |
| `DependencyInjectionRefs.IServiceProvider` | `IServiceProvider` |
| `DependencyInjectionRefs.IServiceScopeFactory` | `IServiceScopeFactory` |
| `DependencyInjectionRefs.IServiceScope` | `IServiceScope` |
| `DependencyInjectionRefs.ServiceDescriptor` | `ServiceDescriptor` |

### LoggingRefs

`Microsoft.Extensions.Logging` types.

| Member | Resolves To |
|--------|-------------|
| `LoggingRefs.ILogger` | `ILogger` |
| `LoggingRefs.ILoggerOf(categoryType)` | `ILogger<T>` |
| `LoggingRefs.ILoggerFactory` | `ILoggerFactory` |
| `LoggingRefs.LogLevel` | `LogLevel` |

```csharp
builder.AddField("_logger", LoggingRefs.ILoggerOf("CustomerService"))
```

### LinqRefs

LINQ and expression types.

| Method | Resolves To |
|--------|-------------|
| `LinqRefs.IQueryable(element)` | `IQueryable<T>` |
| `LinqRefs.IOrderedQueryable(element)` | `IOrderedQueryable<T>` |
| `LinqRefs.Expression(delegate)` | `Expression<TDelegate>` |

### DelegateRefs

`Func<>` and `Action<>` with arbitrary arity.

| Method | Description |
|--------|-------------|
| `DelegateRefs.Func(typeArgs...)` | `Func<T1, ..., TResult>` |
| `DelegateRefs.Action(typeArgs...)` | `Action<T1, ...>` |

```csharp
DelegateRefs.Func("string", "bool")  // Func<string, bool>
DelegateRefs.Action("int")           // Action<int>
```

---

## Extensibility

Each `*Refs` class follows the same pattern — create your own for any namespace:

```csharp
public static class EfCoreRefs
{
    public static NamespaceRef Namespace =>
        NamespaceRef.From("Microsoft.EntityFrameworkCore");

    public static TypeRef DbContext => Namespace.Type("DbContext");
    public static TypeRef DbSet(TypeRef entity) =>
        Namespace.Type($"DbSet<{entity.Value}>");
}

// Usage:
builder.AddField("_db", EfCoreRefs.DbContext)
method.WithReturnType(EfCoreRefs.DbSet("Order"))
```

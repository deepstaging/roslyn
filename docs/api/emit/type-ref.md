# TypeRef

Globally-qualified type references for generated code.

> **See also:** [Emit Overview](index.md) | [TypeBuilder](type-builder.md) | [MethodBuilder](method-builder.md)

## Overview

`TypeRef` produces fully-qualified type names prefixed with `global::` so generated code never conflicts with user-defined types.

```csharp
TypeRef.Tasks.Task("string")          // global::System.Threading.Tasks.Task<string>
TypeRef.Collections.List("int")       // global::System.Collections.Generic.List<int>
TypeRef.Exceptions.ArgumentNull       // global::System.ArgumentNullException
```

## Factory Methods

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

---

## Type Categories

### Exceptions

Common exception types.

| Member | Resolves To |
|--------|-------------|
| `Exceptions.ArgumentNull` | `System.ArgumentNullException` |
| `Exceptions.Argument` | `System.ArgumentException` |
| `Exceptions.ArgumentOutOfRange` | `System.ArgumentOutOfRangeException` |
| `Exceptions.InvalidOperation` | `System.InvalidOperationException` |
| `Exceptions.InvalidCast` | `System.InvalidCastException` |
| `Exceptions.Format` | `System.FormatException` |
| `Exceptions.NotSupported` | `System.NotSupportedException` |
| `Exceptions.NotImplemented` | `System.NotImplementedException` |

```csharp
body.AddStatement($"throw new {TypeRef.Exceptions.ArgumentNull}(nameof(value));")
```

### Collections

Generic collection types from `System.Collections.Generic`.

| Method | Resolves To |
|--------|-------------|
| `Collections.List(element)` | `List<T>` |
| `Collections.Dictionary(key, value)` | `Dictionary<TKey, TValue>` |
| `Collections.HashSet(element)` | `HashSet<T>` |
| `Collections.KeyValuePair(key, value)` | `KeyValuePair<TKey, TValue>` |
| `Collections.IEnumerable(element)` | `IEnumerable<T>` |
| `Collections.ICollection(element)` | `ICollection<T>` |
| `Collections.IList(element)` | `IList<T>` |
| `Collections.IDictionary(key, value)` | `IDictionary<TKey, TValue>` |
| `Collections.ISet(element)` | `ISet<T>` |
| `Collections.IReadOnlyList(element)` | `IReadOnlyList<T>` |
| `Collections.IReadOnlyCollection(element)` | `IReadOnlyCollection<T>` |
| `Collections.IReadOnlyDictionary(key, value)` | `IReadOnlyDictionary<TKey, TValue>` |

```csharp
method.WithReturnType(TypeRef.Collections.IReadOnlyList("string"))
```

### Immutable

Immutable collection types from `System.Collections.Immutable`.

| Method | Resolves To |
|--------|-------------|
| `Immutable.ImmutableArray(element)` | `ImmutableArray<T>` |
| `Immutable.ImmutableList(element)` | `ImmutableList<T>` |
| `Immutable.ImmutableDictionary(key, value)` | `ImmutableDictionary<TKey, TValue>` |

### Tasks

Async types from `System.Threading.Tasks` and `System.Threading`.

| Member | Resolves To |
|--------|-------------|
| `Tasks.Task()` | `Task` |
| `Tasks.Task(result)` | `Task<T>` |
| `Tasks.ValueTask()` | `ValueTask` |
| `Tasks.ValueTask(result)` | `ValueTask<T>` |
| `Tasks.CompletedTask` | `Task.CompletedTask` |
| `Tasks.CompletedValueTask` | `ValueTask.CompletedTask` |
| `Tasks.CancellationToken` | `CancellationToken` |

```csharp
method.WithReturnType(TypeRef.Tasks.Task(TypeRef.Collections.IReadOnlyList("Order")))
method.AddParameter("ct", TypeRef.Tasks.CancellationToken)
```

### Json

`System.Text.Json` types.

| Member | Resolves To |
|--------|-------------|
| `Json.Serializer` | `JsonSerializer` |
| `Json.SerializerOptions` | `JsonSerializerOptions` |
| `Json.Reader` | `Utf8JsonReader` |
| `Json.Writer` | `Utf8JsonWriter` |
| `Json.Converter(valueType)` | `JsonConverter<T>` |
| `Json.ConverterAttribute` | `JsonConverter` (attribute) |

### Encoding

`System.Text.Encoding` instances.

| Member | Resolves To |
|--------|-------------|
| `Encoding.UTF8` | `Encoding.UTF8` |
| `Encoding.ASCII` | `Encoding.ASCII` |
| `Encoding.Unicode` | `Encoding.Unicode` |

### Http

`System.Net.Http` types and HTTP method constants.

| Member | Resolves To |
|--------|-------------|
| `Http.Client` | `HttpClient` |
| `Http.RequestMessage` | `HttpRequestMessage` |
| `Http.ResponseMessage` | `HttpResponseMessage` |
| `Http.Method` | `HttpMethod` |
| `Http.Get` | `HttpMethod.Get` |
| `Http.Post` | `HttpMethod.Post` |
| `Http.Put` | `HttpMethod.Put` |
| `Http.Patch` | `HttpMethod.Patch` |
| `Http.Delete` | `HttpMethod.Delete` |
| `Http.Verb(string)` | `new HttpMethod("VERB")` |
| `Http.Content` | `HttpContent` |
| `Http.StringContent` | `StringContent` |
| `Http.ByteArrayContent` | `ByteArrayContent` |
| `Http.StreamContent` | `StreamContent` |

### DependencyInjection

`Microsoft.Extensions.DependencyInjection` types.

| Member | Resolves To |
|--------|-------------|
| `DependencyInjection.IServiceCollection` | `IServiceCollection` |
| `DependencyInjection.IServiceProvider` | `IServiceProvider` |
| `DependencyInjection.IServiceScopeFactory` | `IServiceScopeFactory` |
| `DependencyInjection.IServiceScope` | `IServiceScope` |
| `DependencyInjection.ServiceDescriptor` | `ServiceDescriptor` |

### Logging

`Microsoft.Extensions.Logging` types.

| Member | Resolves To |
|--------|-------------|
| `Logging.ILogger` | `ILogger` |
| `Logging.ILoggerOf(categoryType)` | `ILogger<T>` |
| `Logging.ILoggerFactory` | `ILoggerFactory` |
| `Logging.LogLevel` | `LogLevel` |

```csharp
builder.AddField("_logger", TypeRef.Logging.ILoggerOf("CustomerService"))
```

### Linq

LINQ and expression types.

| Method | Resolves To |
|--------|-------------|
| `Linq.IQueryable(element)` | `IQueryable<T>` |
| `Linq.IOrderedQueryable(element)` | `IOrderedQueryable<T>` |
| `Linq.Expression(delegate)` | `Expression<TDelegate>` |

### Delegates

`Func<>` and `Action<>` with arbitrary arity.

| Method | Description |
|--------|-------------|
| `Delegates.Func(typeArgs...)` | `Func<T1, ..., TResult>` |
| `Delegates.Action(typeArgs...)` | `Action<T1, ...>` |

```csharp
TypeRef.Delegates.Func("string", "bool")  // Func<string, bool>
TypeRef.Delegates.Action("int")            // Action<int>
```

---

## Instance Methods

Transform a `TypeRef` after creation.

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

---

## Tuples

Named tuples with arbitrary arity:

```csharp
TypeRef.Tuple(
    (TypeRef.From("string"), "Name"),
    (TypeRef.From("int"), "Age"))
// (string Name, int Age)
```

---

## Implicit Conversions

`TypeRef` converts implicitly to and from `string`:

```csharp
TypeRef typeRef = "string";                    // from string
string code = TypeRef.Tasks.Task("string");    // to string
```

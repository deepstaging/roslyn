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

### Type Modifiers

| Method | Returns | Description |
|--------|---------|-------------|
| `Of(params TypeRef[])` | `TypeRef` | Add generic type arguments |
| `Nullable()` | `TypeRef` | Append `?` |
| `Array()` | `TypeRef` | Single-dimensional array |
| `Array(int rank)` | `TypeRef` | Multi-dimensional array |

```csharp
TypeRef.From("MyService").Nullable()      // MyService?
TypeRef.From("byte").Array()              // byte[]
TypeRef.Global("MyApp.IHandler").Of("string", "int")  // global::MyApp.IHandler<string, int>
```

### Expression Gateways

These methods cross from the type domain into the expression domain, returning `ExpressionRef`.

| Method | Returns | Description |
|--------|---------|-------------|
| `New(params args)` | `ExpressionRef` | Constructor call: `new Type(args)` |
| `Call(method, params args)` | `ExpressionRef` | Static method call: `Type.Method(args)` |
| `Member(name)` | `ExpressionRef` | Member access: `Type.Member` |
| `TypeOf()` | `ExpressionRef` | `typeof(Type)` |
| `Default()` | `ExpressionRef` | `default(Type)` |
| `NameOf()` | `ExpressionRef` | `nameof(Type)` |
| `Invoke(params args)` | `ExpressionRef` | Delegate invocation: `value?.Invoke(args)` |
| `As(TypeRef)` | `ExpressionRef` | Safe cast: `value as Type` |
| `Cast(TypeRef)` | `ExpressionRef` | Direct cast: `(Type)value` |
| `OrDefault(fallback)` | `ExpressionRef` | Null coalescing: `value ?? fallback` |

```csharp
ExceptionRefs.ArgumentNull.New("nameof(value)")           // new global::System.ArgumentNullException(nameof(value))
TypeRef.From("Guid").Call("Parse", "input", "provider")   // Guid.Parse(input, provider)
TypeRef.From("string").Member("Empty")                    // string.Empty
JsonRefs.Converter("OrderId").TypeOf()                    // typeof(global::...JsonConverter<OrderId>)
TaskRefs.CancellationToken.Default()                      // default(global::System.Threading.CancellationToken)
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

## ExpressionRef

Fluent builder for C# expression strings — the expression-domain counterpart to `TypeRef`.

A `TypeRef` crosses into expression domain via gateway methods (`New`, `Call`, `Member`, etc.).
Once in expression domain, chaining continues through `ExpressionRef`. Both types convert
implicitly to `string`. `TypeRef` converts implicitly to `ExpressionRef` (one-way gate).

### Chaining Methods

| Method | Description |
|--------|-------------|
| `Call(method, params args)` | Method call: `expr.Method(args)` |
| `Member(name)` | Member access: `expr.Member` |
| `Invoke(params args)` | Delegate invocation: `expr?.Invoke(args)` |
| `As(TypeRef)` | Safe cast: `expr as Type` |
| `Cast(TypeRef)` | Direct cast: `(Type)expr` |
| `Is(TypeRef)` | Type check: `expr is Type` |
| `Is(TypeRef, name)` | Pattern variable: `expr is Type name` |
| `OrDefault(fallback)` | Null coalescing: `expr ?? fallback` |
| `NullForgiving()` | Null forgiving: `expr!` |
| `NullConditionalMember(name)` | `expr?.Member` |
| `NullConditionalCall(method, args)` | `expr?.Method(args)` |
| `Await()` | `await expr` |
| `ConfigureAwait(bool)` | `expr.ConfigureAwait(false)` |
| `Parenthesize()` | `(expr)` |

### Examples

```csharp
// Chained member access + method call
ExpressionRef.From("value").Member("Name").Call("ToUpper")
// "value.Name.ToUpper()"

// Delegate invocation with fallback
TypeRef.From("OnSave").Invoke("id").OrDefault(TaskRefs.CompletedTask)
// "OnSave?.Invoke(id) ?? global::System.Threading.Tasks.Task.CompletedTask"

// Async dispose pattern
ExpressionRef.From("disposable")
    .Call("DisposeAsync")
    .ConfigureAwait(false)
    .Await()
// "await disposable.DisposeAsync().ConfigureAwait(false)"

// Type check with pattern variable
ExpressionRef.From("obj").Is(TypeRef.From("string"), "text")
// "obj is string text"

// Safe cast with null forgiving
ExpressionRef.From("value").As("string").Parenthesize().NullForgiving()
// "(value as string)!"
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

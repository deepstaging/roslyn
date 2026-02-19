# Expressions

Static factory classes that produce `ExpressionRef` instances for common .NET code patterns.

> **See also:** [TypeRef & Primitives](emit/type-ref.md) | [Types](types.md) | [LanguageExt Expressions](languageext/expressions.md)

## Overview

Expression factories pair with [typed wrappers](types.md) to form the second layer of the type system. Where `TaskTypeRef` represents the *type* `Task<T>`, `TaskExpression` produces *code* like `Task.CompletedTask` or `Task.FromResult(value)`.

```csharp
using Deepstaging.Roslyn.Expressions;

// Produce expression code without string assembly
TaskExpression.CompletedTask           // → "global::System.Threading.Tasks.Task.CompletedTask"
TaskExpression.FromResult("42")        // → "global::System.Threading.Tasks.Task.FromResult(42)"

EqualityComparerExpression.DefaultEquals("string", "_name", "value")
// → "global::System.Collections.Generic.EqualityComparer<string>.Default.Equals(_name, value)"
```

---

## TaskExpression

Async task patterns from `System.Threading.Tasks`.

| Method | Produces |
|--------|----------|
| `CompletedTask` | `Task.CompletedTask` |
| `CompletedValueTask` | `ValueTask.CompletedTask` |
| `FromResult(value)` | `Task.FromResult(value)` |
| `FromResult(type, value)` | `Task.FromResult<T>(value)` |
| `Run(expr)` | `Task.Run(expr)` |
| `Delay(delay)` | `Task.Delay(delay)` |
| `WhenAll(tasks...)` | `Task.WhenAll(t1, t2, ...)` |
| `WhenAny(tasks...)` | `Task.WhenAny(t1, t2, ...)` |

```csharp
// Fallback for nullable delegate
TypeRef.From("OnSave").Invoke("entity")
    .OrDefault(TaskExpression.CompletedTask)
    .Await()
// → "await OnSave?.Invoke(entity) ?? global::System.Threading.Tasks.Task.CompletedTask"

// Wrap a synchronous value
TaskExpression.FromResult("count")
// → "global::System.Threading.Tasks.Task.FromResult(count)"
```

---

## EqualityComparerExpression

Equality comparison via `EqualityComparer<T>.Default`.

| Method | Produces |
|--------|----------|
| `Default(type)` | `EqualityComparer<T>.Default` |
| `DefaultEquals(type, left, right)` | `EqualityComparer<T>.Default.Equals(left, right)` |
| `DefaultGetHashCode(type, value)` | `EqualityComparer<T>.Default.GetHashCode(value)` |

```csharp
EqualityComparerExpression.DefaultEquals("string", "_name", "value")
// → "global::System.Collections.Generic.EqualityComparer<string>.Default.Equals(_name, value)"

EqualityComparerExpression.DefaultGetHashCode("int", "_id")
// → "global::System.Collections.Generic.EqualityComparer<int>.Default.GetHashCode(_id)"
```

---

## CollectionExpression

Constructor expressions for `List<T>`, `Dictionary<K,V>`, and `HashSet<T>`.

| Method | Produces |
|--------|----------|
| `NewList(elementType)` | `new List<T>()` |
| `NewList(elementType, capacity)` | `new List<T>(capacity)` |
| `NewDictionary(keyType, valueType)` | `new Dictionary<K, V>()` |
| `NewDictionary(keyType, valueType, capacity)` | `new Dictionary<K, V>(capacity)` |
| `NewHashSet(elementType)` | `new HashSet<T>()` |
| `NewHashSet(elementType, comparer)` | `new HashSet<T>(comparer)` |

```csharp
CollectionExpression.NewList("Order")
// → "new global::System.Collections.Generic.List<Order>()"

CollectionExpression.NewDictionary("string", "int", "16")
// → "new global::System.Collections.Generic.Dictionary<string, int>(16)"
```

---

## NullableExpression

Member access patterns for `Nullable<T>`.

| Method | Produces |
|--------|----------|
| `HasValue(expr)` | `expr.HasValue` |
| `Value(expr)` | `expr.Value` |
| `GetValueOrDefault(expr)` | `expr.GetValueOrDefault()` |
| `GetValueOrDefault(expr, default)` | `expr.GetValueOrDefault(default)` |

```csharp
NullableExpression.HasValue("age")       // → "age.HasValue"
NullableExpression.GetValueOrDefault("score", "0")  // → "score.GetValueOrDefault(0)"
```

---

## PropertyChangedExpression

`INotifyPropertyChanged` event patterns.

| Method | Produces |
|--------|----------|
| `NewEventArgs(name)` | `new PropertyChangedEventArgs("Name")` |
| `NewChangingEventArgs(name)` | `new PropertyChangingEventArgs("Name")` |
| `Raise(handler, sender, args)` | `handler?.Invoke(sender, args)` |

```csharp
PropertyChangedExpression.NewEventArgs("Name")
// → "new global::System.ComponentModel.PropertyChangedEventArgs(\"Name\")"

PropertyChangedExpression.Raise("PropertyChanged", "this", "e")
// → "PropertyChanged?.Invoke(this, e)"
```

---

## LazyExpression

`Lazy<T>` construction and access.

| Method | Produces |
|--------|----------|
| `New(type, factory)` | `new Lazy<T>(factory)` |
| `New(type, factory, isThreadSafe)` | `new Lazy<T>(factory, isThreadSafe)` |
| `Value(expr)` | `expr.Value` |
| `IsValueCreated(expr)` | `expr.IsValueCreated` |

```csharp
LazyExpression.New("ExpensiveService", "() => new ExpensiveService()")
// → "new global::System.Lazy<ExpensiveService>(() => new ExpensiveService())"

LazyExpression.Value("_lazyService")  // → "_lazyService.Value"
```

---

## DisposableExpression

Dispose patterns for `IDisposable` and `IAsyncDisposable`.

| Method | Produces |
|--------|----------|
| `Dispose(expr)` | `expr.Dispose()` |
| `ConditionalDispose(expr)` | `expr?.Dispose()` |
| `DisposeAsync(expr)` | `await expr.DisposeAsync()` |
| `DisposeAsyncCall(expr)` | `expr.DisposeAsync()` (without await) |
| `ConditionalDisposeAsync(expr)` | `if (expr != null) await expr.DisposeAsync()` |

```csharp
DisposableExpression.Dispose("_connection")
// → "_connection.Dispose()"

DisposableExpression.DisposeAsync("_stream")
// → "await _stream.DisposeAsync()"
```

---

## ComparerExpression

Ordering comparison via `Comparer<T>.Default`.

| Method | Produces |
|--------|----------|
| `Default(type)` | `Comparer<T>.Default` |
| `DefaultCompare(type, left, right)` | `Comparer<T>.Default.Compare(left, right)` |

```csharp
ComparerExpression.DefaultCompare("int", "_priority", "other._priority")
// → "global::System.Collections.Generic.Comparer<int>.Default.Compare(_priority, other._priority)"
```

---

## ImmutableCollectionExpression

Factory methods for immutable collections.

| Method | Produces |
|--------|----------|
| `EmptyArray(type)` | `ImmutableArray<T>.Empty` |
| `CreateArray(type, items...)` | `ImmutableArray.Create<T>(items)` |
| `EmptyDictionary(keyType, valueType)` | `ImmutableDictionary<K, V>.Empty` |
| `CreateDictionaryBuilder(keyType, valueType)` | `ImmutableDictionary.CreateBuilder<K, V>()` |

```csharp
ImmutableCollectionExpression.EmptyArray("string")
// → "global::System.Collections.Immutable.ImmutableArray<string>.Empty"

ImmutableCollectionExpression.CreateArray("int", "1", "2", "3")
// → "global::System.Collections.Immutable.ImmutableArray.Create<int>(1, 2, 3)"
```

---

## Builder Extensions

Expression factories also include builder extensions that wire common patterns into `TypeBuilder` and `PropertyBuilder`.

### WithNotifyingSetter

Configures a property setter with equality guard and `INotifyPropertyChanged` notification.

```csharp
using Deepstaging.Roslyn.Expressions;
using Deepstaging.Roslyn.Emit.Interfaces.Observable;

var type = TypeBuilder.Class("ViewModel")
    .ImplementsINotifyPropertyChanged()
    .AddProperty("Name", "string", p => p
        .WithNotifyingSetter("_name", alsoNotify: ["FullName"]));
```

Generates:

```csharp
public string Name
{
    get => _name;
    set
    {
        if (EqualityComparer<string>.Default.Equals(_name, value)) return;
        _name = value;
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(FullName));
    }
}
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `fieldName` | `string` | The backing field name |
| `onPropertyChangedMethod` | `string` | Method to call (default: `"OnPropertyChanged"`) |
| `alsoNotify` | `params string[]` | Additional properties to notify |

### WithLazyField

Adds a `Lazy<T>` backing field and read-only property exposing `.Value`.

```csharp
var type = TypeBuilder.Class("Service")
    .WithLazyField(
        TypeRef.From("ExpensiveService"),
        "_lazyService",
        ExpressionRef.From("() => new ExpensiveService()"),
        "Service");
```

Generates:

```csharp
private readonly Lazy<ExpensiveService> _lazyService =
    new Lazy<ExpensiveService>(() => new ExpensiveService());

public ExpensiveService Service => _lazyService.Value;
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `valueType` | `TypeRef` | The lazily-initialized type |
| `fieldName` | `string` | The backing field name |
| `factory` | `ExpressionRef` | The factory expression |
| `propertyName` | `string` | The public property name |

# TaskRefs

Async types and well-known API calls for `System.Threading.Tasks` and `System.Threading`.

> **See also:** [Refs Overview](index.md) | [TypeRef & Primitives](../type-ref.md)

---

## Types

| Member | Returns | Produces |
|--------|---------|----------|
| `Task()` | `TypeRef` | `Task` |
| `Task(result)` | `TypeRef` | `Task<T>` |
| `ValueTask()` | `TypeRef` | `ValueTask` |
| `ValueTask(result)` | `TypeRef` | `ValueTask<T>` |
| `CancellationToken` | `TypeRef` | `CancellationToken` |

## Expressions

| Member | Returns | Produces |
|--------|---------|----------|
| `CompletedTask` | `ExpressionRef` | `Task.CompletedTask` |
| `CompletedValueTask` | `ExpressionRef` | `ValueTask.CompletedTask` |

## API Call Helpers

| Method | Returns | Produces |
|--------|---------|----------|
| `FromResult(value)` | `ExpressionRef` | `Task.FromResult(value)` |
| `FromResult(resultType, value)` | `ExpressionRef` | `Task.FromResult<T>(value)` |
| `Run(expression)` | `ExpressionRef` | `Task.Run(expression)` |
| `Delay(delay)` | `ExpressionRef` | `Task.Delay(delay)` |
| `WhenAll(tasks...)` | `ExpressionRef` | `Task.WhenAll(tasks...)` |
| `WhenAny(tasks...)` | `ExpressionRef` | `Task.WhenAny(tasks...)` |

---

## Examples

### Return Types and Parameters

```csharp
// Async method returning Task<List<Order>>
method
    .WithReturnType(TaskRefs.Task(CollectionRefs.List("Order")))
    .AddParameter("ct", TaskRefs.CancellationToken, p => p.WithDefaultValue("default"))
```

### Completed Task Expressions

```csharp
// Return completed task in non-async method
body.AddReturn(TaskRefs.CompletedTask)
// → return global::System.Threading.Tasks.Task.CompletedTask

// Null-coalescing with completed task
TypeRef.From("OnSave").Invoke("id").OrDefault(TaskRefs.CompletedTask)
// → OnSave?.Invoke(id) ?? global::System.Threading.Tasks.Task.CompletedTask
```

### API Call Helpers

```csharp
// Task.FromResult
body.AddReturn(TaskRefs.FromResult("result"))
// → return global::System.Threading.Tasks.Task.FromResult(result)

// Task.FromResult<T> (generic overload)
body.AddReturn(TaskRefs.FromResult(TypeRef.From("string"), "default!"))
// → return global::System.Threading.Tasks.Task.FromResult<string>(default!)

// Task.WhenAll
body.AddStatement($"await {TaskRefs.WhenAll("task1", "task2")}")
// → await global::System.Threading.Tasks.Task.WhenAll(task1, task2)

// Task.Delay with await
body.AddStatement($"await {TaskRefs.Delay("TimeSpan.FromSeconds(1)")}")
// → await global::System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1))
```

### Combined with Other Refs

```csharp
// Full async method
builder.AddMethod("GetByIdAsync", m => m
    .WithReturnType(TaskRefs.Task("Customer?"))
    .Async()
    .AddParameter("id", SystemRefs.Guid)
    .AddParameter("ct", TaskRefs.CancellationToken, p => p.WithDefaultValue("default"))
    .WithBody(b => b
        .AddReturn(EntityFrameworkRefs.FindAsync("_db.Customers", "id").Await())))
```

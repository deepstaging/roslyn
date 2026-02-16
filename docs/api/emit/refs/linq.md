# LinqRefs & DelegateRefs

LINQ query types, expression trees, and delegate type factories.

> **See also:** [Refs Overview](index.md) | [TypeRef & Primitives](../type-ref.md)

---

## LinqRefs

`System.Linq` and `System.Linq.Expressions` types.

### Types

| Method | Returns | Produces |
|--------|---------|----------|
| `IQueryable(element)` | `TypeRef` | `IQueryable<T>` |
| `IOrderedQueryable(element)` | `TypeRef` | `IOrderedQueryable<T>` |
| `Expression(delegate)` | `TypeRef` | `Expression<TDelegate>` |

### Examples

```csharp
// IQueryable return type
method.WithReturnType(LinqRefs.IQueryable("Customer"))
// → global::System.Linq.IQueryable<Customer>

// Expression tree parameter (common in EF Core)
method.AddParameter("predicate",
    LinqRefs.Expression(DelegateRefs.Func("Customer", "bool")))
// → global::System.Linq.Expressions.Expression<global::System.Func<Customer, bool>>

// Ordered queryable
method.WithReturnType(LinqRefs.IOrderedQueryable("Product"))
```

---

## DelegateRefs

`Func<>` and `Action<>` delegate type references with arbitrary arity.

### Types

| Method | Returns | Produces |
|--------|---------|----------|
| `Func(typeArgs...)` | `TypeRef` | `Func<T1, ..., TResult>` |
| `Action()` | `TypeRef` | `Action` |
| `Action(typeArgs...)` | `TypeRef` | `Action<T1, ...>` |

!!! tip "Func requires at least one type argument"
    The last type argument to `Func` is always the return type: `Func("string", "bool")` → `Func<string, bool>`.

### Examples

```csharp
// Simple predicate
DelegateRefs.Func("string", "bool")
// → global::System.Func<string, bool>

// Multi-parameter function
DelegateRefs.Func("int", "string", "bool")
// → global::System.Func<int, string, bool>

// Parameterless action
DelegateRefs.Action()
// → global::System.Action

// Action with parameters
DelegateRefs.Action("int")
// → global::System.Action<int>

// Callback parameter
method.AddParameter("onComplete", DelegateRefs.Action("bool"))
// parameter type: global::System.Action<bool>

// Factory parameter
method.AddParameter("factory", DelegateRefs.Func("IServiceProvider", "IMyService"))

// Combined with LinqRefs for expression trees
LinqRefs.Expression(DelegateRefs.Func("Order", "decimal"))
// → global::System.Linq.Expressions.Expression<global::System.Func<Order, decimal>>
```

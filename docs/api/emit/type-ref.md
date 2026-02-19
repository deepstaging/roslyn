# TypeRef & Primitives

Type-safe primitives for referencing types, expressions, attributes, and namespaces in generated code.

> **See also:** [Types](../types.md) | [Expressions](../expressions.md) | [Emit Overview](index.md) | [Support Types](support-types.md)

## Overview

The type system has four primitive types. Each represents a different domain in generated C# code:

| Primitive | Domain | Example |
|-----------|--------|---------|
| `TypeRef` | Type positions — parameters, return types, generics | `Task<string>` |
| `ExpressionRef` | Value positions — calls, member access, literals | `Task.CompletedTask` |
| `AttributeRef` | Attribute positions — decorators | `[Key]`, `[MaxLength(100)]` |
| `NamespaceRef` | Namespace declarations and using directives | `System.Text.Json` |

For type-safe generic wrappers (e.g., `TaskTypeRef`, `ListTypeRef`), see [Types](../types.md).
For expression factories (e.g., `TaskExpression`, `EqualityComparerExpression`), see [Expressions](../expressions.md).

```csharp
// TypeRef → type positions
var task = new TaskTypeRef("string");
method.WithReturnType(task);  // implicit TypeRef conversion

// ExpressionRef → value positions
body.AddReturn(TaskExpression.CompletedTask)

// AttributeRef → attribute positions
property.WithAttribute(AttributeRef.Global("System.ComponentModel.DataAnnotations.KeyAttribute"))
```

---

## TypeRef

A fully-qualified C# type name. Immutable — every method returns a new instance.

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

These methods cross from the **type domain** into the **expression domain**, returning `ExpressionRef`:

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
TypeRef.Global("System.ArgumentNullException").New("nameof(value)")
// → new global::System.ArgumentNullException(nameof(value))

TypeRef.From("Guid").Call("Parse", "input", "provider")
// → Guid.Parse(input, provider)

TypeRef.From("string").Member("Empty")
// → string.Empty

TypeRef.Global("System.Threading.CancellationToken").Default()
// → default(global::System.Threading.CancellationToken)
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

`TypeRef` converts implicitly to and from `string`, and one-way to `ExpressionRef`:

```csharp
TypeRef typeRef = "string";                        // from string
string code = new TaskTypeRef("string");           // to string (via implicit)
ExpressionRef expr = TypeRef.From("Task");         // to ExpressionRef (one-way)
```

---

## ExpressionRef

A C# expression string — the value-domain counterpart to `TypeRef`.

A `TypeRef` crosses into expression domain via gateway methods (`New`, `Call`, `Member`, etc.).
Once in expression domain, chaining continues through `ExpressionRef`. Both types convert
implicitly to `string`. `TypeRef` → `ExpressionRef` is a one-way gate.

### Factory Methods

| Method | Description |
|--------|-------------|
| `From(string)` | Create from an expression string |

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
TypeRef.From("OnSave").Invoke("id").OrDefault(TaskExpression.CompletedTask)
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

## AttributeRef

A type-safe attribute reference that bridges to `AttributeBuilder`.

`AttributeRef` has implicit conversions to both `string` and `AttributeBuilder`, so it works
directly with all existing `.WithAttribute()` overloads — no new overloads needed.

### Factory Methods

| Method | Description |
|--------|-------------|
| `From(string)` | Create from an attribute type name |
| `Global(string)` | Create with `global::` prefix |

### Bridge to AttributeBuilder

These methods return an `AttributeBuilder` for configuring arguments:

| Method | Returns | Description |
|--------|---------|-------------|
| `WithArgument(value)` | `AttributeBuilder` | Add a positional argument |
| `WithArguments(values...)` | `AttributeBuilder` | Add multiple positional arguments |
| `WithNamedArgument(name, value)` | `AttributeBuilder` | Add a named argument |
| `AddUsing(namespace)` | `AttributeBuilder` | Add a using directive |

### Three Ways to Use

```csharp
// 1. Simple — implicit string conversion
property.WithAttribute(AttributeRef.Global("System.ComponentModel.DataAnnotations.KeyAttribute"))

// 2. With arguments — bridge to AttributeBuilder
property.WithAttribute(
    AttributeRef.Global("System.ComponentModel.DataAnnotations.MaxLengthAttribute")
        .WithArgument("100"))

// 3. With configure callback — implicit string conversion for name
property.WithAttribute(
    AttributeRef.Global("System.ComponentModel.DataAnnotations.Schema.ColumnAttribute"),
    a => a.WithArgument("\"order_date\"").WithNamedArgument("TypeName", "\"date\""))
```

### Implicit Conversions

```csharp
var key = AttributeRef.Global("System.ComponentModel.DataAnnotations.KeyAttribute");
string name = key;              // to string
AttributeBuilder builder = key; // to AttributeBuilder
```

---

## NamespaceRef

A lightweight primitive representing a .NET namespace. Central factory for creating `TypeRef` and `AttributeRef` instances.

### Factory Methods

| Method | Description |
|--------|-------------|
| `From(string)` | Create from a dotted namespace string |

### Type Factories

| Method | Returns | Description |
|--------|---------|-------------|
| `Type(name)` | `TypeRef` | `Namespace.TypeName` (no prefix) |
| `GlobalType(name)` | `TypeRef` | `global::Namespace.TypeName` |
| `Attribute(name)` | `AttributeRef` | `Namespace.TypeName` (no prefix) |
| `GlobalAttribute(name)` | `AttributeRef` | `global::Namespace.TypeName` |

### Utilities

| Method | Returns | Description |
|--------|---------|-------------|
| `Append(segment)` | `NamespaceRef` | Child namespace: `Namespace.Segment` |
| `AsStatic()` | `string` | `static Namespace` (for static usings) |
| `Value` | `string` | The raw namespace string |

### Examples

```csharp
var ns = NamespaceRef.From("MyCompany.Domain");

// Child namespaces
NamespaceRef events = ns.Append("Events");          // MyCompany.Domain.Events
NamespaceRef models = ns.Append("Models");           // MyCompany.Domain.Models

// Type references
TypeRef eventType = events.Type("OrderCreated");     // MyCompany.Domain.Events.OrderCreated
TypeRef globalType = events.GlobalType("OrderCreated"); // global::MyCompany.Domain.Events.OrderCreated

// Attribute references
AttributeRef attr = ns.Attribute("MyAttribute");        // MyCompany.Domain.Events.MyAttribute
AttributeRef globalAttr = ns.GlobalAttribute("MyAttribute"); // global::...MyAttribute

// Static using
builder.AddUsing(ns.AsStatic())  // using static MyCompany.Domain.Events;
```

!!! tip "When to use `Type()` vs `GlobalType()`"
    Use `GlobalType()` (and `GlobalAttribute()`) in source generators — the `global::` prefix
    prevents conflicts with user-defined types. Use `Type()` when the prefix isn't needed.

# Refs

Pre-built, globally-qualified type references for common .NET namespaces.

> **See also:** [TypeRef & Primitives](../type-ref.md) | [Emit Overview](../index.md)

## Overview

Every source generator needs type references — `Task<T>`, `ILogger<T>`, `JsonSerializer`, `[Key]`.
Refs classes provide these as ready-made `TypeRef`, `ExpressionRef`, and `AttributeRef` instances so you never hand-write `"global::System.Threading.Tasks.Task"` strings.

```csharp
// Types
method.WithReturnType(TaskRefs.Task(CollectionRefs.IReadOnlyList("Order")))
field.WithType(LoggingRefs.ILoggerOf("CustomerService"))

// Expressions
body.AddReturn(TaskRefs.CompletedTask)
body.AddStatement($"{ExceptionRefs.ThrowIfNull("value")};")

// Attributes
property.WithAttribute(EntityFrameworkRefs.Key)
property.WithAttribute(EntityFrameworkRefs.MaxLength.WithArgument("100"))
```

Each class follows the same pattern: a `Namespace` property, type factories, and well-known API call helpers.

---

## Quick Reference

### By Domain

| Domain | Class | Namespace | Page |
|--------|-------|-----------|------|
| **Collections** | `CollectionRefs` | `System.Collections.Generic` | [→](collections.md) |
| | `ImmutableCollectionRefs` | `System.Collections.Immutable` | [→](collections.md#immutablecollectionrefs) |
| **Async** | `TaskRefs` | `System.Threading.Tasks` | [→](tasks.md) |
| **Serialization** | `JsonRefs` | `System.Text.Json` | [→](json.md) |
| | `EncodingRefs` | `System.Text` | [→](json.md#encodingrefs) |
| **HTTP** | `HttpRefs` | `System.Net.Http` | [→](http.md) |
| **Exceptions** | `ExceptionRefs` | `System` | [→](exceptions.md) |
| **Hosting** | `DependencyInjectionRefs` | `Microsoft.Extensions.DependencyInjection` | [→](hosting.md) |
| | `ConfigurationRefs` | `Microsoft.Extensions.Configuration` | [→](hosting.md#configurationrefs) |
| | `LoggingRefs` | `Microsoft.Extensions.Logging` | [→](hosting.md#loggingrefs) |
| **LINQ** | `LinqRefs` | `System.Linq` | [→](linq.md) |
| | `DelegateRefs` | `System` | [→](linq.md#delegaterefs) |
| **System** | `SystemRefs` | `System` | [→](system.md) |
| **Diagnostics** | `DiagnosticsRefs` | `System.Diagnostics` | [→](diagnostics.md) |
| **Entity Framework** | `EntityFrameworkRefs` | `Microsoft.EntityFrameworkCore` | [→](efcore.md) |

### By Return Type

Every member returns one of three types. Knowing which type you get tells you what you can do with it:

| Return Type | Used For | Example |
|-------------|----------|---------|
| `TypeRef` | Type positions — parameters, return types, generics, fields | `TaskRefs.Task("string")` |
| `ExpressionRef` | Value positions — method calls, member access, literals | `TaskRefs.CompletedTask` |
| `AttributeRef` | Attribute positions — `[Key]`, `[MaxLength(100)]` | `EntityFrameworkRefs.Key` |

!!! tip "Type Safety"
    The return type prevents misuse at compile time. You can't accidentally use `TaskRefs.CompletedTask` (an expression) as a parameter type, or `EntityFrameworkRefs.Key` (an attribute) as a return type.

---

## Extensibility

Define your own Refs class for any namespace:

```csharp
public static class MediatRRefs
{
    public static NamespaceRef Namespace =>
        NamespaceRef.From("MediatR");

    // Types
    public static TypeRef IMediator => Namespace.GlobalType("IMediator");
    public static TypeRef IRequest(TypeRef response) =>
        Namespace.GlobalType($"IRequest<{response.Value}>");

    // API Calls
    public static ExpressionRef Send(ExpressionRef mediator, ExpressionRef request) =>
        ExpressionRef.From(mediator).Call("Send", request);
}

// Usage in a source generator:
builder
    .AddField("_mediator", MediatRRefs.IMediator)
    .AddMethod("Handle", m => m
        .WithReturnType(TaskRefs.Task("Unit"))
        .AddParameter("request", MediatRRefs.IRequest("MyCommand"))
        .WithBody(b => b
            .AddReturn(MediatRRefs.Send("_mediator", "request").Await())));
```

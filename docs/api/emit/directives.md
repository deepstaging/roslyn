# Directives

Preprocessor directives for conditional compilation in generated code.

## Overview

The `Directives` class provides pre-defined constants for common .NET target frameworks, enabling you to generate code that conditionally compiles based on the target framework.

```csharp
// Conditional interface implementation
TypeBuilder.Struct("MyId")
    .Implements("IEquatable<MyId>")                           // Always
    .Implements("ISpanFormattable", Directives.Net6OrGreater) // NET6+
    .Implements("IParsable<MyId>", Directives.Net7OrGreater)  // NET7+
    .Implements("IUtf8SpanFormattable", Directives.Net8OrGreater); // NET8+
```

Generates:

```csharp
public struct MyId : IEquatable<MyId>,
#if NET6_0_OR_GREATER
        ISpanFormattable,
#endif
#if NET7_0_OR_GREATER
        IParsable<MyId>,
#endif
#if NET8_0_OR_GREATER
        IUtf8SpanFormattable,
#endif
{
    // ...
}
```

---

## Interface-Level Directives

Wrap interface implementations in `#if/#endif` using `ConditionalInterface`:

```csharp
TypeBuilder.Struct("UserId")
    .Implements("IFormattable")
    .Implements("ISpanFormattable", Directives.Net6OrGreater)
    .Implements("IUtf8SpanFormattable", Directives.Net8OrGreater);
```

The `Implements` overload accepting a `Directive` creates a `ConditionalInterface` internally. You can also create them explicitly:

```csharp
// Explicit ConditionalInterface
var conditionalInterface = new ConditionalInterface("ISpanFormattable", Directives.Net6OrGreater);

// String implicitly converts to unconditional interface
ConditionalInterface unconditional = "IEquatable<UserId>";
```

---

## Member-Level Directives

Wrap entire methods, properties, fields, etc. in `#if/#endif`:

```csharp
// Conditional method
TypeBuilder.Struct("MyId")
    .AddMethod("Parse", m => m
        .When(Directives.Net7OrGreater)
        .AsStatic()
        .WithReturnType("MyId")
        .AddParameter("input", "string")
        .AddParameter("provider", "IFormatProvider?")
        .WithBody(b => b.AddReturn("new MyId()")));

// Conditional property
TypeBuilder.Class("MyClass")
    .AddProperty("SpanValue", "ReadOnlySpan<char>", p => p
        .When(Directives.NetCoreApp21OrGreater)
        .WithAutoPropertyAccessors());

// Conditional field
TypeBuilder.Class("MyClass")
    .AddField("_buffer", "Memory<byte>", f => f
        .When(Directives.Net6OrGreater));
```

---

## Statement-Level Directives

Wrap code blocks within method bodies:

```csharp
BodyBuilder.Empty()
    .AddStatement("DoSomething()")
    .When(Directives.Net6OrGreater, b => b
        .AddStatement("var span = value.AsSpan()"))
    .When(Directives.Net7OrGreater, b => b
        .AddStatement("Utf8.TryWrite(buffer, out written, format)"))
    .AddStatement("DoSomethingElse()");
```

---

## Pre-defined Directives

### .NET Version Directives

| Directive | Condition |
|-----------|-----------|
| `Directives.Net5OrGreater` | `NET5_0_OR_GREATER` |
| `Directives.Net6OrGreater` | `NET6_0_OR_GREATER` |
| `Directives.Net6Only` | `NET6_0_OR_GREATER && !NET7_0_OR_GREATER` |
| `Directives.Net7OrGreater` | `NET7_0_OR_GREATER` |
| `Directives.NotNet7OrGreater` | `!NET7_0_OR_GREATER` |
| `Directives.Net8OrGreater` | `NET8_0_OR_GREATER` |
| `Directives.Net9OrGreater` | `NET9_0_OR_GREATER` |

### .NET Core Directives

| Directive | Condition |
|-----------|-----------|
| `Directives.NetCoreApp` | `NETCOREAPP` |
| `Directives.NetCoreApp21OrGreater` | `NETCOREAPP2_1_OR_GREATER` |
| `Directives.NetCoreApp30OrGreater` | `NETCOREAPP3_0_OR_GREATER` |
| `Directives.NetCoreApp31OrGreater` | `NETCOREAPP3_1_OR_GREATER` |

### .NET Standard Directives

| Directive | Condition |
|-----------|-----------|
| `Directives.NetStandard` | `NETSTANDARD` |
| `Directives.NetStandard20OrGreater` | `NETSTANDARD2_0_OR_GREATER` |
| `Directives.NetStandard21OrGreater` | `NETSTANDARD2_1_OR_GREATER` |

### .NET Framework Directives

| Directive | Condition |
|-----------|-----------|
| `Directives.NetFramework` | `NETFRAMEWORK` |
| `Directives.NetFramework461OrGreater` | `NET461_OR_GREATER` |
| `Directives.NetFramework472OrGreater` | `NET472_OR_GREATER` |
| `Directives.NetFramework48OrGreater` | `NET48_OR_GREATER` |

---

## Custom Directives

Create custom directives for feature flags or other conditions:

```csharp
// Custom directive
builder.When(Directives.Custom("MY_FEATURE_FLAG"));
```

---

## Combining Directives

Combine directives with logical operators:

```csharp
// AND: Both conditions must be true
var directive = Directives.Net6OrGreater.And(Directives.Custom("ENABLE_SPANS"));
// Generates: #if NET6_0_OR_GREATER && ENABLE_SPANS

// OR: Either condition can be true
var directive = Directives.Net6OrGreater.Or(Directives.NetStandard21OrGreater);
// Generates: #if NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER

// NOT: Negate a condition
var directive = Directives.NetFramework.Not();
// Generates: #if !NETFRAMEWORK
```

---

## Complete Example

Generating a strongly-typed ID similar to [StronglyTypedId](https://github.com/andrewlock/StronglyTypedId):

```csharp
var result = TypeBuilder
    .Struct("UserId")
    .InNamespace("MyApp.Domain")
    .AsPartial()
    .Implements("IEquatable<UserId>")
    .Implements("IComparable<UserId>")
    .Implements("IFormattable")
    .Implements("ISpanFormattable", Directives.Net6OrGreater)
    .Implements("IParsable<UserId>", Directives.Net7OrGreater)
    .Implements("ISpanParsable<UserId>", Directives.Net7OrGreater)
    .Implements("IUtf8SpanFormattable", Directives.Net8OrGreater)
    .AddProperty("Value", "Guid", p => p.WithAutoPropertyAccessors().AsReadOnly())
    .AddConstructor(ctor => ctor
        .AddParameter("value", "Guid")
        .WithBody(b => b.AddStatement("Value = value")))
    .AddMethod("Parse", m => m
        .When(Directives.Net7OrGreater)
        .AsStatic()
        .WithReturnType("UserId")
        .AddParameter("s", "string")
        .AddParameter("provider", "IFormatProvider?")
        .WithBody(b => b.AddReturn("new UserId(Guid.Parse(s, provider))")))
    .AddMethod("TryFormat", m => m
        .When(Directives.Net6OrGreater)
        .WithReturnType("bool")
        .AddParameter("destination", "Span<char>")
        .AddParameter("charsWritten", "int", p => p.AsOut())
        .AddParameter("format", "ReadOnlySpan<char>", p => p.WithDefaultValue("default"))
        .AddParameter("provider", "IFormatProvider?", p => p.WithDefaultValue("null"))
        .WithBody(b => b.AddReturn("Value.TryFormat(destination, out charsWritten, format)")))
    .Emit();
```

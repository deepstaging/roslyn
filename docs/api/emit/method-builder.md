# MethodBuilder

Create method declarations.

> **See also:** [Emit Overview](index.md) | [TypeBuilder](type-builder.md) | [BodyBuilder](body-builder.md)

## Factory Methods

| Method | Description |
|--------|-------------|
| `For(string name)` | Create a method with the given name |
| `Parse(string signature)` | Parse from signature (e.g., `"public async Task<int> GetCount()"`) |

## Return Type

```csharp
method.WithReturnType("void")
method.WithReturnType("string")
method.WithReturnType("Task<int>")
method.WithReturnType("IAsyncEnumerable<Order>")
```

## Accessibility & Modifiers

```csharp
method.WithAccessibility(Accessibility.Public)
method.WithAccessibility("public")  // from snapshot or ValidSymbol.AccessibilityString
method.AsStatic()
method.AsVirtual()
method.AsOverride()
method.AsAbstract()
method.Async()
```

## Type Parameters

```csharp
method.AddTypeParameter("T")
method.AddTypeParameter("T", tp => tp
    .AsClass()
    .WithNewConstraint())
method.AddTypeParameter(typeParameterBuilder)
```

## Parameters

```csharp
method.AddParameter("name", "string")
method.AddParameter("count", "int", p => p.WithDefaultValue("0"))
method.AddParameter(parameterBuilder)
```

## Body

```csharp
// Block body
method.WithBody(body => body
    .AddStatement("Console.WriteLine(\"Starting\");")
    .AddStatement("DoWork();")
    .AddReturn("result"))

// Expression body
method.WithExpressionBody("_name")

// Append to expression body
method.AppendExpressionBody(".ToList()")
```

## Attributes & XML Documentation

```csharp
method.WithAttribute("HttpGet")
method.WithAttribute("Route", attr => attr.WithArgument("\"/api/items\""))

method.WithXmlDoc("Gets the customer name.")
method.WithXmlDoc(doc => doc
    .Summary("Gets a customer by identifier.")
    .Param("id", "The customer identifier.")
    .Returns("The customer if found; otherwise, null."))
```

## Usings

```csharp
method.AddUsing("System.Linq")
method.AddUsings("System", "System.Threading.Tasks")
```

## Properties

```csharp
method.Name             // string
method.ReturnType       // string?
method.ExtensionTargetType // string? — the type being extended (for extension methods)
```

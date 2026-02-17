# Support Types

Additional types supporting the emit API.

> **See also:** [Emit Overview](index.md)

---

## AttributeBuilder

Create attribute declarations.

### Factory Methods

| Method | Description |
|--------|-------------|
| `For(string name)` | Create an attribute |

### Arguments

```csharp
attr.WithArgument("\"value\"")
attr.WithArguments("1", "2", "3")
attr.WithNamedArgument("Name", "\"value\"")
```

### Usings

```csharp
attr.AddUsing("System.ComponentModel")
```

### Properties

```csharp
attr.Name    // string
attr.Usings  // ImmutableArray<string>
```

---

## XmlDocumentationBuilder

Create XML documentation comments.

### Factory Methods

| Method | Description |
|--------|-------------|
| `Create()` | Create empty documentation |
| `WithSummary(string)` | Create with just a summary |
| `From(XmlDocumentation)` | Create from existing parsed documentation |

### Content

```csharp
doc.Summary("Gets the customer name.")
doc.Remarks("This method queries the database.")
doc.Returns("The customer name if found; otherwise, null.")
doc.Value("The property value.")
doc.Param("id", "The customer identifier.")
doc.TypeParam("T", "The entity type.")
doc.Exception("InvalidOperationException", "Thrown when...")
doc.SeeAlso("OtherClass")
doc.Example("<code>var name = GetName(123);</code>")
```

### Raw XML Mode

When content contains intentional XML markup (e.g., `<see cref="..."/>`, `<list>` elements), enable raw XML mode to skip escaping:

```csharp
doc.WithRawXml()
```

This affects content in params, returns, value, exceptions, and seealso — summary and remarks are always unescaped.

### Properties

```csharp
doc.HasContent  // bool
doc.RawXml      // bool
```

---

## EmitOptions

Configure code emission.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `ValidationLevel` | `ValidationLevel` | None, Syntax, Semantic, Full |
| `Indentation` | `string` | Indentation string (default: 4 spaces) |
| `EndOfLine` | `string` | Line ending (default: `\n`) |
| `HeaderComment` | `string` | Comment at top of file |
| `LicenseHeader` | `string?` | License header text |

### Static Instances

```csharp
EmitOptions.Default      // syntax validation, standard formatting
EmitOptions.NoValidation // skip validation
```

### Usage

```csharp
var options = EmitOptions.Default with
{
    ValidationLevel = ValidationLevel.Semantic,
    HeaderComment = "// Auto-generated"
};

var result = builder.Emit(options);
```

### ValidationLevel Enum

| Value | Description |
|-------|-------------|
| `None` | No validation |
| `Syntax` | Syntax validation only (default) |
| `Semantic` | Semantic validation (requires compilation) |
| `Full` | Full validation |

---

## Directive

Represents a preprocessor directive condition. See [Directives](directives.md) for full documentation.

```csharp
// Pre-defined directives
Directives.Net6OrGreater
Directives.Net7OrGreater

// Custom directive
Directives.Custom("MY_FEATURE_FLAG")

// Combining directives
Directives.Net6OrGreater.And(Directives.Custom("ENABLE_SPANS"))
Directives.Net6OrGreater.Or(Directives.NetStandard21OrGreater)
Directives.NetFramework.Not()
```

---

## ConditionalInterface

Represents an interface implementation that may be conditionally compiled.

```csharp
// Implicit conversion from string (unconditional)
ConditionalInterface iface = "IEquatable<UserId>";

// Explicit conditional interface
var conditional = new ConditionalInterface("ISpanFormattable", Directives.Net6OrGreater);

// Used with TypeBuilder.Implements
builder.Implements("ISpanFormattable", Directives.Net6OrGreater);
```

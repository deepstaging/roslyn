# Other Builders

Additional emit builders for events, indexers, parameters, type parameters, operators, and conversion operators.

> **See also:** [Emit Overview](index.md) | [TypeBuilder](type-builder.md)

---

## OperatorBuilder

Create operator overloads (binary and unary operators).

### Factory Methods - Comparison

| Method | Description |
|--------|-------------|
| `Equality(type)` | Create an equality operator (`==`) |
| `Inequality(type)` | Create an inequality operator (`!=`) |
| `LessThan(type)` | Create a less-than operator (`<`) |
| `GreaterThan(type)` | Create a greater-than operator (`>`) |
| `LessThanOrEqual(type)` | Create a less-than-or-equal operator (`<=`) |
| `GreaterThanOrEqual(type)` | Create a greater-than-or-equal operator (`>=`) |

### Factory Methods - Arithmetic

| Method | Description |
|--------|-------------|
| `Addition(type)` | Create an addition operator (`+`) |
| `Subtraction(type)` | Create a subtraction operator (`-`) |
| `Multiplication(type)` | Create a multiplication operator (`*`) |
| `Division(type)` | Create a division operator (`/`) |
| `Modulus(type)` | Create a modulus operator (`%`) |

### Factory Methods - Unary

| Method | Description |
|--------|-------------|
| `UnaryPlus(type)` | Create a unary plus operator (`+`) |
| `UnaryMinus(type)` | Create a unary minus operator (`-`) |
| `LogicalNot(type)` | Create a logical negation operator (`!`) |
| `BitwiseComplement(type)` | Create a bitwise complement operator (`~`) |
| `Increment(type)` | Create an increment operator (`++`) |
| `Decrement(type)` | Create a decrement operator (`--`) |
| `True(type)` | Create a `true` operator |
| `False(type)` | Create a `false` operator |

### Factory Methods - Generic

| Method | Description |
|--------|-------------|
| `Binary(op, leftType, rightType, returnType)` | Create any binary operator |
| `Unary(op, operandType, returnType)` | Create any unary operator |

### Body

```csharp
// Expression body
op.WithExpressionBody("left.Equals(right)")

// Block body
op.WithBody(body => body
    .AddStatement("var result = left.Value == right.Value;")
    .AddReturn("result"))
```

### Attributes and Documentation

```csharp
op.WithAttribute("MethodImpl", a => a.WithArgument("MethodImplOptions.AggressiveInlining"))
op.WithXmlDoc("Compares two values for equality.")
op.AddUsing("System.Runtime.CompilerServices")
```

### Example

```csharp
// Standalone builder
var equality = OperatorBuilder.Equality("CustomerId")
    .WithExpressionBody("left.Equals(right)");

typeBuilder.AddOperator(equality)

// Convenience methods on TypeBuilder (recommended)
TypeBuilder.Struct("CustomerId")
    .AddEqualityOperator("left.Equals(right)")
    .AddInequalityOperator("!left.Equals(right)")

// Lambda configuration
TypeBuilder.Struct("Money")
    .AddOperator(_ => OperatorBuilder.Addition("Money")
        .WithExpressionBody("new Money(left.Amount + right.Amount)"))
    .AddOperator(_ => OperatorBuilder.UnaryMinus("Money")
        .WithExpressionBody("new Money(-operand.Amount)"))

// Custom binary operator with different types
TypeBuilder.Struct("Vector")
    .AddOperator(_ => OperatorBuilder.Binary("*", "Vector", "int", "Vector")
        .WithExpressionBody("new Vector(left.X * right, left.Y * right)"))
```

### Properties

```csharp
op.Operator    // string — the operator symbol
op.IsUnary     // bool — true for unary, false for binary
op.ReturnType  // string
```

---

## ConversionOperatorBuilder

Create explicit or implicit conversion operators.

### Factory Methods

| Method | Description |
|--------|-------------|
| `Explicit(targetType, sourceType, parameterName = "value")` | Create an explicit conversion operator |
| `Implicit(targetType, sourceType, parameterName = "value")` | Create an implicit conversion operator |

### Body

```csharp
// Expression body
op.WithExpressionBody("new MyType(value)")

// Block body
op.WithBody(body => body
    .AddStatement("var result = new MyType();")
    .AddStatement("result.Value = value;")
    .AddReturn("result"))
```

### Attributes and Documentation

```csharp
op.WithAttribute("Obsolete")
op.WithAttribute("Obsolete", a => a.WithArgument("\"Use other conversion\""))
op.WithXmlDoc("Converts from int to MyType.")
op.WithXmlDoc(doc => doc.Summary("Converts from int to MyType."))
op.AddUsing("System")
```

### Example

```csharp
// Standalone builder
var explicitConversion = ConversionOperatorBuilder.Explicit("UserId", "Guid")
    .WithExpressionBody("new UserId(value)")
    .WithXmlDoc("Creates a UserId from a Guid.");

typeBuilder.AddConversionOperator(explicitConversion)

// Convenience methods on TypeBuilder (recommended)
TypeBuilder.Struct("UserId")
    .AddExplicitConversion("Guid", op => op
        .WithExpressionBody("new UserId(value)"))
    .AddExplicitConversionTo("Guid", op => op
        .WithExpressionBody("value.Value"))
    .AddImplicitConversion("string", op => op
        .WithExpressionBody("new UserId(Guid.Parse(value))"))
    .AddImplicitConversionTo("string", op => op
        .WithExpressionBody("value.Value.ToString()"))
```

### Properties

```csharp
op.IsExplicit   // bool — true for explicit, false for implicit
op.TargetType   // string
op.SourceType   // string
```

---

## EventBuilder

Create event declarations.

### Factory Methods

| Method | Description |
|--------|-------------|
| `For(string name, string type)` | Create an event (e.g., `"PropertyChanged"`, `"PropertyChangedEventHandler?"`) |

### Modifiers

```csharp
event.WithAccessibility(Accessibility.Public)
event.AsStatic()
event.AsVirtual()
event.AsOverride()
event.AsAbstract()
```

### Attributes and Documentation

```csharp
event.WithAttribute("NonSerialized")
event.WithAttribute("Obsolete", a => a.WithArgument("\"Use NewEvent\""))
event.WithXmlDoc("Raised when a property changes.")
event.AddUsing("System.ComponentModel")
```

### Example

```csharp
var changed = EventBuilder.For("PropertyChanged", "PropertyChangedEventHandler?")
    .WithAccessibility(Accessibility.Public)
    .WithXmlDoc("Occurs when a property value changes.");

// Add to type
typeBuilder.AddEvent(changed)
```

### Properties

```csharp
event.Name  // string
event.Type  // string
```

---

## IndexerBuilder

Create indexer declarations (`this[...]`).

### Factory Methods

| Method | Description |
|--------|-------------|
| `For(string type)` | Create an indexer with the specified return type |

### Parameters

```csharp
indexer.AddParameter("index", "int")
indexer.AddParameter("key", "string", p => p.WithAttribute("NotNull"))
```

### Accessors

```csharp
// Expression-bodied getter
indexer.WithGetter("_items[index]")

// Block-bodied accessors
indexer.WithGetter(b => b.AddReturn("_items[index]"))
indexer.WithSetter(b => b.AddStatement("_items[index] = value"))

// Read-only
indexer.AsReadOnly()

// Init-only setter
indexer.WithInitOnlySetter()
```

### Modifiers

```csharp
indexer.WithAccessibility(Accessibility.Public)
indexer.AsVirtual()
indexer.AsOverride()
indexer.AsAbstract()
indexer.AsSealed()
```

### Example

```csharp
var indexer = IndexerBuilder.For("T")
    .AddParameter("index", "int")
    .WithGetter("_items[index]")
    .WithSetter(b => b.AddStatement("_items[index] = value"))
    .WithXmlDoc("Gets or sets the element at the specified index.");

// Add to type
typeBuilder.AddIndexer(indexer)
```

### Properties

```csharp
indexer.Type  // string (return type)
```

---

## ParameterBuilder

Create parameter declarations.

### Factory Methods

| Method | Description |
|--------|-------------|
| `For(string name, string type)` | Create a parameter |

### Configuration

```csharp
param.WithDefaultValue("0")
param.WithDefaultValue("\"default\"")
param.AsRef()
param.AsOut()
param.AsIn()
param.AsParams()
param.AsThis()  // for extension method target
```

### Properties

```csharp
param.Name              // string
param.Type              // string
param.IsExtensionTarget // bool
```

---

## TypeParameterBuilder

Create generic type parameter declarations.

### Factory Methods

| Method | Description |
|--------|-------------|
| `For(string name)` | Create a type parameter |

### Constraints

```csharp
tp.WithConstraint("IDisposable")
tp.AsClass()       // where T : class
tp.AsStruct()      // where T : struct
tp.AsNotNull()     // where T : notnull
tp.WithNewConstraint()  // where T : new()
```

### Properties

```csharp
tp.Name           // string
tp.HasConstraints // bool
```

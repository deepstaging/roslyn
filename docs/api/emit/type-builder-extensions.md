# TypeBuilder Extensions

High-level extensions for implementing common interfaces and operators on generated types.

> **See also:** [TypeBuilder](type-builder.md) | [Patterns](patterns.md) | [Directives](directives.md)

## Overview

These extensions automate the implementation of standard .NET interfaces and operators. They analyze the backing type to generate semantically correct implementations with proper null-handling, conditional compilation for newer APIs, and appropriate XML documentation.

Most extensions support two patterns:

1. **Semantic analysis** - Pass a `ValidSymbol<INamedTypeSymbol>` and the extension infers the correct implementation
2. **Custom expressions** - Provide explicit expression bodies when semantic detection isn't sufficient

---

## Interface Extensions

### `IEquatable<T>`

Implements `IEquatable<T>` with `Equals`, `GetHashCode`, and equality operators.

```csharp
// Using semantic analysis (recommended)
builder.ImplementsIEquatable(backingType, "Value")

// With string comparison option
builder.ImplementsIEquatable(backingType, "Value", StringComparison.OrdinalIgnoreCase)

// Using custom expressions
builder.ImplementsIEquatable(
    equalsExpression: "Value.Equals(other.Value)",
    hashCodeExpression: "Value.GetHashCode()")

// Using PropertyBuilder or FieldBuilder
builder.ImplementsIEquatable(backingType, valueProperty)
```

**Generated members:**
- `bool Equals(T other)`
- `override bool Equals(object? obj)`
- `override int GetHashCode()`
- `operator ==` and `operator !=`

---

### `IComparable<T>`

Implements `IComparable<T>` and `IComparable` with comparison operators.

```csharp
// Using semantic analysis
builder.ImplementsIComparable(backingType, "Value")

// Using custom expression
builder.ImplementsIComparable(compareToExpression: "Value.CompareTo(other.Value)")
```

**Generated members:**
- `int CompareTo(T other)`
- `int CompareTo(object? obj)`
- `operator <`, `>`, `<=`, `>=`

---

### IFormattable

Implements `IFormattable` for format string support.

```csharp
// Using semantic analysis
builder.ImplementsIFormattable(backingType, "Value")

// Using custom expression
builder.ImplementsIFormattable(formatExpression: "Value.ToString(format, formatProvider)")
```

**Generated members:**
- `string ToString(string? format, IFormatProvider? formatProvider)`

---

### ISpanFormattable (NET6+)

Implements `ISpanFormattable` for high-performance formatting to spans.

```csharp
// Using semantic analysis
builder.ImplementsISpanFormattable(backingType, "Value")

// Using custom expression
builder.ImplementsISpanFormattable(
    tryFormatExpression: "Value.TryFormat(destination, out charsWritten, format, provider)")
```

**Generated members:**
- `bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)`

---

### IUtf8SpanFormattable (NET8+)

Implements `IUtf8SpanFormattable` for UTF-8 formatting.

```csharp
// Using semantic analysis
builder.ImplementsIUtf8SpanFormattable(backingType, "Value")

// Using custom expression
builder.ImplementsIUtf8SpanFormattable(
    tryFormatExpression: "Value.TryFormat(utf8Destination, out bytesWritten, format, provider)")
```

**Generated members:**
- `bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider)`

---

### `IParsable<T>` (NET7+)

Implements `IParsable<T>` for parsing from strings.

```csharp
// Using semantic analysis
builder.ImplementsIParsable(backingType, "Value")

// Using custom expressions
builder.ImplementsIParsable(
    parseExpression: "new MyType(int.Parse(s, provider))",
    tryParseExpression: "int.TryParse(s, provider, out var v) ? new MyType(v) : null")
```

**Generated members:**
- `static T Parse(string s, IFormatProvider? provider)`
- `static bool TryParse(string? s, IFormatProvider? provider, out T result)`

---

### `ISpanParsable<T>` (NET7+)

Implements `ISpanParsable<T>` for parsing from character spans.

```csharp
// Using semantic analysis
builder.ImplementsISpanParsable(backingType, "Value")

// Using custom expressions
builder.ImplementsISpanParsable(
    parseExpression: "new MyType(int.Parse(s, provider))",
    tryParseExpression: "int.TryParse(s, provider, out var v) ? new MyType(v) : null")
```

**Generated members:**
- `static T Parse(ReadOnlySpan<char> s, IFormatProvider? provider)`
- `static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out T result)`

---

### `IUtf8SpanParsable<T>` (NET8+)

Implements `IUtf8SpanParsable<T>` for parsing from UTF-8 byte spans.

```csharp
// Using semantic analysis
builder.ImplementsIUtf8SpanParsable(backingType, "Value")
```

**Generated members:**
- `static T Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)`
- `static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out T result)`

---

### ICloneable

Implements `ICloneable` for object cloning.

```csharp
// Value types (returns this)
builder.ImplementsICloneable()

// Reference types with copy expression
builder.ImplementsICloneable(cloneExpression: "new MyClass(Value)")
```

**Generated members:**
- `object Clone()`

---

### IConvertible

Implements `IConvertible` for type conversion support.

```csharp
builder.ImplementsIConvertible(backingType, "Value")
```

**Generated members:** All `IConvertible` methods delegating to the backing value.

---

### INotifyPropertyChanged

Implements `INotifyPropertyChanged` for observable properties.

```csharp
builder.ImplementsINotifyPropertyChanged()
```

**Generated members:**
- `event PropertyChangedEventHandler? PropertyChanged`
- `void OnPropertyChanged(string? propertyName)`
- Uses `[CallerMemberName]` attribute

---

### `IComparer<T>`

Creates a comparer class that implements `IComparer<T>`.

```csharp
// For reference types (with null checks)
TypeBuilder.Class("PersonComparer")
    .ImplementsIComparer("Person", "Name")

// For value types (simpler)
TypeBuilder.Class("MyIdComparer")
    .ImplementsIComparerForValueType("MyId", "Value", "global::System.Guid")

// With custom body
builder.ImplementsIComparer("Item", b => b
    .AddStatement("return x.Priority.CompareTo(y.Priority);"))
```

**Generated members:**
- `int Compare(T? x, T? y)` with null handling for reference types

---

### `IEqualityComparer<T>`

Creates a comparer class that implements `IEqualityComparer<T>`.

```csharp
// Single property comparison
TypeBuilder.Class("PersonEqualityComparer")
    .ImplementsIEqualityComparer("Person", "Id")

// For value types
TypeBuilder.Class("MyIdEqualityComparer")
    .ImplementsIEqualityComparerForValueType("MyId", "Value", "global::System.Guid")

// Multiple properties (uses HashCode.Combine)
builder.ImplementsIEqualityComparer("Order", "CustomerId", "OrderDate", "OrderNumber")
```

**Generated members:**
- `bool Equals(T? x, T? y)` with null handling
- `int GetHashCode(T obj)`

---

### IDisposable

Implements `IDisposable` with the full dispose pattern.

```csharp
// With explicit statements
builder.ImplementsIDisposable("_connection?.Close();", "_stream?.Dispose();")

// For a single field
builder.ImplementsIDisposableForField("_stream")

// For multiple fields
builder.ImplementsIDisposableForFields("_reader", "_writer", "_connection")
```

**Generated members:**
- `private bool _disposed` field
- `void Dispose()` - calls `Dispose(true)` and `GC.SuppressFinalize`
- `protected virtual void Dispose(bool disposing)` - dispose pattern

---

### IAsyncDisposable (NET Core 3.0+)

Implements `IAsyncDisposable` for async resource cleanup.

```csharp
// With explicit statements
builder.ImplementsIAsyncDisposable("await _connection.CloseAsync().ConfigureAwait(false);")

// For a single async-disposable field
builder.ImplementsIAsyncDisposableForField("_asyncStream")

// Combined with IDisposable
builder.ImplementsIDisposableAndIAsyncDisposable(
    ["_stream?.Dispose();"],
    ["await _stream.DisposeAsync().ConfigureAwait(false);"])
```

**Generated members:**
- `private bool _disposed` field
- `async ValueTask DisposeAsync()`
- `protected virtual async ValueTask DisposeAsyncCore()`

---

### `IEnumerable<T>`

Implements `IEnumerable<T>` by delegating to an inner collection.

```csharp
// Delegate to collection field
builder.ImplementsIEnumerable("Item", "_items")

// Custom enumerator expression
builder.ImplementsIEnumerableWith("Item", "_items.Where(x => x.IsActive).GetEnumerator()")
```

**Generated members:**
- `IEnumerator<T> GetEnumerator()`
- `IEnumerator IEnumerable.GetEnumerator()`

---

### `IReadOnlyCollection<T>`

Implements `IReadOnlyCollection<T>` with enumeration and count.

```csharp
builder.ImplementsIReadOnlyCollection("Item", "_items")
```

**Generated members:**
- All `IEnumerable<T>` members
- `int Count` property

---

### `IReadOnlyList<T>`

Implements `IReadOnlyList<T>` with indexed access.

```csharp
builder.ImplementsIReadOnlyList("Item", "_items")
```

**Generated members:**
- All `IReadOnlyCollection<T>` members
- `T this[int index]` indexer

---

### `IAsyncEnumerable<T>` (NET Core 3.0+)

Implements `IAsyncEnumerable<T>` for async iteration.

```csharp
// Delegate to async collection
builder.ImplementsIAsyncEnumerable("Item", "_asyncItems")

// Custom expression
builder.ImplementsIAsyncEnumerableWith("Item", "_source.WhereAsync(x => x.IsValid)")

// With iterator body
builder.ImplementsIAsyncEnumerableWithIterator("int", b => b
    .AddStatement("for (int i = 0; i < 10; i++)")
    .AddStatement("{")
    .AddStatement("    await Task.Delay(100, cancellationToken);")
    .AddStatement("    yield return i;")
    .AddStatement("}"))
```

**Generated members:**
- `IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)`

---

### `IObserver<T>`

Implements `IObserver<T>` for reactive programming.

```csharp
// With expression bodies
builder.ImplementsIObserver("Message", "_handler(value)")

// With custom handlers
builder.ImplementsIObserver(
    "Event",
    "_logger.Log(value)",
    "_logger.LogError(error)",
    "_logger.LogCompleted()")

// Virtual methods for subclassing
builder.ImplementsIObserverVirtual("Event")

// With action delegate fields
builder.ImplementsIObserverWithActionFields("Item")
```

**Generated members:**
- `void OnNext(T value)`
- `void OnError(Exception error)`
- `void OnCompleted()`

---

## Operator Extensions

### Arithmetic Operators (NET7+)

Implements generic math interfaces for arithmetic operations.

```csharp
// Individual operators
builder.ImplementsAdditionOperator(backingType, "Value")
builder.ImplementsSubtractionOperator(backingType, "Value")
builder.ImplementsMultiplicationOperator(backingType, "Value")
builder.ImplementsDivisionOperator(backingType, "Value")
builder.ImplementsModulusOperator(backingType, "Value")

// Unary operators
builder.ImplementsUnaryPlusOperator(backingType, "Value")
builder.ImplementsUnaryNegationOperator(backingType, "Value")
builder.ImplementsIncrementOperator(backingType, "Value")
builder.ImplementsDecrementOperator(backingType, "Value")

// Using custom expressions
builder.ImplementsAdditionOperator("new Money(left.Value + right.Value)")
```

**Generated interfaces (NET7+):**
- `IAdditionOperators<T, T, T>`
- `ISubtractionOperators<T, T, T>`
- `IMultiplicationOperators<T, T, T>`
- `IDivisionOperators<T, T, T>`
- `IModulusOperators<T, T, T>`
- `IUnaryPlusOperators<T, T>`
- `IUnaryNegationOperators<T, T>`
- `IIncrementOperators<T>`
- `IDecrementOperators<T>`

---

### Bitwise Operators (NET7+)

Implements bitwise operation interfaces.

```csharp
builder.ImplementsBitwiseAndOperator(backingType, "Value")
builder.ImplementsBitwiseOrOperator(backingType, "Value")
builder.ImplementsExclusiveOrOperator(backingType, "Value")
builder.ImplementsBitwiseComplementOperator(backingType, "Value")
builder.ImplementsLeftShiftOperator(backingType, "Value")
builder.ImplementsRightShiftOperator(backingType, "Value")
builder.ImplementsUnsignedRightShiftOperator(backingType, "Value")  // NET7+
```

**Generated interfaces (NET7+):**
- `IBitwiseOperators<T, T, T>`
- `IShiftOperators<T, int, T>`

---

### Backing Type Conversions

Adds conversion operators between the type and its backing type.

```csharp
// Both implicit from backing and explicit to backing
builder.WithBackingConversions(backingType, "Value")

// Custom expressions
builder.WithBackingConversions(
    backingTypeName: "int",
    fromBackingExpression: "new MyType(value)",
    toBackingExpression: "value.Value")

// Individual conversions
builder.WithImplicitConversionFromBacking(backingType)
builder.WithExplicitConversionToBacking(backingType, "Value")
```

**Generated operators:**
- `implicit operator T(BackingType value)` - Create from backing value
- `explicit operator BackingType(T value)` - Extract backing value

---

## Factory Pattern Extensions

Extensions for common static factory patterns.

### `WithEmptyFactory`

Adds a `public static readonly Empty` field with the specified initialization.

```csharp
builder.WithEmptyFactory("new UserId(Guid.Empty)")
// Generates: public static readonly UserId Empty = new UserId(Guid.Empty);
```

### `WithDefaultFactory`

Adds a `public static readonly Default` field.

```csharp
builder.WithDefaultFactory("new Config(\"default\")")
// Generates: public static readonly Config Default = new Config("default");
```

### `WithNewFactory`

Adds a `public static T New()` factory method.

```csharp
builder.WithNewFactory("new UserId(Guid.NewGuid())")
// Generates: public static UserId New() => new UserId(Guid.NewGuid());
```

### `WithEmptyAndNewFactory`

Combines `WithEmptyFactory` and `WithNewFactory` for ID types.

```csharp
builder.WithEmptyAndNewFactory(
    emptyExpression: "new UserId(Guid.Empty)",
    newExpression: "new UserId(Guid.NewGuid())")
```

---

## Converter Extensions

Extensions for adding nested converter classes with associated attributes.

### `WithTypeConverter`

Adds a nested `System.ComponentModel.TypeConverter` class.

```csharp
// Full control over method bodies
builder.WithTypeConverter(
    "UserIdTypeConverter",
    canConvertFromBody: "return sourceType == typeof(Guid) || base.CanConvertFrom(context, sourceType);",
    convertFromBody: "return value is Guid g ? new UserId(g) : base.ConvertFrom(context, culture, value);",
    canConvertToBody: "return sourceType == typeof(Guid) || base.CanConvertTo(context, sourceType);",
    convertToBody: "return value is UserId id ? id.Value : base.ConvertTo(context, culture, value, destinationType);",
    addAttribute: true)

// Using configure callback
builder.WithTypeConverter("UserIdTypeConverter", converter => converter
    .AddMethod(MethodBuilder.Parse("public override bool CanConvertFrom(...)")
        .WithBody(...)))
```

**Generated:**
- Nested `TypeConverter` class with all override methods
- `[TypeConverter(typeof(...))]` attribute on parent type (optional)

### `WithJsonConverter`

Adds a nested `System.Text.Json.Serialization.JsonConverter<T>` class.

```csharp
builder.WithJsonConverter(
    "UserIdJsonConverter",
    readExpression: "new(reader.GetGuid())",
    writeExpression: "writer.WriteStringValue(value.Value)",
    // Optional NET6+ property name methods
    readAsPropertyNameExpression: "new(Guid.Parse(reader.GetString()!))",
    writeAsPropertyNameExpression: "writer.WritePropertyName(value.Value.ToString())",
    addAttribute: true)
```

**Generated:**
- Nested `JsonConverter<T>` class
- `Read` and `Write` methods
- `ReadAsPropertyName` and `WriteAsPropertyName` (NET6+, conditional)
- `[JsonConverter(typeof(...))]` attribute on parent type (optional)

### `WithEfCoreValueConverter`

Adds a nested Entity Framework Core `ValueConverter<T, TProvider>` class.

```csharp
// Full control
builder.WithEfCoreValueConverter(
    backingType: "global::System.Guid",
    toProviderExpression: "id => id.Value",
    fromProviderExpression: "value => new UserId(value)")

// Simple pattern for types with Value property
builder.WithEfCoreValueConverterForValue("global::System.Guid")
// Infers: id => id.Value and value => new T(value)

// Custom value accessor
builder.WithEfCoreValueConverterForValue(
    backingType: "global::System.Guid",
    valueAccessor: "Id")  // Uses id.Id instead of id.Value
```

**Generated:**
- Nested `EfCoreValueConverter` class extending `ValueConverter<T, TProvider>`
- Constructor with `ConverterMappingHints?` parameter

---

## Complete Example

Generating a strongly-typed ID with full interface support:

```csharp
// backingType is ValidSymbol<INamedTypeSymbol> for Guid
var result = TypeBuilder
    .Struct("OrderId")
    .InNamespace("MyApp.Domain")
    .AsPartial()
    .AddProperty("Value", "Guid", p => p.WithAutoPropertyAccessors().AsReadOnly())
    .AddConstructor(ctor => ctor
        .AddParameter("value", "Guid")
        .WithBody(b => b.AddStatement("Value = value;")))
    // Equality
    .ImplementsIEquatable(backingType, "Value")
    // Comparison
    .ImplementsIComparable(backingType, "Value")
    // Formatting
    .ImplementsIFormattable(backingType, "Value")
    .ImplementsISpanFormattable(backingType, "Value")
    .ImplementsIUtf8SpanFormattable(backingType, "Value")
    // Parsing
    .ImplementsIParsable(backingType, "Value")
    .ImplementsISpanParsable(backingType, "Value")
    .ImplementsIUtf8SpanParsable(backingType, "Value")
    // Conversions
    .WithBackingConversions(backingType, "Value")
    // ToString
    .OverridesToString("Value")
    .Emit();
```

This generates a type similar to [StronglyTypedId](https://github.com/andrewlock/StronglyTypedId) with:
- Full equality and comparison support
- Multi-target formatting (string, span, UTF-8)
- Multi-target parsing (string, span, UTF-8)
- Implicit/explicit conversions
- Proper conditional compilation for NET6+/NET7+/NET8+

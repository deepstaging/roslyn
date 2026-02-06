# Other Builders

Additional emit builders for events, indexers, parameters, and type parameters.

> **See also:** [Emit Overview](index.md) | [TypeBuilder](type-builder.md)

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

# PropertyBuilder

Create property declarations.

> **See also:** [Emit Overview](index.md) | [TypeBuilder](type-builder.md) | [FieldBuilder](field-builder.md)

## Factory Methods

| Method | Description |
|--------|-------------|
| `For(string name, string type)` | Create a property |
| `Parse(string signature)` | Parse from signature (e.g., `"public string Name { get; set; }"`) |

## Accessor Styles

```csharp
// Auto-property { get; set; }
prop.WithAutoPropertyAccessors()

// Read-only auto-property { get; }
prop.WithAutoPropertyAccessors().AsReadOnly()

// Expression-bodied getter => expression
prop.WithGetter("_name")
prop.WithGetter("=> _name")  // "=>" is optional

// Block-bodied getter
prop.WithGetter(body => body
    .AddStatement("if (_name == null) _name = LoadName();")
    .AddReturn("_name"))

// Block-bodied setter
prop.WithSetter(body => body
    .AddStatement("_name = value;")
    .AddStatement("OnPropertyChanged();"))
```

## Modifiers

```csharp
prop.WithAccessibility(Accessibility.Public)
prop.AsStatic()
prop.AsVirtual()
prop.AsOverride()
prop.AsAbstract()
prop.AsReadOnly()  // removes setter
```

## Initialization

```csharp
prop.WithInitializer("new()")
prop.WithInitializer("default")
prop.WithInitializer("\"Default Value\"")
prop.WithBackingField("_name")  // references a backing field
```

## Attributes & XML Documentation

```csharp
prop.WithAttribute("JsonProperty")
prop.WithXmlDoc("Gets or sets the customer name.")
```

## Usings

```csharp
prop.AddUsing("System.Text.Json.Serialization")
```

## Properties

```csharp
prop.Name   // string
prop.Type   // string
```

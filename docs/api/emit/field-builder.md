# FieldBuilder

Create field declarations.

> **See also:** [Emit Overview](index.md) | [TypeBuilder](type-builder.md) | [PropertyBuilder](property-builder.md)

## Factory Methods

| Method | Description |
|--------|-------------|
| `For(string name, string type)` | Create a field |
| `Parse(string signature)` | Parse from signature |

## Modifiers

```csharp
field.WithAccessibility(Accessibility.Private)
field.AsStatic()
field.AsReadonly()
field.AsConst()
```

## Initialization

```csharp
field.WithInitializer("string.Empty")
field.WithInitializer("42")
```

## Attributes & XML Documentation

```csharp
field.WithAttribute("NonSerialized")
field.WithXmlDoc("The backing field for Name.")
```

## Usings

```csharp
field.AddUsing("System")
```

## Properties

```csharp
field.Name   // string
field.Type   // string
```

# ConstructorBuilder

Create constructor declarations.

> **See also:** [Emit Overview](index.md) | [TypeBuilder](type-builder.md) | [MethodBuilder](method-builder.md)

## Factory Methods

| Method | Description |
|--------|-------------|
| `For(string typeName)` | Create a constructor for the given type |

## Modifiers

```csharp
ctor.WithAccessibility(Accessibility.Public)
ctor.WithAccessibility("public")  // from snapshot or ValidSymbol.AccessibilityString
ctor.AsStatic()
ctor.AsPrimary()  // for primary constructors
```

## Parameters

```csharp
ctor.AddParameter("name", "string")
ctor.AddParameter("email", "string", p => p.WithDefaultValue("null"))
ctor.AddParameter(parameterBuilder)
```

## Body

```csharp
ctor.WithBody(body => body
    .AddStatement("Name = name;")
    .AddStatement("Email = email;"))
```

## Constructor Chaining

```csharp
ctor.CallsBase("name", "email")  // : base(name, email)
ctor.CallsThis("name")           // : this(name)
```

## Attributes & XML Documentation

```csharp
ctor.WithAttribute("Obsolete")
ctor.WithXmlDoc("Initializes a new instance of the Customer class.")
```

## Properties

```csharp
ctor.IsPrimary      // bool
ctor.Parameters     // ImmutableArray<ParameterBuilder>
ctor.Usings         // ImmutableArray<string>
```

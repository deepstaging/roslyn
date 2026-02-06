# BodyBuilder

Build method and property bodies.

> **See also:** [Emit Overview](index.md) | [MethodBuilder](method-builder.md) | [PropertyBuilder](property-builder.md)

## Factory Methods

| Method | Description |
|--------|-------------|
| `Empty()` | Create an empty body |

## Adding Statements

```csharp
body.AddStatement("Console.WriteLine(\"Hello\");")
body.AddStatements("var a = 1;\nvar b = 2;")  // multi-line string
body.AddReturn("result")
body.AddReturn()  // void return
body.AddThrow("new InvalidOperationException()")
body.AddCustom(statementSyntax)  // raw Roslyn syntax
```

## Properties

```csharp
body.IsEmpty  // bool
```

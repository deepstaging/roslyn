# Emit API

Fluent builders for generating C# code using Roslyn's SyntaxFactory.

> **See also:** [Full Emit Documentation](../Docs/Emit.md) | [Queries](../Docs/Queries.md) | [Projections](../Docs/Projections.md) | [Roslyn Toolkit README](../README.md)

## Quick Start

```csharp
var result = TypeBuilder
    .Class("Customer")
    .InNamespace("MyApp.Domain")
    .AddProperty("Id", "Guid", prop => prop
        .WithAccessibility(Accessibility.Public)
        .WithAutoPropertyAccessors())
    .AddProperty("Name", "string", prop => prop
        .WithAccessibility(Accessibility.Public)
        .WithAutoPropertyAccessors())
    .Emit();

if (result.IsValid(out var validEmit))
{
    string code = validEmit.Code;  // Valid, compilable C#
}
```

## Parse API (New!)

Build from natural C# signatures - parse the signature, then customize:

```csharp
// Parse a method signature, then add the body
var method = MethodBuilder.Parse("public async Task<bool> ProcessAsync(string input, CancellationToken ct = default)")
    .WithBody(b => b
        .AddStatement("await Task.Delay(100, ct)")
        .AddReturn("true"));

// Parse a property, optionally customize further
var property = PropertyBuilder.Parse("public string Name { get; set; }")
    .WithAttribute("Required");

// Parse a field
var field = FieldBuilder.Parse("private readonly ILogger _logger");

// Parse a type signature with base types, then add members
var service = TypeBuilder.Parse("public sealed class CustomerService : ICustomerService")
    .InNamespace("MyApp.Services")
    .AddField(field)
    .AddProperty(property)
    .AddMethod(method);
```

### What Parse Handles

| Parsed from Signature | Added via Builder Methods |
|-----------------------|---------------------------|
| Name, type, return type | Method/property bodies |
| Accessibility (public, private, etc.) | Attributes |
| Modifiers (static, async, virtual, etc.) | XML documentation |
| Parameters with modifiers and defaults | Namespace and usings |
| Generic type parameters and constraints | Additional members |
| Base types and interfaces | |

## Components

### Builders
- **TypeBuilder** - Classes, interfaces, structs, records
- **PropertyBuilder** - Auto-properties, expression-bodied, block-bodied
- **MethodBuilder** - Instance, static, async methods
- **ConstructorBuilder** - With parameter and this/base chaining
- **FieldBuilder** - Backing fields with modifiers
- **ParameterBuilder** - Method/constructor parameters
- **BodyBuilder** - String-based statement composition

### Projections
- **ValidEmit** - Guaranteed successful emission (non-null code & syntax)
- **OptionalEmit** - May contain diagnostics or errors

### Configuration
- **EmitOptions** - Formatting (indentation, line endings) and validation levels

## Three Builder Patterns

### 1. Parse + Customize (Natural syntax)
```csharp
MethodBuilder.Parse("public async Task<string> FetchAsync(int id)")
    .WithBody(b => b.AddReturn("await _client.GetAsync(id)"))
```

### 2. Lambda Configuration (Concise)
```csharp
.AddMethod("Process", method => method
    .WithReturnType("bool")
    .AddParameter("value", "string")
    .WithBody(b => b.AddReturn("true")))
```

### 3. Separate Builders (Composable)
```csharp
var processMethod = MethodBuilder
    .For("Process")
    .WithReturnType("bool")
    .AddParameter("value", "string")
    .WithBody(b => b.AddReturn("true"));

typeBuilder.AddMethod(processMethod)
```

## Philosophy

The Emit API is the **write counterpart** to the read API:

| Reading | Writing |
|---------|---------|
| TypeQuery | TypeBuilder |
| ValidSymbol | ValidEmit |
| String filters | String types |
| Fluent, immutable | Fluent, immutable |

## Complete Documentation

See **[Emit.md](../Docs/Emit.md)** for comprehensive documentation with examples for every builder and feature.

## Features

✅ Classes, interfaces, structs, records  
✅ Properties (auto, expression-bodied, block-bodied)  
✅ Methods (instance, static, async, virtual, override)  
✅ Constructors (with this/base chaining)  
✅ Fields (readonly, const, static)  
✅ Parameters (with modifiers: ref, out, in, params)  
✅ String-based body building (simple, maintainable)  
✅ Syntax validation (default, opt-out)  
✅ Generates valid, compilable C# code  
✅ **Parse API** - Build from C# signature strings  

## Status

**Phase 1: MVP Complete** ✅  
- String-based type references
- All core builders implemented
- Tested and validated

**Phase 1.5: Parse API** ✅  
- Parse C# signatures into builders
- MethodBuilder.Parse(), PropertyBuilder.Parse(), FieldBuilder.Parse(), TypeBuilder.Parse()

**Phase 2: Future**  
- Symbol-based type references (TypeReference struct)
- Compilation context support
- Semantic validation

**Phase 3: Future**  
- Expression builder API (if needed)
- Control flow helpers

**Phase 4: Future**  
- Auto-import inference
- EditorConfig integration
- Full C# feature coverage

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](../../../LICENSE) for the full legal text.

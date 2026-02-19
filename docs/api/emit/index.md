# Emit

Fluent builders for generating compilable C# code.

> **See also:** [Queries](../queries/index.md) | [Projections](../projections/index.md) | [Extensions](../extensions/index.md)

## Overview

Emit builders construct Roslyn syntax trees using a fluent, immutable API. Where queries find code, emit builders create it.

| Builder | Description |
|---------|-------------|
| [TypeBuilder](type-builder.md) | Create classes, interfaces, structs, records |
| [TypeBuilder Extensions](type-builder-extensions.md) | Interface and operator implementations |
| [MethodBuilder](method-builder.md) | Create methods |
| [PropertyBuilder](property-builder.md) | Create properties |
| [FieldBuilder](field-builder.md) | Create fields |
| [ConstructorBuilder](constructor-builder.md) | Create constructors |
| [Other Builders](other-builders.md) | Events, indexers, parameters, type parameters |
| [BodyBuilder](body-builder.md) | Create method/property bodies |
| [Patterns](patterns.md) | Builder, Singleton, ToString extensions |
| [Directives](directives.md) | Preprocessor directives for conditional compilation |
| [TypeRef](type-ref.md) | Type-safe primitives for type, expression, and attribute references |
| [Types](../types.md) | Typed wrappers for common .NET types (Task, List, EqualityComparer, etc.) |
| [Expressions](../expressions.md) | Expression factories and builder extensions for common patterns |
| [GlobalUsings](global-usings.md) | Emit `global using` directives as a complete source file |
| [Support Types](support-types.md) | AttributeBuilder, XmlDocumentationBuilder, EmitOptions |

All builders are **immutable** — each method returns a new instance.

```csharp
var result = TypeBuilder
    .Class("Customer")
    .InNamespace("MyApp.Domain")
    .AsPartial()
    .AddProperty("Name", "string", p => p
        .WithAccessibility(Accessibility.Public)
        .WithAutoPropertyAccessors())
    .Emit();

if (result.IsValid(out var valid))
{
    string code = valid.Code;                    // Formatted C# code
    CompilationUnitSyntax syntax = valid.Syntax; // Roslyn syntax tree
}
```

---

## Parse API

Build from natural C# signatures — parse the signature, then customize with builder methods:

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
| Modifiers (static, async, virtual, readonly, etc.) | XML documentation |
| Parameters with modifiers and defaults | Namespace and usings |
| Generic type parameters and constraints | Additional members |
| Base types and interfaces | |
| Property accessors ({ get; set; }) | |
| Field initializers | |

### Parse Examples

```csharp
// Methods with generics and constraints
MethodBuilder.Parse("public T Convert<T>(object value) where T : class")

// Abstract and virtual methods
MethodBuilder.Parse("protected virtual void OnPropertyChanged(string name)")

// Properties with various accessors
PropertyBuilder.Parse("public int Count { get; }")
PropertyBuilder.Parse("public List<string> Items { get; set; } = new()")

// Fields with modifiers
FieldBuilder.Parse("private static int _count = 0")
FieldBuilder.Parse("public const int MaxRetries = 3")

// Types with inheritance
TypeBuilder.Parse("public abstract class BaseHandler<T> : IHandler<T> where T : class")
TypeBuilder.Parse("public partial record OrderDto(string Id, decimal Total)")
```

---

## OptionalEmit

The result of emitting code.

```csharp
var result = builder.Emit();

// Check if valid
if (result.IsValid(out var valid))
{
    string code = valid.Code;                    // Formatted C# code
    CompilationUnitSyntax syntax = valid.Syntax; // Roslyn syntax tree
}

// Check if invalid
if (result.IsNotValid(out var diagnostics))
{
    foreach (var diagnostic in diagnostics)
    {
        Console.WriteLine(diagnostic.GetMessage());
    }
}

// Get diagnostics (warnings even if valid)
ImmutableArray<Diagnostic> diags = result.Diagnostics;
```

### ValidEmit Part Extraction

Once validated, `ValidEmit` exposes structured access to the generated compilation unit:

```csharp
valid.Usings          // ImmutableArray<UsingDirectiveSyntax>
valid.Types           // ImmutableArray<MemberDeclarationSyntax> (unwrapped from namespaces)
valid.Namespace       // string? — the namespace name, or null for global scope
valid.LeadingTrivia   // SyntaxTriviaList — header comments, nullable directives, etc.
```

### Combining Emit Results

Use `Combine()` to merge multiple emit results into a single compilation unit. Usings are deduplicated and types are grouped by namespace.

```csharp
// Combine two ValidEmit results
var combined = emitA.Combine(emitB);

// Chain multiple
var all = emitA.Combine(emitB).Combine(emitC);

// Works with OptionalEmit too — propagates failures and aggregates diagnostics
OptionalEmit merged = optionalA.Combine(optionalB);
```

---

## Complete Example

```csharp
var result = TypeBuilder
    .Class("CustomerRepository")
    .InNamespace("MyApp.Data")
    .AddUsing("System")
    .AddUsing("System.Threading.Tasks")
    .Implements("ICustomerRepository")
    .WithXmlDoc("Repository for customer data access.")
    .AddField("_context", "DbContext", f => f
        .WithAccessibility(Accessibility.Private)
        .AsReadonly())
    .AddConstructor(ctor => ctor
        .WithAccessibility(Accessibility.Public)
        .AddParameter("context", "DbContext")
        .WithBody(body => body.AddStatement("_context = context;")))
    .AddMethod("GetByIdAsync", m => m
        .WithReturnType("Task<Customer?>")
        .WithAccessibility(Accessibility.Public)
        .Async()
        .AddParameter("id", "Guid")
        .AddParameter("cancellationToken", "CancellationToken", p => p.WithDefaultValue("default"))
        .WithXmlDoc(doc => doc
            .Summary("Gets a customer by identifier.")
            .Param("id", "The customer identifier.")
            .Param("cancellationToken", "Cancellation token.")
            .Returns("The customer if found; otherwise, null."))
        .WithBody(body => body
            .AddReturn("await _context.Customers.FindAsync(new object[] { id }, cancellationToken)")))
    .Emit();

if (result.IsValid(out var valid))
{
    context.AddSource("CustomerRepository.g.cs", valid.Code);
}
```

---

## Conditional Compilation

Use [Directives](directives.md) to generate framework-specific code:

```csharp
TypeBuilder.Struct("UserId")
    .Implements("IEquatable<UserId>")
    .Implements("ISpanFormattable", Directives.Net6OrGreater)
    .Implements("IParsable<UserId>", Directives.Net7OrGreater)
    .AddMethod("TryFormat", m => m
        .When(Directives.Net6OrGreater)
        .WithReturnType("bool")
        .WithBody(...));
```

---

## Real-World Usage

### Generating from Analyzed Symbols

```csharp
// Generate a module class from analyzed type information
return TypeBuilder.Parse($"public static partial class {model.EffectsContainerName}")
    .AddUsings(usings)
    .InNamespace(model.Namespace)
    .AddNestedType(module)
    .Emit(options ?? EmitOptions.Default);
```

### Adding Methods Dynamically

```csharp
var builder = TypeBuilder.Class("Generated");

foreach (var method in methods)
{
    builder = builder.AddMethod(method.Name, m => m
        .AsStatic()
        .WithReturnType($"Eff<RT, {method.ReturnType}>")
        .AddParameter("input", method.InputType)
        .WithXmlDoc(method.XmlDocumentation)
        .WithExpressionBody(GenerateBody(method)));
}
```

### Using Parse for Complex Signatures

```csharp
// Parse handles complex signatures more naturally
var method = MethodBuilder.Parse("public async Task<IEnumerable<Customer>> GetAllAsync()")
    .AddParameter("filter", "CustomerFilter", p => p.WithDefaultValue("null"))
    .WithBody(body => body
        .AddStatement("var query = _context.Customers.AsQueryable();")
        .AddStatement("if (filter != null) query = query.Where(filter.ToPredicate());")
        .AddReturn("await query.ToListAsync()"));
```

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](https://github.com/deepstaging/roslyn/blob/main/LICENSE) for the full legal text.

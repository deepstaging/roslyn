# SymbolTestContext

Query and extract symbols from compiled source code in tests.

> **See also:** [RoslynTestBase](RoslynTestBase.md) | [Roslyn Queries](../../Deepstaging.Roslyn/Docs/Queries.md)

## Overview

`SymbolTestContext` wraps a `Compilation` and provides convenient methods for finding and querying symbols. It's the foundation for symbol-based testing.

```csharp
var ctx = SymbolsFor(@"
    public class Customer 
    {
        public string Name { get; set; }
        public void Save() { }
    }");

var type = ctx.RequireNamedType("Customer");
var prop = ctx.RequireProperty("Name");
var method = ctx.RequireMethod("Save");
```

---

## Get vs Require

Every symbol accessor has two variants:

| Pattern | Returns | On Not Found |
|---------|---------|--------------|
| `Get*` | `OptionalSymbol<T>` | Returns empty |
| `Require*` | `ValidSymbol<T>` | Throws exception |

```csharp
// Optional - check before use
var maybeType = ctx.GetNamedType("Customer");
if (maybeType.IsNotValid(out var valid))
{
    // Handle missing type
    return;
}
// Use valid.Value

// Required - throws if not found
var type = ctx.RequireNamedType("Customer");
// Use type.Value directly
```

---

## Symbol Accessors

### Types

```csharp
// Named types (classes, structs, interfaces, records, enums)
OptionalSymbol<INamedTypeSymbol> type = ctx.GetNamedType("Customer");
ValidSymbol<INamedTypeSymbol> type = ctx.RequireNamedType("Customer");

// Any type (includes type parameters, arrays, etc.)
OptionalSymbol<ITypeSymbol> type = ctx.GetType("Customer");
ValidSymbol<ITypeSymbol> type = ctx.RequireType("Customer");
```

### Namespaces

```csharp
OptionalSymbol<INamespaceSymbol> ns = ctx.GetNamespace("MyApp.Services");
ValidSymbol<INamespaceSymbol> ns = ctx.RequireNamespace("MyApp.Services");
```

### Members

These search across all types in the compilation:

```csharp
// Methods
ValidSymbol<IMethodSymbol> method = ctx.RequireMethod("ProcessAsync");

// Properties
ValidSymbol<IPropertySymbol> prop = ctx.RequireProperty("Name");

// Fields
ValidSymbol<IFieldSymbol> field = ctx.RequireField("_logger");

// Parameters (from all methods)
ValidSymbol<IParameterSymbol> param = ctx.RequireParameter("customerId");
```

---

## Fluent Type Queries

### Query Members on a Specific Type

```csharp
// Get all public methods on Customer
var methods = ctx.Type("Customer")
    .Methods()
    .ThatArePublic()
    .GetAll();

// Get required properties
var required = ctx.Type("Customer")
    .Properties()
    .ThatAreRequired()
    .GetAll();

// Get constructors
var ctors = ctx.Type("Customer")
    .Constructors()
    .GetAll();
```

### Query Types in Source Code

The `Types()` method queries only types defined in your source (excludes referenced assemblies):

```csharp
// All public classes in source
var classes = ctx.Types()
    .ThatAreClasses()
    .ThatArePublic()
    .GetAll();

// Interfaces with a specific attribute
var marked = ctx.Types()
    .ThatAreInterfaces()
    .WithAttribute("ServiceContract")
    .GetAll();
```

### Query All Types in Compilation

Use `AllTypesInCompilation()` when you need to include types from referenced assemblies:

```csharp
var allWithAttribute = ctx.AllTypesInCompilation()
    .ThatAreClasses()
    .WithAttribute("Serializable")
    .Query()
    .GetAll();
```

---

## Projections

### Map Types

Transform a symbol into a custom model:

```csharp
var model = ctx.Map("Customer", typeSymbol => 
    new CustomerModel
    {
        Name = typeSymbol.OrNull()?.Name,
        PropertyCount = typeSymbol.QueryProperties().GetAll().Length
    });
```

### Query with Compilation Context

Some projections need both the symbol and compilation:

```csharp
var info = ctx.Query(
    symbols => symbols.RequireNamedType("MyRuntime"),
    (symbol, compilation) => symbol.QuerySystemInfo(compilation));
```

---

## Accessing the Compilation

```csharp
var ctx = SymbolsFor(source);

// Get the underlying compilation
Compilation compilation = ctx.Compilation;

// Use for advanced scenarios
var semanticModel = compilation.GetSemanticModel(compilation.SyntaxTrees.First());
```

---

## Common Patterns

### Testing Symbol Properties

```csharp
[Test]
public async Task CustomerHasExpectedProperties()
{
    var ctx = SymbolsFor(@"
        public class Customer 
        {
            public required string Name { get; set; }
            public int Age { get; set; }
        }");

    var props = ctx.Type("Customer").Properties().GetAll();
    
    await Assert.That(props).HasCount(2);
    await Assert.That(props.Any(p => p.Name == "Name" && p.IsRequired)).IsTrue();
}
```

### Testing Method Signatures

```csharp
[Test]
public async Task ProcessAsyncHasCorrectSignature()
{
    var ctx = SymbolsFor(@"
        public class Service 
        {
            public async Task<bool> ProcessAsync(string input, CancellationToken ct) 
                => true;
        }");

    var method = ctx.RequireMethod("ProcessAsync");
    
    await Assert.That(method.Value.IsAsync).IsTrue();
    await Assert.That(method.Value.Parameters).HasCount(2);
    await Assert.That(method.Value.ReturnType.Name).IsEqualTo("Task");
}
```

### Testing Attribute Data

```csharp
[Test]
public async Task TypeHasExpectedAttribute()
{
    var ctx = SymbolsFor(@"
        [Obsolete(""Use CustomerV2"")]
        public class Customer { }");

    var type = ctx.RequireNamedType("Customer");
    var attr = type.Value.GetAttribute("Obsolete");
    
    await Assert.That(attr.IsValid).IsTrue();
}
```

---

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](../../../LICENSE) for the full legal text.

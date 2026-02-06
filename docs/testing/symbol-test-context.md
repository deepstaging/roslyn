# SymbolTestContext

Query and extract symbols from compiled source code in tests.

> **See also:** [RoslynTestBase](roslyn-test-base.md) | [Roslyn Queries](../api/queries/index.md)

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

Use the fluent TUnit assertions from `Deepstaging.Roslyn.Testing.Assertions`:

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

    var type = ctx.RequireNamedType("Customer");
    var props = ctx.Type("Customer").Properties().GetAll();
    
    // Use Deepstaging.Roslyn.Testing assertions
    await Assert.That(type).IsClassSymbol();
    await Assert.That(type).IsPublicSymbol();
    await Assert.That(props).HasCount(2);
    
    var nameProp = props.First(p => p.Name == "Name");
    await Assert.That(nameProp).IsRequired();
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
    
    // Fluent assertions for method symbols
    await Assert.That(method).IsAsyncSymbol();
    await Assert.That(method).IsPublicSymbol();
    await Assert.That(method).HasParameterCount(2);
    await Assert.That(method).HasReturnType();
}
```

### Testing Type Symbols

```csharp
[Test]
public async Task TypeHasExpectedShape()
{
    var ctx = SymbolsFor(@"
        public partial record Customer<T> where T : class { }");

    var type = ctx.RequireNamedType("Customer");
    
    await Assert.That(type).IsRecordSymbol();
    await Assert.That(type).IsPartialSymbol();
    await Assert.That(type).IsGenericSymbol();
    await Assert.That(type).HasTypeParameterCount(1);
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
    
    // Fluent attribute assertions
    await Assert.That(type).HasAttribute("Obsolete");
    await Assert.That(type).DoesNotHaveAttribute("Serializable");
}
```

### Testing Field Symbols

```csharp
[Test]
public async Task FieldHasExpectedModifiers()
{
    var ctx = SymbolsFor(@"
        public class Service 
        {
            private readonly ILogger _logger;
            public static int Counter;
        }");

    var logger = ctx.RequireField("_logger");
    var counter = ctx.RequireField("Counter");
    
    await Assert.That(logger).IsPrivateSymbol();
    await Assert.That(logger).IsReadOnly();
    await Assert.That(counter).IsPublicSymbol();
    await Assert.That(counter).IsStaticSymbol();
}
```

---

## Available Assertions

Deepstaging.Roslyn.Testing provides fluent TUnit assertions for all symbol types:

### ValidSymbol&lt;T&gt; (All Symbols)

| Assertion | Description |
|-----------|-------------|
| `IsNamed(name)` | Symbol has the specified name |
| `NameStartsWith(prefix)` | Name starts with prefix |
| `NameEndsWith(suffix)` | Name ends with suffix |
| `NameContains(substring)` | Name contains substring |
| `IsPublicSymbol()` | Symbol is public |
| `IsPrivateSymbol()` | Symbol is private |
| `IsInternalSymbol()` | Symbol is internal |
| `IsProtectedSymbol()` | Symbol is protected |
| `IsStaticSymbol()` | Symbol is static |
| `IsAbstractSymbol()` | Symbol is abstract |
| `IsSealedSymbol()` | Symbol is sealed |
| `IsVirtualSymbol()` | Symbol is virtual |
| `HasAttribute(name)` | Symbol has the attribute |
| `DoesNotHaveAttribute(name)` | Symbol lacks the attribute |

### ValidSymbol&lt;INamedTypeSymbol&gt;

| Assertion | Description |
|-----------|-------------|
| `IsClassSymbol()` | Type is a class |
| `IsInterfaceSymbol()` | Type is an interface |
| `IsStructSymbol()` | Type is a struct |
| `IsRecordSymbol()` | Type is a record |
| `IsEnumSymbol()` | Type is an enum |
| `IsDelegateSymbol()` | Type is a delegate |
| `IsPartialSymbol()` | Type is partial |
| `IsGenericSymbol()` | Type is generic |
| `HasTypeParameterCount(n)` | Has n type parameters |

### ValidSymbol&lt;IMethodSymbol&gt;

| Assertion | Description |
|-----------|-------------|
| `IsAsyncSymbol()` | Method is async |
| `ReturnsVoid()` | Method returns void |
| `HasReturnType()` | Method has a return type |
| `HasParameterCount(n)` | Has n parameters |
| `IsExtension()` | Is an extension method |
| `IsGeneric()` | Is a generic method |
| `IsConstructor()` | Is a constructor |
| `IsOperator()` | Is an operator |

### ValidSymbol&lt;IPropertySymbol&gt;

| Assertion | Description |
|-----------|-------------|
| `IsRequired()` | Property is required |
| `HasGetter()` | Property has a getter |
| `HasSetter()` | Property has a setter |
| `IsIndexer()` | Property is an indexer |

### ValidSymbol&lt;IFieldSymbol&gt;

| Assertion | Description |
|-----------|-------------|
| `IsReadOnly()` | Field is readonly |
| `IsConst()` | Field is const |
| `IsVolatile()` | Field is volatile |

### OptionalEmit

| Assertion | Description |
|-----------|-------------|
| `IsSuccessful()` | Emit succeeded |
| `HasFailed()` | Emit failed |
| `HasValue()` | Has syntax output |
| `IsEmpty()` | No syntax output |
| `HasDiagnostics()` | Has any diagnostics |
| `HasNoDiagnostics()` | No diagnostics |
| `HasErrors()` | Has error diagnostics |
| `HasWarnings()` | Has warning diagnostics |
| `CodeContains(text)` | Output contains text |
| `CodeDoesNotContain(text)` | Output lacks text |

---

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](https://github.com/deepstaging/roslyn/blob/main/LICENSE) for the full legal text.

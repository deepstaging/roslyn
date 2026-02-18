# SymbolTestContext

Compile source code and query symbols with a fluent API.

## Creating

```csharp
var ctx = SymbolsFor("public class Customer { public string Name { get; set; } }");
```

## Properties

| Property | Type | Description |
|----------|------|-------------|
| `Compilation` | `Compilation` | The underlying Roslyn compilation |

## Getting Symbols

Each symbol kind has a `Get*` variant (returns `OptionalSymbol<T>`) and a `Require*` variant (throws if not found, returns `ValidSymbol<T>`):

| Get (optional) | Require (throws) | Returns |
|----------------|-------------------|---------|
| `GetNamedType(name)` | `RequireNamedType(name)` | `INamedTypeSymbol` |
| `GetType(name)` | `RequireType(name)` | `ITypeSymbol` |
| `GetMethod(name)` | `RequireMethod(name)` | `IMethodSymbol` |
| `GetProperty(name)` | `RequireProperty(name)` | `IPropertySymbol` |
| `GetField(name)` | `RequireField(name)` | `IFieldSymbol` |
| `GetParameter(name)` | `RequireParameter(name)` | `IParameterSymbol` |
| `GetNamespace(name)` | `RequireNamespace(name)` | `INamespaceSymbol` |

## Fluent Type Queries

### Single Type

```csharp
// Query members of a specific type
var properties = ctx.Type("Customer").Properties();
var methods = ctx.Type("Customer").Methods();
var constructors = ctx.Type("Customer").Constructors();
```

### All Types

```csharp
// Source types only (excludes referenced assemblies)
TypeQuery sourceTypes = ctx.Types();

// All types including referenced assemblies
var allTypes = ctx.AllTypesInCompilation()
    .ThatArePublic()
    .ThatAreClasses()
    .WithAttribute("Serializable");
```

## Custom Projections

```csharp
// Map a type through a custom function
string name = ctx.Map<string>("Customer", symbol => symbol.Value!.Name);

// Query with compilation access
var result = ctx.Query<INamedTypeSymbol, bool>(
    c => c.RequireNamedType("Customer"),
    (symbol, compilation) => symbol.IsPartial());
```

## Example

```csharp
[Test]
public async Task Customer_has_expected_properties()
{
    var ctx = SymbolsFor("""
        public class Customer
        {
            public string Name { get; set; }
            public int Age { get; init; }
        }
        """);

    var type = ctx.RequireNamedType("Customer");
    await Assert.That(type.IsClassSymbol()).IsTrue();

    var name = ctx.RequireProperty("Name");
    await Assert.That(name.HasGetter()).IsTrue();
    await Assert.That(name.HasSetter()).IsTrue();

    var age = ctx.RequireProperty("Age");
    await Assert.That(age.HasInitOnlySetter()).IsTrue();
}
```

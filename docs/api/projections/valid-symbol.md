# ValidSymbol<T>

A validated symbol where the underlying value is guaranteed non-null. Created by validating an `OptionalSymbol`.

> **See also:** [Projections Overview](index.md) | [OptionalSymbol](optional-symbol.md)

## Creating

```csharp
ValidSymbol<INamedTypeSymbol>.From(typeSymbol)      // throws if null
ValidSymbol<INamedTypeSymbol>.TryFrom(typeSymbol)   // returns null if input is null

// From OptionalSymbol validation (preferred)
if (optional.IsValid(out var valid)) { /* use valid */ }
```

## Properties

Same properties as `OptionalSymbol`, but return non-nullable types:

```csharp
valid.Value                 // TSymbol — the underlying symbol (guaranteed non-null)
valid.Name                  // string (not nullable)
valid.FullyQualifiedName    // string (not nullable)
valid.Accessibility         // Accessibility (not nullable)
```

## Additional Task Properties

```csharp
valid.IsValueTask           // bool
valid.IsGenericTask         // bool — Task<T>
valid.IsGenericValueTask    // bool — ValueTask<T>
valid.IsNonGenericTask      // bool — Task (no type argument)
valid.IsNonGenericValueTask // bool — ValueTask (no type argument)
```

## Transforming

```csharp
TResult result = valid.Map(s => s.Name);
ValidSymbol<IMethodSymbol> method = valid.MapTo(s => (IMethodSymbol)s);
ValidSymbol<T>? filtered = valid.Where(s => s.IsPublic);
ValidSymbol<IMethodSymbol>? method = valid.OfType<IMethodSymbol>();
valid.Do(s => Console.WriteLine(s.Name));
```

## Attribute Access

```csharp
// Get all attributes
valid.GetAttributes()                           // IEnumerable<ValidAttribute>

// Get by name (supports "Name" or "NameAttribute")
valid.GetAttributes("Obsolete")                 // IEnumerable<ValidAttribute>
valid.GetAttribute("Obsolete")                  // OptionalAttribute

// Get by generic type parameter
valid.GetAttributes<ObsoleteAttribute>()        // IEnumerable<ValidAttribute>
valid.GetAttribute<ObsoleteAttribute>()         // OptionalAttribute

// Get by System.Type (supports open generics for finding any instantiation)
valid.GetAttributes(typeof(MyGenericAttribute<>))  // IEnumerable<ValidAttribute>
valid.GetAttribute(typeof(MyGenericAttribute<>))   // OptionalAttribute

// Check presence
valid.HasAttribute("Obsolete")                  // bool
valid.HasAttribute<ObsoleteAttribute>()         // bool
valid.HasAttribute(typeof(MyGenericAttribute<>)) // bool
valid.LacksAttribute("Obsolete")                // bool
```

### Finding Generic Attributes

For generic attributes like `[HttpClient<TConfig>]`, use `Type` overloads or metadata name:

```csharp
// Using Type with open generic
var attrs = symbol.GetAttributes(typeof(HttpClientAttribute<>));

// Using metadata name with arity
var attrs = symbol.Value.GetAttributesByMetadataName("HttpClientAttribute`1");
```

## Type Hierarchy (INamedTypeSymbol)

```csharp
valid.BaseType                          // OptionalSymbol<INamedTypeSymbol>
valid.GetBaseTypes()                    // IEnumerable<ValidSymbol<INamedTypeSymbol>>
valid.GetInterfaces()                   // IEnumerable<ValidSymbol<INamedTypeSymbol>>
valid.GetAllInterfaces()                // IEnumerable<ValidSymbol<INamedTypeSymbol>>
valid.ImplementsInterface("IDisposable") // bool
valid.InheritsFrom("BaseClass")         // bool
```

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

## Type Hierarchy (INamedTypeSymbol)

```csharp
valid.BaseType                          // OptionalSymbol<INamedTypeSymbol>
valid.GetBaseTypes()                    // IEnumerable<ValidSymbol<INamedTypeSymbol>>
valid.GetInterfaces()                   // IEnumerable<ValidSymbol<INamedTypeSymbol>>
valid.GetAllInterfaces()                // IEnumerable<ValidSymbol<INamedTypeSymbol>>
valid.ImplementsInterface("IDisposable") // bool
valid.InheritsFrom("BaseClass")         // bool
```

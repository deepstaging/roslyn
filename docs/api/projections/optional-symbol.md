# OptionalSymbol<T>

Wraps a Roslyn symbol that may or may not be present.

> **See also:** [Projections Overview](index.md) | [ValidSymbol](valid-symbol.md)

## Creating

```csharp
OptionalSymbol<INamedTypeSymbol>.WithValue(typeSymbol)
OptionalSymbol<INamedTypeSymbol>.Empty()
OptionalSymbol<INamedTypeSymbol>.FromNullable(maybeNull)
```

## Checking Presence

```csharp
if (optional.HasValue) { /* has symbol */ }
if (optional.IsEmpty) { /* no symbol */ }
```

## Extracting Values

```csharp
// Validate to non-nullable wrapper (preferred pattern)
if (optional.IsValid(out var valid))
{
    // valid is ValidSymbol<T> with guaranteed non-null
    Console.WriteLine(valid.Name);
}

// Early exit pattern
if (optional.IsNotValid(out var valid))
    return;
// valid is now ValidSymbol<T>

// Other extraction methods
var symbol = optional.OrThrow("Symbol required");
var maybeNull = optional.OrNull();
var validated = optional.Validate();           // OptionalSymbol → ValidSymbol?
var validated = optional.ValidateOrThrow();    // throws if empty
```

## Transforming

```csharp
// Map to a different type
OptionalValue<string> name = optional.Map(s => s.FullyQualifiedName);

// Filter
OptionalSymbol<T> filtered = optional.Where(s => s.IsPublic());

// Cast to derived type
OptionalSymbol<IMethodSymbol> method = optional.OfType<IMethodSymbol>();

// Select (alias for Map)
OptionalValue<int> count = optional.Select(s => s.GetMembers().Length);
```

## Symbol Identity Properties

```csharp
optional.Name                   // string? — symbol name
optional.Namespace              // string? — containing namespace
optional.FullyQualifiedName     // string? — e.g. "MyApp.Domain.Customer"
optional.GloballyQualifiedName  // string? — e.g. "global::MyApp.Domain.Customer"
optional.DisplayName            // string? — namespace.name format
optional.PropertyName           // string? — suggested property name (PascalCase)
optional.ParameterName          // string? — suggested parameter name (camelCase)
optional.Location               // Location — primary source location
```

## Accessibility Properties

```csharp
optional.Accessibility          // Accessibility? — enum value
optional.AccessibilityString    // string? — "public", "private", etc.
optional.IsPublic               // bool
optional.IsInternal             // bool
optional.IsPrivate              // bool
optional.IsProtected            // bool
```

## Modifier Properties

```csharp
optional.IsStatic               // bool
optional.IsAbstract             // bool
optional.IsSealed               // bool
optional.IsVirtual              // bool
optional.IsOverride             // bool
optional.IsReadOnly             // bool
optional.IsPartial              // bool
optional.IsImplicitlyDeclared   // bool
optional.IsExtern               // bool
```

## Type Classification Properties

```csharp
optional.IsGenericType          // bool
optional.IsValueType            // bool
optional.IsReferenceType        // bool
optional.IsInterface            // bool
optional.IsClass                // bool
optional.IsStruct               // bool
optional.IsRecord               // bool
optional.IsEnum                 // bool
optional.IsDelegate             // bool
optional.IsNullable             // bool
optional.Kind                   // string? — "class", "struct", "interface", etc.
optional.SymbolTypeKind         // TypeKind?
optional.SpecialType            // SpecialType?
```

## Method-Specific Properties

```csharp
optional.IsAsync                // bool
optional.IsExtensionMethod      // bool
```

## Type Hierarchy

```csharp
optional.ContainingType         // OptionalSymbol<INamedTypeSymbol>
optional.BaseType               // OptionalSymbol<INamedTypeSymbol>
optional.Interfaces             // ImmutableArray<ValidSymbol<INamedTypeSymbol>>

// Get all base types in inheritance chain
optional.GetBaseTypes()         // IEnumerable<ValidSymbol<INamedTypeSymbol>>

// Get interfaces
optional.GetInterfaces()        // IEnumerable<ValidSymbol<INamedTypeSymbol>> — direct interfaces
optional.GetAllInterfaces()     // IEnumerable<ValidSymbol<INamedTypeSymbol>> — includes inherited

// Check inheritance and interface implementation
optional.ImplementsInterface("IDisposable")  // bool — checks all interfaces
optional.InheritsFrom("BaseClass")           // bool — checks inheritance chain
```

## Generic Type Support

```csharp
optional.Arity                  // int — number of type parameters
optional.GetTypeArguments()     // ImmutableArray<ValidSymbol<INamedTypeSymbol>>
optional.GetTypeArgument(0)     // OptionalArgument<INamedTypeSymbol>
optional.GetTypeArgumentSymbol(0) // OptionalSymbol<ITypeSymbol>
optional.GetFirstTypeArgument() // OptionalSymbol<ITypeSymbol>
optional.SingleTypeArgument     // OptionalSymbol<ITypeSymbol> (for arity-1 generics)
optional.GetTypeParameters()    // IEnumerable<ValidSymbol<ITypeParameterSymbol>>
optional.GetMethodTypeParameters() // method-specific type parameters
```

## Task Type Support

```csharp
optional.IsTask                 // bool — Task, Task<T>, ValueTask, or ValueTask<T>
optional.InnerTaskType          // OptionalSymbol<ITypeSymbol> — T in Task<T>
```

## Attributes

```csharp
optional.GetAttributes()                    // IEnumerable<ValidAttribute>
optional.GetAttributes("MyAttribute")       // IEnumerable<ValidAttribute>
optional.GetAttributes<ObsoleteAttribute>() // IEnumerable<ValidAttribute>
optional.GetAttribute("MyAttribute")        // OptionalAttribute (first match)
optional.GetAttribute<ObsoleteAttribute>()  // OptionalAttribute (first match)
optional.HasAttributes()                    // bool
optional.HasAttribute("MyAttribute")        // bool
optional.LacksAttributes()                  // bool
optional.LacksAttribute("MyAttribute")      // bool
```

## XML Documentation

```csharp
optional.XmlDocumentationRaw    // string? — raw XML
optional.XmlDocumentation       // XmlDocumentation — parsed structure
optional.HasXmlDocumentation    // bool
```

## Utility Methods

```csharp
optional.Do(s => Console.WriteLine(s.Name));

optional.Match(
    whenPresent: s => HandleSymbol(s),
    whenEmpty: () => HandleEmpty());

optional.Equals(otherSymbol);
optional.DoesNotEqual(otherSymbol);
```

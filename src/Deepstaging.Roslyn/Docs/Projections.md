# Projections

Optional and validated wrappers that make null-checking less painful.

> **See also:** [Queries](Queries.md) | [Emit](Emit.md) | [Extensions](Extensions.md) | [Roslyn Toolkit README](../README.md)

## Overview

Roslyn symbols are often nullable, requiring constant null checks. Projections wrap these nullable values in types that provide safe access and fluent transformations:

| Type | Purpose |
|------|---------|
| `OptionalSymbol<T>` | A symbol that may or may not be present |
| `ValidSymbol<T>` | A symbol guaranteed to be non-null |
| `OptionalAttribute` | An attribute that may or may not be present |
| `ValidAttribute` | An attribute guaranteed to be non-null |
| `OptionalArgument<T>` | An attribute argument that may or may not exist |
| `OptionalValue<T>` | A general-purpose optional wrapper |
| `OptionalSyntax<T>` | A syntax node that may or may not be present |
| `ValidSyntax<T>` | A syntax node guaranteed to be non-null |
| `ValidTypeSyntax<T>` | A type declaration syntax with rich helpers |
| `XmlDocumentation` | Parsed XML documentation from a symbol |

## The Pattern

```csharp
// Without projections — null checks everywhere
var attr = symbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "MyAttribute");
if (attr == null) return;
var value = attr.ConstructorArguments.FirstOrDefault().Value;
if (value is not string s) return;
// finally use s

// With projections — fluent, null-safe operations
var value = symbol
    .GetAttribute("MyAttribute")
    .ConstructorArg<string>(0)
    .OrDefault("fallback");
```

---

## OptionalSymbol<T>

Wraps a Roslyn symbol that may or may not be present.

### Creating

```csharp
OptionalSymbol<INamedTypeSymbol>.WithValue(typeSymbol)
OptionalSymbol<INamedTypeSymbol>.Empty()
OptionalSymbol<INamedTypeSymbol>.FromNullable(maybeNull)
```

### Checking Presence

```csharp
if (optional.HasValue) { /* has symbol */ }
if (optional.IsEmpty) { /* no symbol */ }
```

### Extracting Values

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

### Transforming

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

### Symbol Identity Properties

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

### Accessibility Properties

```csharp
optional.Accessibility          // Accessibility? — enum value
optional.AccessibilityString    // string? — "public", "private", etc.
optional.IsPublic               // bool
optional.IsInternal             // bool
optional.IsPrivate              // bool
optional.IsProtected            // bool
```

### Modifier Properties

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

### Type Classification Properties

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

### Method-Specific Properties

```csharp
optional.IsAsync                // bool
optional.IsExtensionMethod      // bool
```

### Type Hierarchy

```csharp
optional.ContainingType         // OptionalSymbol<INamedTypeSymbol>
optional.BaseType               // OptionalSymbol<INamedTypeSymbol>
optional.Interfaces             // ImmutableArray<INamedTypeSymbol>

// Get all base types in inheritance chain
optional.GetBaseTypes()         // IEnumerable<ValidSymbol<INamedTypeSymbol>>

// Get interfaces
optional.GetInterfaces()        // IEnumerable<ValidSymbol<INamedTypeSymbol>> — direct interfaces
optional.GetAllInterfaces()     // IEnumerable<ValidSymbol<INamedTypeSymbol>> — includes inherited

// Check inheritance and interface implementation
optional.ImplementsInterface("IDisposable")  // bool — checks all interfaces
optional.InheritsFrom("BaseClass")           // bool — checks inheritance chain
```

### Generic Type Support

```csharp
optional.Arity                  // int — number of type parameters
optional.GetTypeArguments()     // ImmutableArray<OptionalSymbol<INamedTypeSymbol>>
optional.GetTypeArgument(0)     // OptionalArgument<INamedTypeSymbol>
optional.GetTypeArgumentSymbol(0) // OptionalSymbol<ITypeSymbol>
optional.GetFirstTypeArgument() // OptionalSymbol<ITypeSymbol>
optional.SingleTypeArgument     // OptionalSymbol<ITypeSymbol> (for arity-1 generics)
optional.GetTypeParameters()    // IEnumerable<OptionalSymbol<ITypeParameterSymbol>>
optional.GetMethodTypeParameters() // method-specific type parameters
```

### Task Type Support

```csharp
optional.IsTask                 // bool — Task, Task<T>, ValueTask, or ValueTask<T>
optional.InnerTaskType          // OptionalSymbol<ITypeSymbol> — T in Task<T>
```

### Attributes

```csharp
optional.GetAttributes()                    // IEnumerable<OptionalAttribute>
optional.GetAttributes("MyAttribute")       // IEnumerable<ValidAttribute>
optional.GetAttributes<ObsoleteAttribute>() // IEnumerable<ValidAttribute>
optional.GetAttribute("MyAttribute")        // OptionalAttribute (first match)
optional.GetAttribute<ObsoleteAttribute>()  // OptionalAttribute (first match)
optional.HasAttributes()                    // bool
optional.HasAttribute("MyAttribute")        // bool
optional.LacksAttributes()                  // bool
optional.LacksAttribute("MyAttribute")      // bool
```

### XML Documentation

```csharp
optional.XmlDocumentationRaw    // string? — raw XML
optional.XmlDocumentation       // XmlDocumentation — parsed structure
optional.HasXmlDocumentation    // bool
```

### Utility Methods

```csharp
optional.Do(s => Console.WriteLine(s.Name));

optional.Match(
    whenPresent: s => HandleSymbol(s),
    whenEmpty: () => HandleEmpty());

optional.Equals(otherSymbol);
optional.DoesNotEqual(otherSymbol);
```

---

## ValidSymbol<T>

A validated symbol where the underlying value is guaranteed non-null. Created by validating an `OptionalSymbol`.

### Creating

```csharp
ValidSymbol<INamedTypeSymbol>.From(typeSymbol)      // throws if null
ValidSymbol<INamedTypeSymbol>.TryFrom(typeSymbol)   // returns null if input is null

// From OptionalSymbol validation (preferred)
if (optional.IsValid(out var valid)) { /* use valid */ }
```

### Properties

Same properties as `OptionalSymbol`, but return non-nullable types:

```csharp
valid.Value                 // TSymbol — the underlying symbol (guaranteed non-null)
valid.Name                  // string (not nullable)
valid.FullyQualifiedName    // string (not nullable)
valid.Accessibility         // Accessibility (not nullable)
```

### Additional Task Properties

```csharp
valid.IsValueTask           // bool
valid.IsGenericTask         // bool — Task<T>
valid.IsGenericValueTask    // bool — ValueTask<T>
valid.IsNonGenericTask      // bool — Task (no type argument)
valid.IsNonGenericValueTask // bool — ValueTask (no type argument)
```

### Transforming

```csharp
TResult result = valid.Map(s => s.Name);
ValidSymbol<IMethodSymbol> method = valid.MapTo(s => (IMethodSymbol)s);
ValidSymbol<T>? filtered = valid.Where(s => s.IsPublic);
ValidSymbol<IMethodSymbol>? method = valid.OfType<IMethodSymbol>();
valid.Do(s => Console.WriteLine(s.Name));
```

### Type Hierarchy (INamedTypeSymbol)

```csharp
valid.BaseType                          // OptionalSymbol<INamedTypeSymbol>
valid.GetBaseTypes()                    // IEnumerable<ValidSymbol<INamedTypeSymbol>>
valid.GetInterfaces()                   // IEnumerable<ValidSymbol<INamedTypeSymbol>>
valid.GetAllInterfaces()                // IEnumerable<ValidSymbol<INamedTypeSymbol>>
valid.ImplementsInterface("IDisposable") // bool
valid.InheritsFrom("BaseClass")         // bool
```

---

## OptionalAttribute

Wraps an `AttributeData` that may or may not be present.

### Creating

```csharp
OptionalAttribute.WithValue(attributeData)
OptionalAttribute.Empty()
OptionalAttribute.FromNullable(maybeNull)
```

### Getting Arguments

```csharp
// Constructor arguments by index
OptionalArgument<string> name = attr.ConstructorArg<string>(0);
OptionalArgument<int> count = attr.ConstructorArg<int>(1);

// Named arguments
OptionalArgument<int> retries = attr.NamedArg<int>("MaxRetries");
OptionalArgument<string> message = attr.NamedArg<string>("Message");
```

### Generic Attribute Type Arguments

For generic attributes like `[MyAttribute<TRuntime, TEvent>]`:

```csharp
attr.GetTypeArguments()         // ImmutableArray<OptionalSymbol<INamedTypeSymbol>>
attr.GetTypeArgument(0)         // OptionalArgument<INamedTypeSymbol>
attr.GetTypeArgumentSymbol(0)   // OptionalSymbol<ITypeSymbol>
attr.AttributeClass             // OptionalSymbol<INamedTypeSymbol>
```

### Transforming

```csharp
// Map to a result type
OptionalArgument<MyConfig> config = attr.Map(a => new MyConfig(a));

// Extract multiple arguments at once
OptionalArgument<MyConfig> config = attr.WithArgs(a => new MyConfig
{
    Name = a.ConstructorArg<string>(0).OrDefault("Default"),
    Retries = a.NamedArg<int>("MaxRetries").OrDefault(3)
});
```

### Validation

```csharp
if (attr.IsValid(out var valid)) { /* use valid */ }
if (attr.IsNotValid(out var valid)) return;

attr.Validate();           // OptionalAttribute → ValidAttribute?
attr.ValidateOrThrow();    // throws if empty
attr.TryValidate(out var valid);
```

### Other Methods

```csharp
attr.Do(a => Console.WriteLine(a.AttributeClass?.Name));
attr.OrElse(() => fallbackAttribute);
attr.OrNull();
attr.OrThrow("Attribute required");
attr.OrDefault(fallbackValue);
attr.Match(whenPresent: ..., whenEmpty: ...);
attr.PropertyName           // string? — suggested property name
attr.ParameterName          // string? — suggested parameter name
```

---

## ValidAttribute

A validated attribute with guaranteed non-null `AttributeData`.

```csharp
// Same argument extraction methods as OptionalAttribute
OptionalArgument<string> arg = validAttr.ConstructorArg<string>(0);
OptionalArgument<int> retries = validAttr.NamedArg<int>("MaxRetries");
validAttr.GetNamedArgument<bool>("Enabled");  // alternate syntax

// Direct access
validAttr.Value             // AttributeData
validAttr.AttributeClass    // INamedTypeSymbol

// Generic attribute type arguments return ValidSymbol
ImmutableArray<ValidSymbol<INamedTypeSymbol>> typeArgs = validAttr.GetTypeArguments();
```

---

## OptionalArgument<T>

Wraps an attribute argument value that may or may not be present.

### Extracting Values

```csharp
string value = arg.OrDefault("fallback");
int count = arg.OrDefault(() => ComputeDefault());
string value = arg.OrThrow("Argument required");
string value = arg.OrThrow(() => new CustomException());
string? maybeNull = arg.OrNull();
```

### Transforming

```csharp
OptionalArgument<int> length = arg.Map(s => s.Length);
OptionalArgument<int> length = arg.Select(s => s.Length);  // alias
OptionalArgument<MyEnum> enumValue = arg.ToEnum<MyEnum>(); // Roslyn stores enums as ints
```

### Pattern Matching

```csharp
string result = arg.Match(
    whenPresent: v => $"Value: {v}",
    whenEmpty: () => "No value");

if (arg.TryGetValue(out var value)) { /* use value */ }
if (arg.IsMissing(out var value)) return;  // early exit pattern
```

### State & Actions

```csharp
arg.HasValue    // bool
arg.IsEmpty     // bool
arg.Value       // T (throws if empty)
arg.Do(v => Console.WriteLine(v));
arg.OrElse(() => fallback);
```

---

## OptionalValue<T>

Generic optional wrapper for any value (not specific to Roslyn symbols). Same API as `OptionalArgument`:

```csharp
OptionalValue<string>.WithValue("hello")
OptionalValue<string>.Empty()

value.Map(s => s.Length)
value.OrDefault("fallback")
value.OrThrow()
value.Match(whenPresent: ..., whenEmpty: ...)
```

---

## OptionalSyntax<T> / ValidSyntax<T>

Wrappers for Roslyn syntax nodes.

### OptionalSyntax<T>

```csharp
OptionalSyntax<ClassDeclarationSyntax>.WithValue(classDecl)
OptionalSyntax<ClassDeclarationSyntax>.Empty()
OptionalSyntax<ClassDeclarationSyntax>.FromNullable(maybeNull)

optional.Node               // TSyntax?
optional.Location           // Location
optional.Span               // TextSpan
optional.FullSpan           // TextSpan

optional.Map(n => n.Identifier.Text)
optional.Where(n => n.Modifiers.Any(SyntaxKind.PublicKeyword))
optional.OfType<RecordDeclarationSyntax>()

optional.Parent             // OptionalSyntax<SyntaxNode>
optional.Ancestor<T>()      // OptionalSyntax<T>
optional.Ancestors<T>()     // IEnumerable<T>

if (optional.IsValid(out var valid)) { /* use valid */ }
```

### ValidSyntax<T>

```csharp
ValidSyntax<ClassDeclarationSyntax>.From(classDecl)
ValidSyntax<ClassDeclarationSyntax>.TryFrom(maybeNull)

valid.Node                  // TSyntax (guaranteed non-null)
valid.Location              // Location
valid.SyntaxTree            // SyntaxTree
valid.Text                  // string — the node's text
valid.FullText              // string — text with trivia

valid.Parent                // ValidSyntax<SyntaxNode>?
valid.Ancestor<T>()         // ValidSyntax<T>?
valid.Ancestors<T>()        // IEnumerable<T>
valid.Descendant<T>()       // ValidSyntax<T>?
valid.Descendants<T>()      // IEnumerable<T>

valid.LeadingTrivia         // SyntaxTriviaList
valid.TrailingTrivia        // SyntaxTriviaList

// Implicit conversion to the underlying node
ClassDeclarationSyntax node = valid;
```

---

## ValidTypeSyntax<T>

Specialized wrapper for type declaration syntax (`ClassDeclarationSyntax`, `RecordDeclarationSyntax`, etc.) with rich helpers.

```csharp
ValidTypeSyntax<ClassDeclarationSyntax>.From(classDecl)

syntax.Name                 // string
syntax.Identifier           // SyntaxToken
syntax.Keyword              // SyntaxToken (e.g., "class", "record")
syntax.Location             // Location
syntax.IdentifierLocation   // Location

// Modifiers
syntax.Modifiers            // SyntaxTokenList
syntax.HasModifier(SyntaxKind.PublicKeyword)
syntax.IsPartial            // bool
syntax.IsStatic             // bool
syntax.IsAbstract           // bool
syntax.IsSealed             // bool
syntax.IsPublic             // bool
syntax.IsInternal           // bool
syntax.IsPrivate            // bool
syntax.IsProtected          // bool
syntax.IsReadOnly           // bool
syntax.IsFile               // bool

// Modifier manipulation (returns new syntax)
syntax.AddModifier(SyntaxKind.PartialKeyword)
syntax.RemoveModifier(SyntaxKind.SealedKeyword)
syntax.WithModifiers(newModifiers)

// Structure
syntax.BaseList             // BaseListSyntax?
syntax.TypeParameterList    // TypeParameterListSyntax?
syntax.ConstraintClauses    // SyntaxList<TypeParameterConstraintClauseSyntax>
syntax.AttributeLists       // SyntaxList<AttributeListSyntax>
syntax.Members              // SyntaxList<MemberDeclarationSyntax>

syntax.HasBaseList          // bool
syntax.IsGeneric            // bool
syntax.Arity                // int

// Navigation
syntax.ContainingType       // ValidTypeSyntax<TypeDeclarationSyntax>?
syntax.ContainingNamespace  // ValidSyntax<BaseNamespaceDeclarationSyntax>?
syntax.NestedTypes          // IEnumerable<ValidTypeSyntax<TypeDeclarationSyntax>>

// Member access
syntax.Methods              // IEnumerable<MethodDeclarationSyntax>
syntax.Properties           // IEnumerable<PropertyDeclarationSyntax>
syntax.Fields               // IEnumerable<FieldDeclarationSyntax>
syntax.Constructors         // IEnumerable<ConstructorDeclarationSyntax>

// Conversions
TypeDeclarationSyntax node = syntax;
ValidSyntax<ClassDeclarationSyntax> validSyntax = syntax;
```

---

## XmlDocumentation

Parsed XML documentation from a symbol.

```csharp
var doc = symbol.XmlDocumentation;      // from OptionalSymbol/ValidSymbol
var doc = XmlDocumentation.FromSymbol(symbol);

doc.HasValue                // bool
doc.IsEmpty                 // bool

// Content
doc.Summary                 // string?
doc.Remarks                 // string?
doc.Returns                 // string?
doc.Value                   // string?
doc.Example                 // string?
doc.RawXml                  // string?

// Parameters
doc.Params                  // ImmutableDictionary<string, string>
doc.GetParam("name")        // string?

// Type parameters
doc.TypeParams              // ImmutableDictionary<string, string>
doc.GetTypeParam("T")       // string?

// Exceptions and references
doc.Exceptions              // ImmutableArray<(string Type, string Description)>
doc.SeeAlso                 // ImmutableArray<string>
```

---

## Real-World Examples

### Extract Attribute Configuration

```csharp
var config = symbol
    .GetAttribute("RetryAttribute")
    .WithArgs(a => new RetryConfig
    {
        MaxRetries = a.NamedArg<int>("MaxRetries").OrDefault(3),
        DelayMs = a.NamedArg<int>("DelayMs").OrDefault(1000),
        ExponentialBackoff = a.NamedArg<bool>("Exponential").OrDefault(false)
    })
    .OrDefault(RetryConfig.Default);
```

### Early Exit Pattern in Analyzers

```csharp
protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type)
{
    var target = GetFirstInvalidTarget(type);
    return target.HasValue;
}

private static OptionalSymbol<INamedTypeSymbol> GetFirstInvalidTarget(ValidSymbol<INamedTypeSymbol> type)
{
    return OptionalSymbol<INamedTypeSymbol>.FromNullable(
        type.GetAttributes("EffectsModule")
            .FirstOrDefault(t => !t.TargetType.IsInterface)
            ?.TargetType.Value
    );
}
```

### Safe Type Navigation

```csharp
var elementType = typeSymbol
    .AsOptional()
    .Where(t => t.IsGenericType && t.Name == "List")
    .Map(t => t.SingleTypeArgument)
    .OrDefault(OptionalSymbol<ITypeSymbol>.Empty());
```

### Validate Before Processing

```csharp
public void Process(OptionalSymbol<IMethodSymbol> method)
{
    if (method.IsNotValid(out var valid))
    {
        ReportError("Method symbol required");
        return;
    }
    
    // valid is ValidSymbol<IMethodSymbol> — no null checks needed
    var name = valid.Name;
    var isAsync = valid.IsAsync;
    var parameters = valid.Value.Parameters;
}
```

### Chain Optional Operations

```csharp
var serviceName = symbol
    .GetAttribute("ServiceAttribute")
    .ConstructorArg<INamedTypeSymbol>(0)
    .Map(t => t.Name)
    .OrDefault(() => symbol.Name + "Service");
```

### Work with Generic Attributes

```csharp
// For [Handler<TRequest, TResponse>]
var attr = symbol.GetAttribute("Handler");
var requestType = attr.GetTypeArgument(0).OrThrow("Request type required");
var responseType = attr.GetTypeArgument(1).OrThrow("Response type required");
```

### Check Type Hierarchy

```csharp
// Check if a type implements IDisposable
if (typeSymbol.ImplementsInterface("IDisposable"))
{
    // Generate dispose pattern
}

// Check if inherits from a specific base class
if (typeSymbol.InheritsFrom("ControllerBase"))
{
    // Handle controller-specific generation
}

// Iterate all base types
foreach (var baseType in typeSymbol.GetBaseTypes())
{
    Console.WriteLine($"Inherits from: {baseType.Name}");
}

// Get all interfaces including inherited ones
var allInterfaces = typeSymbol.GetAllInterfaces()
    .Select(i => i.FullyQualifiedName)
    .ToList();
```

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](../../../LICENSE) for the full legal text.

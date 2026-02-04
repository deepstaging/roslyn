# Extensions

Convenience methods for common Roslyn operations.

> **See also:** [Queries](Queries.md) | [Projections](Projections.md) | [Emit](Emit.md) | [Roslyn Toolkit README](../README.md)

## Overview

Extension methods organized by the type they extend. These wrap common Roslyn operations in fluent, null-safe APIs.

---

## ISymbol Extensions

Available on all Roslyn symbols.

### Symbol Equality

```csharp
bool same = symbol.IsSymbol(otherSymbol);      // semantic equality
bool different = symbol.DoesNotEqual(otherSymbol);
```

### Attributes

```csharp
// Get all attributes as ValidAttribute projections
IEnumerable<ValidAttribute> attrs = symbol.GetAttributes();

// Get by name (handles both "Name" and "NameAttribute" forms)
IEnumerable<ValidAttribute> attrs = symbol.GetAttributesByName("Obsolete");

// Get by type
IEnumerable<ValidAttribute> attrs = symbol.GetAttributesByType<ObsoleteAttribute>();
```

### Accessibility Checks

```csharp
symbol.IsPublic()
symbol.IsPrivate()
symbol.IsProtected()
symbol.IsInternal()
symbol.IsProtectedInternal()
symbol.IsPrivateProtected()
```

### Modifier Checks

```csharp
symbol.IsVirtual()
symbol.IsOverride()
symbol.IsSealed()
symbol.IsAbstract()
symbol.IsStatic()
symbol.IsExtern()
symbol.IsObsolete()          // has [Obsolete] attribute
symbol.IsImplicitlyDeclared()
```

### Source Location

```csharp
symbol.IsFromSource()        // defined in source code
symbol.IsFromMetadata()      // from referenced assembly
```

---

## ITypeSymbol Extensions

Start query builders and check type characteristics.

### Query Builders

```csharp
MethodQuery methods = typeSymbol.QueryMethods();
PropertyQuery properties = typeSymbol.QueryProperties();
FieldQuery fields = typeSymbol.QueryFields();
ConstructorQuery constructors = typeSymbol.QueryConstructors();
EventQuery events = typeSymbol.QueryEvents();
ImmutableArray<AttributeData> attrs = typeSymbol.QueryAttributes();
```

### Task Type Checks

```csharp
typeSymbol.IsTaskType()              // Task or ValueTask (with or without T)
typeSymbol.IsValueTaskType()         // ValueTask or ValueTask<T>
typeSymbol.IsGenericTaskType()       // Task<T>
typeSymbol.IsGenericValueTaskType()  // ValueTask<T>
typeSymbol.IsNonGenericTaskType()    // Task (no type arg)
typeSymbol.IsNonGenericValueTaskType() // ValueTask (no type arg)
```

### Collection Type Checks

```csharp
typeSymbol.IsEnumerableType()        // IEnumerable<T>
typeSymbol.IsCollectionType()        // ICollection<T>
typeSymbol.IsListType()              // IList<T>
typeSymbol.IsDictionaryType()        // IDictionary<TKey, TValue>
typeSymbol.IsQueryableType()         // IQueryable<T>
typeSymbol.IsObservableType()        // IObservable<T>
```

### Special Type Checks

```csharp
typeSymbol.IsNullableValueType()     // Nullable<T>
typeSymbol.IsFuncType()              // Func<...>
typeSymbol.IsActionType()            // Action or Action<...>
typeSymbol.IsLazyType()              // Lazy<T>
typeSymbol.IsTupleType()             // ValueTuple
typeSymbol.IsArrayType()             // T[]
typeSymbol.IsPointerType()           // T*
```

### Type Kind Checks

```csharp
typeSymbol.IsDelegateType()
typeSymbol.IsEnumType()
typeSymbol.IsInterfaceType()
typeSymbol.IsRecordType()            // record class or record struct
typeSymbol.IsStructType()            // struct (excluding enums)
typeSymbol.IsClassType()
typeSymbol.IsAbstractType()
typeSymbol.IsSealedType()
typeSymbol.IsStaticType()
```

### Inheritance

```csharp
bool result = typeSymbol.ImplementsOrInheritsFrom(baseType);
bool isException = typeSymbol.IsOrInheritsFrom("Exception", "System");
bool inherits = typeSymbol.InheritsFrom("Controller", "Microsoft.AspNetCore.Mvc");
ITypeSymbol? baseType = typeSymbol.GetBaseTypeByName("DbContext");
```

### Type Arguments

```csharp
ITypeSymbol? elementType = typeSymbol.GetSingleTypeArgument();
```

---

## INamedTypeSymbol Extensions

Additional extensions for named types.

### Modifier Checks

```csharp
namedType.IsPartial()
namedType.IsGeneric()
namedType.IsOpenGeneric()
namedType.IsNestedType()
namedType.IsGeneratedCode()
namedType.IsNotGeneratedCode()
```

### Constructors

```csharp
namedType.HasParameterlessConstructor()
```

### Interface Checks

```csharp
namedType.ImplementsInterface("IDisposable")
```

### Attribute Checks

```csharp
namedType.HasAttributes()
namedType.HasAttribute("Serializable")
```

### Arity

```csharp
int arity = namedType.GetArity();
```

---

## IMethodSymbol Extensions

Extensions for method symbols.

### Query Parameters

```csharp
ParameterQuery params = methodSymbol.QueryParameters();
```

### Async Analysis

```csharp
methodSymbol.IsAsync()
methodSymbol.IsAsyncVoid()           // async void
methodSymbol.IsAsyncValue()          // async with return value
methodSymbol.GetAsyncKind()          // enum: None, Void, Task, ValueTask, GenericTask, etc.
ITypeSymbol? returnType = methodSymbol.GetAsyncReturnType(); // T from Task<T>
```

### Method Classification

```csharp
methodSymbol.IsExtensionMethod()
methodSymbol.IsGenericMethod()
methodSymbol.IsOperator()
methodSymbol.IsConstructor()
methodSymbol.IsDestructor()
methodSymbol.IsPropertyAccessor()
methodSymbol.IsEventAccessor()
methodSymbol.IsPartialMethod()
```

### Return Type

```csharp
methodSymbol.ReturnsVoid()
```

### Parameters

```csharp
methodSymbol.HasParameters()
methodSymbol.HasRefOrOutParameters()
```

---

## INamespaceSymbol Extensions

Navigate namespace hierarchies.

```csharp
TypeQuery types = namespaceSymbol.QueryTypes();
```

---

## Compilation Extensions

Query types from a compilation.

```csharp
TypeQuery types = compilation.QueryTypes();
```

---

## AttributeData Extensions

Extract attribute arguments fluently.

### Wrap in Projection

```csharp
OptionalAttribute optional = attributeData.Query();
```

### Type Checking

```csharp
bool isObsolete = attributeData.Is<ObsoleteAttribute>();
T? typed = attributeData.As<ObsoleteAttribute>();
```

### Get Arguments

```csharp
// Named argument
OptionalArgument<int> retries = attributeData.GetNamedArgument<int>("MaxRetries");
int value = retries.OrDefault(3);

// Constructor argument by index
OptionalArgument<string> name = attributeData.GetConstructorArgument<string>(0);
```

---

## String Extensions

Case conversion utilities.

```csharp
"myPropertyName".ToSnakeCase()    // "my_property_name"
"my_property_name".ToCamelCase()  // "myPropertyName"
"my_property_name".ToPascalCase() // "MyPropertyName"
"myPropertyName".ToKebabCase()    // "my-property-name"
"myPropertyName".ToTrainCase()    // "My-Property-Name"
"my_property_name".ToTitleCase()  // "My Property Name"
```

---

## Domain-Specific Extensions

### Entity Framework Core

```csharp
// Check if property is a DbSet<T>
bool isDbSet = propertySymbol.IsEfDbSet();

// Check if type is a DbContext
bool isContext = validSymbol.IsEfDbContext();
bool isNotContext = validSymbol.IsNotEfDbContext();
```

### LanguageExt

```csharp
// Filter methods returning LanguageExt Eff
var effMethods = typeSymbol.QueryMethods()
    .ReturningLanguageExtEff()
    .GetAll();

// Check if symbol is LanguageExt Eff type
bool isEff = symbol.IsLanguageExtEff();
```

---

## Usage Examples

### Find Async Methods Returning Entity Types

```csharp
var asyncMethods = typeSymbol.QueryMethods()
    .ThatArePublic()
    .ThatAreAsync()
    .Where(m => m.ReturnType.IsGenericTaskType())
    .GetAll();
```

### Check If Type Is Repository

```csharp
bool isRepo = typeSymbol.IsInterfaceType() &&
              typeSymbol.Name.EndsWith("Repository") &&
              ((INamedTypeSymbol)typeSymbol).ImplementsInterface("IRepository");
```

### Extract Attribute Configuration

```csharp
var config = symbol.GetAttributesByName("Configure")
    .FirstOrDefault()
    ?.Query()
    .WithArgs(attr => new Config
    {
        Name = attr.ConstructorArg<string>(0).OrDefault("Default"),
        Enabled = attr.NamedArg<bool>("Enabled").OrDefault(true),
        Priority = attr.NamedArg<int>("Priority").OrDefault(0)
    })
    .OrDefault(Config.Default);
```

### Check Type Hierarchy

```csharp
if (typeSymbol.InheritsFrom("ControllerBase", "Microsoft.AspNetCore.Mvc"))
{
    var actions = typeSymbol.QueryMethods()
        .ThatArePublic()
        .ThatAreInstance()
        .WithoutAttribute<NonActionAttribute>()
        .GetAll();
}
```

### Analyze Async Method Signatures

```csharp
var method = typeSymbol.QueryMethods()
    .WithName("ProcessAsync")
    .FirstOrDefault();

if (method.IsValid(out var valid))
{
    var asyncKind = valid.Value.GetAsyncKind();
    var innerType = valid.Value.GetAsyncReturnType();
    
    if (valid.Value.IsAsyncVoid())
    {
        // Report diagnostic: async void is problematic
    }
}
```

### Filter by Generated Code

```csharp
var userDefinedTypes = TypeQuery.From(compilation)
    .ThatAreClasses()
    .Where(t => t.IsNotGeneratedCode())
    .GetAll();
```

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](../../../LICENSE) for the full legal text.

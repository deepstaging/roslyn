# Symbol Extensions

Extensions for Roslyn symbol types.

> **See also:** [Extensions Overview](index.md)

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

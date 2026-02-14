# Snapshots

Pipeline-safe materializations of Roslyn symbols. Snapshots capture all needed symbol data as primitives, strings, and records — safe for use in incremental generator pipeline models.

> **See also:** [ValidSymbol](valid-symbol.md) | [EquatableArray](equatable-array.md) | [PipelineModel](pipeline-model.md)

## The Problem

`ValidSymbol<T>` wraps a live `ISymbol` reference. Storing it in a pipeline model retains the **entire Compilation** in memory — every syntax tree, every symbol table — preventing garbage collection across edits. It also has broken equality semantics.

## The Solution

Snapshot types extract all relevant data from symbols into plain `record` types with correct equality. Use them in pipeline models instead of `ValidSymbol<T>` or `ISymbol`.

```csharp
// ❌ Retains entire Compilation
public required ValidSymbol<INamedTypeSymbol> TargetType { get; init; }

// ✅ Pipeline-safe, correct equality
public required TypeSnapshot TargetType { get; init; }
```

---

## Snapshot Hierarchy

All snapshots inherit from `SymbolSnapshot`:

| Snapshot | Mirrors | Key Properties |
|----------|---------|----------------|
| `SymbolSnapshot` | `ValidSymbol<ISymbol>` (base) | Name, Namespace, FQN, accessibility, modifiers, documentation |
| `TypeSnapshot` | `ValidSymbol<INamedTypeSymbol>` | Type identity, generics, base type, interfaces |
| `MethodSnapshot` | `ValidSymbol<IMethodSymbol>` | Return type, async kind, parameters |
| `PropertySnapshot` | `ValidSymbol<IPropertySymbol>` | Type, getter/setter, init-only, required |
| `FieldSnapshot` | `ValidSymbol<IFieldSymbol>` | Type, const, volatile, constant value |
| `ParameterSnapshot` | `ValidSymbol<IParameterSymbol>` | Type, default value, ref kind, params |
| `EventSnapshot` | `ValidSymbol<IEventSymbol>` | Type |

---

## Creating Snapshots

### From ValidSymbol (`.ToSnapshot()`)

Every `ValidSymbol<T>` specialization has a `.ToSnapshot()` extension:

```csharp
ValidSymbol<INamedTypeSymbol> symbol = ...;
TypeSnapshot snapshot = symbol.ToSnapshot();

ValidSymbol<IMethodSymbol> method = ...;
MethodSnapshot snapshot = method.ToSnapshot();
```

### From Query Builders (`.Snapshots()` / `.Snapshot()`)

All query builders have snapshot terminals that materialize results directly:

```csharp
// Multiple results
EquatableArray<MethodSnapshot> methods = type.QueryMethods()
    .ThatArePublic()
    .ThatAreInstance()
    .Snapshots();

// Single result (first or null)
TypeSnapshot? baseType = compilation.QueryTypes()
    .WithName("MyBase")
    .Snapshot();
```

Available on all query builders:

| Query | `.Snapshots()` returns | `.Snapshot()` returns |
|-------|----------------------|---------------------|
| `TypeQuery` | `EquatableArray<TypeSnapshot>` | `TypeSnapshot?` |
| `MethodQuery` | `EquatableArray<MethodSnapshot>` | `MethodSnapshot?` |
| `PropertyQuery` | `EquatableArray<PropertySnapshot>` | `PropertySnapshot?` |
| `FieldQuery` | `EquatableArray<FieldSnapshot>` | `FieldSnapshot?` |
| `ParameterQuery` | `EquatableArray<ParameterSnapshot>` | `ParameterSnapshot?` |
| `EventQuery` | `EquatableArray<EventSnapshot>` | `EventSnapshot?` |
| `ConstructorQuery` | `EquatableArray<MethodSnapshot>` | `MethodSnapshot?` |

---

## SymbolSnapshot (Base)

Properties shared by all snapshots:

### Name & Display

```csharp
snapshot.Name                    // "MyClass"
snapshot.Namespace               // "MyApp.Models" (null for global)
snapshot.FullyQualifiedName      // "MyApp.Models.MyClass"
snapshot.GloballyQualifiedName   // "global::MyApp.Models.MyClass"
snapshot.DisplayName             // "MyApp.Models.MyClass" (computed)
snapshot.PropertyName            // "MyClass"
snapshot.ParameterName           // "myClass"
```

### Accessibility

```csharp
snapshot.AccessibilityString     // "public", "internal", etc.
snapshot.IsPublic                // bool
snapshot.IsInternal              // bool
```

### Modifiers

```csharp
snapshot.IsStatic
snapshot.IsAbstract
snapshot.IsSealed
snapshot.IsVirtual
snapshot.IsOverride
snapshot.IsReadOnly
snapshot.IsPartial
```

### Type Classification

```csharp
snapshot.Kind                    // "class", "struct", "interface", etc.
snapshot.IsValueType
snapshot.IsReferenceType
snapshot.IsNullable
```

### Documentation

```csharp
snapshot.Documentation           // DocumentationSnapshot
snapshot.Documentation.Summary   // "Description of the symbol"
snapshot.Documentation.HasValue  // true if any documentation exists
```

---

## TypeSnapshot

Extends `SymbolSnapshot` with type-specific data:

```csharp
TypeSnapshot type = symbol.ToSnapshot();

// Type identity
type.IsInterface      // bool
type.IsClass          // bool
type.IsStruct         // bool
type.IsRecord         // bool
type.IsEnum           // bool
type.IsDelegate       // bool

// Generics
type.IsGenericType    // bool
type.Arity            // int (number of type parameters)
type.TypeArgumentNames // EquatableArray<string> — FQN of type args

// Hierarchy
type.BaseTypeName     // "global::System.Object" or null
type.InterfaceNames   // EquatableArray<string> — directly implemented
```

---

## MethodSnapshot

```csharp
MethodSnapshot method = symbol.ToSnapshot();

method.ReturnType          // "global::System.Threading.Tasks.Task<int>"
method.ReturnsVoid         // bool
method.AsyncKind           // AsyncMethodKind enum
method.AsyncReturnType     // "global::System.Int32" (T from Task<T>)
method.MethodKind          // MethodKind enum
method.IsExtensionMethod   // bool
method.IsGenericMethod     // bool
method.IsPartialMethod     // bool
method.IsAsync             // bool
method.Parameters          // EquatableArray<ParameterSnapshot>
```

---

## PropertySnapshot

```csharp
PropertySnapshot prop = symbol.ToSnapshot();

prop.Type          // "global::System.String"
prop.HasGetter     // bool
prop.HasSetter     // bool
prop.IsInitOnly    // bool (init-only setter)
prop.IsRequired    // bool
prop.IsIndexer     // bool
prop.Parameters    // EquatableArray<ParameterSnapshot> (indexer params)
```

---

## FieldSnapshot

```csharp
FieldSnapshot field = symbol.ToSnapshot();

field.Type                     // "global::System.Int32"
field.IsConst                  // bool
field.IsVolatile               // bool
field.HasConstantValue         // bool
field.ConstantValueExpression  // "42" or "\"hello\"" (C# expression string)
```

---

## ParameterSnapshot

```csharp
ParameterSnapshot param = symbol.ToSnapshot();

param.Type                     // "global::System.String"
param.HasExplicitDefaultValue  // bool
param.DefaultValueExpression   // "\"default\"" (C# expression string)
param.RefKind                  // RefKind enum (None, Ref, Out, In)
param.IsParams                 // bool
param.IsOptional               // bool
```

---

## EventSnapshot

```csharp
EventSnapshot evt = symbol.ToSnapshot();

evt.Type    // "global::System.EventHandler"
```

---

## DocumentationSnapshot

Pipeline-safe replacement for `XmlDocumentation`:

```csharp
DocumentationSnapshot docs = symbol.XmlDocumentation.ToSnapshot();

docs.Summary        // "Summary text"
docs.Remarks        // "Additional remarks"
docs.Returns        // "Return value description"
docs.Value          // "Property value description"
docs.Example        // "Example code"
docs.HasValue       // true if any documentation exists
docs.Params         // EquatableArray<ParamDocumentation>
docs.TypeParams     // EquatableArray<ParamDocumentation>
docs.Exceptions     // EquatableArray<ExceptionDocumentation>
docs.SeeAlso        // EquatableArray<string>
```

Supporting types:

```csharp
// Parameter documentation
record ParamDocumentation(string Name, string Description);

// Exception documentation
record ExceptionDocumentation(string Type, string Description);
```

---

## TypeRef Integration

`TypeRef.From()` accepts snapshots so Emit writers work unchanged:

```csharp
TypeSnapshot type = ...;
TypeRef typeRef = TypeRef.From(type);  // uses GloballyQualifiedName
```

---

## Real-World Example

### Before (broken caching)

```csharp
public sealed record CapabilityModel
{
    public required ValidSymbol<INamedTypeSymbol> DependencyType { get; init; }
    public required ImmutableArray<ValidSymbol<IMethodSymbol>> Methods { get; init; }
}
```

### After (correct caching)

```csharp
[PipelineModel]
public sealed record CapabilityModel
{
    public required TypeSnapshot DependencyType { get; init; }
    public required EquatableArray<MethodSnapshot> Methods { get; init; }
}
```

### Projection code

```csharp
// Before
DependencyType = attribute.TargetType,
Methods = attribute.TargetType.QueryMethods().GetAll(),

// After
DependencyType = attribute.TargetType.ToSnapshot(),
Methods = attribute.TargetType.QueryMethods().Snapshots(),
```

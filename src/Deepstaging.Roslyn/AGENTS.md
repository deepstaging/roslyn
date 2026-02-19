# Deepstaging.Roslyn — Agent Guide

Fluent toolkit wrapping Roslyn APIs for source generators, analyzers, and code fixes.

Docs: https://deepstaging.github.io/roslyn

## Architecture

Four layers — reading and writing are symmetric:

| Layer           | Purpose                                              | Key Types                                                                                                     |
|-----------------|------------------------------------------------------|---------------------------------------------------------------------------------------------------------------|
| **Queries**     | Find symbols with chainable filters                  | `TypeQuery`, `MethodQuery`, `PropertyQuery`, `FieldQuery`, `ConstructorQuery`, `EventQuery`, `ParameterQuery` |
| **Projections** | Safe nullable wrappers over Roslyn symbols           | `OptionalSymbol<T>`, `ValidSymbol<T>`, `OptionalAttribute`, `ValidAttribute`                                  |
| **Emit**        | Fluent builders that produce `CompilationUnitSyntax` | `TypeBuilder`, `MethodBuilder`, `PropertyBuilder`, `FieldBuilder`, `ConstructorBuilder`                       |
| **Scriban**     | Template infrastructure for source generators        | `.scriban-cs` templates                                                                                       |

Symmetry: `TypeQuery` finds types ↔ `TypeBuilder` creates types. Both are fluent and immutable.

### Three-Layer Type System

The core primitives and typed wrappers live outside Emit:

| Namespace                     | Purpose                                                             |
|-------------------------------|---------------------------------------------------------------------|
| `Deepstaging.Roslyn`          | Core primitives: `TypeRef`, `ExpressionRef`, `AttributeRef`, `NamespaceRef` |
| `Deepstaging.Roslyn.Types`    | Typed wrappers: `TaskTypeRef`, `ListTypeRef`, `EqualityComparerTypeRef`, etc. |
| `Deepstaging.Roslyn.Expressions` | Expression factories: `TaskExpression`, `EqualityComparerExpression`, builder extensions |

## Critical Pattern: IsNotValid Early Exit

This is the primary convention. Unwrap optional projections with `IsNotValid` to exit early when data is missing:

```csharp
// ✅ Correct — early-exit with IsNotValid
if (symbol.GetAttribute("MyAttribute").IsNotValid(out var attr))
    return;

// attr is now ValidAttribute — guaranteed non-null
var name = attr.NamedArg<string>("Name").OrDefault("default");
```

**Never** bypass projections by accessing `.Symbol` directly and null-checking manually.

### Full Validation Flow

```
OptionalSymbol<T> / OptionalAttribute
    │
    ├── .IsNotValid(out var valid) → bool     // Early-exit (preferred)
    ├── .TryValidate(out var valid) → bool    // Standard TryParse style
    ├── .Validate() → Valid?                  // Nullable result
    └── .ValidateOrThrow() → Valid            // Throws if invalid
```

## Queries

Readonly structs with immutable filters. Returns actual `ISymbol` instances, not wrappers.

```csharp
// Find symbols
TypeQuery.From(compilation)
    .ThatArePublic()
    .ThatAreClasses()
    .WithAttribute("MyAttribute")
    .GetAll();                          // ImmutableArray<INamedTypeSymbol>

MethodQuery.From(typeSymbol)
    .ThatArePublic()
    .ThatAreAsync()
    .ThatAreNotStatic()
    .GetAll();                          // ImmutableArray<IMethodSymbol>

// Available queries: TypeQuery, MethodQuery, PropertyQuery, FieldQuery,
//                    ConstructorQuery, EventQuery, ParameterQuery
// All follow the same pattern: .From(source).ThatAre*().GetAll()
```

## Projections

Wrap nullable Roslyn data in `Optional` → validate to `Valid` → use safely.

```csharp
// Wrap a nullable symbol
OptionalSymbol<INamedTypeSymbol> type = OptionalSymbol.FromNullable(symbolOrNull);

// Map, filter, extract
var name = type
    .Where(t => t.IsPublic())
    .Map(t => t.Name)
    .OrDefault("Unknown");

// Navigate symbol tree
var methods = type.Methods().ThatArePublic().GetAll();
var baseType = type.BaseType;
var interfaces = type.Interfaces;

// Attribute extraction
var maxRetries = symbol
    .GetAttribute("RetryAttribute")
    .NamedArg<int>("MaxRetries")
    .OrDefault(3);

var position = symbol
    .GetAttribute("PositionAttribute")
    .ConstructorArg<int>(0)
    .OrDefault(0);
```

## Emit

Record struct builders. Call `.Emit()` to get valid Roslyn syntax trees.

```csharp
var result = TypeBuilder
    .Class("Customer")
    .InNamespace("MyApp.Domain")
    .AsPartial()
    .AddUsing("System")
    .Implements("IEquatable<Customer>")
    .AddProperty("Id", "Guid", p => p
        .WithAccessibility(Accessibility.Public)
        .WithAutoPropertyAccessors())
    .AddMethod("ToString", m => m
        .AsOverride()
        .WithReturnType("string")
        .WithExpressionBody("$\"Customer({Id})\""))
    .Emit();

if (result.IsNotValid(out var validEmit))
    return;

string code = validEmit.Code;  // Valid, compilable C#
```

### Builder Factories

```csharp
// Create from scratch
TypeBuilder.Class("Name")
TypeBuilder.Interface("IName")
TypeBuilder.Struct("Name")
TypeBuilder.Record("Name")

// Parse from signature string
TypeBuilder.Parse("public abstract class BaseService")
MethodBuilder.Parse("public async Task<string> GetAsync(int id)")
```

### Design Pattern Extensions

```csharp
// Built-in pattern helpers on TypeBuilder
.AddBuilderPattern()
.AddSingletonPattern()
.AddFactoryPattern()
.AddToStringOverride()

// Interface implementations
.ImplementIEquatable()
.ImplementIComparable()
.ImplementIDisposable()
.ImplementINotifyPropertyChanged()
.ImplementIEnumerable()
```

## Extension Methods

Extensions are organized by what they extend. Key namespaces:

| Namespace                 | Extensions On                         | Purpose                                                                                    |
|---------------------------|---------------------------------------|--------------------------------------------------------------------------------------------|
| `Deepstaging.Roslyn`      | `OptionalSymbol<T>`, `ValidSymbol<T>` | Symbol navigation (`.Methods()`, `.Properties()`, `.Fields()`, `.BaseType`, `.Interfaces`) |
| `Deepstaging.Roslyn`      | `OptionalAttribute`, `ValidAttribute` | Attribute argument extraction                                                              |
| `Deepstaging.Roslyn.Emit` | `TypeBuilder`                         | Design patterns, interface implementations, operators, converters                          |
| `Deepstaging.Roslyn.Emit` | `MethodBuilder`, `PropertyBuilder`    | Modifier helpers                                                                           |

## Key Design Rules

- All query types are `readonly struct` with `ImmutableArray<Func<...>>` filters
- All builders are record structs — immutable, use `with` for modifications
- `.GetAll()` returns `ImmutableArray<TSymbol>` — real Roslyn symbols, not wrappers
- `.Emit()` returns `OptionalEmit` — validate before using the generated code
- Targets `netstandard2.0` for analyzer/generator compatibility

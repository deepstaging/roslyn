# Test Assertions

TUnit assertion extensions for Roslyn symbols, emit results, and compilations.

These extensions integrate with TUnit's `Assert.That(...).` fluent API, providing domain-specific assertions for Deepstaging.Roslyn types.

## ValidSymbol Assertions

Assertions for `ValidSymbol<T>` — a wrapper guaranteeing a non-null Roslyn symbol.

### Name

```csharp
await Assert.That(symbol.IsNamed("Customer")).IsTrue();
await Assert.That(symbol.NameStartsWith("I")).IsTrue();
await Assert.That(symbol.NameEndsWith("Service")).IsTrue();
await Assert.That(symbol.NameContains("Base")).IsTrue();
```

### Accessibility

```csharp
await Assert.That(symbol.IsPublicSymbol()).IsTrue();
await Assert.That(symbol.IsPrivateSymbol()).IsTrue();
await Assert.That(symbol.IsInternalSymbol()).IsTrue();
await Assert.That(symbol.IsProtectedSymbol()).IsTrue();
```

### Modifiers

```csharp
await Assert.That(symbol.IsStaticSymbol()).IsTrue();
await Assert.That(symbol.IsAbstractSymbol()).IsTrue();
await Assert.That(symbol.IsSealedSymbol()).IsTrue();
await Assert.That(symbol.IsVirtualSymbol()).IsTrue();
```

### Attributes

```csharp
await Assert.That(symbol.HasAttribute("ObsoleteAttribute")).IsTrue();
await Assert.That(symbol.DoesNotHaveAttribute("Obsolete")).IsTrue();
```

## Named Type Assertions

For `ValidSymbol<INamedTypeSymbol>` and `OptionalSymbol<INamedTypeSymbol>`:

```csharp
await Assert.That(type.IsClassSymbol()).IsTrue();
await Assert.That(type.IsInterfaceSymbol()).IsTrue();
await Assert.That(type.IsStructSymbol()).IsTrue();
await Assert.That(type.IsRecordSymbol()).IsTrue();
await Assert.That(type.IsEnumSymbol()).IsTrue();
await Assert.That(type.IsDelegateSymbol()).IsTrue();
await Assert.That(type.IsPartialSymbol()).IsTrue();
await Assert.That(type.IsGenericSymbol()).IsTrue();
await Assert.That(type.HasTypeParameterCount(2)).IsTrue();
```

## Method Assertions

For `ValidSymbol<IMethodSymbol>` and `OptionalSymbol<IMethodSymbol>`:

```csharp
await Assert.That(method.IsAsyncSymbol()).IsTrue();
await Assert.That(method.ReturnsVoid()).IsTrue();
await Assert.That(method.HasParameterCount(2)).IsTrue();
await Assert.That(method.IsExtension()).IsTrue();
await Assert.That(method.IsGeneric()).IsTrue();
await Assert.That(method.IsConstructor()).IsTrue();
await Assert.That(method.IsOperator()).IsTrue();
await Assert.That(method.HasReturnType()).IsTrue();
```

## Property Assertions

For `ValidSymbol<IPropertySymbol>` and `OptionalSymbol<IPropertySymbol>`:

```csharp
await Assert.That(prop.IsReadOnlySymbol()).IsTrue();
await Assert.That(prop.IsWriteOnly()).IsTrue();
await Assert.That(prop.HasGetter()).IsTrue();
await Assert.That(prop.HasSetter()).IsTrue();
await Assert.That(prop.IsIndexer()).IsTrue();
await Assert.That(prop.HasInitOnlySetter()).IsTrue();
await Assert.That(prop.IsRequired()).IsTrue();
```

## Field Assertions

For `ValidSymbol<IFieldSymbol>` and `OptionalSymbol<IFieldSymbol>`:

```csharp
await Assert.That(field.IsReadOnlySymbol()).IsTrue();
await Assert.That(field.IsConst()).IsTrue();
await Assert.That(field.IsVolatile()).IsTrue();
await Assert.That(field.HasConstantValue()).IsTrue();
```

## OptionalSymbol Assertions

For `OptionalSymbol<T>` — a nullable wrapper:

```csharp
await Assert.That(symbol.HasValue()).IsTrue();
await Assert.That(symbol.IsEmpty()).IsTrue();
```

## OptionalEmit Assertions

For `OptionalEmit` — the result of calling `.Emit()`:

```csharp
await Assert.That(emit.IsSuccessful()).IsTrue();
await Assert.That(emit.HasFailed()).IsTrue();
await Assert.That(emit.HasValue()).IsTrue();
await Assert.That(emit.IsEmpty()).IsTrue();
await Assert.That(emit.HasDiagnostics()).IsTrue();
await Assert.That(emit.HasNoDiagnostics()).IsTrue();
await Assert.That(emit.HasErrors()).IsTrue();
await Assert.That(emit.HasWarnings()).IsTrue();
await Assert.That(emit.CodeContains("partial class")).IsTrue();
await Assert.That(emit.CodeDoesNotContain("sealed")).IsTrue();
```

## Compilation Assertions

```csharp
await Assert.That(compilation.IsSuccessful()).IsTrue();
```

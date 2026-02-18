# Test Assertions

TUnit source-generated assertion extensions for Roslyn symbols, emit results, and compilations.

These assertions use TUnit's [`[GenerateAssertion]`](https://tunit.dev/docs/assertions/extensibility/source-generator-assertions) attribute to generate fluent `Assert.That(x).IsNamed("Foo")` style assertions automatically.

## ValidSymbol Assertions

Assertions for `ValidSymbol<T>` — a wrapper guaranteeing a non-null Roslyn symbol.

### Name

```csharp
await Assert.That(symbol).IsNamed("Customer");
await Assert.That(symbol).NameStartsWith("I");
await Assert.That(symbol).NameEndsWith("Service");
await Assert.That(symbol).NameContains("Base");
```

### Accessibility

```csharp
await Assert.That(symbol).IsPublicSymbol();
await Assert.That(symbol).IsPrivateSymbol();
await Assert.That(symbol).IsInternalSymbol();
await Assert.That(symbol).IsProtectedSymbol();
```

### Modifiers

```csharp
await Assert.That(symbol).IsStaticSymbol();
await Assert.That(symbol).IsAbstractSymbol();
await Assert.That(symbol).IsSealedSymbol();
await Assert.That(symbol).IsVirtualSymbol();
```

### Attributes

```csharp
await Assert.That(symbol).HasAttribute("ObsoleteAttribute");
await Assert.That(symbol).DoesNotHaveAttribute("Obsolete");
```

## Named Type Assertions

For `ValidSymbol<INamedTypeSymbol>` and `OptionalSymbol<INamedTypeSymbol>`:

```csharp
await Assert.That(type).IsClassSymbol();
await Assert.That(type).IsInterfaceSymbol();
await Assert.That(type).IsStructSymbol();
await Assert.That(type).IsRecordSymbol();
await Assert.That(type).IsEnumSymbol();
await Assert.That(type).IsDelegateSymbol();
await Assert.That(type).IsPartialSymbol();
await Assert.That(type).IsGenericSymbol();
await Assert.That(type).HasTypeParameterCount(2);
```

## Method Assertions

For `ValidSymbol<IMethodSymbol>` and `OptionalSymbol<IMethodSymbol>`:

```csharp
await Assert.That(method).IsAsyncSymbol();
await Assert.That(method).ReturnsVoid();
await Assert.That(method).HasParameterCount(2);
await Assert.That(method).IsExtension();
await Assert.That(method).IsGeneric();
await Assert.That(method).IsConstructor();
await Assert.That(method).IsOperator();
await Assert.That(method).HasReturnType();
```

## Property Assertions

For `ValidSymbol<IPropertySymbol>` and `OptionalSymbol<IPropertySymbol>`:

```csharp
await Assert.That(prop).IsReadOnlySymbol();
await Assert.That(prop).IsWriteOnly();
await Assert.That(prop).HasGetter();
await Assert.That(prop).HasSetter();
await Assert.That(prop).IsIndexer();
await Assert.That(prop).HasInitOnlySetter();
await Assert.That(prop).IsRequired();
```

## Field Assertions

For `ValidSymbol<IFieldSymbol>` and `OptionalSymbol<IFieldSymbol>`:

```csharp
await Assert.That(field).IsReadOnlySymbol();
await Assert.That(field).IsConst();
await Assert.That(field).IsVolatile();
await Assert.That(field).HasConstantValue();
```

## OptionalSymbol Assertions

For `OptionalSymbol<T>` — a nullable wrapper:

```csharp
await Assert.That(symbol).HasValue();
await Assert.That(symbol).IsEmpty();
```

## OptionalEmit Assertions

For `OptionalEmit` — the result of calling `.Emit()`:

```csharp
await Assert.That(emit).IsSuccessful();
await Assert.That(emit).HasFailed();
await Assert.That(emit).HasValue();
await Assert.That(emit).IsEmpty();
await Assert.That(emit).HasDiagnostics();
await Assert.That(emit).HasNoDiagnostics();
await Assert.That(emit).HasErrors();
await Assert.That(emit).HasWarnings();
await Assert.That(emit).CodeContains("partial class");
await Assert.That(emit).CodeDoesNotContain("sealed");
```

## Compilation Assertions

```csharp
await Assert.That(compilation).IsSuccessful();
```

`IsSuccessful` returns an `AssertionResult` with detailed error output including all compilation errors and the source tree on failure.

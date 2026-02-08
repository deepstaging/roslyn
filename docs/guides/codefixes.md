# CodeFix Pattern

Reference the analyzer's DiagnosticId and use provided helpers.

## Basic Structure

```csharp
[CodeFix(AutoNotifyMustBePartialAnalyzer.DiagnosticId)]
public sealed class MakePartialClassCodeFixProvider : ClassCodeFix
{
    protected override CodeAction CreateFix(
        Document document, 
        ValidSyntax<ClassDeclarationSyntax> syntax)
    {
        return document.AddPartialModifierAction(syntax);
    }
}
```

## Available Base Classes

| Base Class | Syntax Type | Use Case |
|------------|-------------|----------|
| `ClassCodeFix` | `ClassDeclarationSyntax` | Fix class declarations |
| `StructCodeFix` | `StructDeclarationSyntax` | Fix struct declarations |
| `InterfaceCodeFix` | `InterfaceDeclarationSyntax` | Fix interface declarations |
| `RecordCodeFix` | `RecordDeclarationSyntax` | Fix record declarations |
| `EnumCodeFix` | `EnumDeclarationSyntax` | Fix enum declarations |
| `MethodCodeFix` | `MethodDeclarationSyntax` | Fix method declarations |
| `PropertyCodeFix` | `PropertyDeclarationSyntax` | Fix property declarations |
| `FieldCodeFix` | `FieldDeclarationSyntax` | Fix field declarations |
| `ConstructorCodeFix` | `ConstructorDeclarationSyntax` | Fix constructor declarations |
| `EventCodeFix` | `EventDeclarationSyntax` | Fix event declarations |
| `ParameterCodeFix` | `ParameterSyntax` | Fix parameter declarations |

### Common Helpers

These extension methods on `Document` create ready-to-use `CodeAction` objects:

#### Type Modifiers

| Helper | Description |
|--------|-------------|
| `AddPartialModifierAction` | Add `partial` modifier to a type |
| `AddSealedModifierAction` | Add `sealed` modifier to a type |
| `AddStaticModifierAction` | Add `static` modifier to a type |
| `AddAbstractModifierAction` | Add `abstract` modifier to a type |
| `AddReadonlyModifierAction` | Add `readonly` modifier to a struct |
| `AddModifierAction` | Add any modifier (generic) |
| `RemoveModifierAction` | Remove a modifier from a type |

#### Method Modifiers

| Helper | Description |
|--------|-------------|
| `AddAsyncModifierAction` | Add `async` modifier to a method |
| `AddVirtualModifierAction` | Add `virtual` modifier to a method |
| `AddOverrideModifierAction` | Add `override` modifier to a method |
| `AddStaticMethodModifierAction` | Add `static` modifier to a method |
| `AddMethodModifierAction` | Add any modifier to a method |
| `RemoveMethodModifierAction` | Remove a modifier from a method |

#### Rename Helpers

| Helper | Description |
|--------|-------------|
| `RenameMethodAction` | Rename a method |
| `AddAsyncSuffixAction` | Add 'Async' suffix to method name |
| `RemoveAsyncSuffixAction` | Remove 'Async' suffix from method name |
| `RenamePropertyAction` | Rename a property |
| `RenameFieldAction` | Rename a field |
| `RenameTypeAction` | Rename a type (class, struct, etc.) |

#### Field & Property Modifiers

| Helper | Description |
|--------|-------------|
| `MakeFieldPrivateAction` | Change field accessibility to private |
| `AddFieldReadonlyModifierAction` | Add `readonly` modifier to a field |
| `AddRequiredModifierAction` | Add `required` modifier to a property |
| `MakePropertyInitOnlyAction` | Replace `set` with `init` accessor |

#### Return Type Helpers

| Helper | Description |
|--------|-------------|
| `ChangeReturnTypeAction` | Change the return type of a method |
| `WrapReturnTypeInTaskAction` | Wrap return type in `Task<T>` |

#### Structure Helpers

| Helper | Description |
|--------|-------------|
| `AddUsingAction` | Add a using directive |
| `AddBaseTypeAction` | Add a base type to a type declaration |
| `AddInterfaceAction` | Add an interface implementation |
| `AddAttributeAction` | Add an attribute to a member |
| `RemoveAttributeAction` | Remove an attribute from a member |
| `SuppressWithPragmaAction` | Suppress a diagnostic with `#pragma warning disable` |

## Examples

### Make Field Private

```csharp
[CodeFix(FieldMustBePrivateAnalyzer.DiagnosticId)]
public sealed class MakeFieldPrivateCodeFix : FieldCodeFix
{
    protected override CodeAction CreateFix(
        Document document,
        ValidSyntax<FieldDeclarationSyntax> syntax)
    {
        return document.MakeFieldPrivateAction(syntax);
    }
}
```

### Add Sealed Modifier

```csharp
[CodeFix(EffectsModuleShouldBeSealedAnalyzer.DiagnosticId)]
public sealed class MakeClassSealedCodeFix : ClassCodeFix
{
    protected override CodeAction? CreateFix(
        Document document,
        ValidSyntax<ClassDeclarationSyntax> syntax)
    {
        return document.AddSealedModifierAction(syntax);
    }
}
```

### Make Struct Readonly

```csharp
[CodeFix(StrongIdShouldBeReadonlyAnalyzer.DiagnosticId)]
public sealed class MakeStructReadonlyCodeFix : StructCodeFix
{
    protected override CodeAction? CreateFix(
        Document document,
        ValidSyntax<StructDeclarationSyntax> syntax)
    {
        return document.AddModifierAction(syntax, SyntaxKind.ReadOnlyKeyword, "Make struct readonly");
    }
}
```

### Add Interface Implementation

```csharp
[CodeFix(MustImplementDisposableAnalyzer.DiagnosticId)]
public sealed class ImplementDisposableCodeFix : ClassCodeFix
{
    protected override CodeAction? CreateFix(
        Document document,
        ValidSyntax<ClassDeclarationSyntax> syntax)
    {
        return document.AddInterfaceAction(syntax, "IDisposable", "Implement IDisposable");
    }
}
```

### Add Missing Attribute

```csharp
[CodeFix(MissingSerializableAnalyzer.DiagnosticId)]
public sealed class AddSerializableAttributeCodeFix : ClassCodeFix
{
    protected override CodeAction? CreateFix(
        Document document,
        ValidSyntax<ClassDeclarationSyntax> syntax)
    {
        return document.AddAttributeAction(syntax, "Serializable", "Add [Serializable] attribute");
    }
}
```

### Remove Modifier

```csharp
[CodeFix(StaticClassCannotHaveInstanceMembersAnalyzer.DiagnosticId)]
public sealed class RemoveStaticModifierCodeFix : ClassCodeFix
{
    protected override CodeAction? CreateFix(
        Document document,
        ValidSyntax<ClassDeclarationSyntax> syntax)
    {
        return document.RemoveModifierAction(syntax, SyntaxKind.StaticKeyword, "Remove 'static' modifier");
    }
}
```

### Add Async Suffix (Method Rename)

```csharp
[CodeFix(AsyncMethodNamingAnalyzer.DiagnosticId)]
public sealed class AddAsyncSuffixCodeFix : MethodCodeFix
{
    protected override CodeAction? CreateFix(
        Document document,
        ValidSyntax<MethodDeclarationSyntax> syntax)
    {
        return document.AddAsyncSuffixAction(syntax);
    }
}
```

### Rename Method

```csharp
[CodeFix(MethodNamingConventionAnalyzer.DiagnosticId)]
public sealed class RenameMethodCodeFix : MethodCodeFix
{
    protected override CodeAction? CreateFix(
        Document document,
        ValidSyntax<MethodDeclarationSyntax> syntax)
    {
        var currentName = syntax.Node.Identifier.Text;
        var newName = currentName.ToPascalCase(); // Your naming logic
        return document.RenameMethodAction(syntax, newName);
    }
}
```

### Change Return Type to Task

```csharp
[CodeFix(AsyncMethodReturnTypeAnalyzer.DiagnosticId)]
public sealed class WrapReturnTypeInTaskCodeFix : MethodCodeFix
{
    protected override CodeAction? CreateFix(
        Document document,
        ValidSyntax<MethodDeclarationSyntax> syntax)
    {
        return document.WrapReturnTypeInTaskAction(syntax);
    }
}
```

### Make Property Init-Only

```csharp
[CodeFix(ImmutablePropertyAnalyzer.DiagnosticId)]
public sealed class MakePropertyInitOnlyCodeFix : PropertyCodeFix
{
    protected override CodeAction? CreateFix(
        Document document,
        ValidSyntax<PropertyDeclarationSyntax> syntax)
    {
        return document.MakePropertyInitOnlyAction(syntax);
    }
}
```

### Add Required Modifier

```csharp
[CodeFix(RequiredPropertyAnalyzer.DiagnosticId)]
public sealed class AddRequiredModifierCodeFix : PropertyCodeFix
{
    protected override CodeAction? CreateFix(
        Document document,
        ValidSyntax<PropertyDeclarationSyntax> syntax)
    {
        return document.AddRequiredModifierAction(syntax);
    }
}
```

### Remove Attribute

```csharp
[CodeFix(ObsoleteAttributeAnalyzer.DiagnosticId)]
public sealed class RemoveObsoleteAttributeCodeFix : ClassCodeFix
{
    protected override CodeAction? CreateFix(
        Document document,
        ValidSyntax<ClassDeclarationSyntax> syntax)
    {
        return document.RemoveAttributeAction(syntax, "Obsolete");
    }
}
```

### Suppress with Pragma

```csharp
[CodeFix(LegacyApiUsageAnalyzer.DiagnosticId)]
public sealed class SuppressLegacyApiWarningCodeFix : ClassCodeFix
{
    protected override CodeAction? CreateFix(
        Document document,
        ValidSyntax<ClassDeclarationSyntax> syntax)
    {
        // Offer suppression as an alternative to fixing
        return document.SuppressWithPragmaAction(
            Diagnostic.Create(LegacyApiUsageAnalyzer.Rule, syntax.Value.GetLocation()));
    }
}
```

## Custom CodeFix Without Base Class

For more complex scenarios, extend `SyntaxCodeFix<T>` directly:

```csharp
[CodeFix(MultipleIssuesAnalyzer.DiagnosticId)]
public sealed class FixMultipleIssuesCodeFix : SyntaxCodeFix<TypeDeclarationSyntax>
{
    protected override CodeAction? CreateFix(
        Document document,
        ValidSyntax<TypeDeclarationSyntax> syntax)
    {
        return CodeAction.Create(
            "Fix all issues",
            async ct =>
            {
                var root = await document.GetSyntaxRootAsync(ct);
                
                // Apply multiple transformations
                var newType = syntax.Value
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.SealedKeyword));
                
                var newRoot = root!.ReplaceNode(syntax.Value, newType);
                return document.WithSyntaxRoot(newRoot);
            });
    }
}
```

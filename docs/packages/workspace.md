# Deepstaging.Roslyn.Workspace

Code fix infrastructure for Roslyn analyzers using the Workspace API.

## What is this?

This library provides base classes and utilities for building Roslyn code fix providers. It simplifies the boilerplate of registering code fixes and working with syntax nodes.

## Installation

```bash
dotnet add package Deepstaging.Roslyn.Workspace
```

## Quick Start

### Real-world examples

Add `partial` modifier to a struct:

```csharp
[Shared]
[CodeFix(StrongIdMustBePartialAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class StrongIdMustBePartialCodeFix : StructCodeFix
{
    protected override CodeAction CreateFix(
        Document document, 
        ValidSyntax<StructDeclarationSyntax> syntax) =>
        document.AddPartialModifierAction(syntax);
}
```

Add `readonly` modifier to a struct:

```csharp
[Shared]
[CodeFix(StrongIdShouldBeReadonlyAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class StrongIdShouldBeReadonlyCodeFix : StructCodeFix
{
    protected override CodeAction CreateFix(
        Document document, 
        ValidSyntax<StructDeclarationSyntax> syntax) =>
        document.AddModifierAction(syntax, SyntaxKind.ReadOnlyKeyword, "Add 'readonly' modifier");
}
```

Add `sealed` modifier to a class:

```csharp
[Shared]
[CodeFix(EffectsModuleShouldBeSealedAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class EffectsModuleShouldBeSealedCodeFix : ClassCodeFix
{
    protected override CodeAction CreateFix(
        Document document, 
        ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.AddSealedModifierAction(syntax);
}
```

The base class handles:

- Reading `[CodeFix]` attributes to populate `FixableDiagnosticIds`
- Finding the syntax node at the diagnostic location
- Registering the code action with proper error handling

## API Reference

### `SyntaxCodeFix<TSyntax>`

Abstract base class for code fix providers:

```csharp
public abstract class SyntaxCodeFix<TSyntax> : CodeFixProvider
    where TSyntax : SyntaxNode
{
    // Automatically populated from [CodeFix] attributes
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; }
    
    // Override to create your fix
    protected abstract CodeAction? CreateFix(
        Document document, 
        ValidSyntax<TSyntax> syntax);
}
```

### CodeFixAttribute

Declarative configuration for code fixes:

```csharp
[CodeFix("MY001")]  // The diagnostic ID this code fix handles
[CodeFix("MY002")]  // Add multiple attributes for multiple diagnostics
```

### Specialized Base Classes

Pre-defined base classes for common syntax types:

```csharp
// Type declarations
public abstract class ClassCodeFix : SyntaxCodeFix<ClassDeclarationSyntax>;
public abstract class StructCodeFix : SyntaxCodeFix<StructDeclarationSyntax>;
public abstract class InterfaceCodeFix : SyntaxCodeFix<InterfaceDeclarationSyntax>;
public abstract class RecordCodeFix : SyntaxCodeFix<RecordDeclarationSyntax>;
public abstract class EnumCodeFix : SyntaxCodeFix<EnumDeclarationSyntax>;

// Member declarations
public abstract class MethodCodeFix : SyntaxCodeFix<MethodDeclarationSyntax>;
public abstract class PropertyCodeFix : SyntaxCodeFix<PropertyDeclarationSyntax>;
public abstract class FieldCodeFix : SyntaxCodeFix<FieldDeclarationSyntax>;
public abstract class ConstructorCodeFix : SyntaxCodeFix<ConstructorDeclarationSyntax>;
public abstract class EventCodeFix : SyntaxCodeFix<EventDeclarationSyntax>;
public abstract class ParameterCodeFix : SyntaxCodeFix<ParameterSyntax>;
```

### Document Extensions

Convenient methods for document manipulation:

```csharp
// Replace a syntax node (async)
Document newDoc = await document.ReplaceNode(oldNode, newNode, cancellationToken);

// Replace a syntax node (sync, when root is already available)
Document newDoc = document.ReplaceNode(root, oldNode, newNode);
```

### Code Fix Actions

Pre-built `CodeAction` factories for common fixes. These return `CodeAction` objects ready for `CodeFixContext.RegisterCodeFix`.

#### Modifier Actions

```csharp
// Add modifiers to type declarations
document.AddPartialModifierAction(typeDecl)    // Add 'partial'
document.AddSealedModifierAction(typeDecl)     // Add 'sealed'
document.AddStaticModifierAction(typeDecl)     // Add 'static'

// Generic modifier add/remove
document.AddModifierAction(typeDecl, SyntaxKind.ReadOnlyKeyword, "Add 'readonly' modifier")
document.RemoveModifierAction(typeDecl, SyntaxKind.AbstractKeyword, "Remove 'abstract' modifier")
```

#### Field Actions

```csharp
// Make a field private (replaces existing accessibility)
document.MakeFieldPrivateAction(fieldDecl)
document.MakeFieldPrivateAction(fieldDecl, "Custom title")
```

#### Using Directive Actions

```csharp
// Add a using directive to the document
document.AddUsingAction("System.Linq")
document.AddUsingAction("Microsoft.CodeAnalysis")
```

#### Base Type Actions

```csharp
// Add a base type or interface
document.AddBaseTypeAction(typeDecl, "BaseClass")
document.AddInterfaceAction(typeDecl, "IDisposable")
```

#### Attribute Actions

```csharp
// Add an attribute to a type
document.AddAttributeAction(typeDecl, "Serializable")
document.AddAttributeAction(typeDecl, "System.Obsolete")
```

#### Pragma Suppression Actions

```csharp
// Suppress a diagnostic with #pragma warning disable/restore
document.SuppressWithPragmaAction(diagnostic)
```

### CodeFixContext Extensions

Helper methods for finding syntax nodes:

```csharp
// Generic find
OptionalSyntax<TSyntax> result = await context.FindDeclaration<TypeDeclarationSyntax>();

// Type-specific helpers
OptionalSyntax<ClassDeclarationSyntax> classDecl = await context.FindClass();
OptionalSyntax<MethodDeclarationSyntax> methodDecl = await context.FindMethod();
OptionalSyntax<PropertyDeclarationSyntax> propDecl = await context.FindProperty();
// Also: FindStruct(), FindInterface(), FindRecord(), FindEnum(),
//       FindField(), FindConstructor(), FindEvent(), FindParameter()
```

## Projections

### `ValidSyntax<T>`

A wrapper guaranteeing a non-null syntax node:

```csharp
protected override CodeAction? CreateFix(Document document, ValidSyntax<TypeDeclarationSyntax> syntax)
{
    // syntax.Node is guaranteed non-null
    string name = syntax.Node.Identifier.Text;
    
    // Access the underlying node
    TypeDeclarationSyntax node = syntax.Node;
}
```

## Comparison

Without this library:

```csharp
public class AddPartialCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => 
        ImmutableArray.Create("ID0001");
    
    public override FixAllProvider GetFixAllProvider() => 
        WellKnownFixAllProviders.BatchFixer;
    
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
        if (root == null) return;
        
        var diagnostic = context.Diagnostics.First();
        var node = root.FindNode(diagnostic.Location.SourceSpan);
        
        if (node.FirstAncestorOrSelf<StructDeclarationSyntax>() is not { } structDecl)
            return;
        
        var codeAction = CodeAction.Create(
            title: "Add partial modifier",
            createChangedDocument: async ct =>
            {
                var newDecl = structDecl.AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.PartialKeyword));
                var newRoot = root.ReplaceNode(structDecl, newDecl);
                return context.Document.WithSyntaxRoot(newRoot);
            },
            equivalenceKey: "AddPartial");
            
        context.RegisterCodeFix(codeAction, diagnostic);
    }
}
```

With this library:

```csharp
[CodeFix("ID0001")]
[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class AddPartialCodeFix : StructCodeFix
{
    protected override CodeAction CreateFix(
        Document document, 
        ValidSyntax<StructDeclarationSyntax> syntax) =>
        document.AddPartialModifierAction(syntax);
}
```

# Deepstaging.Roslyn.Workspace

Code fix infrastructure for Roslyn analyzers using the Workspace API.

> **See also:** [Roslyn Toolkit](../Deepstaging.Roslyn/README.md) | [Testing](../Deepstaging.Roslyn.Testing/README.md)

## What is this?

This library provides base classes and utilities for building Roslyn code fix providers. It simplifies the boilerplate of registering code fixes and working with syntax nodes.

## Quick Start

### 1. Define a code fix with attributes

```csharp
[ExportCodeFixProvider(LanguageNames.CSharp)]
[CodeFix("MY001")]
[CodeFix("MY002")]  // Can fix multiple diagnostics
public class AddPartialModifierCodeFix : SyntaxCodeFix<TypeDeclarationSyntax>
{
    protected override CodeAction? CreateFix(Document document, ValidSyntax<TypeDeclarationSyntax> syntax)
    {
        return CodeAction.Create(
            title: "Add partial modifier",
            createChangedDocument: ct => AddPartialModifier(document, syntax.Node, ct),
            equivalenceKey: "AddPartial");
    }
    
    private async Task<Document> AddPartialModifier(
        Document document, 
        TypeDeclarationSyntax declaration,
        CancellationToken ct)
    {
        var newDeclaration = declaration
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
        
        return await document.ReplaceNode(declaration, newDeclaration, ct);
    }
}
```

### 2. That's it!

The base class handles:
- Reading `[CodeFix]` attributes to populate `FixableDiagnosticIds`
- Finding the syntax node at the diagnostic location
- Registering the code action with proper error handling

## API Reference

### SyntaxCodeFix\<TSyntax\>

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

### ValidSyntax\<T\>

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

## Why Use This?

Without this library:
```csharp
public class MyCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => 
        ImmutableArray.Create("MY001", "MY002");
    
    public override FixAllProvider GetFixAllProvider() => 
        WellKnownFixAllProviders.BatchFixer;
    
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
        if (root == null) return;
        
        var diagnostic = context.Diagnostics.First();
        var node = root.FindNode(diagnostic.Location.SourceSpan);
        
        if (node.FirstAncestorOrSelf<TypeDeclarationSyntax>() is not { } typeDecl)
            return;
        
        var codeAction = CodeAction.Create(...);
        context.RegisterCodeFix(codeAction, diagnostic);
    }
}
```

With this library:
```csharp
[CodeFix("MY001")]
[CodeFix("MY002")]
public class MyCodeFix : SyntaxCodeFix<TypeDeclarationSyntax>
{
    protected override CodeAction? CreateFix(Document document, ValidSyntax<TypeDeclarationSyntax> syntax)
    {
        return CodeAction.Create(...);
    }
}
```

## Related Documentation

- **[Roslyn Toolkit](../Deepstaging.Roslyn/README.md)** - Query builders, projections, and emit API
- **[Testing](../Deepstaging.Roslyn.Testing/README.md)** - Test infrastructure including code fix testing
- **[Main README](../../README.md)** - Project overview

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](../../LICENSE) for the full legal text.

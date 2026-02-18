# Code Fixes

Code fix infrastructure for Roslyn analyzers.

## What is this?

This library provides base classes and utilities for building Roslyn code fix providers. It simplifies the boilerplate of registering code fixes and working with syntax nodes.

!!! note "Bundled package"
    `Deepstaging.Roslyn.CodeFixes` is not published as a separate NuGet package — it ships inside `Deepstaging.Roslyn`.

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

## API Reference

- [Code Fix Base Classes](code-fix-base.md) — `SyntaxCodeFix<T>`, `ProjectCodeFix`, `CodeFixAttribute`, specialized base classes
- [Code Fix Actions](code-fix-actions.md) — Pre-built `CodeAction` factories for modifiers, fields, usings, attributes, and more
- [ProjectFileActionsBuilder](file-actions.md) — Fluent builder for composing multiple file operations into a single `CodeAction`
- [ManagedPropsFile](managed-props.md) — Managed MSBuild `.props` files with defaults and XML helpers
- [Context Extensions](context-extensions.md) — `CodeFixContext` extensions and `ValidSyntax<T>`

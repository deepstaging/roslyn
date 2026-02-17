# Context Extensions

Helper methods for `CodeFixContext` and syntax projections.

## CodeFixContext Extensions

Helper methods for finding syntax nodes at a diagnostic location:

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

## ValidSyntax&lt;T&gt;

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

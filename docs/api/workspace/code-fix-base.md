# Code Fix Base Classes

Base classes for building Roslyn code fix providers with minimal boilerplate.

## SyntaxCodeFix&lt;TSyntax&gt;

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

## CodeFixAttribute

Declarative configuration for code fixes:

```csharp
[CodeFix("MY001")]  // The diagnostic ID this code fix handles
[CodeFix("MY002")]  // Add multiple attributes for multiple diagnostics
```

## Specialized Base Classes

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

## ProjectCodeFix

Base class for code fixes that operate on the project level rather than on syntax nodes. Useful for fixes that modify the project file (e.g., adding MSBuild properties) or write files.

```csharp
public abstract class ProjectCodeFix : CodeFixProvider
{
    // Automatically populated from [CodeFix] attributes
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; }
    
    // Override to create your fix
    protected abstract CodeAction? CreateFix(Project project, Diagnostic diagnostic);
}
```

With automatic symbol resolution:

```csharp
public abstract class ProjectCodeFix<TSymbol> : ProjectCodeFix
    where TSymbol : class, ISymbol
{
    // Symbol is resolved from the diagnostic location
    protected abstract CodeAction? CreateFix(Project project, ValidSymbol<TSymbol> symbol);
}
```

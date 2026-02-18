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

## AdditionalDocumentCodeFix

Base class for code fixes that add non-compilable additional documents (e.g., templates, configuration files) to a project. Unlike `SyntaxCodeFix<TSyntax>` which modifies existing source, this creates new files as additional documents.

```csharp
public abstract class AdditionalDocumentCodeFix : CodeFixProvider
{
    // Automatically populated from [CodeFix] attributes
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; }
    
    // Override to create the document to add
    protected abstract AdditionalDocument? CreateDocument(
        Compilation compilation, 
        Diagnostic diagnostic);
    
    // Customize the lightbulb menu title (default: "Create file: {path}")
    protected virtual string GetTitle(
        AdditionalDocument document, 
        Diagnostic diagnostic);
}
```

With automatic symbol resolution:

```csharp
public abstract class AdditionalDocumentCodeFix<TSymbol> : AdditionalDocumentCodeFix
    where TSymbol : class, ISymbol
{
    // Symbol is resolved from the diagnostic location
    protected abstract AdditionalDocument? CreateDocument(
        Compilation compilation, 
        ValidSymbol<TSymbol> symbol);
}
```

The `AdditionalDocument` struct carries the file path and content:

```csharp
public readonly struct AdditionalDocument
{
    public string Path { get; }     // e.g., "Templates/MyProject/Widget.scriban-cs"
    public string Content { get; }
}
```

## SourceDocumentCodeFix

Base class for code fixes that add compilable source documents (.cs files) to a project. Unlike `AdditionalDocumentCodeFix`, the created files appear in the IDE preview pane as real source code.

```csharp
public abstract class SourceDocumentCodeFix : CodeFixProvider
{
    // Automatically populated from [CodeFix] attributes
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; }
    
    // Override to create the source document to add
    protected abstract SourceDocument? CreateDocument(
        Compilation compilation, 
        Diagnostic diagnostic);
    
    // Customize the lightbulb menu title (default: "Generate file: {path}")
    protected virtual string GetTitle(
        SourceDocument document, 
        Diagnostic diagnostic);
}
```

With automatic symbol resolution:

```csharp
public abstract class SourceDocumentCodeFix<TSymbol> : SourceDocumentCodeFix
    where TSymbol : class, ISymbol
{
    // Symbol is resolved from the diagnostic location
    protected abstract SourceDocument? CreateDocument(
        Compilation compilation, 
        ValidSymbol<TSymbol> symbol);
}
```

The `SourceDocument` struct carries the file path and source code:

```csharp
public readonly struct SourceDocument
{
    public string Path { get; }     // e.g., "Effects/IEmailService.g.cs"
    public string Content { get; }
}
```

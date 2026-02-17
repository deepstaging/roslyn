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

// Remove an attribute
document.RemoveAttributeAction(typeDecl, "Obsolete")

// Replace one attribute with another (preserves namespace prefix)
document.ReplaceAttributeAction(typeDecl, "EffectsModule", "Capability")
```

#### Pragma Suppression Actions

```csharp
// Suppress a diagnostic with #pragma warning disable/restore
document.SuppressWithPragmaAction(diagnostic)
```

### ProjectCodeFix

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

### Project-Level Code Fix Actions

```csharp
// Add an MSBuild property to the first <PropertyGroup>
project.AddProjectPropertyAction("UserSecretsId", Guid.NewGuid().ToString())
project.AddProjectPropertyAction("EnableTrimming", "true", comment: "Enable IL trimming")

// Write a file to the project directory
project.WriteFileAction("appsettings.schema.json", jsonContent)

// Write multiple files in one operation
project.WriteFilesAction("Generate schema files", files)

// Modify an XML file relative to the project directory
project.ModifyXmlFileAction("Update config", "myfile.props", doc => { ... })
```

### ProjectFileActionsBuilder

Fluent builder for composing multiple file operations into a single `CodeAction`. Created via `project.FileActions("title")`.

```csharp
return project.FileActions("Generate configuration files")
    .Write("schema.json", schemaContent)
    .WriteIfNotExists("settings.json", "{}")
    .AppendLine(".gitignore", "secrets.json")
    .MergeJsonFile("appsettings.json", templateJson)
    .SyncJsonFile("config.schema.json", schemaJson)
    .ModifyProjectFile(doc => doc.SetProperty("EnableFeature", "true"))
    .ModifyXmlFile("deepstaging.props", doc => { ... })
    .ToCodeAction();
```

| Method | Description |
|--------|-------------|
| `Write(path, content)` | Write a file, overwriting any existing content |
| `WriteIfNotExists(path, content)` | Write only if the file does not already exist |
| `AppendLine(path, line)` | Append a line if not already present; creates file if needed |
| `MergeJsonFile(path, template)` | Deep merge template JSON, adding missing keys while preserving existing values |
| `SyncJsonFile(path, template)` | Sync JSON with template: add missing keys, remove extra keys, preserve `$`-prefixed keys |
| `ModifyProjectFile(action)` | Modify the `.csproj` XML document directly |
| `ModifyXmlFile(path, action)` | Modify (or create) an XML file relative to the project directory |
| `ToCodeAction()` | Build all operations into a single `CodeAction` |

### ManagedPropsFile

Base class for managed MSBuild `.props` files that generators and code fixes can read from and write to. Subclasses declare a file name and default contents; the framework ensures those defaults exist whenever the file is modified.

```csharp
public sealed class MyGeneratorProps : ManagedPropsFile
{
    public override string FileName => "mygenerator.props";

    protected override void ConfigureDefaults(PropsBuilder builder) =>
        builder
            .Property("CompilerGeneratedFilesOutputPath", "!generated")
            .ItemGroup(items =>
            {
                items.Remove("Compile", "!generated/**");
                items.Include("None", "!generated/**");
            });
}
```

Usage pattern:

1. Subclass `ManagedPropsFile` and override `FileName` and `ConfigureDefaults`
2. In NuGet `build/*.props`, auto-import the local file with a `Condition="Exists(...)"`
3. In code fixes, use `project.ModifyPropsFileAction<T>(...)` or `builder.ModifyPropsFile<T>(...)`

#### PropsBuilder

Fluent builder used inside `ConfigureDefaults`:

```csharp
builder
    .Property("OutputPath", "bin")              // Unlabeled PropertyGroup
    .PropertyGroup("MyLabel", pg =>             // Labeled PropertyGroup
        pg.Property("Feature", "true"))
    .ItemGroup(items =>                         // ItemGroup with Include/Remove/Update
    {
        items.Include("None", "!generated/**");
        items.Remove("Compile", "!generated/**");
        items.Update("AdditionalFiles", "*.json", meta =>
            meta.Add("DependentUpon", "%(Filename).cs"));
    });
```

#### ManagedPropsFile Extensions

```csharp
// Standalone code action — ensures defaults, then applies modification
project.ModifyPropsFileAction<MyGeneratorProps>("Add property", doc =>
    doc.SetProperty("MyProp", "value"))

// Inside a FileActions builder
project.FileActions("Setup generator")
    .ModifyPropsFile<MyGeneratorProps>(doc =>
        doc.SetProperty("MyProp", "value"))
    .Write("template.json", content)
    .ToCodeAction();
```

#### PropsXmlExtensions

Helper methods for manipulating `.props` XML documents:

```csharp
// Set a property (no-op if already exists)
doc.SetProperty("UserSecretsId", Guid.NewGuid().ToString())
doc.SetProperty("Feature", "true", label: "MyLabel", comment: "Enable feature")

// Replace a labeled ItemGroup with new items
doc.SetItemGroup("Generated", items =>
    items.Item("None", "Update", "*.g.cs", meta =>
        meta.Set("DependentUpon", "%(Filename).cs")));
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

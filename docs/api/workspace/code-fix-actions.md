# Code Fix Actions

Pre-built `CodeAction` factories for common fixes. These return `CodeAction` objects ready for `CodeFixContext.RegisterCodeFix`.

## Document Extensions

Convenient methods for document manipulation:

```csharp
// Replace a syntax node (async)
Document newDoc = await document.ReplaceNode(oldNode, newNode, cancellationToken);

// Replace a syntax node (sync, when root is already available)
Document newDoc = document.ReplaceNode(root, oldNode, newNode);
```

## Modifier Actions

```csharp
// Add modifiers to type declarations
document.AddPartialModifierAction(typeDecl)    // Add 'partial'
document.AddSealedModifierAction(typeDecl)     // Add 'sealed'
document.AddStaticModifierAction(typeDecl)     // Add 'static'

// Generic modifier add/remove
document.AddModifierAction(typeDecl, SyntaxKind.ReadOnlyKeyword, "Add 'readonly' modifier")
document.RemoveModifierAction(typeDecl, SyntaxKind.AbstractKeyword, "Remove 'abstract' modifier")
```

## Field Actions

```csharp
// Make a field private (replaces existing accessibility)
document.MakeFieldPrivateAction(fieldDecl)
document.MakeFieldPrivateAction(fieldDecl, "Custom title")
```

## Using Directive Actions

```csharp
// Add a using directive to the document
document.AddUsingAction("System.Linq")
document.AddUsingAction("Microsoft.CodeAnalysis")
```

## Base Type Actions

```csharp
// Add a base type or interface
document.AddBaseTypeAction(typeDecl, "BaseClass")
document.AddInterfaceAction(typeDecl, "IDisposable")
```

## Attribute Actions

```csharp
// Add an attribute to a type
document.AddAttributeAction(typeDecl, "Serializable")
document.AddAttributeAction(typeDecl, "System.Obsolete")

// Remove an attribute
document.RemoveAttributeAction(typeDecl, "Obsolete")

// Replace one attribute with another (preserves namespace prefix)
document.ReplaceAttributeAction(typeDecl, "EffectsModule", "Capability")
```

## Pragma Suppression Actions

```csharp
// Suppress a diagnostic with #pragma warning disable/restore
document.SuppressWithPragmaAction(diagnostic)
```

## Project-Level Actions

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

For composing multiple file operations into a single action, see [ProjectFileActionsBuilder](file-actions.md).

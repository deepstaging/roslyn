# Syntax Wrappers

Optional and validated wrappers for Roslyn syntax nodes.

> **See also:** [Projections Overview](index.md) | [ValidSymbol](valid-symbol.md)

## OptionalSyntax<T>

```csharp
OptionalSyntax<ClassDeclarationSyntax>.WithValue(classDecl)
OptionalSyntax<ClassDeclarationSyntax>.Empty()
OptionalSyntax<ClassDeclarationSyntax>.FromNullable(maybeNull)

optional.Node               // TSyntax?
optional.Location           // Location
optional.Span               // TextSpan
optional.FullSpan           // TextSpan

optional.Map(n => n.Identifier.Text)
optional.Where(n => n.Modifiers.Any(SyntaxKind.PublicKeyword))
optional.OfType<RecordDeclarationSyntax>()

optional.Parent             // OptionalSyntax<SyntaxNode>
optional.Ancestor<T>()      // OptionalSyntax<T>
optional.Ancestors<T>()     // IEnumerable<T>

if (optional.IsValid(out var valid)) { /* use valid */ }
```

## ValidSyntax<T>

```csharp
ValidSyntax<ClassDeclarationSyntax>.From(classDecl)
ValidSyntax<ClassDeclarationSyntax>.TryFrom(maybeNull)

valid.Node                  // TSyntax (guaranteed non-null)
valid.Location              // Location
valid.SyntaxTree            // SyntaxTree
valid.Text                  // string — the node's text
valid.FullText              // string — text with trivia

valid.Parent                // ValidSyntax<SyntaxNode>?
valid.Ancestor<T>()         // ValidSyntax<T>?
valid.Ancestors<T>()        // IEnumerable<T>
valid.Descendant<T>()       // ValidSyntax<T>?
valid.Descendants<T>()      // IEnumerable<T>

valid.LeadingTrivia         // SyntaxTriviaList
valid.TrailingTrivia        // SyntaxTriviaList

// Implicit conversion to the underlying node
ClassDeclarationSyntax node = valid;
```

## ValidTypeSyntax<T>

Specialized wrapper for type declaration syntax (`ClassDeclarationSyntax`, `RecordDeclarationSyntax`, etc.) with rich helpers.

```csharp
ValidTypeSyntax<ClassDeclarationSyntax>.From(classDecl)

syntax.Name                 // string
syntax.Identifier           // SyntaxToken
syntax.Keyword              // SyntaxToken (e.g., "class", "record")
syntax.Location             // Location
syntax.IdentifierLocation   // Location
```

### Modifiers

```csharp
syntax.Modifiers            // SyntaxTokenList
syntax.HasModifier(SyntaxKind.PublicKeyword)
syntax.IsPartial            // bool
syntax.IsStatic             // bool
syntax.IsAbstract           // bool
syntax.IsSealed             // bool
syntax.IsPublic             // bool
syntax.IsInternal           // bool
syntax.IsPrivate            // bool
syntax.IsProtected          // bool
syntax.IsReadOnly           // bool
syntax.IsFile               // bool
```

### Modifier Manipulation

```csharp
// Returns new syntax
syntax.AddModifier(SyntaxKind.PartialKeyword)
syntax.RemoveModifier(SyntaxKind.SealedKeyword)
syntax.WithModifiers(newModifiers)
```

### Structure

```csharp
syntax.BaseList             // BaseListSyntax?
syntax.TypeParameterList    // TypeParameterListSyntax?
syntax.ConstraintClauses    // SyntaxList<TypeParameterConstraintClauseSyntax>
syntax.AttributeLists       // SyntaxList<AttributeListSyntax>
syntax.Members              // SyntaxList<MemberDeclarationSyntax>

syntax.HasBaseList          // bool
syntax.IsGeneric            // bool
syntax.Arity                // int
```

### Navigation

```csharp
syntax.ContainingType       // ValidTypeSyntax<TypeDeclarationSyntax>?
syntax.ContainingNamespace  // ValidSyntax<BaseNamespaceDeclarationSyntax>?
syntax.NestedTypes          // IEnumerable<ValidTypeSyntax<TypeDeclarationSyntax>>
```

### Member Access

```csharp
syntax.Methods              // IEnumerable<MethodDeclarationSyntax>
syntax.Properties           // IEnumerable<PropertyDeclarationSyntax>
syntax.Fields               // IEnumerable<FieldDeclarationSyntax>
syntax.Constructors         // IEnumerable<ConstructorDeclarationSyntax>
```

### Conversions

```csharp
TypeDeclarationSyntax node = syntax;
ValidSyntax<ClassDeclarationSyntax> validSyntax = syntax;
```

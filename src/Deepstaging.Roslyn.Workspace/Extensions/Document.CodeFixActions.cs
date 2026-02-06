// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for Document that create common code fix actions.
/// These return <see cref="CodeAction"/> objects ready to be passed to
/// <c>CodeFixContext.RegisterCodeFix</c>.
/// </summary>
public static class CodeFixActionExtensions
{
    extension(Document document)
    {
        #region Modifier Helpers

        /// <summary>
        /// Creates a code action that adds the 'partial' modifier to a type declaration.
        /// </summary>
        public CodeAction AddPartialModifierAction<T>(ValidSyntax<T> typeDecl)
            where T : TypeDeclarationSyntax
        {
            return document.AddModifierAction(typeDecl, SyntaxKind.PartialKeyword, "Add 'partial' modifier");
        }

        /// <summary>
        /// Creates a code action that adds the 'sealed' modifier to a type declaration.
        /// </summary>
        public CodeAction AddSealedModifierAction<T>(ValidSyntax<T> typeDecl)
            where T : TypeDeclarationSyntax
        {
            return document.AddModifierAction(typeDecl, SyntaxKind.SealedKeyword, "Add 'sealed' modifier");
        }

        /// <summary>
        /// Creates a code action that adds the 'static' modifier to a type declaration.
        /// </summary>
        public CodeAction AddStaticModifierAction<T>(ValidSyntax<T> typeDecl)
            where T : TypeDeclarationSyntax
        {
            return document.AddModifierAction(typeDecl, SyntaxKind.StaticKeyword, "Add 'static' modifier");
        }

        /// <summary>
        /// Creates a code action that adds a modifier to a type declaration.
        /// </summary>
        /// <param name="typeDecl">The validated type declaration syntax.</param>
        /// <param name="modifier">The modifier kind to add.</param>
        /// <param name="title">The title for the code action.</param>
        public CodeAction AddModifierAction<T>(
            ValidSyntax<T> typeDecl,
            SyntaxKind modifier,
            string title)
            where T : TypeDeclarationSyntax
        {
            return CodeAction.Create(
                title,
                ct => document.ReplaceNode(
                    typeDecl.Node,
                    typeDecl.AddModifier(modifier),
                    ct),
                title);
        }

        /// <summary>
        /// Creates a code action that removes a modifier from a type declaration.
        /// </summary>
        /// <param name="typeDecl">The validated type declaration syntax.</param>
        /// <param name="modifier">The modifier kind to remove.</param>
        /// <param name="title">The title for the code action.</param>
        public CodeAction RemoveModifierAction<T>(
            ValidSyntax<T> typeDecl,
            SyntaxKind modifier,
            string title)
            where T : TypeDeclarationSyntax
        {
            return CodeAction.Create(
                title,
                ct => document.ReplaceNode(
                    typeDecl.Node,
                    typeDecl.RemoveModifier(modifier),
                    ct),
                title);
        }

        #endregion

        #region Field Modifier Helpers

        /// <summary>
        /// Creates a code action that makes a field private by replacing its accessibility modifiers.
        /// </summary>
        /// <param name="fieldDecl">The validated field declaration syntax.</param>
        /// <param name="title">Optional title for the code action. Defaults to "Make field private".</param>
        public CodeAction MakeFieldPrivateAction(
            ValidSyntax<FieldDeclarationSyntax> fieldDecl,
            string title = "Make field private")
        {
            return CodeAction.Create(
                title,
                ct => document.ReplaceNode(
                    fieldDecl.Node,
                    MakeFieldPrivate(fieldDecl.Node),
                    ct),
                title);
        }

        #endregion

        #region Using Directive Helpers

        /// <summary>
        /// Creates a code action that adds a using directive to the document.
        /// </summary>
        /// <param name="namespaceName">The namespace to add (e.g., "System.Linq").</param>
        public CodeAction AddUsingAction(string namespaceName)
        {
            var title = $"Add 'using {namespaceName};'";
            return CodeAction.Create(
                title,
                ct => AddUsingDirectiveAsync(document, namespaceName, ct),
                title);
        }

        #endregion

        #region Base Type Helpers

        /// <summary>
        /// Creates a code action that adds a base type or interface to a type declaration.
        /// </summary>
        /// <param name="typeDecl">The validated type declaration syntax.</param>
        /// <param name="baseTypeName">The name of the base type or interface to add.</param>
        public CodeAction AddBaseTypeAction<T>(
            ValidSyntax<T> typeDecl,
            string baseTypeName)
            where T : TypeDeclarationSyntax
        {
            var title = $"Add '{baseTypeName}'";
            return CodeAction.Create(
                title,
                ct => AddBaseTypeAsync(document, typeDecl, baseTypeName, ct),
                title);
        }

        /// <summary>
        /// Creates a code action that adds an interface to a type declaration.
        /// </summary>
        public CodeAction AddInterfaceAction<T>(
            ValidSyntax<T> typeDecl,
            string interfaceName)
            where T : TypeDeclarationSyntax
        {
            var title = $"Implement '{interfaceName}'";
            return CodeAction.Create(
                title,
                ct => AddBaseTypeAsync(document, typeDecl, interfaceName, ct),
                title);
        }

        #endregion

        #region Attribute Helpers

        /// <summary>
        /// Creates a code action that adds an attribute to a type declaration.
        /// </summary>
        /// <param name="typeDecl">The validated type declaration syntax.</param>
        /// <param name="attributeName">The fully qualified attribute name (e.g., "Deepstaging.Runtime").</param>
        public CodeAction AddAttributeAction<T>(
            ValidSyntax<T> typeDecl,
            string attributeName)
            where T : TypeDeclarationSyntax
        {
            var shortName = attributeName.Contains('.')
                ? attributeName.Substring(attributeName.LastIndexOf('.') + 1)
                : attributeName;
            var title = $"Add [{shortName}] attribute";
            return CodeAction.Create(
                title,
                ct => AddAttributeAsync(document, typeDecl, attributeName, ct),
                title);
        }

        #endregion

        #region Pragma Suppression Helpers

        /// <summary>
        /// Creates a code action that suppresses a diagnostic with a pragma directive.
        /// </summary>
        /// <param name="diagnostic">The diagnostic to suppress.</param>
        public CodeAction SuppressWithPragmaAction(Diagnostic diagnostic)
        {
            var title = $"Suppress '{diagnostic.Id}' with #pragma";
            return CodeAction.Create(
                title,
                ct => SuppressWithPragmaAsync(document, diagnostic, ct),
                title);
        }

        #endregion
    }

    #region Private Helper Methods

    private static readonly SyntaxKind[] AccessibilityModifiers =
    [
        SyntaxKind.PublicKeyword,
        SyntaxKind.ProtectedKeyword,
        SyntaxKind.InternalKeyword,
        SyntaxKind.PrivateKeyword
    ];

    private static FieldDeclarationSyntax MakeFieldPrivate(FieldDeclarationSyntax fieldDeclaration)
    {
        var newModifiers = fieldDeclaration.Modifiers
            .Where(m => !AccessibilityModifiers.Contains(m.Kind()))
            .ToList();

        newModifiers.Insert(0, SyntaxFactory.Token(SyntaxKind.PrivateKeyword));

        return fieldDeclaration.WithModifiers(SyntaxFactory.TokenList(newModifiers));
    }

    private static async Task<Document> AddUsingDirectiveAsync(
        Document document,
        string namespaceName,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is not CompilationUnitSyntax compilationUnit)
            return document;

        var usingDirective = SyntaxFactory.UsingDirective(
                SyntaxFactory.ParseName(namespaceName))
            .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        // Check if using already exists
        if (compilationUnit.Usings.Any(u => u.Name?.ToString() == namespaceName))
            return document;

        var newRoot = compilationUnit.AddUsings(usingDirective);
        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> AddBaseTypeAsync<T>(
        Document document,
        ValidSyntax<T> typeDecl,
        string baseTypeName,
        CancellationToken cancellationToken)
        where T : TypeDeclarationSyntax
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
            return document;

        var node = typeDecl.Node;
        var baseType = SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseTypeName));

        TypeDeclarationSyntax newNode;
        if (node.BaseList is null)
        {
            var baseList = SyntaxFactory.BaseList(
                    SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(baseType))
                .WithLeadingTrivia(SyntaxFactory.Space);
            newNode = node.WithBaseList(baseList);
        }
        else
        {
            var newBaseList = node.BaseList.AddTypes(baseType);
            newNode = node.WithBaseList(newBaseList);
        }

        var newRoot = root.ReplaceNode(node, newNode);
        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> SuppressWithPragmaAsync(
        Document document,
        Diagnostic diagnostic,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
            return document;

        var diagnosticSpan = diagnostic.Location.SourceSpan;
        var token = root.FindToken(diagnosticSpan.Start);
        var node = token.Parent;

        // Find the statement or member containing the diagnostic
        var targetNode = node?.AncestorsAndSelf()
            .FirstOrDefault(n => n is StatementSyntax or MemberDeclarationSyntax);

        if (targetNode is null)
            return document;

        var leadingTrivia = targetNode.GetLeadingTrivia();
        var indentation = leadingTrivia.LastOrDefault(t => t.IsKind(SyntaxKind.WhitespaceTrivia));

        var pragmaDisable = SyntaxFactory.Trivia(
            SyntaxFactory.PragmaWarningDirectiveTrivia(
                SyntaxFactory.Token(SyntaxKind.DisableKeyword),
                SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                    SyntaxFactory.IdentifierName(diagnostic.Id)),
                true));

        var pragmaRestore = SyntaxFactory.Trivia(
            SyntaxFactory.PragmaWarningDirectiveTrivia(
                SyntaxFactory.Token(SyntaxKind.RestoreKeyword),
                SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                    SyntaxFactory.IdentifierName(diagnostic.Id)),
                true));

        var newLeadingTrivia = leadingTrivia
            .Add(pragmaDisable)
            .Add(SyntaxFactory.CarriageReturnLineFeed)
            .Add(indentation);

        var trailingTrivia = targetNode.GetTrailingTrivia()
            .Add(SyntaxFactory.CarriageReturnLineFeed)
            .Add(indentation)
            .Add(pragmaRestore);

        var newNode = targetNode
            .WithLeadingTrivia(newLeadingTrivia)
            .WithTrailingTrivia(trailingTrivia);

        var newRoot = root.ReplaceNode(targetNode, newNode);
        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> AddAttributeAsync<T>(
        Document document,
        ValidSyntax<T> typeDecl,
        string attributeName,
        CancellationToken cancellationToken)
        where T : TypeDeclarationSyntax
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
            return document;

        var node = typeDecl.Node;
        var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(attributeName));

        // Get leading trivia from first existing attribute or the type itself
        var leadingTrivia = node.AttributeLists.Count > 0
            ? node.AttributeLists[0].GetLeadingTrivia()
            : node.GetLeadingTrivia();

        var attributeList = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(attribute))
            .WithLeadingTrivia(leadingTrivia)
            .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        TypeDeclarationSyntax newNode;
        if (node.AttributeLists.Count > 0)
        {
            // Remove leading trivia from first existing attribute to avoid duplication
            var firstAttr = node.AttributeLists[0].WithLeadingTrivia(SyntaxFactory.TriviaList());
            var updatedLists = node.AttributeLists.Replace(node.AttributeLists[0], firstAttr);
            newNode = node.WithAttributeLists(updatedLists.Insert(0, attributeList));
        }
        else
        {
            newNode = node
                .WithLeadingTrivia(SyntaxFactory.TriviaList())
                .WithAttributeLists(node.AttributeLists.Add(attributeList));
        }

        var newRoot = root.ReplaceNode(node, newNode);
        return document.WithSyntaxRoot(newRoot);
    }

    #endregion
}
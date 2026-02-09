// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.Roslyn;

/// <summary>
/// Code fix action helpers for property declarations.
/// </summary>
public static class PropertyCodeFixActions
{
    extension(Document document)
    {
        #region Property Modifier Helpers

        /// <summary>
        /// Creates a code action that adds the 'partial' modifier to a property.
        /// </summary>
        public CodeAction AddPartialModifierAction(
            ValidSyntax<PropertyDeclarationSyntax> propertyDecl,
            string title = "Add 'partial' modifier")
        {
            return CodeAction.Create(
                title,
                ct => document.ReplaceNode(
                    propertyDecl.Node,
                    AddModifierWithOrdering(propertyDecl.Node, SyntaxKind.PartialKeyword),
                    ct),
                title);
        }

        /// <summary>
        /// Creates a code action that adds the 'required' modifier to a property.
        /// </summary>
        public CodeAction AddRequiredModifierAction(
            ValidSyntax<PropertyDeclarationSyntax> propertyDecl,
            string title = "Add 'required' modifier")
        {
            return CodeAction.Create(
                title,
                ct => document.ReplaceNode(
                    propertyDecl.Node,
                    propertyDecl.Node.AddModifiers(SyntaxFactory.Token(SyntaxKind.RequiredKeyword)),
                    ct),
                title);
        }

        /// <summary>
        /// Creates a code action that adds init accessor to property (replaces set with init).
        /// </summary>
        public CodeAction MakePropertyInitOnlyAction(
            ValidSyntax<PropertyDeclarationSyntax> propertyDecl,
            string title = "Make property init-only")
        {
            return CodeAction.Create(
                title,
                ct =>
                {
                    var accessors = propertyDecl.Node.AccessorList?.Accessors ?? [];
                    var newAccessors = accessors.Select(a =>
                        a.IsKind(SyntaxKind.SetAccessorDeclaration)
                            ? SyntaxFactory.AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
                                .WithModifiers(a.Modifiers)
                                .WithBody(a.Body)
                                .WithExpressionBody(a.ExpressionBody)
                                .WithSemicolonToken(a.SemicolonToken)
                            : a);

                    var newAccessorList = SyntaxFactory.AccessorList(SyntaxFactory.List(newAccessors));
                    var newProperty = propertyDecl.Node.WithAccessorList(newAccessorList);
                    return document.ReplaceNode(propertyDecl.Node, newProperty, ct);
                },
                title);
        }

        #endregion

        #region Property Rename Helpers

        /// <summary>
        /// Creates a code action that renames a property.
        /// </summary>
        /// <param name="propertyDecl">The validated property declaration syntax.</param>
        /// <param name="newName">The new name for the property.</param>
        /// <param name="title">Optional title. Defaults to "Rename to 'newName'".</param>
        public CodeAction RenamePropertyAction(
            ValidSyntax<PropertyDeclarationSyntax> propertyDecl,
            string newName,
            string? title = null)
        {
            title ??= $"Rename to '{newName}'";
            return CodeAction.Create(
                title,
                ct => document.ReplaceNode(
                    propertyDecl.Node,
                    propertyDecl.Node.WithIdentifier(SyntaxFactory.Identifier(newName)),
                    ct),
                title);
        }

        #endregion
    }

    #region Private Helper Methods

    private static PropertyDeclarationSyntax AddModifierWithOrdering(
        PropertyDeclarationSyntax node,
        SyntaxKind kind)
    {
        if (node.Modifiers.Any(m => m.IsKind(kind)))
            return node;

        var token = SyntaxFactory.Token(kind).WithTrailingTrivia(SyntaxFactory.Space);
        var insertIndex = GetModifierInsertIndex(node.Modifiers, kind);
        var newModifiers = node.Modifiers.Insert(insertIndex, token);
        return node.WithModifiers(newModifiers);
    }

    private static int GetModifierInsertIndex(SyntaxTokenList modifiers, SyntaxKind kind)
    {
        var priority = GetModifierPriority(kind);

        for (var i = 0; i < modifiers.Count; i++)
            if (GetModifierPriority(modifiers[i].Kind()) > priority)
                return i;

        return modifiers.Count;
    }

    private static int GetModifierPriority(SyntaxKind kind) =>
        kind switch
        {
            // Accessibility modifiers come first
            SyntaxKind.PublicKeyword or
            SyntaxKind.PrivateKeyword or
            SyntaxKind.ProtectedKeyword or
            SyntaxKind.InternalKeyword => 0,

            // Then static/abstract/sealed/virtual/override/new/readonly/required
            SyntaxKind.StaticKeyword or
            SyntaxKind.AbstractKeyword or
            SyntaxKind.SealedKeyword or
            SyntaxKind.VirtualKeyword or
            SyntaxKind.OverrideKeyword or
            SyntaxKind.NewKeyword or
            SyntaxKind.ReadOnlyKeyword or
            SyntaxKind.RequiredKeyword => 1,

            // Then partial
            SyntaxKind.PartialKeyword => 2,

            // Then unsafe
            SyntaxKind.UnsafeKeyword => 3,

            _ => 4
        };

    #endregion
}

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
}

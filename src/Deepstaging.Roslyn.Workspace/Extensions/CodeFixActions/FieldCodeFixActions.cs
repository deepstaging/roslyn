// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.Roslyn;

/// <summary>
/// Code fix action helpers for field declarations.
/// </summary>
public static class FieldCodeFixActions
{
    extension(Document document)
    {
        #region Field Modifier Helpers

        /// <summary>
        /// Creates a code action that makes a field private by replacing its accessibility modifiers.
        /// </summary>
        /// <param name="fieldDecl">The validated field declaration syntax.</param>
        /// <param name="title">Optional title for the code action. Defaults to "Make field private".</param>
        public CodeAction MakeFieldPrivateAction(
            ValidSyntax<FieldDeclarationSyntax> fieldDecl,
            string title = "Make field private") =>
            CodeAction.Create(
                title,
                ct => document.ReplaceNode(
                    fieldDecl.Node,
                    MakeFieldPrivate(fieldDecl.Node),
                    ct),
                title);

        /// <summary>
        /// Creates a code action that adds the 'readonly' modifier to a field.
        /// </summary>
        public CodeAction AddFieldReadonlyModifierAction(
            ValidSyntax<FieldDeclarationSyntax> fieldDecl,
            string title = "Add 'readonly' modifier") =>
            CodeAction.Create(
                title,
                ct => document.ReplaceNode(
                    fieldDecl.Node,
                    fieldDecl.Node.AddModifiers(SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)),
                    ct),
                title);

        #endregion

        #region Field Rename Helpers

        /// <summary>
        /// Creates a code action that renames a field.
        /// </summary>
        /// <param name="fieldDecl">The validated field declaration syntax.</param>
        /// <param name="variableIndex">The index of the variable declarator to rename (for multi-variable declarations).</param>
        /// <param name="newName">The new name for the field.</param>
        /// <param name="title">Optional title. Defaults to "Rename to 'newName'".</param>
        public CodeAction RenameFieldAction(
            ValidSyntax<FieldDeclarationSyntax> fieldDecl,
            int variableIndex,
            string newName,
            string? title = null)
        {
            title ??= $"Rename to '{newName}'";

            return CodeAction.Create(
                title,
                ct =>
                {
                    var variable = fieldDecl.Node.Declaration.Variables[variableIndex];
                    var newVariable = variable.WithIdentifier(SyntaxFactory.Identifier(newName));
                    var newVariables = fieldDecl.Node.Declaration.Variables.Replace(variable, newVariable);
                    var newDeclaration = fieldDecl.Node.Declaration.WithVariables(newVariables);
                    var newField = fieldDecl.Node.WithDeclaration(newDeclaration);
                    return document.ReplaceNode(fieldDecl.Node, newField, ct);
                },
                title);
        }

        /// <summary>
        /// Creates a code action that renames the first variable in a field declaration.
        /// </summary>
        public CodeAction RenameFieldAction(
            ValidSyntax<FieldDeclarationSyntax> fieldDecl,
            string newName,
            string? title = null) =>
            document.RenameFieldAction(fieldDecl, 0, newName, title);

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

    #endregion
}
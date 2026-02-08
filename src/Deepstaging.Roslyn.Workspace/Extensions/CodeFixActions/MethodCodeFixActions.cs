// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.Roslyn;

/// <summary>
/// Code fix action helpers for method declarations.
/// </summary>
public static class MethodCodeFixActions
{
    extension(Document document)
    {
        #region Method Modifier Helpers

        /// <summary>
        /// Creates a code action that adds the 'async' modifier to a method declaration.
        /// </summary>
        public CodeAction AddAsyncModifierAction(ValidSyntax<MethodDeclarationSyntax> methodDecl)
        {
            return document.AddMethodModifierAction(methodDecl, SyntaxKind.AsyncKeyword, "Add 'async' modifier");
        }

        /// <summary>
        /// Creates a code action that adds the 'virtual' modifier to a method declaration.
        /// </summary>
        public CodeAction AddVirtualModifierAction(ValidSyntax<MethodDeclarationSyntax> methodDecl)
        {
            return document.AddMethodModifierAction(methodDecl, SyntaxKind.VirtualKeyword, "Add 'virtual' modifier");
        }

        /// <summary>
        /// Creates a code action that adds the 'override' modifier to a method declaration.
        /// </summary>
        public CodeAction AddOverrideModifierAction(ValidSyntax<MethodDeclarationSyntax> methodDecl)
        {
            return document.AddMethodModifierAction(methodDecl, SyntaxKind.OverrideKeyword, "Add 'override' modifier");
        }

        /// <summary>
        /// Creates a code action that adds the 'static' modifier to a method declaration.
        /// </summary>
        public CodeAction AddStaticMethodModifierAction(ValidSyntax<MethodDeclarationSyntax> methodDecl)
        {
            return document.AddMethodModifierAction(methodDecl, SyntaxKind.StaticKeyword, "Add 'static' modifier");
        }

        /// <summary>
        /// Creates a code action that adds a modifier to a method declaration.
        /// </summary>
        public CodeAction AddMethodModifierAction(
            ValidSyntax<MethodDeclarationSyntax> methodDecl,
            SyntaxKind modifier,
            string title)
        {
            return CodeAction.Create(
                title,
                ct => document.ReplaceNode(
                    methodDecl.Node,
                    methodDecl.Node.AddModifiers(SyntaxFactory.Token(modifier)),
                    ct),
                title);
        }

        /// <summary>
        /// Creates a code action that removes a modifier from a method declaration.
        /// </summary>
        public CodeAction RemoveMethodModifierAction(
            ValidSyntax<MethodDeclarationSyntax> methodDecl,
            SyntaxKind modifier,
            string title)
        {
            return CodeAction.Create(
                title,
                ct =>
                {
                    var newModifiers = methodDecl.Node.Modifiers
                        .Where(m => !m.IsKind(modifier));
                    var newNode = methodDecl.Node.WithModifiers(SyntaxFactory.TokenList(newModifiers));
                    return document.ReplaceNode(methodDecl.Node, newNode, ct);
                },
                title);
        }

        #endregion

        #region Method Rename Helpers

        /// <summary>
        /// Creates a code action that renames a method.
        /// </summary>
        /// <param name="methodDecl">The validated method declaration syntax.</param>
        /// <param name="newName">The new name for the method.</param>
        /// <param name="title">Optional title. Defaults to "Rename to 'newName'".</param>
        public CodeAction RenameMethodAction(
            ValidSyntax<MethodDeclarationSyntax> methodDecl,
            string newName,
            string? title = null)
        {
            title ??= $"Rename to '{newName}'";
            return CodeAction.Create(
                title,
                ct => document.ReplaceNode(
                    methodDecl.Node,
                    methodDecl.Node.WithIdentifier(SyntaxFactory.Identifier(newName)),
                    ct),
                title);
        }

        /// <summary>
        /// Creates a code action that adds the 'Async' suffix to a method name.
        /// </summary>
        public CodeAction AddAsyncSuffixAction(ValidSyntax<MethodDeclarationSyntax> methodDecl)
        {
            var newName = methodDecl.Node.Identifier.Text + "Async";
            return document.RenameMethodAction(methodDecl, newName, $"Rename to '{newName}'");
        }

        /// <summary>
        /// Creates a code action that removes the 'Async' suffix from a method name.
        /// </summary>
        public CodeAction RemoveAsyncSuffixAction(ValidSyntax<MethodDeclarationSyntax> methodDecl)
        {
            var currentName = methodDecl.Node.Identifier.Text;
            var newName = currentName.EndsWith("Async")
                ? currentName.Substring(0, currentName.Length - 5)
                : currentName;
            return document.RenameMethodAction(methodDecl, newName, $"Rename to '{newName}'");
        }

        #endregion

        #region Return Type Helpers

        /// <summary>
        /// Creates a code action that changes the return type of a method.
        /// </summary>
        public CodeAction ChangeReturnTypeAction(
            ValidSyntax<MethodDeclarationSyntax> methodDecl,
            string newReturnType,
            string? title = null)
        {
            title ??= $"Change return type to '{newReturnType}'";
            return CodeAction.Create(
                title,
                ct => document.ReplaceNode(
                    methodDecl.Node,
                    methodDecl.Node.WithReturnType(SyntaxFactory.ParseTypeName(newReturnType).WithTrailingTrivia(SyntaxFactory.Space)),
                    ct),
                title);
        }

        /// <summary>
        /// Creates a code action that wraps the return type in Task&lt;T&gt;.
        /// </summary>
        public CodeAction WrapReturnTypeInTaskAction(ValidSyntax<MethodDeclarationSyntax> methodDecl)
        {
            var currentType = methodDecl.Node.ReturnType.ToString().Trim();
            var newType = currentType == "void" ? "Task" : $"Task<{currentType}>";
            return document.ChangeReturnTypeAction(methodDecl, newType, $"Change return type to '{newType}'");
        }

        #endregion
    }
}

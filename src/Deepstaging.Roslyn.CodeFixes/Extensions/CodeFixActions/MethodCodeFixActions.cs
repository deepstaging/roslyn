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
    #region Method Modifier Helpers

    /// <summary>
    /// Creates a code action that adds the 'partial' modifier to a method declaration.
    /// </summary>
    public static CodeAction AddPartialModifierAction(
        this Document document,
        ValidSyntax<MethodDeclarationSyntax> methodDecl) =>
        document.AddMethodModifierAction(methodDecl, SyntaxKind.PartialKeyword, "Add 'partial' modifier");

    /// <summary>
    /// Creates a code action that adds the 'async' modifier to a method declaration.
    /// </summary>
    public static CodeAction AddAsyncModifierAction(this Document document, ValidSyntax<MethodDeclarationSyntax> methodDecl) =>
        document.AddMethodModifierAction(methodDecl, SyntaxKind.AsyncKeyword, "Add 'async' modifier");

    /// <summary>
    /// Creates a code action that adds the 'virtual' modifier to a method declaration.
    /// </summary>
    public static CodeAction AddVirtualModifierAction(
        this Document document,
        ValidSyntax<MethodDeclarationSyntax> methodDecl) =>
        document.AddMethodModifierAction(methodDecl, SyntaxKind.VirtualKeyword, "Add 'virtual' modifier");

    /// <summary>
    /// Creates a code action that adds the 'override' modifier to a method declaration.
    /// </summary>
    public static CodeAction AddOverrideModifierAction(
        this Document document,
        ValidSyntax<MethodDeclarationSyntax> methodDecl) =>
        document.AddMethodModifierAction(methodDecl, SyntaxKind.OverrideKeyword, "Add 'override' modifier");

    /// <summary>
    /// Creates a code action that adds the 'static' modifier to a method declaration.
    /// </summary>
    public static CodeAction AddStaticMethodModifierAction(
        this Document document,
        ValidSyntax<MethodDeclarationSyntax> methodDecl) =>
        document.AddMethodModifierAction(methodDecl, SyntaxKind.StaticKeyword, "Add 'static' modifier");

    /// <summary>
    /// Creates a code action that adds a modifier to a method declaration.
    /// </summary>
    public static CodeAction AddMethodModifierAction(
        this Document document,
        ValidSyntax<MethodDeclarationSyntax> methodDecl,
        SyntaxKind modifier,
        string title) =>
        CodeAction.Create(
            title,
            ct => document.ReplaceNode(
                methodDecl.Node,
                AddModifierWithOrdering(methodDecl.Node, modifier),
                ct),
            title);

    /// <summary>
    /// Creates a code action that removes a modifier from a method declaration.
    /// </summary>
    public static CodeAction RemoveMethodModifierAction(
        this Document document,
        ValidSyntax<MethodDeclarationSyntax> methodDecl,
        SyntaxKind modifier,
        string title) =>
        CodeAction.Create(
            title,
            ct =>
            {
                var newModifiers = methodDecl.Node.Modifiers
                    .Where(m => !m.IsKind(modifier));

                var newNode = methodDecl.Node.WithModifiers(SyntaxFactory.TokenList(newModifiers));
                return document.ReplaceNode(methodDecl.Node, newNode, ct);
            },
            title);

    #endregion

    #region Method Rename Helpers

    /// <summary>
    /// Creates a code action that renames a method.
    /// </summary>
    /// <param name="document">The document to modify.</param>
    /// <param name="methodDecl">The validated method declaration syntax.</param>
    /// <param name="newName">The new name for the method.</param>
    /// <param name="title">Optional title. Defaults to "Rename to 'newName'".</param>
    public static CodeAction RenameMethodAction(
        this Document document,
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
    public static CodeAction AddAsyncSuffixAction(this Document document, ValidSyntax<MethodDeclarationSyntax> methodDecl)
    {
        var newName = methodDecl.Node.Identifier.Text + "Async";
        return document.RenameMethodAction(methodDecl, newName, $"Rename to '{newName}'");
    }

    /// <summary>
    /// Creates a code action that removes the 'Async' suffix from a method name.
    /// </summary>
    public static CodeAction RemoveAsyncSuffixAction(this Document document, ValidSyntax<MethodDeclarationSyntax> methodDecl)
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
    public static CodeAction ChangeReturnTypeAction(
        this Document document,
        ValidSyntax<MethodDeclarationSyntax> methodDecl,
        string newReturnType,
        string? title = null)
    {
        title ??= $"Change return type to '{newReturnType}'";

        return CodeAction.Create(
            title,
            ct => document.ReplaceNode(
                methodDecl.Node,
                methodDecl.Node.WithReturnType(SyntaxFactory.ParseTypeName(newReturnType)
                    .WithTrailingTrivia(SyntaxFactory.Space)),
                ct),
            title);
    }

    /// <summary>
    /// Creates a code action that wraps the return type in Task&lt;T&gt;.
    /// </summary>
    public static CodeAction WrapReturnTypeInTaskAction(
        this Document document,
        ValidSyntax<MethodDeclarationSyntax> methodDecl)
    {
        var currentType = methodDecl.Node.ReturnType.ToString().Trim();
        var newType = currentType == "void" ? "Task" : $"Task<{currentType}>";
        return document.ChangeReturnTypeAction(methodDecl, newType, $"Change return type to '{newType}'");
    }

    #endregion

    #region Private Helper Methods

    private static MethodDeclarationSyntax AddModifierWithOrdering(
        MethodDeclarationSyntax node,
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

            // Then static/abstract/sealed/virtual/override/new/readonly
            SyntaxKind.StaticKeyword or
                SyntaxKind.AbstractKeyword or
                SyntaxKind.SealedKeyword or
                SyntaxKind.VirtualKeyword or
                SyntaxKind.OverrideKeyword or
                SyntaxKind.NewKeyword or
                SyntaxKind.ReadOnlyKeyword => 1,

            // Then async/extern
            SyntaxKind.AsyncKeyword or
                SyntaxKind.ExternKeyword => 2,

            // Then partial
            SyntaxKind.PartialKeyword => 3,

            // Then unsafe
            SyntaxKind.UnsafeKeyword => 4,

            _ => 5
        };

    #endregion
}
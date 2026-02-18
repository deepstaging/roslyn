// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.Roslyn;

/// <summary>
/// Code fix action helpers for type declarations (class, struct, interface, record, enum).
/// </summary>
public static class TypeCodeFixActions
{
    #region Type Modifier Helpers

    /// <summary>
    /// Creates a code action that adds the 'partial' modifier to a type declaration.
    /// </summary>
    public static CodeAction AddPartialModifierAction<T>(this Document document, ValidSyntax<T> typeDecl)
        where T : TypeDeclarationSyntax =>
        document.AddModifierAction(typeDecl, SyntaxKind.PartialKeyword, "Add 'partial' modifier");

    /// <summary>
    /// Creates a code action that adds the 'sealed' modifier to a type declaration.
    /// </summary>
    public static CodeAction AddSealedModifierAction<T>(this Document document, ValidSyntax<T> typeDecl)
        where T : TypeDeclarationSyntax =>
        document.AddModifierAction(typeDecl, SyntaxKind.SealedKeyword, "Add 'sealed' modifier");

    /// <summary>
    /// Creates a code action that adds the 'static' modifier to a type declaration.
    /// </summary>
    public static CodeAction AddStaticModifierAction<T>(this Document document, ValidSyntax<T> typeDecl)
        where T : TypeDeclarationSyntax =>
        document.AddModifierAction(typeDecl, SyntaxKind.StaticKeyword, "Add 'static' modifier");

    /// <summary>
    /// Creates a code action that adds the 'abstract' modifier to a type declaration.
    /// </summary>
    public static CodeAction AddAbstractModifierAction<T>(this Document document, ValidSyntax<T> typeDecl)
        where T : TypeDeclarationSyntax =>
        document.AddModifierAction(typeDecl, SyntaxKind.AbstractKeyword, "Add 'abstract' modifier");

    /// <summary>
    /// Creates a code action that adds the 'readonly' modifier to a struct declaration.
    /// </summary>
    public static CodeAction AddReadonlyModifierAction(this Document document, ValidSyntax<StructDeclarationSyntax> structDecl) =>
        document.AddModifierAction(structDecl, SyntaxKind.ReadOnlyKeyword, "Add 'readonly' modifier");

    /// <summary>
    /// Creates a code action that adds a modifier to a type declaration.
    /// </summary>
    /// <param name="document">The document to modify.</param>
    /// <param name="typeDecl">The validated type declaration syntax.</param>
    /// <param name="modifier">The modifier kind to add.</param>
    /// <param name="title">The title for the code action.</param>
    public static CodeAction AddModifierAction<T>(
        this Document document,
        ValidSyntax<T> typeDecl,
        SyntaxKind modifier,
        string title)
        where T : TypeDeclarationSyntax =>
        CodeAction.Create(
            title,
            ct => document.ReplaceNode(
                typeDecl.Node,
                typeDecl.AddModifier(modifier),
                ct),
            title);

    /// <summary>
    /// Creates a code action that removes a modifier from a type declaration.
    /// </summary>
    /// <param name="document">The document to modify.</param>
    /// <param name="typeDecl">The validated type declaration syntax.</param>
    /// <param name="modifier">The modifier kind to remove.</param>
    /// <param name="title">The title for the code action.</param>
    public static CodeAction RemoveModifierAction<T>(
        this Document document,
        ValidSyntax<T> typeDecl,
        SyntaxKind modifier,
        string title)
        where T : TypeDeclarationSyntax =>
        CodeAction.Create(
            title,
            ct => document.ReplaceNode(
                typeDecl.Node,
                typeDecl.RemoveModifier(modifier),
                ct),
            title);

    #endregion

    #region Member Insertion Helpers

    /// <summary>
    /// Creates a code action that adds new members to a type declaration.
    /// </summary>
    /// <param name="document">The document to modify.</param>
    /// <param name="typeDecl">The validated type declaration syntax.</param>
    /// <param name="title">The title for the code action.</param>
    /// <param name="members">The member declarations to insert.</param>
    public static CodeAction AddMembersAction<T>(
        this Document document,
        ValidSyntax<T> typeDecl,
        string title,
        params MemberDeclarationSyntax[] members)
        where T : TypeDeclarationSyntax =>
        CodeAction.Create(
            title,
            ct => AddMembersAsync(document, typeDecl, members, ct),
            title);

    /// <summary>
    /// Creates a code action that adds new members (parsed from source text) to a type declaration.
    /// </summary>
    /// <param name="document">The document to modify.</param>
    /// <param name="typeDecl">The validated type declaration syntax.</param>
    /// <param name="title">The title for the code action.</param>
    /// <param name="memberSource">The C# source text of the members to insert.</param>
    public static CodeAction AddMembersFromSourceAction<T>(
        this Document document,
        ValidSyntax<T> typeDecl,
        string title,
        string memberSource)
        where T : TypeDeclarationSyntax
    {
        var members = SyntaxFactory.ParseCompilationUnit($"class __Temp {{ {memberSource} }}")
            .Members.OfType<TypeDeclarationSyntax>()
            .SelectMany(t => t.Members)
            .ToArray();

        return CodeAction.Create(
            title,
            ct => AddMembersAsync(document, typeDecl, members, ct),
            title);
    }

    #endregion

    #region Type Rename Helpers

    /// <summary>
    /// Creates a code action that renames a type (class, struct, interface, etc.).
    /// </summary>
    /// <param name="document">The document to modify.</param>
    /// <param name="typeDecl">The validated type declaration syntax.</param>
    /// <param name="newName">The new name for the type.</param>
    /// <param name="title">Optional title. Defaults to "Rename to 'newName'".</param>
    public static CodeAction RenameTypeAction<T>(
        this Document document,
        ValidSyntax<T> typeDecl,
        string newName,
        string? title = null)
        where T : TypeDeclarationSyntax
    {
        title ??= $"Rename to '{newName}'";

        return CodeAction.Create(
            title,
            ct => document.ReplaceNode(
                typeDecl.Node,
                typeDecl.Node.WithIdentifier(SyntaxFactory.Identifier(newName)),
                ct),
            title);
    }

    #endregion

    #region Base Type Helpers

    /// <summary>
    /// Creates a code action that adds a base type or interface to a type declaration.
    /// </summary>
    /// <param name="document">The document to modify.</param>
    /// <param name="typeDecl">The validated type declaration syntax.</param>
    /// <param name="baseTypeName">The name of the base type or interface to add.</param>
    public static CodeAction AddBaseTypeAction<T>(
        this Document document,
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
    public static CodeAction AddInterfaceAction<T>(
        this Document document,
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

    #region Private Helper Methods

    private static async Task<Document> AddMembersAsync<T>(
        Document document,
        ValidSyntax<T> typeDecl,
        MemberDeclarationSyntax[] members,
        CancellationToken cancellationToken)
        where T : TypeDeclarationSyntax
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        if (root is null)
            return document;

        var node = typeDecl.Node;
        var newNode = node.AddMembers(members);
        var newRoot = root.ReplaceNode(node, newNode);
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

    #endregion
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for ValidSyntax with type declaration syntax nodes.
/// Provides modifier manipulation for class, struct, interface, and record declarations.
/// </summary>
public static class ValidSyntaxTypeDeclarationExtensions
{
    /// <summary>
    /// Adds a modifier to the type declaration with proper trivia and canonical ordering.
    /// C# modifier order: accessibility → static/abstract/sealed/new/readonly → partial → unsafe
    /// </summary>
    /// <typeparam name="TSyntax">The type of type declaration syntax.</typeparam>
    /// <param name="syntax">The validated syntax node.</param>
    /// <param name="kind">The modifier kind to add.</param>
    /// <returns>A new syntax node with the modifier added in the correct position.</returns>
    public static TSyntax AddModifier<TSyntax>(this ValidSyntax<TSyntax> syntax, SyntaxKind kind)
        where TSyntax : TypeDeclarationSyntax
    {
        var node = syntax.Node;
        if (node.Modifiers.Any(m => m.IsKind(kind)))
            return node;

        var token = SyntaxFactory.Token(kind).WithTrailingTrivia(SyntaxFactory.Space);
        var insertIndex = GetModifierInsertIndex(node.Modifiers, kind);
        var newModifiers = node.Modifiers.Insert(insertIndex, token);
        return (TSyntax)node.WithModifiers(newModifiers);
    }

    private static int GetModifierInsertIndex(SyntaxTokenList modifiers, SyntaxKind kind)
    {
        var priority = GetModifierPriority(kind);

        for (var i = 0; i < modifiers.Count; i++)
        {
            if (GetModifierPriority(modifiers[i].Kind()) > priority)
                return i;
        }

        return modifiers.Count;
    }

    private static int GetModifierPriority(SyntaxKind kind) => kind switch
    {
        // Accessibility modifiers come first
        SyntaxKind.PublicKeyword => 0,
        SyntaxKind.PrivateKeyword => 0,
        SyntaxKind.ProtectedKeyword => 0,
        SyntaxKind.InternalKeyword => 0,

        // Then static/abstract/sealed/virtual/override/new/readonly
        SyntaxKind.StaticKeyword => 1,
        SyntaxKind.AbstractKeyword => 1,
        SyntaxKind.SealedKeyword => 1,
        SyntaxKind.VirtualKeyword => 1,
        SyntaxKind.OverrideKeyword => 1,
        SyntaxKind.NewKeyword => 1,
        SyntaxKind.ReadOnlyKeyword => 1,

        // Then async/extern
        SyntaxKind.AsyncKeyword => 2,
        SyntaxKind.ExternKeyword => 2,

        // Then partial
        SyntaxKind.PartialKeyword => 3,

        // Then unsafe
        SyntaxKind.UnsafeKeyword => 4,

        _ => 5
    };

    /// <summary>
    /// Removes a modifier from the type declaration.
    /// </summary>
    /// <typeparam name="TSyntax">The type of type declaration syntax.</typeparam>
    /// <param name="syntax">The validated syntax node.</param>
    /// <param name="kind">The modifier kind to remove.</param>
    /// <returns>A new syntax node with the modifier removed.</returns>
    public static TSyntax RemoveModifier<TSyntax>(this ValidSyntax<TSyntax> syntax, SyntaxKind kind)
        where TSyntax : TypeDeclarationSyntax
    {
        var node = syntax.Node;
        var modifierToRemove = node.Modifiers.FirstOrDefault(m => m.IsKind(kind));
        if (modifierToRemove == default)
            return node;

        var newModifiers = node.Modifiers.Remove(modifierToRemove);
        return (TSyntax)node.WithModifiers(newModifiers);
    }

    /// <summary>
    /// Checks if the type declaration has a specific modifier.
    /// </summary>
    /// <typeparam name="TSyntax">The type of type declaration syntax.</typeparam>
    /// <param name="syntax">The validated syntax node.</param>
    /// <param name="kind">The modifier kind to check.</param>
    /// <returns>True if the modifier is present.</returns>
    public static bool HasModifier<TSyntax>(this ValidSyntax<TSyntax> syntax, SyntaxKind kind)
        where TSyntax : TypeDeclarationSyntax
    {
        return syntax.Node.Modifiers.Any(m => m.IsKind(kind));
    }

    /// <summary>
    /// Gets a value indicating whether the type is partial.
    /// </summary>
    public static bool IsPartial<TSyntax>(this ValidSyntax<TSyntax> syntax)
        where TSyntax : TypeDeclarationSyntax
    {
        return syntax.HasModifier(SyntaxKind.PartialKeyword);
    }

    /// <summary>
    /// Gets a value indicating whether the type is static.
    /// </summary>
    public static bool IsStatic<TSyntax>(this ValidSyntax<TSyntax> syntax)
        where TSyntax : TypeDeclarationSyntax
    {
        return syntax.HasModifier(SyntaxKind.StaticKeyword);
    }

    /// <summary>
    /// Gets a value indicating whether the type is abstract.
    /// </summary>
    public static bool IsAbstract<TSyntax>(this ValidSyntax<TSyntax> syntax)
        where TSyntax : TypeDeclarationSyntax
    {
        return syntax.HasModifier(SyntaxKind.AbstractKeyword);
    }

    /// <summary>
    /// Gets a value indicating whether the type is sealed.
    /// </summary>
    public static bool IsSealed<TSyntax>(this ValidSyntax<TSyntax> syntax)
        where TSyntax : TypeDeclarationSyntax
    {
        return syntax.HasModifier(SyntaxKind.SealedKeyword);
    }

    /// <summary>
    /// Gets the name of the type declaration.
    /// </summary>
    public static string Name<TSyntax>(this ValidSyntax<TSyntax> syntax)
        where TSyntax : TypeDeclarationSyntax
    {
        return syntax.Node.Identifier.Text;
    }
}

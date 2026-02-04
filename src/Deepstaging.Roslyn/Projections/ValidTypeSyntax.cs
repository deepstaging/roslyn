// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Deepstaging.Roslyn;

/// <summary>
/// A validated type declaration syntax node (class, struct, interface, record) with guaranteed non-null access.
/// Provides fluent projections for common type declaration properties and transformations.
/// Create via ValidTypeSyntax.From() or by casting from ValidSyntax&lt;TypeDeclarationSyntax&gt;.
/// </summary>
/// <typeparam name="TSyntax">The specific type of type declaration syntax.</typeparam>
public readonly struct ValidTypeSyntax<TSyntax> : IProjection<TSyntax>
    where TSyntax : TypeDeclarationSyntax
{
    private readonly TSyntax _node;

    private ValidTypeSyntax(TSyntax node)
    {
        _node = node ?? throw new ArgumentNullException(nameof(node));
    }

    #region Factory Methods

    /// <summary>
    /// Creates a validated type syntax from a non-null type declaration.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if node is null.</exception>
    public static ValidTypeSyntax<TSyntax> From(TSyntax node)
    {
        return new ValidTypeSyntax<TSyntax>(node);
    }

    /// <summary>
    /// Attempts to create a validated type syntax from a nullable type declaration.
    /// Returns null if the node is null.
    /// </summary>
    public static ValidTypeSyntax<TSyntax>? TryFrom(TSyntax? node)
    {
        return node != null ? new ValidTypeSyntax<TSyntax>(node) : null;
    }

    /// <summary>
    /// Creates a validated type syntax from a ValidSyntax wrapper.
    /// </summary>
    public static ValidTypeSyntax<TSyntax> From(ValidSyntax<TSyntax> valid)
    {
        return new ValidTypeSyntax<TSyntax>(valid.Node);
    }

    #endregion

    #region Core Properties

    /// <summary>
    /// Gets the underlying non-null type declaration syntax node.
    /// </summary>
    public TSyntax Node => _node;

    #endregion

    #region IProjection Implementation

    /// <summary>
    /// Always returns true for validated syntax nodes.
    /// </summary>
    public bool HasValue => true;

    /// <summary>
    /// Always returns false for validated syntax nodes.
    /// </summary>
    public bool IsEmpty => false;

    /// <summary>
    /// Returns the guaranteed non-null syntax node.
    /// </summary>
    public TSyntax OrThrow(string? message = null) => _node;

    /// <summary>
    /// Returns the guaranteed non-null syntax node.
    /// </summary>
    public TSyntax OrNull() => _node;

    #endregion

    #region Identity Properties

    /// <summary>
    /// Gets the name of the type.
    /// </summary>
    public string Name => _node.Identifier.Text;

    /// <summary>
    /// Gets the identifier token of the type.
    /// </summary>
    public SyntaxToken Identifier => _node.Identifier;

    /// <summary>
    /// Gets the keyword token (class, struct, interface, record).
    /// </summary>
    public SyntaxToken Keyword => _node.Keyword;

    #endregion

    #region Modifier Properties (Read)

    /// <summary>
    /// Gets the modifiers of the type declaration.
    /// </summary>
    public SyntaxTokenList Modifiers => _node.Modifiers;

    /// <summary>
    /// Checks if the type has a specific modifier.
    /// </summary>
    public bool HasModifier(SyntaxKind kind) => _node.Modifiers.Any(m => m.IsKind(kind));

    /// <summary>
    /// Gets a value indicating whether the type is partial.
    /// </summary>
    public bool IsPartial => HasModifier(SyntaxKind.PartialKeyword);

    /// <summary>
    /// Gets a value indicating whether the type is static.
    /// </summary>
    public bool IsStatic => HasModifier(SyntaxKind.StaticKeyword);

    /// <summary>
    /// Gets a value indicating whether the type is abstract.
    /// </summary>
    public bool IsAbstract => HasModifier(SyntaxKind.AbstractKeyword);

    /// <summary>
    /// Gets a value indicating whether the type is sealed.
    /// </summary>
    public bool IsSealed => HasModifier(SyntaxKind.SealedKeyword);

    /// <summary>
    /// Gets a value indicating whether the type is public.
    /// </summary>
    public bool IsPublic => HasModifier(SyntaxKind.PublicKeyword);

    /// <summary>
    /// Gets a value indicating whether the type is internal.
    /// </summary>
    public bool IsInternal => HasModifier(SyntaxKind.InternalKeyword);

    /// <summary>
    /// Gets a value indicating whether the type is private.
    /// </summary>
    public bool IsPrivate => HasModifier(SyntaxKind.PrivateKeyword);

    /// <summary>
    /// Gets a value indicating whether the type is protected.
    /// </summary>
    public bool IsProtected => HasModifier(SyntaxKind.ProtectedKeyword);

    /// <summary>
    /// Gets a value indicating whether the type is readonly (for structs).
    /// </summary>
    public bool IsReadOnly => HasModifier(SyntaxKind.ReadOnlyKeyword);

    /// <summary>
    /// Gets a value indicating whether the type is a file-scoped type.
    /// </summary>
    public bool IsFile => HasModifier(SyntaxKind.FileKeyword);

    #endregion

    #region Modifier Transformations

    /// <summary>
    /// Adds a modifier to the type declaration with proper trivia.
    /// </summary>
    public TSyntax AddModifier(SyntaxKind kind)
    {
        if (HasModifier(kind))
            return _node;

        var token = SyntaxFactory.Token(kind).WithTrailingTrivia(SyntaxFactory.Space);
        var newModifiers = _node.Modifiers.Add(token);
        return (TSyntax)_node.WithModifiers(newModifiers);
    }

    /// <summary>
    /// Removes a modifier from the type declaration.
    /// </summary>
    public TSyntax RemoveModifier(SyntaxKind kind)
    {
        var modifierToRemove = _node.Modifiers.FirstOrDefault(m => m.IsKind(kind));
        if (modifierToRemove == default)
            return _node;

        var newModifiers = _node.Modifiers.Remove(modifierToRemove);
        return (TSyntax)_node.WithModifiers(newModifiers);
    }

    /// <summary>
    /// Replaces all modifiers with the specified modifiers.
    /// </summary>
    public TSyntax WithModifiers(SyntaxTokenList modifiers)
    {
        return (TSyntax)_node.WithModifiers(modifiers);
    }

    #endregion

    #region Structure Properties

    /// <summary>
    /// Gets the base list (base types and interfaces) of the type declaration.
    /// </summary>
    public BaseListSyntax? BaseList => _node.BaseList;

    /// <summary>
    /// Gets the type parameter list for generic types.
    /// </summary>
    public TypeParameterListSyntax? TypeParameterList => _node.TypeParameterList;

    /// <summary>
    /// Gets the constraint clauses for generic types.
    /// </summary>
    public SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses => _node.ConstraintClauses;

    /// <summary>
    /// Gets the attribute lists applied to the type.
    /// </summary>
    public SyntaxList<AttributeListSyntax> AttributeLists => _node.AttributeLists;

    /// <summary>
    /// Gets the members declared in the type.
    /// </summary>
    public SyntaxList<MemberDeclarationSyntax> Members => _node.Members;

    /// <summary>
    /// Gets a value indicating whether the type has any base types or interfaces.
    /// </summary>
    public bool HasBaseList => _node.BaseList != null && _node.BaseList.Types.Count > 0;

    /// <summary>
    /// Gets a value indicating whether the type is generic.
    /// </summary>
    public bool IsGeneric => _node.TypeParameterList != null && _node.TypeParameterList.Parameters.Count > 0;

    /// <summary>
    /// Gets the arity (number of type parameters) of the type.
    /// </summary>
    public int Arity => _node.TypeParameterList?.Parameters.Count ?? 0;

    #endregion

    #region Location Properties

    /// <summary>
    /// Gets the location of the type declaration.
    /// </summary>
    public Location Location => _node.GetLocation();

    /// <summary>
    /// Gets the text span of the type declaration.
    /// </summary>
    public TextSpan Span => _node.Span;

    /// <summary>
    /// Gets the location of the identifier (for diagnostics).
    /// </summary>
    public Location IdentifierLocation => _node.Identifier.GetLocation();

    #endregion

    #region Navigation

    /// <summary>
    /// Gets the containing type if this is a nested type.
    /// </summary>
    public OptionalSyntax<TypeDeclarationSyntax> ContainingType
    {
        get
        {
            var parent = _node.Parent as TypeDeclarationSyntax;
            return OptionalSyntax<TypeDeclarationSyntax>.FromNullable(parent);
        }
    }

    /// <summary>
    /// Gets the containing namespace declaration.
    /// </summary>
    public OptionalSyntax<BaseNamespaceDeclarationSyntax> ContainingNamespace
    {
        get
        {
            var ns = _node.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
            return OptionalSyntax<BaseNamespaceDeclarationSyntax>.FromNullable(ns);
        }
    }

    /// <summary>
    /// Gets all nested types declared within this type.
    /// </summary>
    public IEnumerable<ValidTypeSyntax<TypeDeclarationSyntax>> NestedTypes
    {
        get
        {
            foreach (var member in _node.Members.OfType<TypeDeclarationSyntax>())
                yield return new ValidTypeSyntax<TypeDeclarationSyntax>(member);
        }
    }

    #endregion

    #region Member Access

    /// <summary>
    /// Gets all method declarations in this type.
    /// </summary>
    public IEnumerable<MethodDeclarationSyntax> Methods => _node.Members.OfType<MethodDeclarationSyntax>();

    /// <summary>
    /// Gets all property declarations in this type.
    /// </summary>
    public IEnumerable<PropertyDeclarationSyntax> Properties => _node.Members.OfType<PropertyDeclarationSyntax>();

    /// <summary>
    /// Gets all field declarations in this type.
    /// </summary>
    public IEnumerable<FieldDeclarationSyntax> Fields => _node.Members.OfType<FieldDeclarationSyntax>();

    /// <summary>
    /// Gets all constructor declarations in this type.
    /// </summary>
    public IEnumerable<ConstructorDeclarationSyntax> Constructors => _node.Members.OfType<ConstructorDeclarationSyntax>();

    #endregion

    #region Equality

    /// <summary>
    /// Checks if the validated type syntax equals the specified syntax node.
    /// </summary>
    public bool Equals(TSyntax? other)
    {
        return other != null && _node.IsEquivalentTo(other);
    }

    /// <summary>
    /// Enables equality checks: validated == node
    /// </summary>
    public static bool operator ==(ValidTypeSyntax<TSyntax> left, TSyntax? right) => left.Equals(right);

    /// <summary>
    /// Enables inequality checks: validated != node
    /// </summary>
    public static bool operator !=(ValidTypeSyntax<TSyntax> left, TSyntax? right) => !left.Equals(right);

    /// <summary>
    /// Determines whether the current instance equals the specified object (always returns false).
    /// </summary>
    public override bool Equals(object? obj) => false;

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    public override int GetHashCode() => _node.GetHashCode();

    #endregion

    #region Implicit Conversions

    /// <summary>
    /// Implicitly converts to the underlying syntax node for seamless interop with Roslyn APIs.
    /// </summary>
    public static implicit operator TSyntax(ValidTypeSyntax<TSyntax> valid) => valid._node;

    /// <summary>
    /// Implicitly converts to ValidSyntax for generic syntax operations.
    /// </summary>
    public static implicit operator ValidSyntax<TSyntax>(ValidTypeSyntax<TSyntax> valid) => ValidSyntax<TSyntax>.From(valid._node);

    #endregion
}

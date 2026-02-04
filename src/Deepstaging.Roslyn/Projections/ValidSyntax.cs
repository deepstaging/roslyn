// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Deepstaging.Roslyn;

/// <summary>
/// A validated Roslyn syntax node where the node is guaranteed non-null.
/// Provides a non-nullable API surface, eliminating the need for null checks on core properties.
/// Create via OptionalSyntax.Validate() or ValidSyntax.From().
/// </summary>
/// <typeparam name="TSyntax">The type of Roslyn syntax node.</typeparam>
public readonly struct ValidSyntax<TSyntax> : IProjection<TSyntax>
    where TSyntax : SyntaxNode
{
    private readonly TSyntax _node;

    private ValidSyntax(TSyntax node)
    {
        _node = node ?? throw new ArgumentNullException(nameof(node));
    }

    #region Factory Methods

    /// <summary>
    /// Creates a validated projection from a non-null syntax node.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if node is null.</exception>
    public static ValidSyntax<TSyntax> From(TSyntax node)
    {
        return new ValidSyntax<TSyntax>(node);
    }

    /// <summary>
    /// Attempts to create a validated projection from a nullable syntax node.
    /// Returns null if the node is null.
    /// </summary>
    public static ValidSyntax<TSyntax>? TryFrom(TSyntax? node)
    {
        return node != null ? new ValidSyntax<TSyntax>(node) : null;
    }

    #endregion

    #region Core Properties

    /// <summary>
    /// Gets the underlying non-null syntax node.
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
    public TSyntax OrThrow(string? message = null)
    {
        return _node;
    }

    /// <summary>
    /// Returns the guaranteed non-null syntax node.
    /// </summary>
    public TSyntax OrNull()
    {
        return _node;
    }

    #endregion

    #region Projection Operations (Map, Filter, Cast)

    /// <summary>
    /// Maps the validated syntax node to a different type.
    /// </summary>
    public TResult Map<TResult>(Func<TSyntax, TResult> mapper)
    {
        return mapper(_node);
    }

    /// <summary>
    /// Maps the validated syntax node to another validated syntax node.
    /// </summary>
    public ValidSyntax<TDerived> MapTo<TDerived>(Func<TSyntax, TDerived> mapper)
        where TDerived : SyntaxNode
    {
        return new ValidSyntax<TDerived>(mapper(_node));
    }

    /// <summary>
    /// Filters the syntax node based on a predicate.
    /// Returns null if predicate fails.
    /// </summary>
    public ValidSyntax<TSyntax>? Where(Func<TSyntax, bool> predicate)
    {
        return predicate(_node) ? this : null;
    }

    /// <summary>
    /// Attempts to cast to a derived syntax type.
    /// Returns null if cast fails.
    /// </summary>
    public ValidSyntax<TDerived>? OfType<TDerived>() where TDerived : SyntaxNode
    {
        return _node is TDerived derived ? new ValidSyntax<TDerived>(derived) : null;
    }

    /// <summary>
    /// Executes an action with the syntax node.
    /// </summary>
    public ValidSyntax<TSyntax> Do(Action<TSyntax> action)
    {
        action(_node);
        return this;
    }

    #endregion

    #region Navigation

    /// <summary>
    /// Gets the parent of the syntax node as the specified type.
    /// </summary>
    public OptionalSyntax<TParent> Parent<TParent>() where TParent : SyntaxNode
    {
        return _node.Parent is TParent parent
            ? OptionalSyntax<TParent>.WithValue(parent)
            : OptionalSyntax<TParent>.Empty();
    }

    /// <summary>
    /// Finds the first ancestor of the specified type.
    /// </summary>
    public OptionalSyntax<TAncestor> Ancestor<TAncestor>() where TAncestor : SyntaxNode
    {
        var ancestor = _node.Ancestors().OfType<TAncestor>().FirstOrDefault();
        return OptionalSyntax<TAncestor>.FromNullable(ancestor);
    }

    /// <summary>
    /// Gets all ancestors of the specified type.
    /// </summary>
    public IEnumerable<ValidSyntax<TAncestor>> Ancestors<TAncestor>() where TAncestor : SyntaxNode
    {
        foreach (var ancestor in _node.Ancestors().OfType<TAncestor>())
            yield return new ValidSyntax<TAncestor>(ancestor);
    }

    /// <summary>
    /// Finds the first descendant of the specified type.
    /// </summary>
    public OptionalSyntax<TDescendant> Descendant<TDescendant>() where TDescendant : SyntaxNode
    {
        var descendant = _node.DescendantNodes().OfType<TDescendant>().FirstOrDefault();
        return OptionalSyntax<TDescendant>.FromNullable(descendant);
    }

    /// <summary>
    /// Gets all descendants of the specified type.
    /// </summary>
    public IEnumerable<ValidSyntax<TDescendant>> Descendants<TDescendant>() where TDescendant : SyntaxNode
    {
        foreach (var descendant in _node.DescendantNodes().OfType<TDescendant>())
            yield return new ValidSyntax<TDescendant>(descendant);
    }

    #endregion

    #region Location Properties

    /// <summary>
    /// Gets the location of the syntax node.
    /// </summary>
    public Location Location => _node.GetLocation();

    /// <summary>
    /// Gets the text span of the syntax node.
    /// </summary>
    public TextSpan Span => _node.Span;

    /// <summary>
    /// Gets the full text span of the syntax node (including trivia).
    /// </summary>
    public TextSpan FullSpan => _node.FullSpan;

    /// <summary>
    /// Gets the syntax tree containing this node.
    /// </summary>
    public SyntaxTree SyntaxTree => _node.SyntaxTree;

    #endregion

    #region Text Properties

    /// <summary>
    /// Gets the full text of the syntax node including trivia.
    /// </summary>
    public string FullText => _node.ToFullString();

    /// <summary>
    /// Gets the text of the syntax node without trivia.
    /// </summary>
    public string Text => _node.ToString();

    #endregion

    #region Trivia

    /// <summary>
    /// Gets the leading trivia of the syntax node.
    /// </summary>
    public SyntaxTriviaList LeadingTrivia => _node.GetLeadingTrivia();

    /// <summary>
    /// Gets the trailing trivia of the syntax node.
    /// </summary>
    public SyntaxTriviaList TrailingTrivia => _node.GetTrailingTrivia();

    #endregion

    #region Equality

    /// <summary>
    /// Checks if the validated syntax node equals the specified syntax node.
    /// </summary>
    public bool Equals(TSyntax? other)
    {
        return other != null && _node.IsEquivalentTo(other);
    }

    /// <summary>
    /// Enables equality checks: validated == node
    /// </summary>
    public static bool operator ==(ValidSyntax<TSyntax> left, TSyntax? right) => left.Equals(right);

    /// <summary>
    /// Enables inequality checks: validated != node
    /// </summary>
    public static bool operator !=(ValidSyntax<TSyntax> left, TSyntax? right) => !left.Equals(right);

    /// <summary>
    /// Determines whether the current instance equals the specified object (always returns false).
    /// </summary>
    public override bool Equals(object? obj) => false;

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    public override int GetHashCode() => _node.GetHashCode();

    #endregion

    #region Implicit Conversion

    /// <summary>
    /// Implicitly converts to the underlying syntax node for seamless interop with Roslyn APIs.
    /// </summary>
    public static implicit operator TSyntax(ValidSyntax<TSyntax> valid) => valid._node;

    #endregion
}

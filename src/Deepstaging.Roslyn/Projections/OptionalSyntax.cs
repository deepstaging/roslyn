// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Deepstaging.Roslyn;

/// <summary>
/// An optional Roslyn syntax node that may or may not contain a value.
/// Provides a fluent API for querying and transforming syntax nodes with null-safety.
/// Use Validate() or IsNotValid() to get a ValidSyntax&lt;T&gt; with guaranteed non-null access.
/// </summary>
/// <typeparam name="TSyntax">The type of Roslyn syntax node.</typeparam>
public readonly struct OptionalSyntax<TSyntax> : IValidatableProjection<TSyntax, ValidSyntax<TSyntax>>
    where TSyntax : SyntaxNode
{
    private readonly TSyntax? _node;

    private OptionalSyntax(TSyntax? node)
    {
        _node = node;
    }

    #region Factory Methods

    /// <summary>
    /// Creates an optional syntax with a value.
    /// </summary>
    public static OptionalSyntax<TSyntax> WithValue(TSyntax node) => new(node);

    /// <summary>
    /// Creates an empty optional syntax without a value.
    /// </summary>
    public static OptionalSyntax<TSyntax> Empty() => new(null);

    /// <summary>
    /// Creates an optional syntax from a nullable syntax reference.
    /// </summary>
    public static OptionalSyntax<TSyntax> FromNullable(TSyntax? node) => node != null ? WithValue(node) : Empty();

    #endregion

    #region Core Properties

    /// <summary>
    /// Gets the underlying syntax node, which may be null.
    /// </summary>
    public TSyntax? Node => _node;

    #endregion

    #region IValidatableProjection Implementation

    /// <summary>
    /// Gets a value indicating whether the syntax node is present.
    /// </summary>
    public bool HasValue => _node != null;

    /// <summary>
    /// Gets a value indicating whether the syntax node is absent.
    /// </summary>
    public bool IsEmpty => _node == null;

    /// <summary>
    /// Returns the syntax node or throws an exception if absent.
    /// </summary>
    public TSyntax OrThrow(string? message = null)
    {
        return _node ?? throw new InvalidOperationException(message ?? "Syntax node is not present.");
    }

    /// <summary>
    /// Returns the syntax node or throws an exception with a lazily-computed message if absent.
    /// </summary>
    public TSyntax OrThrow(Func<string> messageFactory)
    {
        return _node ?? throw new InvalidOperationException(messageFactory());
    }

    /// <summary>
    /// Returns the syntax node or null if absent.
    /// </summary>
    public TSyntax? OrNull() => _node;

    /// <summary>
    /// Validates the optional syntax to a ValidSyntax with guaranteed non-null access.
    /// </summary>
    public ValidSyntax<TSyntax>? Validate()
    {
        return _node != null ? ValidSyntax<TSyntax>.From(_node) : null;
    }

    /// <summary>
    /// Validates the optional syntax or throws an exception if absent.
    /// </summary>
    public ValidSyntax<TSyntax> ValidateOrThrow(string? message = null)
    {
        return _node != null
            ? ValidSyntax<TSyntax>.From(_node)
            : throw new InvalidOperationException(message ?? "Cannot validate an empty syntax projection.");
    }

    /// <summary>
    /// Attempts to validate the optional syntax. Returns true if successful (non-null).
    /// </summary>
    public bool TryValidate(out ValidSyntax<TSyntax> validated)
    {
        if (_node != null)
        {
            validated = ValidSyntax<TSyntax>.From(_node);
            return true;
        }

        validated = default;
        return false;
    }

    /// <summary>
    /// Checks if the optional syntax is not valid (empty). Returns true if invalid.
    /// </summary>
    public bool IsNotValid(out ValidSyntax<TSyntax> validated) => !IsValid(out validated);

    /// <summary>
    /// Checks if the optional syntax is valid (has value). Returns true if valid.
    /// </summary>
    public bool IsValid(out ValidSyntax<TSyntax> validated) => TryValidate(out validated);

    #endregion

    #region Projection Operations (Map, Filter, Cast)

    /// <summary>
    /// Maps the syntax node to a different type using the provided function.
    /// </summary>
    public OptionalValue<TResult> Map<TResult>(Func<ValidSyntax<TSyntax>, TResult> mapper)
    {
        return _node != null
            ? OptionalValue<TResult>.WithValue(mapper(ValidSyntax<TSyntax>.From(_node)))
            : OptionalValue<TResult>.Empty();
    }

    /// <summary>
    /// Alias for Map. Maps the syntax node to a different type.
    /// </summary>
    public OptionalValue<TResult> Select<TResult>(Func<ValidSyntax<TSyntax>, TResult> selector) => Map(selector);

    /// <summary>
    /// Filters the syntax node based on a predicate.
    /// </summary>
    public OptionalSyntax<TSyntax> Where(Func<TSyntax, bool> predicate)
    {
        return _node != null && predicate(_node) ? this : Empty();
    }

    /// <summary>
    /// Attempts to cast the syntax node to a derived syntax type.
    /// </summary>
    public OptionalSyntax<TDerived> OfType<TDerived>() where TDerived : SyntaxNode
    {
        return _node is TDerived derived
            ? OptionalSyntax<TDerived>.WithValue(derived)
            : OptionalSyntax<TDerived>.Empty();
    }

    #endregion

    #region Navigation

    /// <summary>
    /// Gets the parent of the syntax node as the specified type.
    /// </summary>
    public OptionalSyntax<TParent> Parent<TParent>() where TParent : SyntaxNode
    {
        return _node?.Parent is TParent parent
            ? OptionalSyntax<TParent>.WithValue(parent)
            : OptionalSyntax<TParent>.Empty();
    }

    /// <summary>
    /// Finds the first ancestor of the specified type.
    /// </summary>
    public OptionalSyntax<TAncestor> Ancestor<TAncestor>() where TAncestor : SyntaxNode
    {
        var ancestor = _node?.Ancestors().OfType<TAncestor>().FirstOrDefault();
        return OptionalSyntax<TAncestor>.FromNullable(ancestor);
    }

    /// <summary>
    /// Gets all ancestors of the specified type.
    /// </summary>
    public IEnumerable<ValidSyntax<TAncestor>> Ancestors<TAncestor>() where TAncestor : SyntaxNode
    {
        if (_node == null) yield break;

        foreach (var ancestor in _node.Ancestors().OfType<TAncestor>())
            yield return ValidSyntax<TAncestor>.From(ancestor);
    }

    #endregion

    #region Location Properties

    /// <summary>
    /// Gets the location of the syntax node, or Location.None if absent.
    /// </summary>
    public Location Location => _node?.GetLocation() ?? Location.None;

    /// <summary>
    /// Gets the text span of the syntax node.
    /// </summary>
    public TextSpan? Span => _node?.Span;

    /// <summary>
    /// Gets the full text span of the syntax node (including trivia).
    /// </summary>
    public TextSpan? FullSpan => _node?.FullSpan;

    #endregion

    #region Equality

    /// <summary>
    /// Checks if the syntax node equals another syntax node.
    /// </summary>
    public bool Equals(TSyntax? other)
    {
        return _node != null && other != null && _node.IsEquivalentTo(other);
    }

    /// <summary>
    /// Enables equality checks: optional == node
    /// </summary>
    public static bool operator ==(OptionalSyntax<TSyntax> left, TSyntax? right) => left.Equals(right);

    /// <summary>
    /// Enables inequality checks: optional != node
    /// </summary>
    public static bool operator !=(OptionalSyntax<TSyntax> left, TSyntax? right) => !left.Equals(right);

    /// <summary>
    /// Determines whether the current instance equals the specified object (always returns false).
    /// </summary>
    public override bool Equals(object? obj) => false;

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    public override int GetHashCode() => _node?.GetHashCode() ?? 0;

    #endregion

    #region Utility Methods

    /// <summary>
    /// Executes an action if the syntax node is present.
    /// </summary>
    public OptionalSyntax<TSyntax> Do(Action<TSyntax> action)
    {
        if (_node != null)
            action(_node);
        return this;
    }

    /// <summary>
    /// Pattern matching with discriminated union semantics.
    /// </summary>
    public void Match(Action<TSyntax> whenPresent, Action whenEmpty)
    {
        if (_node != null)
            whenPresent(_node);
        else
            whenEmpty();
    }

    /// <summary>
    /// Pattern matching that returns a value.
    /// </summary>
    public TResult Match<TResult>(Func<TSyntax, TResult> whenPresent, Func<TResult> whenEmpty)
    {
        return _node != null ? whenPresent(_node) : whenEmpty();
    }

    #endregion
}

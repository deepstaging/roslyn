// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// A type-safe wrapper representing an <c>EqualityComparer&lt;T&gt;</c> type reference.
/// Carries the compared type so expression builders can produce
/// <c>EqualityComparer&lt;T&gt;.Default.Equals(a, b)</c> without losing the generic argument.
/// </summary>
public readonly record struct EqualityComparerTypeRef
{
    /// <summary>Gets the type being compared (e.g., <c>"string"</c>).</summary>
    public TypeRef ComparedType { get; }

    /// <summary>Creates an <c>EqualityComparerTypeRef</c> for the given compared type.</summary>
    public EqualityComparerTypeRef(TypeRef comparedType) => ComparedType = comparedType;

    /// <summary>Gets the globally qualified <c>EqualityComparer&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Collections.Generic.EqualityComparer<{ComparedType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(EqualityComparerTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(EqualityComparerTypeRef self) =>
        self.ToString();
}
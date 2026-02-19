// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// A type-safe wrapper representing a <c>Comparer&lt;T&gt;</c> type reference.
/// Carries the compared type so expression builders can produce
/// <c>Comparer&lt;T&gt;.Default.Compare(a, b)</c> without losing the generic argument.
/// </summary>
public readonly record struct ComparerTypeRef
{
    /// <summary>Gets the type being compared (e.g., <c>"int"</c>).</summary>
    public TypeRef ComparedType { get; }

    /// <summary>Creates a <c>ComparerTypeRef</c> for the given compared type.</summary>
    public ComparerTypeRef(TypeRef comparedType) => ComparedType = comparedType;

    /// <summary>Gets the globally qualified <c>Comparer&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Collections.Generic.Comparer<{ComparedType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(ComparerTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(ComparerTypeRef self) =>
        self.ToString();
}
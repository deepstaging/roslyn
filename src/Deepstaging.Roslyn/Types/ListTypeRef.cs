// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// A type-safe wrapper representing a <c>List&lt;T&gt;</c> type reference.
/// Carries the element type for typed expression building.
/// </summary>
public readonly record struct ListTypeRef
{
    /// <summary>Gets the element type (e.g., <c>"string"</c>).</summary>
    public TypeRef ElementType { get; }

    /// <summary>Creates a <c>ListTypeRef</c> for the given element type.</summary>
    public ListTypeRef(TypeRef elementType) => ElementType = elementType;

    /// <summary>Gets the globally qualified <c>List&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Collections.Generic.List<{ElementType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(ListTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(ListTypeRef self) =>
        self.ToString();
}
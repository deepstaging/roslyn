// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// A type-safe wrapper representing a <c>HashSet&lt;T&gt;</c> type reference.
/// Carries the element type for typed expression building.
/// </summary>
public readonly record struct HashSetTypeRef
{
    /// <summary>Gets the element type (e.g., <c>"int"</c>).</summary>
    public TypeRef ElementType { get; }

    /// <summary>Creates a <c>HashSetTypeRef</c> for the given element type.</summary>
    public HashSetTypeRef(TypeRef elementType) => ElementType = elementType;

    /// <summary>Gets the globally qualified <c>HashSet&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Collections.Generic.HashSet<{ElementType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(HashSetTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(HashSetTypeRef self) =>
        self.ToString();
}
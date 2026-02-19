// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Types;

/// <summary>
/// A type-safe wrapper representing a TypeScript <c>Set&lt;T&gt;</c> type reference.
/// Carries the element type for typed expression building.
/// </summary>
public readonly record struct TsSetTypeRef
{
    /// <summary>Gets the element type (e.g., <c>"string"</c>).</summary>
    public TsTypeRef ElementType { get; }

    /// <summary>Creates a <c>TsSetTypeRef</c> for the given element type.</summary>
    /// <param name="elementType">The type of elements in the set.</param>
    public TsSetTypeRef(TsTypeRef elementType) => ElementType = elementType;

    /// <summary>Gets the <c>Set&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"Set<{ElementType.Value}>";

    /// <summary>Implicitly converts to <see cref="TsTypeRef"/> for use in type declarations.</summary>
    public static implicit operator TsTypeRef(TsSetTypeRef self) =>
        TsTypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TsSetTypeRef self) =>
        self.ToString();
}

// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Types;

/// <summary>
/// A type-safe wrapper representing a TypeScript <c>T[]</c> array type reference.
/// Carries the element type for typed expression building.
/// </summary>
public readonly record struct TsArrayTypeRef
{
    /// <summary>Gets the element type (e.g., <c>"string"</c>).</summary>
    public TsTypeRef ElementType { get; }

    /// <summary>Creates a <c>TsArrayTypeRef</c> for the given element type.</summary>
    /// <param name="elementType">The type of elements in the array.</param>
    public TsArrayTypeRef(TsTypeRef elementType) => ElementType = elementType;

    /// <summary>Gets the <c>T[]</c> type string.</summary>
    public override string ToString() =>
        $"{ElementType.Value}[]";

    /// <summary>Implicitly converts to <see cref="TsTypeRef"/> for use in type declarations.</summary>
    public static implicit operator TsTypeRef(TsArrayTypeRef self) =>
        TsTypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TsArrayTypeRef self) =>
        self.ToString();
}

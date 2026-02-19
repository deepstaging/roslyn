// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Types;

/// <summary>
/// A type-safe wrapper representing a TypeScript nullable type: <c>T | null</c>.
/// Carries the inner type for typed expression building.
/// </summary>
public readonly record struct TsNullableTypeRef
{
    /// <summary>Gets the inner type before the null union (e.g., <c>"string"</c>).</summary>
    public TsTypeRef InnerType { get; }

    /// <summary>Creates a <c>TsNullableTypeRef</c> for the given inner type.</summary>
    /// <param name="innerType">The type to make nullable.</param>
    public TsNullableTypeRef(TsTypeRef innerType) => InnerType = innerType;

    /// <summary>Gets the <c>T | null</c> type string.</summary>
    public override string ToString() =>
        $"{InnerType.Value} | null";

    /// <summary>Implicitly converts to <see cref="TsTypeRef"/> for use in type declarations.</summary>
    public static implicit operator TsTypeRef(TsNullableTypeRef self) =>
        TsTypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TsNullableTypeRef self) =>
        self.ToString();
}

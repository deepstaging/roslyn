// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Types;

/// <summary>
/// A type-safe wrapper representing a TypeScript <c>Map&lt;K, V&gt;</c> type reference.
/// Carries the key and value types for typed expression building.
/// </summary>
public readonly record struct TsMapTypeRef
{
    /// <summary>Gets the key type (e.g., <c>"string"</c>).</summary>
    public TsTypeRef KeyType { get; }

    /// <summary>Gets the value type (e.g., <c>"number"</c>).</summary>
    public TsTypeRef ValueType { get; }

    /// <summary>Creates a <c>TsMapTypeRef</c> for the given key and value types.</summary>
    /// <param name="keyType">The type of keys in the map.</param>
    /// <param name="valueType">The type of values in the map.</param>
    public TsMapTypeRef(TsTypeRef keyType, TsTypeRef valueType)
    {
        KeyType = keyType;
        ValueType = valueType;
    }

    /// <summary>Gets the <c>Map&lt;K, V&gt;</c> type string.</summary>
    public override string ToString() =>
        $"Map<{KeyType.Value}, {ValueType.Value}>";

    /// <summary>Implicitly converts to <see cref="TsTypeRef"/> for use in type declarations.</summary>
    public static implicit operator TsTypeRef(TsMapTypeRef self) =>
        TsTypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TsMapTypeRef self) =>
        self.ToString();
}

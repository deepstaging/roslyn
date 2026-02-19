// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Types;

/// <summary>
/// A type-safe wrapper representing a TypeScript <c>Record&lt;K, V&gt;</c> utility type reference.
/// Carries the key and value types for typed expression building.
/// </summary>
public readonly record struct TsRecordTypeRef
{
    /// <summary>Gets the key type (e.g., <c>"string"</c>).</summary>
    public TsTypeRef KeyType { get; }

    /// <summary>Gets the value type (e.g., <c>"number"</c>).</summary>
    public TsTypeRef ValueType { get; }

    /// <summary>Creates a <c>TsRecordTypeRef</c> for the given key and value types.</summary>
    /// <param name="keyType">The type of keys in the record.</param>
    /// <param name="valueType">The type of values in the record.</param>
    public TsRecordTypeRef(TsTypeRef keyType, TsTypeRef valueType)
    {
        KeyType = keyType;
        ValueType = valueType;
    }

    /// <summary>Gets the <c>Record&lt;K, V&gt;</c> type string.</summary>
    public override string ToString() =>
        $"Record<{KeyType.Value}, {ValueType.Value}>";

    /// <summary>Implicitly converts to <see cref="TsTypeRef"/> for use in type declarations.</summary>
    public static implicit operator TsTypeRef(TsRecordTypeRef self) =>
        TsTypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TsRecordTypeRef self) =>
        self.ToString();
}

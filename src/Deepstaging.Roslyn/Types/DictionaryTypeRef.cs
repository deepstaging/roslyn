// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// A type-safe wrapper representing a <c>Dictionary&lt;TKey, TValue&gt;</c> type reference.
/// Carries the key and value types for typed expression building.
/// </summary>
public readonly record struct DictionaryTypeRef
{
    /// <summary>Gets the key type (e.g., <c>"string"</c>).</summary>
    public TypeRef KeyType { get; }

    /// <summary>Gets the value type (e.g., <c>"int"</c>).</summary>
    public TypeRef ValueType { get; }

    /// <summary>Creates a <c>DictionaryTypeRef</c> for the given key and value types.</summary>
    public DictionaryTypeRef(TypeRef keyType, TypeRef valueType)
    {
        KeyType = keyType;
        ValueType = valueType;
    }

    /// <summary>Gets the globally qualified <c>Dictionary&lt;TKey, TValue&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Collections.Generic.Dictionary<{KeyType.Value}, {ValueType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(DictionaryTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(DictionaryTypeRef self) =>
        self.ToString();
}
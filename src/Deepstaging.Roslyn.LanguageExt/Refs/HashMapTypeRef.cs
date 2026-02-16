// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Refs;

using Emit.Refs;

/// <summary>
/// A type-safe wrapper representing a <c>HashMap&lt;K, V&gt;</c> type reference.
/// Carries the key and value types so expression builders and type declarations
/// can distinguish "this is a HashMap" at compile time.
/// </summary>
public readonly record struct HashMapTypeRef
{
    /// <summary>Gets the key type (e.g., <c>"string"</c>).</summary>
    public TypeRef KeyType { get; }

    /// <summary>Gets the value type (e.g., <c>"int"</c>).</summary>
    public TypeRef ValueType { get; }

    /// <summary>Creates a <c>HashMapTypeRef</c> with the given key and value types.</summary>
    public HashMapTypeRef(TypeRef keyType, TypeRef valueType)
    {
        KeyType = keyType;
        ValueType = valueType;
    }

    /// <summary>Gets the globally qualified <c>HashMap&lt;K, V&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::LanguageExt.HashMap<{KeyType.Value}, {ValueType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(HashMapTypeRef hashMap) =>
        TypeRef.From(hashMap.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(HashMapTypeRef hashMap) =>
        hashMap.ToString();
}

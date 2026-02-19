// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// A type-safe wrapper representing an <c>ImmutableArray&lt;T&gt;</c> type reference.
/// Carries the element type for typed expression building.
/// </summary>
public readonly record struct ImmutableArrayTypeRef
{
    /// <summary>Gets the element type (e.g., <c>"string"</c>).</summary>
    public TypeRef ElementType { get; }

    /// <summary>Creates an <c>ImmutableArrayTypeRef</c> for the given element type.</summary>
    public ImmutableArrayTypeRef(TypeRef elementType) => ElementType = elementType;

    /// <summary>Gets the globally qualified <c>ImmutableArray&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Collections.Immutable.ImmutableArray<{ElementType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(ImmutableArrayTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(ImmutableArrayTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing an <c>ImmutableDictionary&lt;TKey, TValue&gt;</c> type reference.
/// Carries the key and value types for typed expression building.
/// </summary>
public readonly record struct ImmutableDictionaryTypeRef
{
    /// <summary>Gets the key type.</summary>
    public TypeRef KeyType { get; }

    /// <summary>Gets the value type.</summary>
    public TypeRef ValueType { get; }

    /// <summary>Creates an <c>ImmutableDictionaryTypeRef</c> for the given key and value types.</summary>
    public ImmutableDictionaryTypeRef(TypeRef keyType, TypeRef valueType)
    {
        KeyType = keyType;
        ValueType = valueType;
    }

    /// <summary>Gets the globally qualified <c>ImmutableDictionary&lt;TKey, TValue&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Collections.Immutable.ImmutableDictionary<{KeyType.Value}, {ValueType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(ImmutableDictionaryTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(ImmutableDictionaryTypeRef self) =>
        self.ToString();
}
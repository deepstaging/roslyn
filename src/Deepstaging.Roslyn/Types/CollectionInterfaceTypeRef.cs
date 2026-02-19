// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// A type-safe wrapper representing an <c>IEnumerable&lt;T&gt;</c> type reference.
/// Carries the element type for typed expression building.
/// </summary>
public readonly record struct EnumerableTypeRef
{
    /// <summary>Gets the element type (e.g., <c>"string"</c>).</summary>
    public TypeRef ElementType { get; }

    /// <summary>Creates an <c>EnumerableTypeRef</c> for the given element type.</summary>
    public EnumerableTypeRef(TypeRef elementType) => ElementType = elementType;

    /// <summary>Gets the globally qualified <c>IEnumerable&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Collections.Generic.IEnumerable<{ElementType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(EnumerableTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(EnumerableTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing an <c>ICollection&lt;T&gt;</c> type reference.
/// Carries the element type for typed expression building.
/// </summary>
public readonly record struct CollectionInterfaceTypeRef
{
    /// <summary>Gets the element type (e.g., <c>"string"</c>).</summary>
    public TypeRef ElementType { get; }

    /// <summary>Creates a <c>CollectionInterfaceTypeRef</c> for the given element type.</summary>
    public CollectionInterfaceTypeRef(TypeRef elementType) => ElementType = elementType;

    /// <summary>Gets the globally qualified <c>ICollection&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Collections.Generic.ICollection<{ElementType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(CollectionInterfaceTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(CollectionInterfaceTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing an <c>IList&lt;T&gt;</c> type reference.
/// Carries the element type for typed expression building.
/// </summary>
public readonly record struct ListInterfaceTypeRef
{
    /// <summary>Gets the element type (e.g., <c>"string"</c>).</summary>
    public TypeRef ElementType { get; }

    /// <summary>Creates a <c>ListInterfaceTypeRef</c> for the given element type.</summary>
    public ListInterfaceTypeRef(TypeRef elementType) => ElementType = elementType;

    /// <summary>Gets the globally qualified <c>IList&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Collections.Generic.IList<{ElementType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(ListInterfaceTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(ListInterfaceTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing an <c>ISet&lt;T&gt;</c> type reference.
/// Carries the element type for typed expression building.
/// </summary>
public readonly record struct SetInterfaceTypeRef
{
    /// <summary>Gets the element type (e.g., <c>"string"</c>).</summary>
    public TypeRef ElementType { get; }

    /// <summary>Creates a <c>SetInterfaceTypeRef</c> for the given element type.</summary>
    public SetInterfaceTypeRef(TypeRef elementType) => ElementType = elementType;

    /// <summary>Gets the globally qualified <c>ISet&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Collections.Generic.ISet<{ElementType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(SetInterfaceTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(SetInterfaceTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing an <c>IReadOnlyList&lt;T&gt;</c> type reference.
/// Carries the element type for typed expression building.
/// </summary>
public readonly record struct ReadOnlyListTypeRef
{
    /// <summary>Gets the element type (e.g., <c>"string"</c>).</summary>
    public TypeRef ElementType { get; }

    /// <summary>Creates a <c>ReadOnlyListTypeRef</c> for the given element type.</summary>
    public ReadOnlyListTypeRef(TypeRef elementType) => ElementType = elementType;

    /// <summary>Gets the globally qualified <c>IReadOnlyList&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Collections.Generic.IReadOnlyList<{ElementType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(ReadOnlyListTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(ReadOnlyListTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing an <c>IReadOnlyCollection&lt;T&gt;</c> type reference.
/// Carries the element type for typed expression building.
/// </summary>
public readonly record struct ReadOnlyCollectionTypeRef
{
    /// <summary>Gets the element type (e.g., <c>"string"</c>).</summary>
    public TypeRef ElementType { get; }

    /// <summary>Creates a <c>ReadOnlyCollectionTypeRef</c> for the given element type.</summary>
    public ReadOnlyCollectionTypeRef(TypeRef elementType) => ElementType = elementType;

    /// <summary>Gets the globally qualified <c>IReadOnlyCollection&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Collections.Generic.IReadOnlyCollection<{ElementType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(ReadOnlyCollectionTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(ReadOnlyCollectionTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing an <c>IDictionary&lt;TKey, TValue&gt;</c> type reference.
/// Carries the key and value types for typed expression building.
/// </summary>
public readonly record struct DictionaryInterfaceTypeRef
{
    /// <summary>Gets the key type (e.g., <c>"string"</c>).</summary>
    public TypeRef KeyType { get; }

    /// <summary>Gets the value type (e.g., <c>"int"</c>).</summary>
    public TypeRef ValueType { get; }

    /// <summary>Creates a <c>DictionaryInterfaceTypeRef</c> for the given key and value types.</summary>
    public DictionaryInterfaceTypeRef(TypeRef keyType, TypeRef valueType)
    {
        KeyType = keyType;
        ValueType = valueType;
    }

    /// <summary>Gets the globally qualified <c>IDictionary&lt;TKey, TValue&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Collections.Generic.IDictionary<{KeyType.Value}, {ValueType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(DictionaryInterfaceTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(DictionaryInterfaceTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing an <c>IReadOnlyDictionary&lt;TKey, TValue&gt;</c> type reference.
/// Carries the key and value types for typed expression building.
/// </summary>
public readonly record struct ReadOnlyDictionaryTypeRef
{
    /// <summary>Gets the key type (e.g., <c>"string"</c>).</summary>
    public TypeRef KeyType { get; }

    /// <summary>Gets the value type (e.g., <c>"int"</c>).</summary>
    public TypeRef ValueType { get; }

    /// <summary>Creates a <c>ReadOnlyDictionaryTypeRef</c> for the given key and value types.</summary>
    public ReadOnlyDictionaryTypeRef(TypeRef keyType, TypeRef valueType)
    {
        KeyType = keyType;
        ValueType = valueType;
    }

    /// <summary>Gets the globally qualified <c>IReadOnlyDictionary&lt;TKey, TValue&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Collections.Generic.IReadOnlyDictionary<{KeyType.Value}, {ValueType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(ReadOnlyDictionaryTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(ReadOnlyDictionaryTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing a <c>KeyValuePair&lt;TKey, TValue&gt;</c> type reference.
/// Carries the key and value types for typed expression building.
/// </summary>
public readonly record struct KeyValuePairTypeRef
{
    /// <summary>Gets the key type (e.g., <c>"string"</c>).</summary>
    public TypeRef KeyType { get; }

    /// <summary>Gets the value type (e.g., <c>"int"</c>).</summary>
    public TypeRef ValueType { get; }

    /// <summary>Creates a <c>KeyValuePairTypeRef</c> for the given key and value types.</summary>
    public KeyValuePairTypeRef(TypeRef keyType, TypeRef valueType)
    {
        KeyType = keyType;
        ValueType = valueType;
    }

    /// <summary>Gets the globally qualified <c>KeyValuePair&lt;TKey, TValue&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Collections.Generic.KeyValuePair<{KeyType.Value}, {ValueType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(KeyValuePairTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(KeyValuePairTypeRef self) =>
        self.ToString();
}

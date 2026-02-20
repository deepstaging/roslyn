// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// Convenience <see cref="NamespaceRef"/> and factory methods for <c>System.Collections.Generic</c> types.
/// </summary>
public static class CollectionTypes
{
    /// <summary>Gets the <c>System.Collections.Generic</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System.Collections.Generic");

    /// <summary>Creates a <c>List&lt;T&gt;</c> type reference.</summary>
    public static ListTypeRef List(TypeRef elementType) => new(elementType);

    /// <summary>Creates a <c>Dictionary&lt;TKey, TValue&gt;</c> type reference.</summary>
    public static DictionaryTypeRef Dictionary(TypeRef keyType, TypeRef valueType) => new(keyType, valueType);

    /// <summary>Creates a <c>HashSet&lt;T&gt;</c> type reference.</summary>
    public static HashSetTypeRef HashSet(TypeRef elementType) => new(elementType);

    /// <summary>Creates an <c>IEnumerable&lt;T&gt;</c> type reference.</summary>
    public static EnumerableTypeRef Enumerable(TypeRef elementType) => new(elementType);

    /// <summary>Creates an <c>ICollection&lt;T&gt;</c> type reference.</summary>
    public static CollectionInterfaceTypeRef Collection(TypeRef elementType) => new(elementType);

    /// <summary>Creates an <c>IList&lt;T&gt;</c> type reference.</summary>
    public static ListInterfaceTypeRef ListInterface(TypeRef elementType) => new(elementType);

    /// <summary>Creates an <c>ISet&lt;T&gt;</c> type reference.</summary>
    public static SetInterfaceTypeRef SetInterface(TypeRef elementType) => new(elementType);

    /// <summary>Creates an <c>IReadOnlyList&lt;T&gt;</c> type reference.</summary>
    public static ReadOnlyListTypeRef ReadOnlyList(TypeRef elementType) => new(elementType);

    /// <summary>Creates an <c>IReadOnlyCollection&lt;T&gt;</c> type reference.</summary>
    public static ReadOnlyCollectionTypeRef ReadOnlyCollection(TypeRef elementType) => new(elementType);

    /// <summary>Creates an <c>IDictionary&lt;TKey, TValue&gt;</c> type reference.</summary>
    public static DictionaryInterfaceTypeRef DictionaryInterface(TypeRef keyType, TypeRef valueType) => new(keyType, valueType);

    /// <summary>Creates an <c>IReadOnlyDictionary&lt;TKey, TValue&gt;</c> type reference.</summary>
    public static ReadOnlyDictionaryTypeRef ReadOnlyDictionary(TypeRef keyType, TypeRef valueType) => new(keyType, valueType);

    /// <summary>Creates a <c>KeyValuePair&lt;TKey, TValue&gt;</c> type reference.</summary>
    public static KeyValuePairTypeRef KeyValuePair(TypeRef keyType, TypeRef valueType) => new(keyType, valueType);
}

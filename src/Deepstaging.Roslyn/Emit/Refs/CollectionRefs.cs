// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Factory methods for <c>System.Collections.Generic</c> types.
/// </summary>
public static class CollectionRefs
{
    /// <summary>Gets the <c>System.Collections.Generic</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System.Collections.Generic");

    /// <summary>Creates a <c>List&lt;T&gt;</c> type reference.</summary>
    public static TypeRef List(TypeRef elementType) =>
        Namespace.Type($"List<{elementType.Value}>");

    /// <summary>Creates a <c>Dictionary&lt;TKey, TValue&gt;</c> type reference.</summary>
    public static TypeRef Dictionary(TypeRef keyType, TypeRef valueType) =>
        Namespace.Type($"Dictionary<{keyType.Value}, {valueType.Value}>");

    /// <summary>Creates a <c>HashSet&lt;T&gt;</c> type reference.</summary>
    public static TypeRef HashSet(TypeRef elementType) =>
        Namespace.Type($"HashSet<{elementType.Value}>");

    /// <summary>Creates a <c>KeyValuePair&lt;TKey, TValue&gt;</c> type reference.</summary>
    public static TypeRef KeyValuePair(TypeRef keyType, TypeRef valueType) =>
        Namespace.Type($"KeyValuePair<{keyType.Value}, {valueType.Value}>");

    /// <summary>Creates an <c>IEnumerable&lt;T&gt;</c> type reference.</summary>
    public static TypeRef IEnumerable(TypeRef elementType) =>
        Namespace.Type($"IEnumerable<{elementType.Value}>");

    /// <summary>Creates an <c>ICollection&lt;T&gt;</c> type reference.</summary>
    public static TypeRef ICollection(TypeRef elementType) =>
        Namespace.Type($"ICollection<{elementType.Value}>");

    /// <summary>Creates an <c>IList&lt;T&gt;</c> type reference.</summary>
    public static TypeRef IList(TypeRef elementType) =>
        Namespace.Type($"IList<{elementType.Value}>");

    /// <summary>Creates an <c>IDictionary&lt;TKey, TValue&gt;</c> type reference.</summary>
    public static TypeRef IDictionary(TypeRef keyType, TypeRef valueType) =>
        Namespace.Type($"IDictionary<{keyType.Value}, {valueType.Value}>");

    /// <summary>Creates an <c>ISet&lt;T&gt;</c> type reference.</summary>
    public static TypeRef ISet(TypeRef elementType) =>
        Namespace.Type($"ISet<{elementType.Value}>");

    /// <summary>Creates an <c>IReadOnlyList&lt;T&gt;</c> type reference.</summary>
    public static TypeRef IReadOnlyList(TypeRef elementType) =>
        Namespace.Type($"IReadOnlyList<{elementType.Value}>");

    /// <summary>Creates an <c>IReadOnlyCollection&lt;T&gt;</c> type reference.</summary>
    public static TypeRef IReadOnlyCollection(TypeRef elementType) =>
        Namespace.Type($"IReadOnlyCollection<{elementType.Value}>");

    /// <summary>Creates an <c>IReadOnlyDictionary&lt;TKey, TValue&gt;</c> type reference.</summary>
    public static TypeRef IReadOnlyDictionary(TypeRef keyType, TypeRef valueType) =>
        Namespace.Type($"IReadOnlyDictionary<{keyType.Value}, {valueType.Value}>");
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

/// <summary>
/// Builds <c>List&lt;T&gt;</c>, <c>Dictionary&lt;K,V&gt;</c>, and <c>HashSet&lt;T&gt;</c>
/// construction expressions for use in generated code.
/// </summary>
/// <example>
/// <code>
/// CollectionExpression.NewList("string")
///     // "new global::System.Collections.Generic.List&lt;string&gt;()"
///
/// CollectionExpression.NewDictionary("string", "int")
///     // "new global::System.Collections.Generic.Dictionary&lt;string, int&gt;()"
/// </code>
/// </example>
public static class CollectionExpression
{
    // ── List ────────────────────────────────────────────────────────────

    /// <summary>Produces a <c>new List&lt;T&gt;()</c> expression.</summary>
    /// <param name="elementType">The element type.</param>
    public static ExpressionRef NewList(TypeRef elementType) =>
        ((TypeRef)new ListTypeRef(elementType)).New();

    /// <summary>Produces a <c>new List&lt;T&gt;(capacity)</c> expression.</summary>
    /// <param name="elementType">The element type.</param>
    /// <param name="capacity">The initial capacity expression.</param>
    public static ExpressionRef NewList(TypeRef elementType, ExpressionRef capacity) =>
        ((TypeRef)new ListTypeRef(elementType)).New(capacity);

    // ── Dictionary ──────────────────────────────────────────────────────

    /// <summary>Produces a <c>new Dictionary&lt;TKey, TValue&gt;()</c> expression.</summary>
    /// <param name="keyType">The key type.</param>
    /// <param name="valueType">The value type.</param>
    public static ExpressionRef NewDictionary(TypeRef keyType, TypeRef valueType) =>
        ((TypeRef)new DictionaryTypeRef(keyType, valueType)).New();

    /// <summary>Produces a <c>new Dictionary&lt;TKey, TValue&gt;(capacity)</c> expression.</summary>
    /// <param name="keyType">The key type.</param>
    /// <param name="valueType">The value type.</param>
    /// <param name="capacity">The initial capacity expression.</param>
    public static ExpressionRef NewDictionary(TypeRef keyType, TypeRef valueType, ExpressionRef capacity) =>
        ((TypeRef)new DictionaryTypeRef(keyType, valueType)).New(capacity);

    // ── HashSet ─────────────────────────────────────────────────────────

    /// <summary>Produces a <c>new HashSet&lt;T&gt;()</c> expression.</summary>
    /// <param name="elementType">The element type.</param>
    public static ExpressionRef NewHashSet(TypeRef elementType) =>
        ((TypeRef)new HashSetTypeRef(elementType)).New();

    /// <summary>Produces a <c>new HashSet&lt;T&gt;(comparer)</c> expression.</summary>
    /// <param name="elementType">The element type.</param>
    /// <param name="comparer">The equality comparer expression.</param>
    public static ExpressionRef NewHashSet(TypeRef elementType, ExpressionRef comparer) =>
        ((TypeRef)new HashSetTypeRef(elementType)).New(comparer);

    // ── Empty sequences ─────────────────────────────────────────────────

    /// <summary>Produces an <c>Enumerable.Empty&lt;T&gt;()</c> expression.</summary>
    /// <param name="elementType">The element type.</param>
    public static ExpressionRef EmptyEnumerable(TypeRef elementType) =>
        TypeRef.Global("System.Linq.Enumerable").Call($"Empty<{elementType.Value}>");

    /// <summary>Produces an <c>Array.Empty&lt;T&gt;()</c> expression.</summary>
    /// <param name="elementType">The element type.</param>
    public static ExpressionRef EmptyArray(TypeRef elementType) =>
        TypeRef.Global("System.Array").Call($"Empty<{elementType.Value}>");
}
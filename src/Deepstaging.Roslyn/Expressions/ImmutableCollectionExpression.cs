// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

/// <summary>
/// Builds <c>ImmutableArray</c> and <c>ImmutableDictionary</c> expressions for use in generated code.
/// </summary>
/// <example>
/// <code>
/// ImmutableCollectionExpression.EmptyArray("string")
///     // "global::System.Collections.Immutable.ImmutableArray&lt;string&gt;.Empty"
///
/// ImmutableCollectionExpression.CreateArray("string", "item1", "item2")
///     // "global::System.Collections.Immutable.ImmutableArray.Create&lt;string&gt;(item1, item2)"
/// </code>
/// </example>
public static class ImmutableCollectionExpression
{
    private static readonly TypeRef ImmutableArrayType =
        TypeRef.Global("System.Collections.Immutable.ImmutableArray");

    private static readonly TypeRef ImmutableDictionaryType =
        TypeRef.Global("System.Collections.Immutable.ImmutableDictionary");

    private static readonly TypeRef ImmutableListType =
        TypeRef.Global("System.Collections.Immutable.ImmutableList");

    // ── ImmutableArray ──────────────────────────────────────────────────

    /// <summary>
    /// <c>ImmutableArray&lt;T&gt;.Empty</c> — the empty immutable array for the given element type.
    /// </summary>
    /// <param name="elementType">The element type.</param>
    public static ExpressionRef EmptyArray(TypeRef elementType) =>
        ExpressionRef.From($"{new ImmutableArrayTypeRef(elementType)}.Empty");

    /// <summary>
    /// <c>ImmutableArray.Create&lt;T&gt;(items)</c> — creates an immutable array from the given items.
    /// </summary>
    /// <param name="elementType">The element type.</param>
    /// <param name="items">The item expressions.</param>
    public static ExpressionRef CreateArray(TypeRef elementType, params ExpressionRef[] items) =>
        ImmutableArrayType.Call($"Create<{elementType.Value}>", items);

    // ── ImmutableDictionary ─────────────────────────────────────────────

    /// <summary>
    /// <c>ImmutableDictionary&lt;TKey, TValue&gt;.Empty</c> — the empty immutable dictionary.
    /// </summary>
    /// <param name="keyType">The key type.</param>
    /// <param name="valueType">The value type.</param>
    public static ExpressionRef EmptyDictionary(TypeRef keyType, TypeRef valueType) =>
        ExpressionRef.From($"{new ImmutableDictionaryTypeRef(keyType, valueType)}.Empty");

    /// <summary>
    /// <c>ImmutableDictionary.CreateBuilder&lt;TKey, TValue&gt;()</c> — creates a builder for constructing an immutable dictionary.
    /// </summary>
    /// <param name="keyType">The key type.</param>
    /// <param name="valueType">The value type.</param>
    public static ExpressionRef CreateDictionaryBuilder(TypeRef keyType, TypeRef valueType) =>
        ImmutableDictionaryType.Call($"CreateBuilder<{keyType.Value}, {valueType.Value}>");

    // ── ImmutableList ───────────────────────────────────────────────────

    /// <summary>
    /// <c>ImmutableList&lt;T&gt;.Empty</c> — the empty immutable list for the given element type.
    /// </summary>
    /// <param name="elementType">The element type.</param>
    public static ExpressionRef EmptyList(TypeRef elementType) =>
        ExpressionRef.From($"{new ImmutableListTypeRef(elementType)}.Empty");

    /// <summary>
    /// <c>ImmutableList.Create&lt;T&gt;(items)</c> — creates an immutable list from the given items.
    /// </summary>
    /// <param name="elementType">The element type.</param>
    /// <param name="items">The item expressions.</param>
    public static ExpressionRef CreateList(TypeRef elementType, params ExpressionRef[] items) =>
        ImmutableListType.Call($"Create<{elementType.Value}>", items);
}
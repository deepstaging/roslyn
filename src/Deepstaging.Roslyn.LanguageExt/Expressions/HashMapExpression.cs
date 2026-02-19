// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Expressions;

using Types;

/// <summary>
/// Builds <c>HashMap</c> construction expressions using LanguageExt Prelude functions and static members.
/// </summary>
/// <remarks>
/// Prelude functions require <c>using static LanguageExt.Prelude</c> in the generated code.
/// Use <see cref="TypeBuilderExtensions.AddLanguageExtUsings"/> to add the standard usings.
/// </remarks>
public static class HashMapExpression
{
    /// <summary>
    /// <c>HashMap({pairs})</c> — constructs a <c>HashMap&lt;K, V&gt;</c> from key-value tuple expressions.
    /// </summary>
    /// <param name="pairs">The key-value pair expressions (e.g., <c>("key", value)</c> tuples).</param>
    public static ExpressionRef HashMap(params ExpressionRef[] pairs)
    {
        var args = string.Join(", ", pairs.Select(p => p.Value));
        return ExpressionRef.From($"HashMap({args})");
    }

    /// <summary>
    /// <c>toHashMap({expr})</c> — converts an <c>IEnumerable&lt;(K, V)&gt;</c> to <c>HashMap&lt;K, V&gt;</c>.
    /// </summary>
    /// <param name="expr">The enumerable of tuple expressions.</param>
    public static ExpressionRef toHashMap(ExpressionRef expr) =>
        ExpressionRef.From($"toHashMap({expr.Value})");

    /// <summary>
    /// <c>{hashMapType}.Empty</c> — the empty <c>HashMap&lt;K, V&gt;</c> singleton.
    /// </summary>
    /// <param name="hashMapType">The HashMap type reference providing the key and value types.</param>
    public static ExpressionRef Empty(HashMapTypeRef hashMapType) =>
        ((TypeRef)hashMapType).Member("Empty");
}
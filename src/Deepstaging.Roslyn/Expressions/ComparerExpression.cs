// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

/// <summary>
/// Builds <c>Comparer&lt;T&gt;</c> expressions for use in generated code.
/// </summary>
/// <example>
/// <code>
/// ComparerExpression.DefaultCompare("int", "left", "right")
///     // "global::System.Collections.Generic.Comparer&lt;int&gt;.Default.Compare(left, right)"
/// </code>
/// </example>
public static class ComparerExpression
{
    /// <summary>
    /// <c>Comparer&lt;T&gt;.Default</c> — the default comparer instance for the given type.
    /// </summary>
    /// <param name="comparedType">The type to compare.</param>
    public static ExpressionRef Default(TypeRef comparedType) =>
        ExpressionRef.From($"global::System.Collections.Generic.Comparer<{comparedType.Value}>.Default");

    /// <summary>
    /// <c>Comparer&lt;T&gt;.Default.Compare(left, right)</c> — compares two values using the default comparer.
    /// </summary>
    /// <param name="comparedType">The type of the values being compared.</param>
    /// <param name="left">The left expression.</param>
    /// <param name="right">The right expression.</param>
    public static ExpressionRef DefaultCompare(TypeRef comparedType, ExpressionRef left, ExpressionRef right) =>
        Default(comparedType).Call("Compare", left, right);
}
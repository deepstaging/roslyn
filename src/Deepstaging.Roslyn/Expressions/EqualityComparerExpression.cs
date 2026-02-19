// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

/// <summary>
/// Builds <c>EqualityComparer&lt;T&gt;</c> expressions for use in generated code.
/// </summary>
/// <example>
/// <code>
/// EqualityComparerExpression.DefaultEquals("string", "_name", "value")
///     // → "global::System.Collections.Generic.EqualityComparer&lt;string&gt;.Default.Equals(_name, value)"
/// </code>
/// </example>
public static class EqualityComparerExpression
{
    /// <summary>
    /// <c>EqualityComparer&lt;T&gt;.Default</c> — the default comparer instance for the given type.
    /// </summary>
    /// <param name="comparedType">The type to compare.</param>
    public static ExpressionRef Default(TypeRef comparedType) =>
        ExpressionRef.From($"global::System.Collections.Generic.EqualityComparer<{comparedType.Value}>.Default");

    /// <summary>
    /// <c>EqualityComparer&lt;T&gt;.Default</c> — the default comparer instance for the given typed ref.
    /// </summary>
    /// <param name="typeRef">The equality comparer type reference.</param>
    public static ExpressionRef Default(EqualityComparerTypeRef typeRef) =>
        ExpressionRef.From($"{typeRef}.Default");

    /// <summary>
    /// <c>EqualityComparer&lt;T&gt;.Default.Equals(left, right)</c> — compares two values using the default comparer.
    /// </summary>
    /// <param name="comparedType">The type of the values being compared.</param>
    /// <param name="left">The left expression.</param>
    /// <param name="right">The right expression.</param>
    public static ExpressionRef DefaultEquals(TypeRef comparedType, ExpressionRef left, ExpressionRef right) =>
        Default(comparedType).Call("Equals", left, right);

    /// <summary>
    /// <c>EqualityComparer&lt;T&gt;.Default.GetHashCode(value)</c> — computes the hash code using the default comparer.
    /// </summary>
    /// <param name="comparedType">The type of the value.</param>
    /// <param name="value">The value expression.</param>
    public static ExpressionRef DefaultGetHashCode(TypeRef comparedType, ExpressionRef value) =>
        Default(comparedType).Call("GetHashCode", value);
}
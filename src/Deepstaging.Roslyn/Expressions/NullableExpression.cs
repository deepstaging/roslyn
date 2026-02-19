// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

/// <summary>
/// Builds <c>Nullable&lt;T&gt;</c> expressions for use in generated code.
/// </summary>
/// <example>
/// <code>
/// NullableExpression.HasValue("myNullable")
///     // → "myNullable.HasValue"
///
/// NullableExpression.GetValueOrDefault("myNullable")
///     // → "myNullable.GetValueOrDefault()"
/// </code>
/// </example>
public static class NullableExpression
{
    /// <summary>
    /// <c>expr.HasValue</c> — checks whether the nullable has a value.
    /// </summary>
    /// <param name="expr">The nullable expression.</param>
    public static ExpressionRef HasValue(ExpressionRef expr) =>
        expr.Member("HasValue");

    /// <summary>
    /// <c>expr.Value</c> — gets the underlying value (throws if null).
    /// </summary>
    /// <param name="expr">The nullable expression.</param>
    public static ExpressionRef Value(ExpressionRef expr) =>
        expr.Member("Value");

    /// <summary>
    /// <c>expr.GetValueOrDefault()</c> — gets the underlying value or the default for the type.
    /// </summary>
    /// <param name="expr">The nullable expression.</param>
    public static ExpressionRef GetValueOrDefault(ExpressionRef expr) =>
        expr.Call("GetValueOrDefault");

    /// <summary>
    /// <c>expr.GetValueOrDefault(defaultValue)</c> — gets the underlying value or the specified default.
    /// </summary>
    /// <param name="expr">The nullable expression.</param>
    /// <param name="defaultValue">The fallback value expression.</param>
    public static ExpressionRef GetValueOrDefault(ExpressionRef expr, ExpressionRef defaultValue) =>
        expr.Call("GetValueOrDefault", defaultValue);
}
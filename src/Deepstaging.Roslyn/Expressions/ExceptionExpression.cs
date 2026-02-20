// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

using Types;

/// <summary>
/// Builds exception-related expressions for use in generated code.
/// </summary>
/// <example>
/// <code>
/// ExceptionExpression.ThrowNew(ExceptionTypes.ArgumentNull, "nameof(value)")
///     // → "throw new global::System.ArgumentNullException(nameof(value))"
///
/// ExceptionExpression.ThrowIfNull("value")
///     // → "global::System.ArgumentNullException.ThrowIfNull(value)"
///
/// ExceptionExpression.New(ExceptionTypes.InvalidOperation, "\"Item not found\"")
///     // → "new global::System.InvalidOperationException(\"Item not found\")"
/// </code>
/// </example>
public static class ExceptionExpression
{
    /// <summary>
    /// <c>new ExceptionType(args)</c> — creates a new exception instance.
    /// </summary>
    /// <param name="exceptionType">The exception type reference.</param>
    /// <param name="arguments">Constructor arguments.</param>
    public static ExpressionRef New(TypeRef exceptionType, params ExpressionRef[] arguments) =>
        exceptionType.New(arguments);

    /// <summary>
    /// <c>throw new ExceptionType(args)</c> — throws a new exception.
    /// </summary>
    /// <param name="exceptionType">The exception type reference.</param>
    /// <param name="arguments">Constructor arguments.</param>
    public static ExpressionRef ThrowNew(TypeRef exceptionType, params ExpressionRef[] arguments) =>
        ExpressionRef.From($"throw {exceptionType.New(arguments).Value}");

    /// <summary>
    /// <c>ArgumentNullException.ThrowIfNull(paramName)</c> — guard clause for null arguments.
    /// </summary>
    /// <param name="paramName">The parameter name expression.</param>
    public static ExpressionRef ThrowIfNull(ExpressionRef paramName) =>
        ExceptionTypes.ArgumentNull.Call("ThrowIfNull", paramName);

    /// <summary>
    /// <c>ArgumentNullException.ThrowIfNullOrEmpty(paramName)</c> — guard clause for null or empty strings.
    /// </summary>
    /// <param name="paramName">The parameter name expression.</param>
    public static ExpressionRef ThrowIfNullOrEmpty(ExpressionRef paramName) =>
        ExceptionTypes.Argument.Call("ThrowIfNullOrEmpty", paramName);

    /// <summary>
    /// <c>ArgumentNullException.ThrowIfNullOrWhiteSpace(paramName)</c> — guard clause for null or whitespace strings.
    /// </summary>
    /// <param name="paramName">The parameter name expression.</param>
    public static ExpressionRef ThrowIfNullOrWhiteSpace(ExpressionRef paramName) =>
        ExceptionTypes.Argument.Call("ThrowIfNullOrWhiteSpace", paramName);

    /// <summary>
    /// <c>ArgumentOutOfRangeException.ThrowIfNegative(paramName)</c> — guard clause for negative values.
    /// </summary>
    /// <param name="paramName">The parameter name expression.</param>
    public static ExpressionRef ThrowIfNegative(ExpressionRef paramName) =>
        ExceptionTypes.ArgumentOutOfRange.Call("ThrowIfNegative", paramName);

    /// <summary>
    /// <c>ArgumentOutOfRangeException.ThrowIfZero(paramName)</c> — guard clause for zero values.
    /// </summary>
    /// <param name="paramName">The parameter name expression.</param>
    public static ExpressionRef ThrowIfZero(ExpressionRef paramName) =>
        ExceptionTypes.ArgumentOutOfRange.Call("ThrowIfZero", paramName);

    /// <summary>
    /// <c>ObjectDisposedException.ThrowIf(condition, instance)</c> — guard clause for disposed objects.
    /// </summary>
    /// <param name="condition">The boolean condition expression.</param>
    /// <param name="instance">The object instance expression.</param>
    public static ExpressionRef ThrowIfDisposed(ExpressionRef condition, ExpressionRef instance) =>
        ExceptionTypes.ObjectDisposed.Call("ThrowIf", condition, instance);
}

// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Expressions;

/// <summary>Factory methods that produce <see cref="TsExpressionRef"/> values for TypeScript error construction.</summary>
public static class TsErrorExpression
{
    private static readonly TsTypeRef ErrorType = TsTypeRef.From("Error");
    private static readonly TsTypeRef TypeErrorType = TsTypeRef.From("TypeError");
    private static readonly TsTypeRef RangeErrorType = TsTypeRef.From("RangeError");
    private static readonly TsTypeRef ReferenceErrorType = TsTypeRef.From("ReferenceError");

    /// <summary>Produces <c>new Error(message)</c>.</summary>
    /// <param name="message">The error message expression.</param>
    public static TsExpressionRef New(TsExpressionRef message) =>
        ErrorType.New(message);

    /// <summary>Produces <c>new TypeError(message)</c>.</summary>
    /// <param name="message">The error message expression.</param>
    public static TsExpressionRef NewTypeError(TsExpressionRef message) =>
        TypeErrorType.New(message);

    /// <summary>Produces <c>new RangeError(message)</c>.</summary>
    /// <param name="message">The error message expression.</param>
    public static TsExpressionRef NewRangeError(TsExpressionRef message) =>
        RangeErrorType.New(message);

    /// <summary>Produces <c>new ReferenceError(message)</c>.</summary>
    /// <param name="message">The error message expression.</param>
    public static TsExpressionRef NewReferenceError(TsExpressionRef message) =>
        ReferenceErrorType.New(message);
}

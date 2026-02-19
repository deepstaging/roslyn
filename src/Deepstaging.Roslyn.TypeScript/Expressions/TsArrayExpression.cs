// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Expressions;

/// <summary>Factory methods that produce <see cref="TsExpressionRef"/> values for TypeScript <c>Array</c> operations.</summary>
public static class TsArrayExpression
{
    private static readonly TsTypeRef ArrayType = TsTypeRef.From("Array");
    private static readonly TsTypeRef ObjectType = TsTypeRef.From("Object");

    /// <summary>Produces <c>Array.from(iterable)</c>.</summary>
    /// <param name="iterable">The iterable to convert to an array.</param>
    public static TsExpressionRef From(TsExpressionRef iterable) =>
        ArrayType.Call("from", iterable);

    /// <summary>Produces <c>Array.isArray(value)</c>.</summary>
    /// <param name="value">The value to check.</param>
    public static TsExpressionRef IsArray(TsExpressionRef value) =>
        ArrayType.Call("isArray", value);

    /// <summary>Produces <c>[...source]</c> (spread into a new array).</summary>
    /// <param name="source">The source expression to spread.</param>
    public static TsExpressionRef Spread(TsExpressionRef source) =>
        TsExpressionRef.From($"[...{source.Value}]");

    /// <summary>Produces <c>Object.freeze(arrayExpression)</c> for an immutable array.</summary>
    /// <param name="arrayExpression">The array expression to freeze.</param>
    public static TsExpressionRef Frozen(TsExpressionRef arrayExpression) =>
        ObjectType.Call("freeze", arrayExpression);
}

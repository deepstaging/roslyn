// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Expressions;

/// <summary>Factory methods that produce <see cref="TsExpressionRef"/> values for TypeScript <c>Promise</c> patterns.</summary>
public static class TsPromiseExpression
{
    private static readonly TsTypeRef PromiseType = TsTypeRef.From("Promise");

    /// <summary>Produces <c>Promise.resolve(value)</c>.</summary>
    /// <param name="value">The value to resolve.</param>
    public static TsExpressionRef Resolve(TsExpressionRef value) =>
        PromiseType.Call("resolve", value);

    /// <summary>Produces <c>Promise.reject(reason)</c>.</summary>
    /// <param name="reason">The rejection reason.</param>
    public static TsExpressionRef Reject(TsExpressionRef reason) =>
        PromiseType.Call("reject", reason);

    /// <summary>Produces <c>Promise.all([...promises])</c>.</summary>
    /// <param name="promises">The promises to await.</param>
    public static TsExpressionRef All(params TsExpressionRef[] promises) =>
        PromiseType.Call("all", ArrayLiteral(promises));

    /// <summary>Produces <c>Promise.allSettled([...promises])</c>.</summary>
    /// <param name="promises">The promises to settle.</param>
    public static TsExpressionRef AllSettled(params TsExpressionRef[] promises) =>
        PromiseType.Call("allSettled", ArrayLiteral(promises));

    /// <summary>Produces <c>Promise.race([...promises])</c>.</summary>
    /// <param name="promises">The promises to race.</param>
    public static TsExpressionRef Race(params TsExpressionRef[] promises) =>
        PromiseType.Call("race", ArrayLiteral(promises));

    /// <summary>Produces <c>Promise.any([...promises])</c>.</summary>
    /// <param name="promises">The promises to resolve.</param>
    public static TsExpressionRef Any(params TsExpressionRef[] promises) =>
        PromiseType.Call("any", ArrayLiteral(promises));

    /// <summary>Produces <c>new Promise((resolve, reject) =&gt; { ... })</c>.</summary>
    /// <param name="body">The executor body expression.</param>
    public static TsExpressionRef New(TsExpressionRef body) =>
        PromiseType.New(body);

    private static TsExpressionRef ArrayLiteral(TsExpressionRef[] items) =>
        TsExpressionRef.From($"[{string.Join(", ", System.Linq.Enumerable.Select(items, i => i.Value))}]");
}

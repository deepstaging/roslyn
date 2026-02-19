// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Expressions;

/// <summary>Factory methods that produce <see cref="TsExpressionRef"/> values for TypeScript <c>Object</c> operations.</summary>
public static class TsObjectExpression
{
    private static readonly TsTypeRef ObjectType = TsTypeRef.From("Object");

    /// <summary>Produces <c>Object.keys(obj)</c>.</summary>
    /// <param name="obj">The object expression.</param>
    public static TsExpressionRef Keys(TsExpressionRef obj) =>
        ObjectType.Call("keys", obj);

    /// <summary>Produces <c>Object.values(obj)</c>.</summary>
    /// <param name="obj">The object expression.</param>
    public static TsExpressionRef Values(TsExpressionRef obj) =>
        ObjectType.Call("values", obj);

    /// <summary>Produces <c>Object.entries(obj)</c>.</summary>
    /// <param name="obj">The object expression.</param>
    public static TsExpressionRef Entries(TsExpressionRef obj) =>
        ObjectType.Call("entries", obj);

    /// <summary>Produces <c>Object.assign(target, ...sources)</c>.</summary>
    /// <param name="target">The target object.</param>
    /// <param name="sources">The source objects to merge.</param>
    public static TsExpressionRef Assign(TsExpressionRef target, params TsExpressionRef[] sources)
    {
        var allArgs = new TsExpressionRef[sources.Length + 1];
        allArgs[0] = target;
        System.Array.Copy(sources, 0, allArgs, 1, sources.Length);
        return ObjectType.Call("assign", allArgs);
    }

    /// <summary>Produces <c>Object.freeze(obj)</c>.</summary>
    /// <param name="obj">The object expression to freeze.</param>
    public static TsExpressionRef Freeze(TsExpressionRef obj) =>
        ObjectType.Call("freeze", obj);

    /// <summary>Produces <c>{ ...source }</c> (spread into a new object).</summary>
    /// <param name="source">The source expression to spread.</param>
    public static TsExpressionRef Spread(TsExpressionRef source) =>
        TsExpressionRef.From($"{{ ...{source.Value} }}");

    /// <summary>Produces <c>Object.fromEntries(entries)</c>.</summary>
    /// <param name="entries">The entries to convert to an object.</param>
    public static TsExpressionRef FromEntries(TsExpressionRef entries) =>
        ObjectType.Call("fromEntries", entries);
}

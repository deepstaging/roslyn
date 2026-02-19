// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Expressions;

/// <summary>Factory methods that produce <see cref="TsExpressionRef"/> values for TypeScript <c>JSON</c> operations.</summary>
public static class TsJsonExpression
{
    private static readonly TsExpressionRef Json = TsExpressionRef.From("JSON");

    /// <summary>Produces <c>JSON.stringify(value)</c>.</summary>
    /// <param name="value">The value to serialize.</param>
    public static TsExpressionRef Stringify(TsExpressionRef value) =>
        Json.Call("stringify", value);

    /// <summary>Produces <c>JSON.stringify(value, null, indent)</c>.</summary>
    /// <param name="value">The value to serialize.</param>
    /// <param name="indent">The number of spaces for indentation.</param>
    public static TsExpressionRef Stringify(TsExpressionRef value, int indent) =>
        TsExpressionRef.From($"JSON.stringify({value.Value}, null, {indent})");

    /// <summary>Produces <c>JSON.parse(text)</c>.</summary>
    /// <param name="text">The JSON string to parse.</param>
    public static TsExpressionRef Parse(TsExpressionRef text) =>
        Json.Call("parse", text);

    /// <summary>Produces <c>JSON.parse(text) as T</c>.</summary>
    /// <param name="text">The JSON string to parse.</param>
    /// <param name="type">The type to cast to.</param>
    public static TsExpressionRef ParseAs(TsExpressionRef text, TsTypeRef type) =>
        Json.Call("parse", text).As(type);
}

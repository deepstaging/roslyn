// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

using Types;

/// <summary>
/// Expression factory for <c>System.Text.Json.JsonSerializer</c> operations.
/// </summary>
public static class JsonExpression
{
    /// <summary>Produces <c>JsonSerializer.Serialize(value)</c>.</summary>
    public static ExpressionRef Serialize(ExpressionRef value) =>
        JsonTypes.Serializer.Call("Serialize", value);

    /// <summary>Produces <c>JsonSerializer.Serialize(value, options)</c>.</summary>
    public static ExpressionRef Serialize(ExpressionRef value, ExpressionRef options) =>
        JsonTypes.Serializer.Call("Serialize", value, options);

    /// <summary>Produces <c>JsonSerializer.Deserialize&lt;T&gt;(json)</c>.</summary>
    public static ExpressionRef Deserialize(TypeRef type, ExpressionRef json) =>
        ExpressionRef.From($"{JsonTypes.Serializer.Value}.Deserialize<{type.Value}>({json.Value})");

    /// <summary>Produces <c>JsonSerializer.Deserialize&lt;T&gt;(json, options)</c>.</summary>
    public static ExpressionRef Deserialize(TypeRef type, ExpressionRef json, ExpressionRef options) =>
        ExpressionRef.From($"{JsonTypes.Serializer.Value}.Deserialize<{type.Value}>({json.Value}, {options.Value})");
}

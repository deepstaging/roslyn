// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

using Types;

/// <summary>
/// Expression factory for <c>Microsoft.Extensions.Configuration</c> operations.
/// </summary>
public static class ConfigurationExpression
{
    /// <summary>Produces <c>config.GetSection(key)</c>.</summary>
    public static ExpressionRef GetSection(ExpressionRef config, ExpressionRef key) =>
        config.Call("GetSection", key);

    /// <summary>Produces <c>config.GetValue&lt;T&gt;(key)</c>.</summary>
    public static ExpressionRef GetValue(ExpressionRef config, TypeRef type, ExpressionRef key) =>
        ExpressionRef.From($"{config.Value}.GetValue<{type.Value}>({key.Value})");

    /// <summary>Produces <c>config.GetConnectionString(name)</c>.</summary>
    public static ExpressionRef GetConnectionString(ExpressionRef config, ExpressionRef name) =>
        config.Call("GetConnectionString", name);

    /// <summary>Produces <c>config.GetSection(key).Bind(instance)</c>.</summary>
    public static ExpressionRef Bind(ExpressionRef config, ExpressionRef key, ExpressionRef instance) =>
        config.Call("GetSection", key).Call("Bind", instance);
}

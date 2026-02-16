// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Factory methods for <c>Microsoft.Extensions.Configuration</c> types.
/// </summary>
public static class ConfigurationRefs
{
    /// <summary>Gets the <c>Microsoft.Extensions.Configuration</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("Microsoft.Extensions.Configuration");

    /// <summary>Gets an <c>IConfiguration</c> type reference.</summary>
    public static TypeRef IConfiguration => Namespace.GlobalType("IConfiguration");

    /// <summary>Gets an <c>IConfigurationSection</c> type reference.</summary>
    public static TypeRef IConfigurationSection => Namespace.GlobalType("IConfigurationSection");

    /// <summary>Gets an <c>IConfigurationRoot</c> type reference.</summary>
    public static TypeRef IConfigurationRoot => Namespace.GlobalType("IConfigurationRoot");

    /// <summary>Gets an <c>IConfigurationBuilder</c> type reference.</summary>
    public static TypeRef IConfigurationBuilder => Namespace.GlobalType("IConfigurationBuilder");

    // ── Well-Known API Calls ────────────────────────────────────────────

    /// <summary>Produces a <c>config.GetSection(key)</c> expression.</summary>
    public static ExpressionRef GetSection(ExpressionRef config, ExpressionRef key) =>
        ExpressionRef.From(config).Call("GetSection", key);

    /// <summary>Produces a <c>config.GetValue&lt;T&gt;(key)</c> expression.</summary>
    public static ExpressionRef GetValue(ExpressionRef config, TypeRef type, ExpressionRef key) =>
        ExpressionRef.From(config).Call($"GetValue<{type.Value}>", key);

    /// <summary>Produces a <c>config.GetConnectionString(name)</c> expression.</summary>
    public static ExpressionRef GetConnectionString(ExpressionRef config, ExpressionRef name) =>
        ExpressionRef.From(config).Call("GetConnectionString", name);

    /// <summary>Produces a <c>config.Bind(key, instance)</c> expression.</summary>
    public static ExpressionRef Bind(ExpressionRef config, ExpressionRef key, ExpressionRef instance) =>
        ExpressionRef.From(config).Call("Bind", key, instance);
}
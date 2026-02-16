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
    public static TypeRef IConfiguration => Namespace.Type("IConfiguration");

    /// <summary>Gets an <c>IConfigurationSection</c> type reference.</summary>
    public static TypeRef IConfigurationSection => Namespace.Type("IConfigurationSection");

    /// <summary>Gets an <c>IConfigurationRoot</c> type reference.</summary>
    public static TypeRef IConfigurationRoot => Namespace.Type("IConfigurationRoot");

    /// <summary>Gets an <c>IConfigurationBuilder</c> type reference.</summary>
    public static TypeRef IConfigurationBuilder => Namespace.Type("IConfigurationBuilder");
}

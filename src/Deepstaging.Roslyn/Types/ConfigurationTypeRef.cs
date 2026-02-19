// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// Convenience <see cref="TypeRef"/> constants for <c>Microsoft.Extensions.Configuration</c> types.
/// </summary>
public static class ConfigurationTypes
{
    /// <summary>Gets the <c>Microsoft.Extensions.Configuration</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("Microsoft.Extensions.Configuration");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>IConfiguration</c>.</summary>
    public static TypeRef IConfiguration => Namespace.GlobalType("IConfiguration");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>IConfigurationSection</c>.</summary>
    public static TypeRef IConfigurationSection => Namespace.GlobalType("IConfigurationSection");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>IConfigurationRoot</c>.</summary>
    public static TypeRef IConfigurationRoot => Namespace.GlobalType("IConfigurationRoot");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>IConfigurationBuilder</c>.</summary>
    public static TypeRef IConfigurationBuilder => Namespace.GlobalType("IConfigurationBuilder");
}

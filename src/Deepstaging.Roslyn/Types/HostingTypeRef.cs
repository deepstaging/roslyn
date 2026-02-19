// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// Convenience <see cref="TypeRef"/> constants for <c>Microsoft.Extensions.Hosting</c> types.
/// </summary>
public static class HostingTypes
{
    /// <summary>Gets the <c>Microsoft.Extensions.Hosting</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("Microsoft.Extensions.Hosting");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>BackgroundService</c>.</summary>
    public static TypeRef BackgroundService => Namespace.GlobalType("BackgroundService");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>IHostedService</c>.</summary>
    public static TypeRef IHostedService => Namespace.GlobalType("IHostedService");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>IHost</c>.</summary>
    public static TypeRef IHost => Namespace.GlobalType("IHost");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>IHostApplicationLifetime</c>.</summary>
    public static TypeRef IHostApplicationLifetime => Namespace.GlobalType("IHostApplicationLifetime");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>IHostEnvironment</c>.</summary>
    public static TypeRef IHostEnvironment => Namespace.GlobalType("IHostEnvironment");
}

/// <summary>
/// Convenience <see cref="TypeRef"/> constants for <c>System.Threading.Channels</c> types.
/// </summary>
public static class ChannelTypes
{
    /// <summary>Gets the <c>System.Threading.Channels</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System.Threading.Channels");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>Channel&lt;T&gt;</c>.</summary>
    public static TypeRef Channel(TypeRef itemType) =>
        Namespace.GlobalType($"Channel<{itemType.Value}>");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>BoundedChannelOptions</c>.</summary>
    public static TypeRef BoundedChannelOptions => Namespace.GlobalType("BoundedChannelOptions");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>UnboundedChannelOptions</c>.</summary>
    public static TypeRef UnboundedChannelOptions => Namespace.GlobalType("UnboundedChannelOptions");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>BoundedChannelFullMode</c>.</summary>
    public static TypeRef BoundedChannelFullMode => Namespace.GlobalType("BoundedChannelFullMode");
}

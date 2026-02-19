// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// Convenience <see cref="TypeRef"/> constants for <c>Microsoft.Extensions.DependencyInjection</c> types.
/// </summary>
public static class DependencyInjectionTypes
{
    /// <summary>Gets the <c>Microsoft.Extensions.DependencyInjection</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("Microsoft.Extensions.DependencyInjection");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>IServiceCollection</c>.</summary>
    public static TypeRef IServiceCollection => Namespace.GlobalType("IServiceCollection");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>IServiceProvider</c>.</summary>
    public static TypeRef IServiceProvider => TypeRef.From("global::System.IServiceProvider");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>IServiceScopeFactory</c>.</summary>
    public static TypeRef IServiceScopeFactory => Namespace.GlobalType("IServiceScopeFactory");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>IServiceScope</c>.</summary>
    public static TypeRef IServiceScope => Namespace.GlobalType("IServiceScope");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>ServiceDescriptor</c>.</summary>
    public static TypeRef ServiceDescriptor => Namespace.GlobalType("ServiceDescriptor");
}

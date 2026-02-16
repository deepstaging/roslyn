// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Factory methods for <c>Microsoft.Extensions.DependencyInjection</c> types.
/// </summary>
public static class DependencyInjectionRefs
{
    /// <summary>Gets the <c>Microsoft.Extensions.DependencyInjection</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("Microsoft.Extensions.DependencyInjection");

    /// <summary>Gets an <c>IServiceCollection</c> type reference.</summary>
    public static TypeRef IServiceCollection => Namespace.Type("IServiceCollection");

    /// <summary>Gets an <c>IServiceProvider</c> type reference.</summary>
    public static TypeRef IServiceProvider => TypeRef.From("global::System.IServiceProvider");

    /// <summary>Gets an <c>IServiceScopeFactory</c> type reference.</summary>
    public static TypeRef IServiceScopeFactory => Namespace.Type("IServiceScopeFactory");

    /// <summary>Gets an <c>IServiceScope</c> type reference.</summary>
    public static TypeRef IServiceScope => Namespace.Type("IServiceScope");

    /// <summary>Gets a <c>ServiceDescriptor</c> type reference.</summary>
    public static TypeRef ServiceDescriptor => Namespace.Type("ServiceDescriptor");
}
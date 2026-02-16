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
    public static TypeRef IServiceCollection => Namespace.GlobalType("IServiceCollection");

    /// <summary>Gets an <c>IServiceProvider</c> type reference.</summary>
    public static TypeRef IServiceProvider => TypeRef.From("global::System.IServiceProvider");

    /// <summary>Gets an <c>IServiceScopeFactory</c> type reference.</summary>
    public static TypeRef IServiceScopeFactory => Namespace.GlobalType("IServiceScopeFactory");

    /// <summary>Gets an <c>IServiceScope</c> type reference.</summary>
    public static TypeRef IServiceScope => Namespace.GlobalType("IServiceScope");

    /// <summary>Gets a <c>ServiceDescriptor</c> type reference.</summary>
    public static TypeRef ServiceDescriptor => Namespace.GlobalType("ServiceDescriptor");

    // ── Well-Known API Calls ────────────────────────────────────────────

    /// <summary>Produces a <c>services.AddSingleton&lt;TService, TImpl&gt;()</c> expression.</summary>
    public static ExpressionRef AddSingleton(ExpressionRef services, TypeRef serviceType, TypeRef implType) =>
        ExpressionRef.From(services).Call($"AddSingleton<{serviceType.Value}, {implType.Value}>");

    /// <summary>Produces a <c>services.AddSingleton&lt;TService&gt;()</c> expression.</summary>
    public static ExpressionRef AddSingleton(ExpressionRef services, TypeRef serviceType) =>
        ExpressionRef.From(services).Call($"AddSingleton<{serviceType.Value}>");

    /// <summary>Produces a <c>services.AddScoped&lt;TService, TImpl&gt;()</c> expression.</summary>
    public static ExpressionRef AddScoped(ExpressionRef services, TypeRef serviceType, TypeRef implType) =>
        ExpressionRef.From(services).Call($"AddScoped<{serviceType.Value}, {implType.Value}>");

    /// <summary>Produces a <c>services.AddScoped&lt;TService&gt;()</c> expression.</summary>
    public static ExpressionRef AddScoped(ExpressionRef services, TypeRef serviceType) =>
        ExpressionRef.From(services).Call($"AddScoped<{serviceType.Value}>");

    /// <summary>Produces a <c>services.AddTransient&lt;TService, TImpl&gt;()</c> expression.</summary>
    public static ExpressionRef AddTransient(ExpressionRef services, TypeRef serviceType, TypeRef implType) =>
        ExpressionRef.From(services).Call($"AddTransient<{serviceType.Value}, {implType.Value}>");

    /// <summary>Produces a <c>services.AddTransient&lt;TService&gt;()</c> expression.</summary>
    public static ExpressionRef AddTransient(ExpressionRef services, TypeRef serviceType) =>
        ExpressionRef.From(services).Call($"AddTransient<{serviceType.Value}>");

    /// <summary>Produces a <c>provider.GetRequiredService&lt;T&gt;()</c> expression.</summary>
    public static ExpressionRef GetRequiredService(ExpressionRef provider, TypeRef serviceType) =>
        ExpressionRef.From(provider).Call($"GetRequiredService<{serviceType.Value}>");

    /// <summary>Produces a <c>provider.GetService&lt;T&gt;()</c> expression.</summary>
    public static ExpressionRef GetService(ExpressionRef provider, TypeRef serviceType) =>
        ExpressionRef.From(provider).Call($"GetService<{serviceType.Value}>");
}
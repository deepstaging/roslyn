// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

using Types;

/// <summary>
/// Expression factory for <c>Microsoft.Extensions.DependencyInjection</c> service registration patterns.
/// </summary>
public static class DependencyInjectionExpression
{
    /// <summary>Produces <c>services.AddSingleton&lt;TService, TImpl&gt;()</c>.</summary>
    public static ExpressionRef AddSingleton(ExpressionRef services, TypeRef serviceType, TypeRef implType) =>
        ExpressionRef.From($"{services.Value}.AddSingleton<{serviceType.Value}, {implType.Value}>()");

    /// <summary>Produces <c>services.AddSingleton&lt;TService&gt;()</c>.</summary>
    public static ExpressionRef AddSingleton(ExpressionRef services, TypeRef serviceType) =>
        ExpressionRef.From($"{services.Value}.AddSingleton<{serviceType.Value}>()");

    /// <summary>Produces <c>services.AddScoped&lt;TService, TImpl&gt;()</c>.</summary>
    public static ExpressionRef AddScoped(ExpressionRef services, TypeRef serviceType, TypeRef implType) =>
        ExpressionRef.From($"{services.Value}.AddScoped<{serviceType.Value}, {implType.Value}>()");

    /// <summary>Produces <c>services.AddScoped&lt;TService&gt;()</c>.</summary>
    public static ExpressionRef AddScoped(ExpressionRef services, TypeRef serviceType) =>
        ExpressionRef.From($"{services.Value}.AddScoped<{serviceType.Value}>()");

    /// <summary>Produces <c>services.AddTransient&lt;TService, TImpl&gt;()</c>.</summary>
    public static ExpressionRef AddTransient(ExpressionRef services, TypeRef serviceType, TypeRef implType) =>
        ExpressionRef.From($"{services.Value}.AddTransient<{serviceType.Value}, {implType.Value}>()");

    /// <summary>Produces <c>services.AddTransient&lt;TService&gt;()</c>.</summary>
    public static ExpressionRef AddTransient(ExpressionRef services, TypeRef serviceType) =>
        ExpressionRef.From($"{services.Value}.AddTransient<{serviceType.Value}>()");

    /// <summary>Produces <c>provider.GetRequiredService&lt;T&gt;()</c>.</summary>
    public static ExpressionRef GetRequiredService(ExpressionRef provider, TypeRef serviceType) =>
        ExpressionRef.From($"{provider.Value}.GetRequiredService<{serviceType.Value}>()");

    /// <summary>Produces <c>provider.GetService&lt;T&gt;()</c>.</summary>
    public static ExpressionRef GetService(ExpressionRef provider, TypeRef serviceType) =>
        ExpressionRef.From($"{provider.Value}.GetService<{serviceType.Value}>()");
}

// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

using Types;

/// <summary>
/// Expression factory for <c>Microsoft.Extensions.Hosting</c> service registration patterns.
/// </summary>
public static class HostingExpression
{
    /// <summary>Produces <c>services.AddHostedService&lt;T&gt;()</c>.</summary>
    public static ExpressionRef AddHostedService(ExpressionRef services, TypeRef serviceType) =>
        ExpressionRef.From($"{services.Value}.AddHostedService<{serviceType.Value}>()");

    /// <summary>
    /// Produces <c>services.AddHostedService(sp =&gt; sp.GetRequiredService&lt;T&gt;())</c>.
    /// Registers an already-registered service as a hosted service.
    /// </summary>
    public static ExpressionRef AddHostedServiceFromProvider(ExpressionRef services, TypeRef serviceType) =>
        ExpressionRef.From(
            $"{services.Value}.AddHostedService(sp => sp.GetRequiredService<{serviceType.Value}>())");
}

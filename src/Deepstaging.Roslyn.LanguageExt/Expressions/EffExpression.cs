// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Expressions;

/// <summary>
/// Entry point for building LanguageExt effect expressions.
/// </summary>
/// <remarks>
/// Provides factory methods for <see cref="EffLift"/> and <see cref="EffLiftIO"/> builders
/// that compose <c>liftEff</c> and <c>Eff.LiftIO</c> expressions for source generation.
/// </remarks>
public static class EffExpression
{
    /// <summary>
    /// Creates a <see cref="EffLift"/> builder for composing <c>liftEff&lt;RT, T&gt;(...)</c> expressions.
    /// </summary>
    /// <param name="rt">The runtime type parameter name (e.g., <c>"RT"</c>).</param>
    /// <param name="param">The lambda parameter name (e.g., <c>"rt"</c>).</param>
    public static EffLift Lift(string rt, string param) => new(rt, param);

    /// <summary>
    /// Creates an <see cref="EffLiftIO"/> builder for composing <c>{EffType}.LiftIO(...)</c> expressions.
    /// </summary>
    /// <param name="effType">The Eff type to call LiftIO on (e.g., <c>Eff&lt;RT, int&gt;</c>).</param>
    /// <param name="param">The lambda parameter name (e.g., <c>"rt"</c>).</param>
    public static EffLiftIO LiftIO(string effType, string param) => new(effType, param);
}

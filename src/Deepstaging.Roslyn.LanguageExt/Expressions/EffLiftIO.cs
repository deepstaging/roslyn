// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Expressions;

/// <summary>
/// Builds <c>Eff.LiftIO</c> expressions for lifting IO operations into a typed Eff.
/// </summary>
/// <remarks>
/// Captures the Eff type and lambda parameter name, then provides convenience methods
/// for common async lifting patterns. Used primarily for terminal query operations
/// where the Eff return type is already known.
/// </remarks>
/// <param name="effType">The Eff type string to call LiftIO on (e.g., <c>"Eff&lt;RT, int&gt;"</c>).</param>
/// <param name="param">The lambda parameter name (e.g., <c>"rt"</c>).</param>
public readonly struct EffLiftIO(string effType, string param)
{
    /// <summary>
    /// <c>{effType}.LiftIO(async {param} =&gt; await {expr})</c>
    /// </summary>
    /// <param name="expr">The awaitable expression (without <c>await</c>).</param>
    public string Async(string expr) =>
        $"{effType}.LiftIO(async {param} => await {expr})";

    /// <summary>
    /// <c>{effType}.LiftIO(async {param} =&gt; Optional(await {expr}))</c>
    /// </summary>
    /// <param name="expr">The awaitable expression that may return null (without <c>await</c>).</param>
    public string AsyncOptional(string expr) =>
        $"{effType}.LiftIO(async {param} => Optional(await {expr}))";

    /// <summary>
    /// <c>{effType}.LiftIO(async {param} =&gt; (await {expr})!)</c>
    /// </summary>
    /// <param name="expr">The awaitable expression with null-forgiving assertion.</param>
    public string AsyncNonNull(string expr) =>
        $"{effType}.LiftIO(async {param} => (await {expr})!)";

    /// <summary>
    /// <c>{effType}.LiftIO({lambdaBody})</c> — escape hatch for custom lambda bodies.
    /// </summary>
    /// <param name="lambdaBody">The complete lambda body including parameter.</param>
    public string Body(string lambdaBody) =>
        $"{effType}.LiftIO({lambdaBody})";
}

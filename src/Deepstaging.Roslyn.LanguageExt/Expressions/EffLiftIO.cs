// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Expressions;

using Refs;

/// <summary>
/// Builds <c>Eff.LiftIO</c> expressions for lifting IO operations into a typed Eff.
/// </summary>
/// <remarks>
/// Captures the Eff type and lambda parameter name, then provides convenience methods
/// for common async lifting patterns. Used primarily for terminal query operations
/// where the Eff return type is already known.
/// </remarks>
/// <param name="effType">The Eff type reference carrying the runtime and result types.</param>
/// <param name="param">The lambda parameter name (e.g., <c>"rt"</c>).</param>
public readonly struct EffLiftIO(EffTypeRef effType, string param)
{
    /// <summary>
    /// <c>{effType}.LiftIO(async {param} =&gt; await {expr})</c>
    /// </summary>
    /// <param name="expr">The awaitable expression (without <c>await</c>).</param>
    public string Async(string expr) =>
        $"{effType}.LiftIO(async {param} => await {expr})";

    /// <summary>
    /// <c>{effType}.LiftIO(async {param} =&gt; {{ await {expr}; return unit; }})</c>
    /// </summary>
    /// <param name="expr">The awaitable void expression (without <c>await</c>).</param>
    public string AsyncVoid(string expr) =>
        $"{effType}.LiftIO(async {param} => {{ await {expr}; return unit; }})";

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
    /// <c>{effType}.LiftIO({param} =&gt; {expr})</c>
    /// </summary>
    /// <param name="expr">The synchronous expression.</param>
    public string Sync(string expr) =>
        $"{effType}.LiftIO({param} => {expr})";

    /// <summary>
    /// <c>{effType}.LiftIO({param} =&gt; {{ {expr}; return unit; }})</c>
    /// </summary>
    /// <param name="expr">The synchronous void expression.</param>
    public string SyncVoid(string expr) =>
        $"{effType}.LiftIO({param} => {{ {expr}; return unit; }})";

    /// <summary>
    /// <c>{effType}.LiftIO({param} =&gt; Optional({expr}))</c>
    /// </summary>
    /// <param name="expr">The synchronous expression that may return null.</param>
    public string SyncOptional(string expr) =>
        $"{effType}.LiftIO({param} => Optional({expr}))";

    /// <summary>
    /// <c>{effType}.LiftIO({param} =&gt; ({expr})!)</c>
    /// </summary>
    /// <param name="expr">The synchronous expression with null-forgiving assertion.</param>
    public string SyncNonNull(string expr) =>
        $"{effType}.LiftIO({param} => ({expr})!)";

    /// <summary>
    /// <c>{effType}.LiftIO({lambdaBody})</c> — escape hatch for custom lambda bodies.
    /// </summary>
    /// <param name="lambdaBody">The complete lambda body including parameter.</param>
    public string Body(string lambdaBody) =>
        $"{effType}.LiftIO({lambdaBody})";
}

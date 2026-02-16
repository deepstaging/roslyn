// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Expressions;

using Refs;

/// <summary>
/// Builds <c>liftEff</c> expressions for wrapping operations into the Eff monad.
/// </summary>
/// <remarks>
/// Captures the runtime type parameter and lambda parameter name, then provides convenience
/// methods for the common lifting strategies. The caller provides just the result type and
/// inner expression — the builder handles <c>async/await</c>, <c>Optional</c>, and <c>return unit</c> boilerplate.
/// </remarks>
/// <param name="rt">The runtime type parameter name (e.g., <c>"RT"</c>).</param>
/// <param name="param">The lambda parameter name (e.g., <c>"rt"</c>).</param>
public readonly struct EffLift(string rt, string param)
{
    /// <summary>
    /// <c>liftEff&lt;{rt}, {result}&gt;(async {param} =&gt; await {expr})</c>
    /// </summary>
    /// <param name="result">The result type (e.g., <c>"int"</c>, <c>"string"</c>).</param>
    /// <param name="expr">The awaitable expression (without <c>await</c>).</param>
    public string Async(string result, string expr) =>
        $"liftEff<{rt}, {result}>(async {param} => await {expr})";

    /// <summary>
    /// <c>liftEff&lt;{rt}, Unit&gt;(async {param} =&gt; {{ await {expr}; return unit; }})</c>
    /// </summary>
    /// <param name="expr">The awaitable void expression (without <c>await</c>).</param>
    public string AsyncVoid(string expr) =>
        $"liftEff<{rt}, Unit>(async {param} => {{ await {expr}; return unit; }})";

    /// <summary>
    /// <c>liftEff&lt;{rt}, Option&lt;T&gt;&gt;(async {param} =&gt; Optional(await {expr}))</c>
    /// </summary>
    /// <param name="result">The Option type reference carrying the inner type.</param>
    /// <param name="expr">The awaitable expression that may return null (without <c>await</c>).</param>
    public string AsyncOptional(OptionTypeRef result, string expr) =>
        $"liftEff<{rt}, {result}>(async {param} => Optional(await {expr}))";

    /// <summary>
    /// <c>liftEff&lt;{rt}, {result}&gt;({param} =&gt; {expr})</c>
    /// </summary>
    /// <param name="result">The result type.</param>
    /// <param name="expr">The synchronous expression.</param>
    public string Sync(string result, string expr) =>
        $"liftEff<{rt}, {result}>({param} => {expr})";

    /// <summary>
    /// <c>liftEff&lt;{rt}, Unit&gt;({param} =&gt; {{ {expr}; return unit; }})</c>
    /// </summary>
    /// <param name="expr">The synchronous void expression.</param>
    public string SyncVoid(string expr) =>
        $"liftEff<{rt}, Unit>({param} => {{ {expr}; return unit; }})";

    /// <summary>
    /// <c>liftEff&lt;{rt}, Option&lt;T&gt;&gt;({param} =&gt; Optional({expr}))</c>
    /// </summary>
    /// <param name="result">The Option type reference carrying the inner type.</param>
    /// <param name="expr">The synchronous expression that may return null.</param>
    public string SyncOptional(OptionTypeRef result, string expr) =>
        $"liftEff<{rt}, {result}>({param} => Optional({expr}))";

    /// <summary>
    /// <c>liftEff&lt;{rt}, {result}&gt;({lambdaBody})</c> — escape hatch for custom lambda bodies.
    /// </summary>
    /// <param name="result">The result type.</param>
    /// <param name="lambdaBody">The complete lambda body including parameter (e.g., <c>"rt =&gt; {{ ...; return val; }}"</c>).</param>
    public string Body(string result, string lambdaBody) =>
        $"liftEff<{rt}, {result}>({lambdaBody})";
}

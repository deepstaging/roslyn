// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

/// <summary>
/// Builds <c>Task</c> and <c>Task&lt;T&gt;</c> expressions for use in generated code.
/// </summary>
/// <example>
/// <code>
/// TaskExpression.CompletedTask
///     // → "global::System.Threading.Tasks.Task.CompletedTask"
///
/// TaskExpression.FromResult("string", "value")
///     // → "global::System.Threading.Tasks.Task.FromResult&lt;string&gt;(value)"
/// </code>
/// </example>
public static class TaskExpression
{
    private static readonly TypeRef TaskType =
        TypeRef.Global("System.Threading.Tasks.Task");

    private static readonly TypeRef ValueTaskType =
        TypeRef.Global("System.Threading.Tasks.ValueTask");

    // ── Task ────────────────────────────────────────────────────────────

    /// <summary>Gets a <c>Task.CompletedTask</c> expression.</summary>
    public static ExpressionRef CompletedTask => TaskType.Member("CompletedTask");

    /// <summary>Produces a <c>Task.FromResult(value)</c> expression.</summary>
    /// <param name="value">The result value.</param>
    public static ExpressionRef FromResult(ExpressionRef value) =>
        TaskType.Call("FromResult", value);

    /// <summary>Produces a <c>Task.FromResult&lt;T&gt;(value)</c> expression with explicit type argument.</summary>
    /// <param name="resultType">The result type.</param>
    /// <param name="value">The result value.</param>
    public static ExpressionRef FromResult(TypeRef resultType, ExpressionRef value) =>
        TaskType.Call($"FromResult<{resultType.Value}>", value);

    /// <summary>Produces a <c>Task.Run(expression)</c> expression.</summary>
    /// <param name="expression">The delegate expression to run.</param>
    public static ExpressionRef Run(ExpressionRef expression) =>
        TaskType.Call("Run", expression);

    /// <summary>Produces a <c>Task.Delay(delay)</c> expression.</summary>
    /// <param name="delay">The delay expression.</param>
    public static ExpressionRef Delay(ExpressionRef delay) =>
        TaskType.Call("Delay", delay);

    /// <summary>Produces a <c>Task.WhenAll(tasks)</c> expression.</summary>
    /// <param name="tasks">The task expressions.</param>
    public static ExpressionRef WhenAll(params ExpressionRef[] tasks) =>
        TaskType.Call("WhenAll", tasks);

    /// <summary>Produces a <c>Task.WhenAny(tasks)</c> expression.</summary>
    /// <param name="tasks">The task expressions.</param>
    public static ExpressionRef WhenAny(params ExpressionRef[] tasks) =>
        TaskType.Call("WhenAny", tasks);

    // ── ValueTask ───────────────────────────────────────────────────────

    /// <summary>Gets a <c>ValueTask.CompletedTask</c> expression.</summary>
    public static ExpressionRef CompletedValueTask => ValueTaskType.Member("CompletedTask");

    /// <summary>Produces a <c>ValueTask.FromResult(value)</c> expression.</summary>
    /// <param name="value">The result value.</param>
    public static ExpressionRef FromValueTaskResult(ExpressionRef value) =>
        ValueTaskType.Call("FromResult", value);

    /// <summary>Produces a <c>ValueTask.FromResult&lt;T&gt;(value)</c> expression with explicit type argument.</summary>
    /// <param name="resultType">The result type.</param>
    /// <param name="value">The result value.</param>
    public static ExpressionRef FromValueTaskResult(TypeRef resultType, ExpressionRef value) =>
        ValueTaskType.Call($"FromResult<{resultType.Value}>", value);
}
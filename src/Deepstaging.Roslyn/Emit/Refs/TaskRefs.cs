// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Factory methods for <c>System.Threading.Tasks</c> and <c>System.Threading</c> types.
/// </summary>
public static class TaskRefs
{
    /// <summary>Gets the <c>System.Threading.Tasks</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System.Threading.Tasks");

    /// <summary>Gets the <c>System.Threading</c> namespace.</summary>
    public static NamespaceRef ThreadingNamespace => NamespaceRef.From("System.Threading");

    /// <summary>Creates a <c>Task</c> type reference (non-generic).</summary>
    public static TypeRef Task() => Namespace.GlobalType("Task");

    /// <summary>Creates a <c>Task&lt;T&gt;</c> type reference.</summary>
    public static TypeRef Task(TypeRef resultType) =>
        Namespace.GlobalType($"Task<{resultType.Value}>");

    /// <summary>Creates a <c>ValueTask</c> type reference (non-generic).</summary>
    public static TypeRef ValueTask() => Namespace.GlobalType("ValueTask");

    /// <summary>Creates a <c>ValueTask&lt;T&gt;</c> type reference.</summary>
    public static TypeRef ValueTask(TypeRef resultType) =>
        Namespace.GlobalType($"ValueTask<{resultType.Value}>");

    /// <summary>Gets a <c>Task.CompletedTask</c> expression for use in return statements.</summary>
    public static ExpressionRef CompletedTask => ExpressionRef.From("global::System.Threading.Tasks.Task.CompletedTask");

    /// <summary>Gets a <c>ValueTask.CompletedTask</c> expression for use in return statements.</summary>
    public static ExpressionRef CompletedValueTask => ExpressionRef.From("global::System.Threading.Tasks.ValueTask.CompletedTask");

    /// <summary>Gets a <c>CancellationToken</c> type reference.</summary>
    public static TypeRef CancellationToken => ThreadingNamespace.GlobalType("CancellationToken");

    // ── Well-Known API Calls ────────────────────────────────────────────

    /// <summary>Produces a <c>Task.FromResult(value)</c> expression.</summary>
    public static ExpressionRef FromResult(ExpressionRef value) =>
        Task().Call("FromResult", value);

    /// <summary>Produces a <c>Task.FromResult&lt;T&gt;(value)</c> expression.</summary>
    public static ExpressionRef FromResult(TypeRef resultType, ExpressionRef value) =>
        Task().Call($"FromResult<{resultType.Value}>", value);

    /// <summary>Produces a <c>Task.Run(expression)</c> expression.</summary>
    public static ExpressionRef Run(ExpressionRef expression) =>
        Task().Call("Run", expression);

    /// <summary>Produces a <c>Task.Delay(delay)</c> expression.</summary>
    public static ExpressionRef Delay(ExpressionRef delay) =>
        Task().Call("Delay", delay);

    /// <summary>Produces a <c>Task.WhenAll(tasks)</c> expression.</summary>
    public static ExpressionRef WhenAll(params ExpressionRef[] tasks) =>
        Task().Call("WhenAll", tasks);

    /// <summary>Produces a <c>Task.WhenAny(tasks)</c> expression.</summary>
    public static ExpressionRef WhenAny(params ExpressionRef[] tasks) =>
        Task().Call("WhenAny", tasks);
}
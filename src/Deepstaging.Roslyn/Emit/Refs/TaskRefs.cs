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
    public static TypeRef Task() => Namespace.Type("Task");

    /// <summary>Creates a <c>Task&lt;T&gt;</c> type reference.</summary>
    public static TypeRef Task(TypeRef resultType) =>
        Namespace.Type($"Task<{resultType.Value}>");

    /// <summary>Creates a <c>ValueTask</c> type reference (non-generic).</summary>
    public static TypeRef ValueTask() => Namespace.Type("ValueTask");

    /// <summary>Creates a <c>ValueTask&lt;T&gt;</c> type reference.</summary>
    public static TypeRef ValueTask(TypeRef resultType) =>
        Namespace.Type($"ValueTask<{resultType.Value}>");

    /// <summary>Gets a <c>Task.CompletedTask</c> expression for use in return statements.</summary>
    public static TypeRef CompletedTask => TypeRef.From($"global::System.Threading.Tasks.Task.CompletedTask");

    /// <summary>Gets a <c>ValueTask.CompletedTask</c> expression for use in return statements.</summary>
    public static TypeRef CompletedValueTask => TypeRef.From($"global::System.Threading.Tasks.ValueTask.CompletedTask");

    /// <summary>Gets a <c>CancellationToken</c> type reference.</summary>
    public static TypeRef CancellationToken => ThreadingNamespace.Type("CancellationToken");
}
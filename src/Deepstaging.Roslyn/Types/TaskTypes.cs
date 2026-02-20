// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// Convenience <see cref="NamespaceRef"/> and factory methods for <c>System.Threading.Tasks</c> types.
/// </summary>
public static class TaskTypes
{
    /// <summary>Gets the <c>System.Threading.Tasks</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System.Threading.Tasks");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>CancellationToken</c>.</summary>
    public static TypeRef CancellationToken => TypeRef.Global("System.Threading.CancellationToken");

    /// <summary>Gets a <see cref="TypeRef"/> for non-generic <c>Task</c>.</summary>
    public static TypeRef Task() => TypeRef.Global("System.Threading.Tasks.Task");

    /// <summary>Creates a <c>Task&lt;T&gt;</c> type reference.</summary>
    public static TaskTypeRef Task(TypeRef resultType) => new(resultType);

    /// <summary>Gets a <see cref="TypeRef"/> for non-generic <c>ValueTask</c>.</summary>
    public static TypeRef ValueTask() => TypeRef.Global("System.Threading.Tasks.ValueTask");

    /// <summary>Creates a <c>ValueTask&lt;T&gt;</c> type reference.</summary>
    public static ValueTaskTypeRef ValueTask(TypeRef resultType) => new(resultType);
}

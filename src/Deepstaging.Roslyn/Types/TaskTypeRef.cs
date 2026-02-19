// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// A type-safe wrapper representing a <c>Task&lt;T&gt;</c> type reference.
/// Carries the result type so expression builders can produce
/// <c>Task.FromResult&lt;T&gt;(value)</c> without losing the generic argument.
/// </summary>
public readonly record struct TaskTypeRef
{
    /// <summary>Gets the result type (e.g., <c>"int"</c>).</summary>
    public TypeRef ResultType { get; }

    /// <summary>Creates a <c>TaskTypeRef</c> for the given result type.</summary>
    public TaskTypeRef(TypeRef resultType) => ResultType = resultType;

    /// <summary>Gets the globally qualified <c>Task&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Threading.Tasks.Task<{ResultType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(TaskTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TaskTypeRef self) =>
        self.ToString();
}
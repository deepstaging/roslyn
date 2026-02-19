// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// A type-safe wrapper representing a <c>ValueTask&lt;T&gt;</c> type reference.
/// Carries the result type so expression builders can produce typed ValueTask expressions.
/// </summary>
public readonly record struct ValueTaskTypeRef
{
    /// <summary>Gets the result type (e.g., <c>"int"</c>).</summary>
    public TypeRef ResultType { get; }

    /// <summary>Creates a <c>ValueTaskTypeRef</c> for the given result type.</summary>
    public ValueTaskTypeRef(TypeRef resultType) => ResultType = resultType;

    /// <summary>Gets the globally qualified <c>ValueTask&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Threading.Tasks.ValueTask<{ResultType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(ValueTaskTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(ValueTaskTypeRef self) =>
        self.ToString();
}
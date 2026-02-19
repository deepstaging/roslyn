// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// A type-safe wrapper representing a <c>Lazy&lt;T&gt;</c> type reference.
/// Carries the value type for typed expression building.
/// </summary>
public readonly record struct LazyTypeRef
{
    /// <summary>Gets the value type (e.g., <c>"ExpensiveService"</c>).</summary>
    public TypeRef ValueType { get; }

    /// <summary>Creates a <c>LazyTypeRef</c> for the given value type.</summary>
    public LazyTypeRef(TypeRef valueType) => ValueType = valueType;

    /// <summary>Gets the globally qualified <c>Lazy&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Lazy<{ValueType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(LazyTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(LazyTypeRef self) =>
        self.ToString();
}
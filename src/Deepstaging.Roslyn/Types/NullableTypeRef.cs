// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// A type-safe wrapper representing a <c>Nullable&lt;T&gt;</c> type reference.
/// Carries the inner type so expression builders can produce typed nullable expressions.
/// </summary>
public readonly record struct NullableTypeRef
{
    /// <summary>Gets the underlying value type (e.g., <c>"int"</c>).</summary>
    public TypeRef InnerType { get; }

    /// <summary>Creates a <c>NullableTypeRef</c> for the given underlying value type.</summary>
    public NullableTypeRef(TypeRef innerType) => InnerType = innerType;

    /// <summary>Gets the globally qualified <c>Nullable&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Nullable<{InnerType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(NullableTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(NullableTypeRef self) =>
        self.ToString();
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// A type-safe wrapper representing a <c>Span&lt;T&gt;</c> type reference.
/// Carries the element type for typed expression building.
/// </summary>
public readonly record struct SpanTypeRef
{
    /// <summary>Gets the element type (e.g., <c>"byte"</c>).</summary>
    public TypeRef ElementType { get; }

    /// <summary>Creates a <c>SpanTypeRef</c> for the given element type.</summary>
    public SpanTypeRef(TypeRef elementType) => ElementType = elementType;

    /// <summary>Gets the globally qualified <c>Span&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Span<{ElementType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(SpanTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(SpanTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing a <c>ReadOnlySpan&lt;T&gt;</c> type reference.
/// Carries the element type for typed expression building.
/// </summary>
public readonly record struct ReadOnlySpanTypeRef
{
    /// <summary>Gets the element type (e.g., <c>"byte"</c>).</summary>
    public TypeRef ElementType { get; }

    /// <summary>Creates a <c>ReadOnlySpanTypeRef</c> for the given element type.</summary>
    public ReadOnlySpanTypeRef(TypeRef elementType) => ElementType = elementType;

    /// <summary>Gets the globally qualified <c>ReadOnlySpan&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.ReadOnlySpan<{ElementType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(ReadOnlySpanTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(ReadOnlySpanTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing a <c>Memory&lt;T&gt;</c> type reference.
/// Carries the element type for typed expression building.
/// </summary>
public readonly record struct MemoryTypeRef
{
    /// <summary>Gets the element type (e.g., <c>"byte"</c>).</summary>
    public TypeRef ElementType { get; }

    /// <summary>Creates a <c>MemoryTypeRef</c> for the given element type.</summary>
    public MemoryTypeRef(TypeRef elementType) => ElementType = elementType;

    /// <summary>Gets the globally qualified <c>Memory&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Memory<{ElementType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(MemoryTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(MemoryTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing a <c>ReadOnlyMemory&lt;T&gt;</c> type reference.
/// Carries the element type for typed expression building.
/// </summary>
public readonly record struct ReadOnlyMemoryTypeRef
{
    /// <summary>Gets the element type (e.g., <c>"byte"</c>).</summary>
    public TypeRef ElementType { get; }

    /// <summary>Creates a <c>ReadOnlyMemoryTypeRef</c> for the given element type.</summary>
    public ReadOnlyMemoryTypeRef(TypeRef elementType) => ElementType = elementType;

    /// <summary>Gets the globally qualified <c>ReadOnlyMemory&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.ReadOnlyMemory<{ElementType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(ReadOnlyMemoryTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(ReadOnlyMemoryTypeRef self) =>
        self.ToString();
}

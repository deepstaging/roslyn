// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// Convenience <see cref="NamespaceRef"/> and factory methods for <c>System.Span</c>, <c>System.Memory</c>, and their read-only variants.
/// </summary>
public static class MemoryTypes
{
    /// <summary>Creates a <c>Span&lt;T&gt;</c> type reference.</summary>
    public static SpanTypeRef Span(TypeRef elementType) => new(elementType);

    /// <summary>Creates a <c>ReadOnlySpan&lt;T&gt;</c> type reference.</summary>
    public static ReadOnlySpanTypeRef ReadOnlySpan(TypeRef elementType) => new(elementType);

    /// <summary>Creates a <c>Memory&lt;T&gt;</c> type reference.</summary>
    public static MemoryTypeRef Memory(TypeRef elementType) => new(elementType);

    /// <summary>Creates a <c>ReadOnlyMemory&lt;T&gt;</c> type reference.</summary>
    public static ReadOnlyMemoryTypeRef ReadOnlyMemory(TypeRef elementType) => new(elementType);
}

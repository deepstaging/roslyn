// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Factory methods for <c>System.Collections.Immutable</c> types.
/// </summary>
public static class ImmutableCollectionRefs
{
    /// <summary>Gets the <c>System.Collections.Immutable</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System.Collections.Immutable");

    /// <summary>Creates an <c>ImmutableArray&lt;T&gt;</c> type reference.</summary>
    public static TypeRef ImmutableArray(TypeRef elementType) =>
        Namespace.Type($"ImmutableArray<{elementType.Value}>");

    /// <summary>Creates an <c>ImmutableList&lt;T&gt;</c> type reference.</summary>
    public static TypeRef ImmutableList(TypeRef elementType) =>
        Namespace.Type($"ImmutableList<{elementType.Value}>");

    /// <summary>Creates an <c>ImmutableDictionary&lt;TKey, TValue&gt;</c> type reference.</summary>
    public static TypeRef ImmutableDictionary(TypeRef keyType, TypeRef valueType) =>
        Namespace.Type($"ImmutableDictionary<{keyType.Value}, {valueType.Value}>");
}

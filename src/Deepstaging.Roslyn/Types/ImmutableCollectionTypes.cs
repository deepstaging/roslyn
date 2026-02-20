// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// Convenience <see cref="NamespaceRef"/> and factory methods for <c>System.Collections.Immutable</c> types.
/// </summary>
public static class ImmutableCollectionTypes
{
    /// <summary>Gets the <c>System.Collections.Immutable</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System.Collections.Immutable");

    /// <summary>Creates an <c>ImmutableArray&lt;T&gt;</c> type reference.</summary>
    public static ImmutableArrayTypeRef ImmutableArray(TypeRef elementType) => new(elementType);

    /// <summary>Creates an <c>ImmutableList&lt;T&gt;</c> type reference.</summary>
    public static ImmutableListTypeRef ImmutableList(TypeRef elementType) => new(elementType);

    /// <summary>Creates an <c>ImmutableDictionary&lt;TKey, TValue&gt;</c> type reference.</summary>
    public static ImmutableDictionaryTypeRef ImmutableDictionary(TypeRef keyType, TypeRef valueType) => new(keyType, valueType);
}

// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// Convenience factory methods for <c>System.Collections.Generic.EqualityComparer&lt;T&gt;</c>
/// and <c>System.Collections.Generic.Comparer&lt;T&gt;</c> types.
/// </summary>
public static class ComparerTypes
{
    /// <summary>Creates an <c>EqualityComparer&lt;T&gt;</c> type reference.</summary>
    public static EqualityComparerTypeRef EqualityComparer(TypeRef comparedType) => new(comparedType);

    /// <summary>Creates a <c>Comparer&lt;T&gt;</c> type reference.</summary>
    public static ComparerTypeRef Comparer(TypeRef comparedType) => new(comparedType);
}

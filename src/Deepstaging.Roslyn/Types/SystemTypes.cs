// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// Convenience factory methods for <c>System.Nullable&lt;T&gt;</c> and <c>System.Lazy&lt;T&gt;</c> types.
/// </summary>
public static class SystemTypes
{
    /// <summary>Creates a <c>Nullable&lt;T&gt;</c> type reference.</summary>
    public static NullableTypeRef Nullable(TypeRef innerType) => new(innerType);

    /// <summary>Creates a <c>Lazy&lt;T&gt;</c> type reference.</summary>
    public static LazyTypeRef Lazy(TypeRef valueType) => new(valueType);
}

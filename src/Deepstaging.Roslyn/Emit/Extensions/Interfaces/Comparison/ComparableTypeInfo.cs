// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Core;

namespace Deepstaging.Roslyn.Emit.Interfaces.Comparison;

/// <summary>
/// Backing type information for implementing IComparable&lt;T&gt;.
/// Analyzes comparison-related capabilities of the backing type.
/// </summary>
internal readonly struct ComparableTypeInfo
{
    /// <summary>Gets the core backing type information.</summary>
    public BackingTypeCore Core { get; }

    /// <summary>Gets whether the type requires null-safe operations.</summary>
    public bool RequiresNullHandling => Core.RequiresNullHandling;

    /// <summary>Gets the simple type name.</summary>
    public string Name => Core.Name;

    private ComparableTypeInfo(BackingTypeCore core)
    {
        Core = core;
    }

    /// <summary>
    /// Creates a ComparableTypeInfo from the given type symbol.
    /// </summary>
    public static ComparableTypeInfo From(TypeSnapshot type)
    {
        return new ComparableTypeInfo(BackingTypeCore.From(type));
    }
}

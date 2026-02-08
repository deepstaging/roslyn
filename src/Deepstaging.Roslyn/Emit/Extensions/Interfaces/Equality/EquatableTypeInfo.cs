// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Core;

namespace Deepstaging.Roslyn.Emit.Interfaces.Equality;

/// <summary>
/// Backing type information for implementing IEquatable&lt;T&gt;.
/// Analyzes equality-related capabilities of the backing type.
/// </summary>
internal readonly struct EquatableTypeInfo
{
    /// <summary>Gets the core backing type information.</summary>
    public BackingTypeCore Core { get; }

    /// <summary>Gets whether the type requires null-safe operations.</summary>
    public bool RequiresNullHandling => Core.RequiresNullHandling;

    /// <summary>Gets the simple type name.</summary>
    public string Name => Core.Name;

    private EquatableTypeInfo(BackingTypeCore core)
    {
        Core = core;
    }

    /// <summary>
    /// Creates an EquatableTypeInfo from the given type symbol.
    /// </summary>
    public static EquatableTypeInfo From(ValidSymbol<INamedTypeSymbol> type)
    {
        return new EquatableTypeInfo(BackingTypeCore.From(type));
    }
}

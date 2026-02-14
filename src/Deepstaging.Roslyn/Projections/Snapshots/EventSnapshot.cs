// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Pipeline-safe snapshot of an event symbol.
/// Mirrors <see cref="ValidSymbol{TSymbol}"/> for <c>IEventSymbol</c>.
/// </summary>
public sealed record EventSnapshot : SymbolSnapshot, IEquatable<EventSnapshot>
{
    /// <summary>
    /// Gets the globally qualified type name of the event.
    /// </summary>
    public required string Type { get; init; }
}

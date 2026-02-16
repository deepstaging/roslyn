// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Pipeline-safe snapshot of a property symbol.
/// Mirrors <see cref="ValidSymbol{TSymbol}"/> for <c>IPropertySymbol</c>.
/// </summary>
public sealed record PropertySnapshot : SymbolSnapshot, IEquatable<PropertySnapshot>
{
    /// <summary>
    /// Gets the globally qualified type name of the property.
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Gets a value indicating whether the property has a getter.
    /// </summary>
    public bool HasGetter { get; init; }

    /// <summary>
    /// Gets a value indicating whether the property has a setter.
    /// </summary>
    public bool HasSetter { get; init; }

    /// <summary>
    /// Gets a value indicating whether the property has an init-only setter.
    /// </summary>
    public bool IsInitOnly { get; init; }

    /// <summary>
    /// Gets a value indicating whether the property is required.
    /// </summary>
    public bool IsRequired { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is an indexer.
    /// </summary>
    public bool IsIndexer { get; init; }

    /// <summary>
    /// Gets the parameters of the indexer, or empty if not an indexer.
    /// </summary>
    public EquatableArray<ParameterSnapshot> Parameters { get; init; } = [];
}
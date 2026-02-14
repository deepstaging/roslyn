// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Pipeline-safe snapshot of a parameter symbol.
/// Mirrors <see cref="ValidSymbol{TSymbol}"/> for <c>IParameterSymbol</c>.
/// </summary>
public sealed record ParameterSnapshot : SymbolSnapshot, IEquatable<ParameterSnapshot>
{
    /// <summary>
    /// Gets the globally qualified type name of the parameter.
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Gets a value indicating whether the parameter has an explicit default value.
    /// </summary>
    public bool HasExplicitDefaultValue { get; init; }

    /// <summary>
    /// Gets the default value expression as a string, or null if no default.
    /// </summary>
    public string? DefaultValueExpression { get; init; }

    /// <summary>
    /// Gets the ref kind of the parameter (None, Ref, Out, In, RefReadOnlyParameter).
    /// </summary>
    public RefKind RefKind { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is a params parameter.
    /// </summary>
    public bool IsParams { get; init; }

    /// <summary>
    /// Gets a value indicating whether this parameter is optional.
    /// </summary>
    public bool IsOptional { get; init; }
}

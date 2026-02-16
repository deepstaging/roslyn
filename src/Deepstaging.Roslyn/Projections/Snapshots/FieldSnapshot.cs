// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Pipeline-safe snapshot of a field symbol.
/// Mirrors <see cref="ValidSymbol{TSymbol}"/> for <c>IFieldSymbol</c>.
/// </summary>
public sealed record FieldSnapshot : SymbolSnapshot, IEquatable<FieldSnapshot>
{
    /// <summary>
    /// Gets the globally qualified type name of the field.
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Gets a value indicating whether the field is const.
    /// </summary>
    public bool IsConst { get; init; }

    /// <summary>
    /// Gets a value indicating whether the field is volatile.
    /// </summary>
    public bool IsVolatile { get; init; }

    /// <summary>
    /// Gets a value indicating whether the field has a constant value.
    /// </summary>
    public bool HasConstantValue { get; init; }

    /// <summary>
    /// Gets the constant value expression as a string, or null if no constant.
    /// </summary>
    public string? ConstantValueExpression { get; init; }
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Core;

namespace Deepstaging.Roslyn.Emit.Operators.Arithmetic;

/// <summary>
/// Backing type information for implementing arithmetic operators.
/// Analyzes arithmetic capabilities of the backing type.
/// </summary>
internal readonly struct ArithmeticTypeInfo
{
    /// <summary>Gets the core backing type information.</summary>
    public BackingTypeCore Core { get; }

    /// <summary>Gets whether the type supports arithmetic operations.</summary>
    public bool SupportsArithmetic => Core.IsNumericType;

    /// <summary>Gets whether the type supports bitwise operations (integer types only).</summary>
    public bool SupportsBitwise => IsInteger;

    /// <summary>Gets whether the type supports shift operations (integer types only).</summary>
    public bool SupportsShift => IsInteger;

    /// <summary>Gets whether the type is an integer type (supports increment/decrement).</summary>
    public bool IsInteger => Core.IsInt32 || Core.IsInt64 || Core.IsInt16 || Core.IsByte;

    /// <summary>Gets whether the type is a floating-point type.</summary>
    public bool IsFloatingPoint => Core.IsSingle || Core.IsDouble;

    /// <summary>Gets whether the type is a decimal type.</summary>
    public bool IsDecimal => Core.IsDecimal;

    /// <summary>Gets the C# keyword for the backing type.</summary>
    public string? CSharpKeyword => Core.CSharpKeyword;

    private ArithmeticTypeInfo(BackingTypeCore core) => Core = core;

    /// <summary>
    /// Creates an ArithmeticTypeInfo from the given type symbol.
    /// </summary>
    public static ArithmeticTypeInfo From(TypeSnapshot type) => new(BackingTypeCore.From(type));
}
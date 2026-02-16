// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Core;

namespace Deepstaging.Roslyn.Emit.Interfaces.Parsing;

/// <summary>
/// Backing type information for implementing ISpanParsable&lt;T&gt;.
/// Analyzes span parsing-related capabilities of the backing type.
/// </summary>
internal readonly struct SpanParsableTypeInfo
{
    /// <summary>Gets the core backing type information.</summary>
    public BackingTypeCore Core { get; }

    /// <summary>Gets the simple type name.</summary>
    public string Name => Core.Name;

    /// <summary>Gets whether the type is System.Guid.</summary>
    public bool IsGuid => Core.IsGuid;

    /// <summary>Gets whether the type is System.String.</summary>
    public bool IsString => Core.IsString;

    /// <summary>Gets whether the type is a numeric type.</summary>
    public bool IsNumericType => Core.IsNumericType;

    /// <summary>Gets whether the type is a parsable primitive.</summary>
    public bool IsParsablePrimitive => Core.IsParsablePrimitive;

    /// <summary>Gets the C# keyword for the type, or null if none.</summary>
    public string? CSharpKeyword => Core.CSharpKeyword;

    private SpanParsableTypeInfo(BackingTypeCore core) => Core = core;

    /// <summary>
    /// Creates a SpanParsableTypeInfo from the given type symbol.
    /// </summary>
    public static SpanParsableTypeInfo From(TypeSnapshot type) => new(BackingTypeCore.From(type));
}
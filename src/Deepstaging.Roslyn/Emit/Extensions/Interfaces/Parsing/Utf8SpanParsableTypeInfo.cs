// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Core;

namespace Deepstaging.Roslyn.Emit.Interfaces.Parsing;

/// <summary>
/// Backing type information for implementing IUtf8SpanParsable&lt;T&gt;.
/// Analyzes UTF-8 span parsing-related capabilities of the backing type.
/// </summary>
internal readonly struct Utf8SpanParsableTypeInfo
{
    /// <summary>Gets the core backing type information.</summary>
    public BackingTypeCore Core { get; }

    /// <summary>Gets the simple type name.</summary>
    public string Name => Core.Name;

    /// <summary>Gets whether the type is System.Int32.</summary>
    public bool IsInt32 => Core.IsInt32;

    /// <summary>Gets whether the type is System.Int64.</summary>
    public bool IsInt64 => Core.IsInt64;

    /// <summary>Gets whether the type supports UTF-8 parsing (only int and long in .NET 8).</summary>
    public bool SupportsUtf8Parsing { get; }

    private Utf8SpanParsableTypeInfo(BackingTypeCore core, bool supportsUtf8Parsing)
    {
        Core = core;
        SupportsUtf8Parsing = supportsUtf8Parsing;
    }

    /// <summary>
    /// Creates a Utf8SpanParsableTypeInfo from the given type symbol.
    /// </summary>
    public static Utf8SpanParsableTypeInfo From(TypeSnapshot type)
    {
        var core = BackingTypeCore.From(type);
        var supportsUtf8Parsing = core.IsInt32 || core.IsInt64;

        return new Utf8SpanParsableTypeInfo(core, supportsUtf8Parsing);
    }
}
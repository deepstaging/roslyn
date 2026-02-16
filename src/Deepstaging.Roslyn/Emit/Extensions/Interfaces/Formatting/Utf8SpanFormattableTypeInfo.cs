// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Core;

namespace Deepstaging.Roslyn.Emit.Interfaces.Formatting;

/// <summary>
/// Backing type information for implementing IUtf8SpanFormattable.
/// Analyzes UTF-8 span formatting-related capabilities of the backing type.
/// </summary>
internal readonly struct Utf8SpanFormattableTypeInfo
{
    /// <summary>Gets the core backing type information.</summary>
    public BackingTypeCore Core { get; }

    /// <summary>Gets whether the type requires null-safe operations.</summary>
    public bool RequiresNullHandling => Core.RequiresNullHandling;

    /// <summary>Gets the simple type name.</summary>
    public string Name => Core.Name;

    /// <summary>Gets whether the type is System.Guid.</summary>
    public bool IsGuid => Core.IsGuid;

    /// <summary>Gets whether the type is a numeric type.</summary>
    public bool IsNumericType => Core.IsNumericType;

    private Utf8SpanFormattableTypeInfo(BackingTypeCore core) => Core = core;

    /// <summary>
    /// Creates a Utf8SpanFormattableTypeInfo from the given type symbol.
    /// </summary>
    public static Utf8SpanFormattableTypeInfo From(TypeSnapshot type) => new(BackingTypeCore.From(type));

    /// <summary>
    /// Gets the StringSyntax attribute string for the format parameter, if applicable.
    /// Returns empty string if no attribute is needed.
    /// </summary>
    public string GetStringSyntaxAttributeString()
    {
        if (IsGuid)
            return
                "[global::System.Diagnostics.CodeAnalysis.StringSyntax(global::System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.GuidFormat)] ";

        if (IsNumericType)
            return
                "[global::System.Diagnostics.CodeAnalysis.StringSyntax(global::System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.NumericFormat)] ";

        return "";
    }
}
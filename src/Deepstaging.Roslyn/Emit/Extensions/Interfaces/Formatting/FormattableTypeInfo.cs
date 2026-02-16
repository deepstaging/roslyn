// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Core;

namespace Deepstaging.Roslyn.Emit.Interfaces.Formatting;

/// <summary>
/// Backing type information for implementing IFormattable.
/// Analyzes formatting-related capabilities of the backing type.
/// </summary>
internal readonly struct FormattableTypeInfo
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
    public bool IsNumericType { get; }

    private FormattableTypeInfo(BackingTypeCore core, bool isNumericType)
    {
        Core = core;
        IsNumericType = isNumericType;
    }

    /// <summary>
    /// Creates a FormattableTypeInfo from the given type symbol.
    /// </summary>
    public static FormattableTypeInfo From(TypeSnapshot type)
    {
        var core = BackingTypeCore.From(type);
        return new FormattableTypeInfo(core, core.IsNumericType);
    }

    /// <summary>
    /// Gets the StringSyntax attribute for the format parameter, if applicable.
    /// </summary>
    public AttributeBuilder? GetStringSyntaxAttribute()
    {
        if (IsGuid)
            return AttributeBuilder.For("global::System.Diagnostics.CodeAnalysis.StringSyntax")
                .WithArgument("global::System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.GuidFormat");

        if (IsNumericType)
            return AttributeBuilder.For("global::System.Diagnostics.CodeAnalysis.StringSyntax")
                .WithArgument("global::System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.NumericFormat");

        return null;
    }
}
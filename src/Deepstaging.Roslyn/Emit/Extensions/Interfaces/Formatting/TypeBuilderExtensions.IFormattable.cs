// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Interfaces.Formatting;

/// <summary>
/// TypeBuilder extensions for implementing IFormattable.
/// Adds ToString(string?, IFormatProvider?) method.
/// </summary>
public static class TypeBuilderFormattableExtensions
{
    /// <summary>
    /// Implements IFormattable using semantic analysis of the backing type.
    /// Automatically generates appropriate ToString(format, provider) method.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="backingType">The backing type symbol to delegate to.</param>
    /// <param name="valueAccessor">The property/field name to access the backing value (e.g., "Value").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIFormattable(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        string valueAccessor)
    {
        var info = FormattableTypeInfo.From(backingType);
        var formatExpression = BuildFormatExpression(info, valueAccessor);
        var stringSyntaxAttr = info.GetStringSyntaxAttribute();

        if (info.IsNumericType)
            return builder
                .Implements("global::System.IFormattable")
                .AddMethod(BuildToStringMethod(stringSyntaxAttr, formatExpression)
                    .When(Directives.Net7OrGreater))
                .AddMethod(MethodBuilder
                    .Parse("public string ToString(string? format, global::System.IFormatProvider? formatProvider)")
                    .When(Directives.NotNet7OrGreater)
                    .WithInheritDoc("global::System.IFormattable")
                    .WithExpressionBody(formatExpression));

        return builder
            .Implements("global::System.IFormattable")
            .AddMethod(BuildToStringMethod(stringSyntaxAttr, formatExpression));
    }

    /// <summary>
    /// Implements IFormattable using a custom expression body.
    /// Use when semantic detection isn't sufficient for your type.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="toStringExpression">Expression body for ToString(format, provider) method.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIFormattable(
        this TypeBuilder builder,
        string toStringExpression) => builder
        .Implements("global::System.IFormattable")
        .AddMethod(MethodBuilder
            .Parse("public string ToString(string? format, global::System.IFormatProvider? formatProvider)")
            .WithInheritDoc("global::System.IFormattable")
            .WithExpressionBody(toStringExpression));

    private static MethodBuilder BuildToStringMethod(AttributeBuilder? stringSyntaxAttr, string formatExpression) =>
        MethodBuilder
            .Parse("public string ToString(string? format, global::System.IFormatProvider? formatProvider)")
            .WithInheritDoc("global::System.IFormattable")
            .WithExpressionBody(formatExpression)
            .If(stringSyntaxAttr.HasValue,
                m => m.ConfigureParameter("format", p => p.WithAttribute(stringSyntaxAttr!.Value)));

    private static string BuildFormatExpression(FormattableTypeInfo info, string valueAccessor)
    {
        if (info.RequiresNullHandling)
            return $"{valueAccessor} ?? string.Empty";

        return $"{valueAccessor}.ToString(format, formatProvider)";
    }

    /// <summary>
    /// Implements IFormattable using a property as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsIFormattable(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        PropertyBuilder property) =>
        builder.ImplementsIFormattable(backingType, property.Name);

    /// <summary>
    /// Implements IFormattable using a field as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsIFormattable(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        FieldBuilder field) =>
        builder.ImplementsIFormattable(backingType, field.Name);
}
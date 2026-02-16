// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Interfaces.Formatting;

/// <summary>
/// TypeBuilder extensions for implementing IUtf8SpanFormattable (NET8+).
/// Adds TryFormat(Span&lt;byte&gt;, ...) method for UTF-8 formatting.
/// </summary>
public static class TypeBuilderUtf8SpanFormattableExtensions
{
    /// <summary>
    /// Implements IUtf8SpanFormattable using semantic analysis of the backing type.
    /// Automatically generates appropriate TryFormat method (NET8+).
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="backingType">The backing type symbol to delegate to.</param>
    /// <param name="valueAccessor">The property/field name to access the backing value (e.g., "Value").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIUtf8SpanFormattable(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        string valueAccessor)
    {
        var info = Utf8SpanFormattableTypeInfo.From(backingType);
        var stringSyntaxAttr = info.GetStringSyntaxAttributeString();

        return builder
            .Implements("global::System.IUtf8SpanFormattable", Directives.Net8OrGreater)
            .AddMethod(BuildTryFormatMethod(info, valueAccessor, stringSyntaxAttr));
    }

    /// <summary>
    /// Implements IUtf8SpanFormattable using a custom expression body.
    /// Use when semantic detection isn't sufficient for your type.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="tryFormatExpression">Expression body for TryFormat method.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIUtf8SpanFormattable(
        this TypeBuilder builder,
        string tryFormatExpression) => builder
        .Implements("global::System.IUtf8SpanFormattable", Directives.Net8OrGreater)
        .AddMethod(MethodBuilder
            .Parse(
                "public bool TryFormat(global::System.Span<byte> utf8Destination, out int bytesWritten, global::System.ReadOnlySpan<char> format, global::System.IFormatProvider? provider)")
            .When(Directives.Net8OrGreater)
            .WithInheritDoc("global::System.IUtf8SpanFormattable.TryFormat")
            .WithExpressionBody(tryFormatExpression));

    private static MethodBuilder BuildTryFormatMethod(
        Utf8SpanFormattableTypeInfo info,
        string valueAccessor,
        string stringSyntaxAttr)
    {
        var method = MethodBuilder
            .Parse(
                $"public bool TryFormat(global::System.Span<byte> utf8Destination, out int bytesWritten, {stringSyntaxAttr}global::System.ReadOnlySpan<char> format, global::System.IFormatProvider? provider)")
            .When(Directives.Net8OrGreater)
            .WithInheritDoc("global::System.IUtf8SpanFormattable.TryFormat");

        if (info.RequiresNullHandling)
            return method.WithBody(b => b.AddStatements($$"""
                                                          var bytes = global::System.Text.Encoding.UTF8.GetBytes({{valueAccessor}} ?? string.Empty);
                                                          if (bytes.AsSpan().TryCopyTo(utf8Destination))
                                                          {
                                                              bytesWritten = bytes.Length;
                                                              return true;
                                                          }
                                                          bytesWritten = 0;
                                                          return false;
                                                          """));

        // Guid doesn't use provider
        if (info.IsGuid)
            return method.WithExpressionBody($"{valueAccessor}.TryFormat(utf8Destination, out bytesWritten, format)");

        return method.WithExpressionBody(
            $"{valueAccessor}.TryFormat(utf8Destination, out bytesWritten, format, provider)");
    }

    /// <summary>
    /// Implements IUtf8SpanFormattable using a property as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsIUtf8SpanFormattable(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        PropertyBuilder property) =>
        builder.ImplementsIUtf8SpanFormattable(backingType, property.Name);

    /// <summary>
    /// Implements IUtf8SpanFormattable using a field as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsIUtf8SpanFormattable(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        FieldBuilder field) =>
        builder.ImplementsIUtf8SpanFormattable(backingType, field.Name);
}
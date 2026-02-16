// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Interfaces.Formatting;

/// <summary>
/// TypeBuilder extensions for implementing ISpanFormattable (NET6+).
/// Adds TryFormat(Span&lt;char&gt;, ...) method.
/// </summary>
public static class TypeBuilderSpanFormattableExtensions
{
    /// <summary>
    /// Implements ISpanFormattable using semantic analysis of the backing type.
    /// Automatically generates appropriate TryFormat method (NET6+).
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="backingType">The backing type symbol to delegate to.</param>
    /// <param name="valueAccessor">The property/field name to access the backing value (e.g., "Value").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsISpanFormattable(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        string valueAccessor)
    {
        var info = SpanFormattableTypeInfo.From(backingType);
        var stringSyntaxAttr = info.GetStringSyntaxAttributeString();

        if (info.IsNumericType)
            return builder
                .Implements("global::System.ISpanFormattable", Directives.Net6OrGreater)
                // NET7+ with attribute
                .AddMethod(BuildTryFormatMethod(info, valueAccessor, stringSyntaxAttr)
                    .When(Directives.Net7OrGreater))
                // NET6 only without attribute
                .AddMethod(BuildTryFormatMethod(info, valueAccessor, "")
                    .When(Directives.Net6Only))
                // Simple overload NET7+
                .AddMethod(BuildSimpleTryFormatMethod(info, valueAccessor, stringSyntaxAttr)
                    .When(Directives.Net7OrGreater))
                // Simple overload NET6 only
                .AddMethod(BuildSimpleTryFormatMethod(info, valueAccessor, "")
                    .When(Directives.Net6Only));

        return builder
            .Implements("global::System.ISpanFormattable", Directives.Net6OrGreater)
            .AddMethod(BuildTryFormatMethod(info, valueAccessor, stringSyntaxAttr)
                .When(Directives.Net6OrGreater))
            .AddMethod(BuildSimpleTryFormatMethod(info, valueAccessor, stringSyntaxAttr)
                .When(Directives.Net6OrGreater));
    }

    /// <summary>
    /// Implements ISpanFormattable using a custom expression body.
    /// Use when semantic detection isn't sufficient for your type.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="tryFormatExpression">Expression body for TryFormat method.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsISpanFormattable(
        this TypeBuilder builder,
        string tryFormatExpression) => builder
        .Implements("global::System.ISpanFormattable", Directives.Net6OrGreater)
        .AddMethod(MethodBuilder
            .Parse(
                "public bool TryFormat(global::System.Span<char> destination, out int charsWritten, global::System.ReadOnlySpan<char> format, global::System.IFormatProvider? provider)")
            .When(Directives.Net6OrGreater)
            .WithInheritDoc("global::System.ISpanFormattable")
            .WithExpressionBody(tryFormatExpression));

    private static MethodBuilder BuildTryFormatMethod(
        SpanFormattableTypeInfo info,
        string valueAccessor,
        string stringSyntaxAttr)
    {
        var method = MethodBuilder
            .Parse(
                $"public bool TryFormat(global::System.Span<char> destination, out int charsWritten, {stringSyntaxAttr}global::System.ReadOnlySpan<char> format, global::System.IFormatProvider? provider)")
            .WithInheritDoc("global::System.ISpanFormattable");

        if (info.RequiresNullHandling)
            return method.WithBody(b => b.AddStatements($$"""
                                                          var span = ({{valueAccessor}} ?? string.Empty).AsSpan();
                                                          if (span.TryCopyTo(destination))
                                                          {
                                                              charsWritten = span.Length;
                                                              return true;
                                                          }
                                                          charsWritten = 0;
                                                          return false;
                                                          """));

        // Guid doesn't use provider
        if (info.IsGuid)
            return method.WithExpressionBody($"{valueAccessor}.TryFormat(destination, out charsWritten, format)");

        return method.WithExpressionBody($"{valueAccessor}.TryFormat(destination, out charsWritten, format, provider)");
    }

    private static MethodBuilder BuildSimpleTryFormatMethod(
        SpanFormattableTypeInfo info,
        string valueAccessor,
        string stringSyntaxAttr)
    {
        var method = MethodBuilder
            .Parse(
                $"public bool TryFormat(global::System.Span<char> destination, out int charsWritten, {stringSyntaxAttr}global::System.ReadOnlySpan<char> format = default)")
            .WithInheritDoc("global::System.ISpanFormattable");

        if (info.RequiresNullHandling)
            return method.WithBody(b => b.AddStatements($$"""
                                                          var span = ({{valueAccessor}} ?? string.Empty).AsSpan();
                                                          if (span.TryCopyTo(destination))
                                                          {
                                                              charsWritten = span.Length;
                                                              return true;
                                                          }
                                                          charsWritten = 0;
                                                          return false;
                                                          """));

        // Guid doesn't use provider
        if (info.IsGuid)
            return method.WithExpressionBody($"{valueAccessor}.TryFormat(destination, out charsWritten, format)");

        return method.WithExpressionBody($"{valueAccessor}.TryFormat(destination, out charsWritten, format, null)");
    }

    /// <summary>
    /// Implements ISpanFormattable using a property as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsISpanFormattable(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        PropertyBuilder property) =>
        builder.ImplementsISpanFormattable(backingType, property.Name);

    /// <summary>
    /// Implements ISpanFormattable using a field as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsISpanFormattable(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        FieldBuilder field) =>
        builder.ImplementsISpanFormattable(backingType, field.Name);
}
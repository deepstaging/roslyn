// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Interfaces.Parsing;

/// <summary>
/// TypeBuilder extensions for implementing IUtf8SpanParsable&lt;T&gt; (NET8+).
/// Adds UTF-8 span-based Parse and TryParse methods.
/// Note: Only numeric types (int, long) support UTF-8 parsing in .NET 8.
/// </summary>
public static class TypeBuilderUtf8SpanParsableExtensions
{
    /// <summary>
    /// Implements IUtf8SpanParsable&lt;T&gt; using semantic analysis of the backing type.
    /// Automatically generates appropriate UTF-8 Parse and TryParse methods (NET8+).
    /// Note: Only available for int and long backing types.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="backingType">The backing type symbol to delegate to.</param>
    /// <returns>The modified type builder (unchanged if backing type doesn't support UTF-8 parsing).</returns>
    public static TypeBuilder ImplementsIUtf8SpanParsable(
        this TypeBuilder builder,
        ValidSymbol<INamedTypeSymbol> backingType)
    {
        var typeName = builder.Name;
        var info = Utf8SpanParsableTypeInfo.From(backingType);

        if (!info.SupportsUtf8Parsing)
            return builder;

        return builder
            .Implements($"global::System.IUtf8SpanParsable<{typeName}>", Directives.Net8OrGreater)
            .AddMethod(BuildParseMethod(typeName, info))
            .AddMethod(BuildTryParseMethod(typeName, info));
    }

    /// <summary>
    /// Implements IUtf8SpanParsable&lt;T&gt; using custom expressions.
    /// Use when semantic detection isn't sufficient for your type.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="parseExpression">Expression body for Parse method.</param>
    /// <param name="tryParseBody">Full body for TryParse method.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIUtf8SpanParsable(
        this TypeBuilder builder,
        string parseExpression,
        string tryParseBody)
    {
        var typeName = builder.Name;

        return builder
            .Implements($"global::System.IUtf8SpanParsable<{typeName}>", Directives.Net8OrGreater)
            .AddMethod(MethodBuilder
                .Parse($"public static {typeName} Parse(global::System.ReadOnlySpan<byte> utf8Text, global::System.IFormatProvider? provider)")
                .When(Directives.Net8OrGreater)
                .WithInheritDoc("global::System.IUtf8SpanParsable{{TSelf}}.Parse(global::System.ReadOnlySpan{{byte}}, global::System.IFormatProvider?)")
                .WithExpressionBody(parseExpression))
            .AddMethod(MethodBuilder
                .Parse($"public static bool TryParse(global::System.ReadOnlySpan<byte> utf8Text, global::System.IFormatProvider? provider, out {typeName} result)")
                .When(Directives.Net8OrGreater)
                .WithInheritDoc("global::System.IUtf8SpanParsable{{TSelf}}.TryParse(global::System.ReadOnlySpan{{byte}}, global::System.IFormatProvider?, out TSelf)")
                .WithBody(b => b.AddStatements(tryParseBody)));
    }

    private static MethodBuilder BuildParseMethod(string typeName, Utf8SpanParsableTypeInfo info)
    {
        var backingParseName = info.IsInt32 ? "int" : "long";
        var parseExpression = $"new({backingParseName}.Parse(utf8Text, provider))";

        return MethodBuilder
            .Parse($"public static {typeName} Parse(global::System.ReadOnlySpan<byte> utf8Text, global::System.IFormatProvider? provider)")
            .When(Directives.Net8OrGreater)
            .WithInheritDoc("global::System.IUtf8SpanParsable{{TSelf}}.Parse(global::System.ReadOnlySpan{{byte}}, global::System.IFormatProvider?)")
            .WithExpressionBody(parseExpression);
    }

    private static MethodBuilder BuildTryParseMethod(string typeName, Utf8SpanParsableTypeInfo info)
    {
        var backingParseName = info.IsInt32 ? "int" : "long";

        var tryParseBody = $$"""
            if ({{backingParseName}}.TryParse(utf8Text, provider, out var value))
            {
                result = new {{typeName}}(value);
                return true;
            }

            result = default;
            return false;
            """;

        return MethodBuilder
            .Parse($"public static bool TryParse(global::System.ReadOnlySpan<byte> utf8Text, global::System.IFormatProvider? provider, out {typeName} result)")
            .When(Directives.Net8OrGreater)
            .WithInheritDoc("global::System.IUtf8SpanParsable{{TSelf}}.TryParse(global::System.ReadOnlySpan{{byte}}, global::System.IFormatProvider?, out TSelf)")
            .WithBody(b => b.AddStatements(tryParseBody));
    }
}

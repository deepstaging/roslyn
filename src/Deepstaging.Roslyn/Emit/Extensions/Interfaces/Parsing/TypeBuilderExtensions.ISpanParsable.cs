// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Interfaces.Parsing;

/// <summary>
/// TypeBuilder extensions for implementing ISpanParsable&lt;T&gt; (NET7+).
/// Adds span-based Parse and TryParse methods.
/// </summary>
public static class TypeBuilderSpanParsableExtensions
{
    /// <summary>
    /// Implements ISpanParsable&lt;T&gt; using semantic analysis of the backing type.
    /// Automatically generates appropriate span-based Parse and TryParse methods.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="backingType">The backing type symbol to delegate to.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsISpanParsable(
        this TypeBuilder builder,
        TypeSnapshot backingType)
    {
        var typeName = builder.Name;
        var info = SpanParsableTypeInfo.From(backingType);

        return builder
            .Implements($"global::System.ISpanParsable<{typeName}>", Directives.Net7OrGreater)
            // Basic Parse(ReadOnlySpan<char>) available in NETCOREAPP2_1+
            .AddMethod(BuildBasicParseMethod(typeName, info))
            // Parse with provider (NET6+)
            .AddMethod(BuildParseWithProviderMethod(typeName, info))
            // TryParse (NET6+)
            .AddMethod(BuildTryParseMethod(typeName, info));
    }

    /// <summary>
    /// Implements ISpanParsable&lt;T&gt; using custom expressions.
    /// Use when semantic detection isn't sufficient for your type.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="parseExpression">Expression body for Parse method.</param>
    /// <param name="tryParseBody">Full body for TryParse method.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsISpanParsable(
        this TypeBuilder builder,
        string parseExpression,
        string tryParseBody)
    {
        var typeName = builder.Name;

        return builder
            .Implements($"global::System.ISpanParsable<{typeName}>", Directives.Net7OrGreater)
            .AddMethod(MethodBuilder
                .Parse(
                    $"public static {typeName} Parse(global::System.ReadOnlySpan<char> input, global::System.IFormatProvider? provider)")
                .When(Directives.Net6OrGreater)
                .WithInheritDoc("global::System.ISpanParsable{{TSelf}}")
                .WithExpressionBody(parseExpression))
            .AddMethod(MethodBuilder
                .Parse(
                    $"public static bool TryParse(global::System.ReadOnlySpan<char> input, global::System.IFormatProvider? provider, out {typeName} result)")
                .When(Directives.Net6OrGreater)
                .WithInheritDoc("global::System.ISpanParsable{{TSelf}}")
                .WithBody(b => b.AddStatements(tryParseBody)));
    }

    private static MethodBuilder BuildBasicParseMethod(string typeName, SpanParsableTypeInfo info)
    {
        string parseExpression;

        if (info.IsGuid)
            parseExpression = "new(global::System.Guid.Parse(input))";
        else if (info.IsString)
            parseExpression = "new(input.ToString())";
        else if (info.CSharpKeyword is { } keyword)
            parseExpression = $"new({keyword}.Parse(input))";
        else
            parseExpression = "default";

        return MethodBuilder
            .Parse($"public static {typeName} Parse(global::System.ReadOnlySpan<char> input)")
            .When(Directives.NetCoreApp21OrGreater)
            .WithExpressionBody(parseExpression);
    }

    private static MethodBuilder BuildParseWithProviderMethod(string typeName, SpanParsableTypeInfo info)
    {
        string parseExpression;

        if (info.IsGuid)
            parseExpression = "new(global::System.Guid.Parse(input, provider))";
        else if (info.IsString)
            parseExpression = "new(input.ToString())";
        else if (info.CSharpKeyword is { } keyword)
            parseExpression = $"new({keyword}.Parse(input, provider))";
        else
            parseExpression = "default";

        return MethodBuilder
            .Parse(
                $"public static {typeName} Parse(global::System.ReadOnlySpan<char> input, global::System.IFormatProvider? provider)")
            .When(Directives.Net6OrGreater)
            .WithInheritDoc("global::System.ISpanParsable{{TSelf}}")
            .WithExpressionBody(parseExpression);
    }

    private static MethodBuilder BuildTryParseMethod(string typeName, SpanParsableTypeInfo info)
    {
        string tryParseBody;

        if (info.IsGuid)
            tryParseBody = """
                           if (global::System.Guid.TryParse(input, provider, out var guid))
                           {
                               result = new(guid);
                               return true;
                           }
                           else
                           {
                               result = default;
                               return false;
                           }
                           """;
        else if (info.IsString)
            tryParseBody = """
                           result = new(input.ToString());
                           return true;
                           """;
        else if (info.CSharpKeyword is { } keyword)
            tryParseBody = $$"""
                             if ({{keyword}}.TryParse(input, provider, out var value))
                             {
                                 result = new(value);
                                 return true;
                             }
                             else
                             {
                                 result = default;
                                 return false;
                             }
                             """;
        else
            tryParseBody = """
                           result = default;
                           return false;
                           """;

        return MethodBuilder
            .Parse(
                $"public static bool TryParse(global::System.ReadOnlySpan<char> input, global::System.IFormatProvider? provider, out {typeName} result)")
            .When(Directives.Net6OrGreater)
            .WithInheritDoc("global::System.ISpanParsable{{TSelf}}")
            .WithBody(b => b.AddStatements(tryParseBody));
    }
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Interfaces.Parsing;

/// <summary>
/// TypeBuilder extensions for implementing IParsable&lt;T&gt; (NET7+).
/// Adds Parse and TryParse methods.
/// </summary>
public static class TypeBuilderParsableExtensions
{
    /// <summary>
    /// Implements IParsable&lt;T&gt; using semantic analysis of the backing type.
    /// Automatically generates appropriate Parse and TryParse methods (NET7+).
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="backingType">The backing type symbol to delegate to.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIParsable(
        this TypeBuilder builder,
        TypeSnapshot backingType)
    {
        var typeName = builder.Name;
        var info = ParsableTypeInfo.From(backingType);

        return builder
            .Implements($"global::System.IParsable<{typeName}>", Directives.Net7OrGreater)
            .AddMethod(BuildParseMethod(typeName, info))
            .AddMethod(BuildTryParseMethod(typeName, info));
    }

    /// <summary>
    /// Implements IParsable&lt;T&gt; using custom expressions.
    /// Use when semantic detection isn't sufficient for your type.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="parseExpression">Expression body for Parse method (e.g., "new(Guid.Parse(input, provider))").</param>
    /// <param name="tryParseBody">Full body for TryParse method.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIParsable(
        this TypeBuilder builder,
        string parseExpression,
        string tryParseBody)
    {
        var typeName = builder.Name;

        return builder
            .Implements($"global::System.IParsable<{typeName}>", Directives.Net7OrGreater)
            .AddMethod(MethodBuilder
                .Parse($"public static {typeName} Parse(string input, global::System.IFormatProvider? provider)")
                .When(Directives.Net7OrGreater)
                .WithInheritDoc("global::System.IParsable{{TSelf}}")
                .WithExpressionBody(parseExpression))
            .AddMethod(MethodBuilder
                .Parse($"public static bool TryParse([global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] string? input, global::System.IFormatProvider? provider, out {typeName} result)")
                .When(Directives.Net7OrGreater)
                .WithInheritDoc("global::System.IParsable{{TSelf}}")
                .WithBody(b => b.AddStatements(tryParseBody)));
    }

    private static MethodBuilder BuildParseMethod(string typeName, ParsableTypeInfo info)
    {
        string parseExpression;
        if (info.IsGuid)
            parseExpression = "new(global::System.Guid.Parse(input, provider))";
        else if (info.IsString)
            parseExpression = "new(input)";
        else if (info.CSharpKeyword is { } keyword)
            parseExpression = $"new({keyword}.Parse(input, provider))";
        else
            parseExpression = "new(input)";

        return MethodBuilder
            .Parse($"public static {typeName} Parse(string input, global::System.IFormatProvider? provider)")
            .When(Directives.Net7OrGreater)
            .WithInheritDoc("global::System.IParsable{{TSelf}}")
            .WithExpressionBody(parseExpression);
    }

    private static MethodBuilder BuildTryParseMethod(string typeName, ParsableTypeInfo info)
    {
        string tryParseBody;
        if (info.IsGuid)
        {
            tryParseBody = """
                if (input is null)
                {
                    result = default;
                    return false;
                }

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
        }
        else if (info.IsString)
        {
            tryParseBody = """
                result = new(input ?? string.Empty);
                return true;
                """;
        }
        else if (info.CSharpKeyword is { } keyword)
        {
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
        }
        else
        {
            tryParseBody = """
                result = default;
                return false;
                """;
        }

        return MethodBuilder
            .Parse($"public static bool TryParse([global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] string? input, global::System.IFormatProvider? provider, out {typeName} result)")
            .When(Directives.Net7OrGreater)
            .WithInheritDoc("global::System.IParsable{{TSelf}}")
            .WithBody(b => b.AddStatements(tryParseBody));
    }
}

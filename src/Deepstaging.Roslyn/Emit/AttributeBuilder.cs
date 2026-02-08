// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for attributes.
/// Supports named and positional arguments.
/// Immutable - each method returns a new instance.
/// </summary>
public record struct AttributeBuilder
{
    /// <summary>Gets or sets the attribute name.</summary>
    public string Name { get; init; }

    /// <summary>Gets or sets the positional arguments.</summary>
    public ImmutableArray<string> Arguments { get; init; }

    /// <summary>Gets or sets the named arguments.</summary>
    public ImmutableArray<(string Name, string Value)> NamedArguments { get; init; }

    /// <summary>Gets or sets the using directives.</summary>
    public ImmutableArray<string> Usings { get; init; }

    #region Factory Methods

    /// <summary>
    /// Creates an attribute builder for the specified attribute type.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "Serializable", "Obsolete", "JsonProperty").</param>
    public static AttributeBuilder For(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Attribute name cannot be null or empty.", nameof(name));

        return new AttributeBuilder { Name = name };
    }

    #endregion

    #region Arguments

    /// <summary>
    /// Adds a positional argument to the attribute.
    /// </summary>
    /// <param name="value">The argument value as a code expression (e.g., "\"message\"", "typeof(int)", "42").</param>
    public readonly AttributeBuilder WithArgument(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Argument value cannot be null or empty.", nameof(value));

        var args = Arguments.IsDefault ? [] : Arguments;
        return this with { Arguments = args.Add(value) };
    }

    /// <summary>
    /// Adds multiple positional arguments to the attribute.
    /// </summary>
    /// <param name="values">The argument values as code expressions.</param>
    public readonly AttributeBuilder WithArguments(params string[] values)
    {
        var builder = this;
        foreach (var value in values) builder = builder.WithArgument(value);
        return builder;
    }

    /// <summary>
    /// Adds a named argument to the attribute.
    /// </summary>
    /// <param name="name">The argument name (e.g., "ErrorMessage", "AllowMultiple").</param>
    /// <param name="value">The argument value as a code expression (e.g., "\"message\"", "true").</param>
    public readonly AttributeBuilder WithNamedArgument(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Argument name cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Argument value cannot be null or empty.", nameof(value));

        var namedArgs = NamedArguments.IsDefault ? [] : NamedArguments;
        return this with { NamedArguments = namedArgs.Add((name, value)) };
    }

    #endregion

    #region Usings

    /// <summary>
    /// Adds a using directive that will be collected by the containing TypeBuilder.
    /// </summary>
    /// <param name="namespace">The namespace to add (e.g., "System.Linq", "static System.Math").</param>
    public readonly AttributeBuilder AddUsing(string @namespace)
    {
        var usings = Usings.IsDefault ? [] : Usings;
        return this with { Usings = usings.Add(@namespace) };
    }

    #endregion

    #region Building

    /// <summary>
    /// Builds the attribute as an attribute syntax node.
    /// </summary>
    internal readonly AttributeSyntax Build()
    {
        var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(Name));

        var allArguments = new List<AttributeArgumentSyntax>();

        // Add positional arguments
        foreach (var arg in Arguments.IsDefault ? [] : Arguments)
            allArguments.Add(
                SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression(arg)));

        // Add named arguments
        foreach (var (name, value) in NamedArguments.IsDefault ? [] : NamedArguments)
            allArguments.Add(
                SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression(value))
                    .WithNameEquals(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(name))));

        if (allArguments.Count > 0)
            attribute = attribute.WithArgumentList(
                SyntaxFactory.AttributeArgumentList(
                    SyntaxFactory.SeparatedList(allArguments)));

        return attribute;
    }

    /// <summary>
    /// Builds the attribute as an attribute list syntax node.
    /// </summary>
    internal readonly AttributeListSyntax BuildList()
    {
        return SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(Build()));
    }

    #endregion
}
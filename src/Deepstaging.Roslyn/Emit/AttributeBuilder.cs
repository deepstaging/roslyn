// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for attributes.
/// Supports named and positional arguments.
/// Immutable - each method returns a new instance.
/// </summary>
public readonly struct AttributeBuilder
{
    private readonly string _name;
    private readonly ImmutableArray<string> _arguments;
    private readonly ImmutableArray<(string Name, string Value)> _namedArguments;
    private readonly ImmutableArray<string> _usings;

    private AttributeBuilder(
        string name,
        ImmutableArray<string> arguments,
        ImmutableArray<(string Name, string Value)> namedArguments,
        ImmutableArray<string> usings)
    {
        _name = name;
        _arguments = arguments.IsDefault ? ImmutableArray<string>.Empty : arguments;
        _namedArguments = namedArguments.IsDefault ? ImmutableArray<(string, string)>.Empty : namedArguments;
        _usings = usings.IsDefault ? ImmutableArray<string>.Empty : usings;
    }

    #region Factory Methods

    /// <summary>
    /// Creates an attribute builder for the specified attribute type.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "Serializable", "Obsolete", "JsonProperty").</param>
    public static AttributeBuilder For(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Attribute name cannot be null or empty.", nameof(name));

        return new AttributeBuilder(
            name,
            ImmutableArray<string>.Empty,
            ImmutableArray<(string, string)>.Empty,
            ImmutableArray<string>.Empty);
    }

    #endregion

    #region Arguments

    /// <summary>
    /// Adds a positional argument to the attribute.
    /// </summary>
    /// <param name="value">The argument value as a code expression (e.g., "\"message\"", "typeof(int)", "42").</param>
    public AttributeBuilder WithArgument(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Argument value cannot be null or empty.", nameof(value));

        return new AttributeBuilder(_name, _arguments.Add(value), _namedArguments, _usings);
    }

    /// <summary>
    /// Adds multiple positional arguments to the attribute.
    /// </summary>
    /// <param name="values">The argument values as code expressions.</param>
    public AttributeBuilder WithArguments(params string[] values)
    {
        var builder = this;
        foreach (var value in values)
        {
            builder = builder.WithArgument(value);
        }
        return builder;
    }

    /// <summary>
    /// Adds a named argument to the attribute.
    /// </summary>
    /// <param name="name">The argument name (e.g., "ErrorMessage", "AllowMultiple").</param>
    /// <param name="value">The argument value as a code expression (e.g., "\"message\"", "true").</param>
    public AttributeBuilder WithNamedArgument(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Argument name cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Argument value cannot be null or empty.", nameof(value));

        return new AttributeBuilder(_name, _arguments, _namedArguments.Add((name, value)), _usings);
    }

    #endregion

    #region Usings

    /// <summary>
    /// Adds a using directive that will be collected by the containing TypeBuilder.
    /// </summary>
    /// <param name="namespace">The namespace to add (e.g., "System.Linq", "static System.Math").</param>
    public AttributeBuilder AddUsing(string @namespace)
    {
        return new AttributeBuilder(_name, _arguments, _namedArguments, _usings.Add(@namespace));
    }

    /// <summary>
    /// Gets the using directives for this attribute.
    /// </summary>
    internal ImmutableArray<string> Usings => _usings;

    #endregion

    #region Building

    /// <summary>
    /// Builds the attribute as an attribute syntax node.
    /// </summary>
    internal AttributeSyntax Build()
    {
        var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(_name));

        var allArguments = new List<AttributeArgumentSyntax>();

        // Add positional arguments
        foreach (var arg in _arguments)
        {
            allArguments.Add(
                SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression(arg)));
        }

        // Add named arguments
        foreach (var (name, value) in _namedArguments)
        {
            allArguments.Add(
                SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression(value))
                    .WithNameEquals(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(name))));
        }

        if (allArguments.Count > 0)
        {
            attribute = attribute.WithArgumentList(
                SyntaxFactory.AttributeArgumentList(
                    SyntaxFactory.SeparatedList(allArguments)));
        }

        return attribute;
    }

    /// <summary>
    /// Builds the attribute as an attribute list syntax node.
    /// </summary>
    internal AttributeListSyntax BuildList()
    {
        return SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(Build()));
    }

    /// <summary>
    /// Gets the attribute name.
    /// </summary>
    public string Name => _name;

    #endregion
}

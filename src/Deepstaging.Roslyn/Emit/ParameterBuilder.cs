// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for method and constructor parameters.
/// Immutable - each method returns a new instance.
/// </summary>
public readonly struct ParameterBuilder
{
    private readonly string _name;
    private readonly string _type;
    private readonly string? _defaultValue;
    private readonly ParameterModifier _modifier;
    private readonly ImmutableArray<AttributeBuilder> _attributes;

    private ParameterBuilder(
        string name,
        string type,
        string? defaultValue,
        ParameterModifier modifier,
        ImmutableArray<AttributeBuilder> attributes)
    {
        _name = name;
        _type = type;
        _defaultValue = defaultValue;
        _modifier = modifier;
        _attributes = attributes.IsDefault ? ImmutableArray<AttributeBuilder>.Empty : attributes;
    }

    #region Factory Methods

    /// <summary>
    /// Creates a parameter builder for the specified parameter.
    /// </summary>
    /// <param name="name">The parameter name (e.g., "value", "id").</param>
    /// <param name="type">The parameter type (e.g., "string", "int", "List&lt;T&gt;").</param>
    public static ParameterBuilder For(string name, string type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Parameter name cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Parameter type cannot be null or empty.", nameof(type));

        return new ParameterBuilder(name, type, null, ParameterModifier.None, ImmutableArray<AttributeBuilder>.Empty);
    }

    #endregion

    #region Configuration

    /// <summary>
    /// Sets the default value for the parameter.
    /// </summary>
    /// <param name="defaultValue">The default value expression (e.g., "null", "0", "default").</param>
    public ParameterBuilder WithDefaultValue(string defaultValue)
    {
        return new ParameterBuilder(_name, _type, defaultValue, _modifier, _attributes);
    }

    /// <summary>
    /// Marks the parameter as 'ref'.
    /// </summary>
    public ParameterBuilder AsRef()
    {
        return new ParameterBuilder(_name, _type, _defaultValue, ParameterModifier.Ref, _attributes);
    }

    /// <summary>
    /// Marks the parameter as 'out'.
    /// </summary>
    public ParameterBuilder AsOut()
    {
        return new ParameterBuilder(_name, _type, _defaultValue, ParameterModifier.Out, _attributes);
    }

    /// <summary>
    /// Marks the parameter as 'in'.
    /// </summary>
    public ParameterBuilder AsIn()
    {
        return new ParameterBuilder(_name, _type, _defaultValue, ParameterModifier.In, _attributes);
    }

    /// <summary>
    /// Marks the parameter as 'params'.
    /// </summary>
    public ParameterBuilder AsParams()
    {
        return new ParameterBuilder(_name, _type, _defaultValue, ParameterModifier.Params, _attributes);
    }

    /// <summary>
    /// Marks the parameter as 'this' (extension method target).
    /// </summary>
    public ParameterBuilder AsThis()
    {
        return new ParameterBuilder(_name, _type, _defaultValue, ParameterModifier.This, _attributes);
    }

    #endregion

    #region Attributes

    /// <summary>
    /// Adds an attribute to the parameter.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "CallerMemberName", "NotNull").</param>
    public ParameterBuilder WithAttribute(string name)
    {
        var attribute = AttributeBuilder.For(name);
        return new ParameterBuilder(_name, _type, _defaultValue, _modifier, _attributes.Add(attribute));
    }

    /// <summary>
    /// Adds an attribute to the parameter with configuration.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "CallerMemberName", "FromQuery").</param>
    /// <param name="configure">Configuration callback for the attribute.</param>
    public ParameterBuilder WithAttribute(string name, Func<AttributeBuilder, AttributeBuilder> configure)
    {
        var attribute = configure(AttributeBuilder.For(name));
        return new ParameterBuilder(_name, _type, _defaultValue, _modifier, _attributes.Add(attribute));
    }

    /// <summary>
    /// Adds a pre-configured attribute to the parameter.
    /// </summary>
    public ParameterBuilder WithAttribute(AttributeBuilder attribute)
    {
        return new ParameterBuilder(_name, _type, _defaultValue, _modifier, _attributes.Add(attribute));
    }

    #endregion

    #region Building

    /// <summary>
    /// Builds the parameter as a parameter syntax node.
    /// </summary>
    internal ParameterSyntax Build()
    {
        var parameter = SyntaxFactory.Parameter(
            SyntaxFactory.Identifier(_name))
            .WithType(SyntaxFactory.ParseTypeName(_type));

        // Add attributes
        if (_attributes.Length > 0)
        {
            var attributeLists = _attributes.Select(a => a.BuildList()).ToArray();
            parameter = parameter.WithAttributeLists(SyntaxFactory.List(attributeLists));
        }

        // Add modifier if specified
        if (_modifier != ParameterModifier.None)
        {
            var modifierToken = _modifier switch
            {
                ParameterModifier.Ref => SyntaxKind.RefKeyword,
                ParameterModifier.Out => SyntaxKind.OutKeyword,
                ParameterModifier.In => SyntaxKind.InKeyword,
                ParameterModifier.Params => SyntaxKind.ParamsKeyword,
                ParameterModifier.This => SyntaxKind.ThisKeyword,
                _ => throw new InvalidOperationException($"Unknown parameter modifier: {_modifier}")
            };
            parameter = parameter.WithModifiers(
                SyntaxFactory.TokenList(SyntaxFactory.Token(modifierToken)));
        }

        // Add default value if specified
        if (_defaultValue != null)
        {
            parameter = parameter.WithDefault(
                SyntaxFactory.EqualsValueClause(
                    SyntaxFactory.ParseExpression(_defaultValue)));
        }

        return parameter;
    }

    /// <summary>
    /// Gets the parameter name.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Gets the parameter type.
    /// </summary>
    public string Type => _type;

    /// <summary>
    /// Gets whether this parameter is an extension method target (has 'this' modifier).
    /// </summary>
    public bool IsExtensionTarget => _modifier == ParameterModifier.This;

    /// <summary>
    /// Gets the using directives from all attributes on this parameter.
    /// </summary>
    internal ImmutableArray<string> Usings => _attributes.SelectMany(a => a.Usings).ToImmutableArray();

    #endregion
}

/// <summary>
/// Parameter modifiers (ref, out, in, params, this).
/// </summary>
internal enum ParameterModifier
{
    None,
    Ref,
    Out,
    In,
    Params,
    This
}

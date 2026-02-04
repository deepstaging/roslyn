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

    private ParameterBuilder(
        string name,
        string type,
        string? defaultValue,
        ParameterModifier modifier)
    {
        _name = name;
        _type = type;
        _defaultValue = defaultValue;
        _modifier = modifier;
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

        return new ParameterBuilder(name, type, null, ParameterModifier.None);
    }

    #endregion

    #region Configuration

    /// <summary>
    /// Sets the default value for the parameter.
    /// </summary>
    /// <param name="defaultValue">The default value expression (e.g., "null", "0", "default").</param>
    public ParameterBuilder WithDefaultValue(string defaultValue)
    {
        return new ParameterBuilder(_name, _type, defaultValue, _modifier);
    }

    /// <summary>
    /// Marks the parameter as 'ref'.
    /// </summary>
    public ParameterBuilder AsRef()
    {
        return new ParameterBuilder(_name, _type, _defaultValue, ParameterModifier.Ref);
    }

    /// <summary>
    /// Marks the parameter as 'out'.
    /// </summary>
    public ParameterBuilder AsOut()
    {
        return new ParameterBuilder(_name, _type, _defaultValue, ParameterModifier.Out);
    }

    /// <summary>
    /// Marks the parameter as 'in'.
    /// </summary>
    public ParameterBuilder AsIn()
    {
        return new ParameterBuilder(_name, _type, _defaultValue, ParameterModifier.In);
    }

    /// <summary>
    /// Marks the parameter as 'params'.
    /// </summary>
    public ParameterBuilder AsParams()
    {
        return new ParameterBuilder(_name, _type, _defaultValue, ParameterModifier.Params);
    }

    /// <summary>
    /// Marks the parameter as 'this' (extension method target).
    /// </summary>
    public ParameterBuilder AsThis()
    {
        return new ParameterBuilder(_name, _type, _defaultValue, ParameterModifier.This);
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

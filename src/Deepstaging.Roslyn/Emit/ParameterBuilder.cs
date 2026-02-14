// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for method and constructor parameters.
/// Immutable - each method returns a new instance.
/// </summary>
public record struct ParameterBuilder
{
    /// <summary>Gets the parameter name.</summary>
    public string Name { get; init; }

    /// <summary>Gets the parameter type.</summary>
    public string Type { get; init; }

    /// <summary>Gets the default value expression.</summary>
    public string? DefaultValue { get; init; }

    /// <summary>Gets the parameter modifier.</summary>
    public ParameterModifier Modifier { get; init; }

    /// <summary>Gets the attributes applied to this parameter.</summary>
    public ImmutableArray<AttributeBuilder> Attributes { get; init; }

    /// <summary>Gets the validation rules for this parameter.</summary>
    public ImmutableArray<ParameterValidation> Validations { get; init; }

    /// <summary>Gets the member name this parameter should be assigned to (if any).</summary>
    public string? AssignsToMember { get; init; }

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

        return new ParameterBuilder
        {
            Name = name,
            Type = type
        };
    }

    /// <summary>
    /// Creates a parameter builder using a symbol's globally qualified name as the type.
    /// </summary>
    /// <param name="name">The parameter name (e.g., "value", "id").</param>
    /// <param name="type">The parameter type symbol.</param>
    public static ParameterBuilder For<T>(string name, ValidSymbol<T> type) where T : class, ITypeSymbol
        => For(name, type.GloballyQualifiedName);

    #endregion

    #region Configuration

    /// <summary>
    /// Sets the default value for the parameter.
    /// </summary>
    /// <param name="defaultValue">The default value expression (e.g., "null", "0", "default").</param>
    public ParameterBuilder WithDefaultValue(string defaultValue) =>
        this with { DefaultValue = defaultValue };

    /// <summary>
    /// Marks the parameter as 'ref'.
    /// </summary>
    public ParameterBuilder AsRef() =>
        this with { Modifier = ParameterModifier.Ref };

    /// <summary>
    /// Marks the parameter as 'out'.
    /// </summary>
    public ParameterBuilder AsOut() =>
        this with { Modifier = ParameterModifier.Out };

    /// <summary>
    /// Marks the parameter as 'in'.
    /// </summary>
    public ParameterBuilder AsIn() =>
        this with { Modifier = ParameterModifier.In };

    /// <summary>
    /// Marks the parameter as 'params'.
    /// </summary>
    public ParameterBuilder AsParams() =>
        this with { Modifier = ParameterModifier.Params };

    /// <summary>
    /// Marks the parameter as 'this' (extension method target).
    /// </summary>
    public ParameterBuilder AsThis() =>
        this with { Modifier = ParameterModifier.This };

    #endregion

    #region Attributes

    /// <summary>
    /// Adds an attribute to the parameter.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "CallerMemberName", "NotNull").</param>
    public ParameterBuilder WithAttribute(string name)
    {
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(AttributeBuilder.For(name)) };
    }

    /// <summary>
    /// Adds an attribute to the parameter with configuration.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "CallerMemberName", "FromQuery").</param>
    /// <param name="configure">Configuration callback for the attribute.</param>
    public ParameterBuilder WithAttribute(string name, Func<AttributeBuilder, AttributeBuilder> configure)
    {
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(configure(AttributeBuilder.For(name))) };
    }

    /// <summary>
    /// Adds a pre-configured attribute to the parameter.
    /// </summary>
    public ParameterBuilder WithAttribute(AttributeBuilder attribute)
    {
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(attribute) };
    }

    #endregion

    #region Building

    /// <summary>
    /// Builds the parameter as a parameter syntax node.
    /// </summary>
    internal ParameterSyntax Build()
    {
        var parameter = SyntaxFactory.Parameter(
                SyntaxFactory.Identifier(Name))
            .WithType(SyntaxFactory.ParseTypeName(Type));

        // Add attributes
        var attributes = Attributes.IsDefault ? [] : Attributes;
        if (attributes.Length > 0)
        {
            var attributeLists = attributes.Select(a => a.BuildList()).ToArray();
            parameter = parameter.WithAttributeLists(SyntaxFactory.List(attributeLists));
        }

        // Add modifier if specified
        if (Modifier != ParameterModifier.None)
        {
            var modifierToken = Modifier switch
            {
                ParameterModifier.Ref => SyntaxKind.RefKeyword,
                ParameterModifier.Out => SyntaxKind.OutKeyword,
                ParameterModifier.In => SyntaxKind.InKeyword,
                ParameterModifier.Params => SyntaxKind.ParamsKeyword,
                ParameterModifier.This => SyntaxKind.ThisKeyword,
                _ => throw new InvalidOperationException($"Unknown parameter modifier: {Modifier}")
            };
            parameter = parameter.WithModifiers(
                SyntaxFactory.TokenList(SyntaxFactory.Token(modifierToken)));
        }

        // Add default value if specified
        if (DefaultValue != null)
            parameter = parameter.WithDefault(
                SyntaxFactory.EqualsValueClause(
                    SyntaxFactory.ParseExpression(DefaultValue)));

        return parameter;
    }

    /// <summary>
    /// Gets whether this parameter is an extension method target (has 'this' modifier).
    /// </summary>
    public bool IsExtensionTarget => Modifier == ParameterModifier.This;

    /// <summary>
    /// Gets the using directives from all attributes on this parameter.
    /// </summary>
    internal ImmutableArray<string> Usings
    {
        get
        {
            var attributes = Attributes.IsDefault ? [] : Attributes;
            return attributes.SelectMany(a => a.Usings.IsDefault ? [] : a.Usings).ToImmutableArray();
        }
    }

    #endregion
}

/// <summary>
/// Parameter modifiers (ref, out, in, params, this).
/// </summary>
public enum ParameterModifier
{
    /// <summary>No modifier.</summary>
    None,
    /// <summary>The ref modifier.</summary>
    Ref,
    /// <summary>The out modifier.</summary>
    Out,
    /// <summary>The in modifier.</summary>
    In,
    /// <summary>The params modifier.</summary>
    Params,
    /// <summary>The this modifier for extension methods.</summary>
    This
}

/// <summary>
/// Represents a validation rule for a parameter.
/// </summary>
public readonly record struct ParameterValidation(ParameterValidationKind Kind, string? MinValue = null, string? MaxValue = null);

/// <summary>
/// The kinds of parameter validation.
/// </summary>
public enum ParameterValidationKind
{
    /// <summary>Throws ArgumentNullException if null.</summary>
    ThrowIfNull,
    /// <summary>Throws ArgumentException if null or empty.</summary>
    ThrowIfNullOrEmpty,
    /// <summary>Throws ArgumentException if null, empty, or whitespace.</summary>
    ThrowIfNullOrWhiteSpace,
    /// <summary>Throws ArgumentOutOfRangeException if outside specified range.</summary>
    ThrowIfOutOfRange,
    /// <summary>Throws ArgumentOutOfRangeException if zero or negative.</summary>
    ThrowIfNotPositive,
    /// <summary>Throws ArgumentOutOfRangeException if negative.</summary>
    ThrowIfNegative,
    /// <summary>Throws ArgumentOutOfRangeException if zero.</summary>
    ThrowIfZero
}
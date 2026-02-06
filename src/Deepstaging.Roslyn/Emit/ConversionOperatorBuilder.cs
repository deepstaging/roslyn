// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for conversion operators (explicit/implicit).
/// Immutable - each method returns a new instance.
/// </summary>
/// <example>
/// <code>
/// // Explicit conversion from int to MyType
/// ConversionOperatorBuilder.Explicit("MyType", "int", "value")
///     .WithExpressionBody("new MyType(value)");
///
/// // Implicit conversion from MyType to int  
/// ConversionOperatorBuilder.Implicit("int", "MyType", "source")
///     .WithExpressionBody("source.Value");
/// </code>
/// </example>
public readonly struct ConversionOperatorBuilder
{
    private readonly bool _isExplicit;
    private readonly string _targetType;
    private readonly string _sourceType;
    private readonly string _parameterName;
    private readonly ImmutableArray<AttributeBuilder> _attributes;
    private readonly ImmutableArray<string> _usings;
    private readonly BodyBuilder? _body;
    private readonly string? _expressionBody;
    private readonly XmlDocumentationBuilder? _xmlDoc;

    private ConversionOperatorBuilder(
        bool isExplicit,
        string targetType,
        string sourceType,
        string parameterName,
        ImmutableArray<AttributeBuilder> attributes,
        ImmutableArray<string> usings,
        BodyBuilder? body,
        string? expressionBody,
        XmlDocumentationBuilder? xmlDoc)
    {
        _isExplicit = isExplicit;
        _targetType = targetType;
        _sourceType = sourceType;
        _parameterName = parameterName;
        _attributes = attributes.IsDefault ? ImmutableArray<AttributeBuilder>.Empty : attributes;
        _usings = usings.IsDefault ? ImmutableArray<string>.Empty : usings;
        _body = body;
        _expressionBody = expressionBody;
        _xmlDoc = xmlDoc;
    }

    #region Factory Methods

    /// <summary>
    /// Creates an explicit conversion operator.
    /// </summary>
    /// <param name="targetType">The target type to convert to.</param>
    /// <param name="sourceType">The source type to convert from.</param>
    /// <param name="parameterName">The parameter name (default: "value").</param>
    public static ConversionOperatorBuilder Explicit(string targetType, string sourceType, string parameterName = "value")
    {
        if (string.IsNullOrWhiteSpace(targetType))
            throw new ArgumentException("Target type cannot be null or empty.", nameof(targetType));
        if (string.IsNullOrWhiteSpace(sourceType))
            throw new ArgumentException("Source type cannot be null or empty.", nameof(sourceType));

        return new ConversionOperatorBuilder(
            true,
            targetType,
            sourceType,
            parameterName,
            ImmutableArray<AttributeBuilder>.Empty,
            ImmutableArray<string>.Empty,
            null,
            null,
            null);
    }

    /// <summary>
    /// Creates an implicit conversion operator.
    /// </summary>
    /// <param name="targetType">The target type to convert to.</param>
    /// <param name="sourceType">The source type to convert from.</param>
    /// <param name="parameterName">The parameter name (default: "value").</param>
    public static ConversionOperatorBuilder Implicit(string targetType, string sourceType, string parameterName = "value")
    {
        if (string.IsNullOrWhiteSpace(targetType))
            throw new ArgumentException("Target type cannot be null or empty.", nameof(targetType));
        if (string.IsNullOrWhiteSpace(sourceType))
            throw new ArgumentException("Source type cannot be null or empty.", nameof(sourceType));

        return new ConversionOperatorBuilder(
            false,
            targetType,
            sourceType,
            parameterName,
            ImmutableArray<AttributeBuilder>.Empty,
            ImmutableArray<string>.Empty,
            null,
            null,
            null);
    }

    #endregion

    #region Body

    /// <summary>
    /// Sets a statement body for the conversion operator.
    /// </summary>
    public ConversionOperatorBuilder WithBody(Func<BodyBuilder, BodyBuilder> configure)
    {
        var body = configure(BodyBuilder.Empty());
        return new ConversionOperatorBuilder(_isExplicit, _targetType, _sourceType, _parameterName,
            _attributes, _usings, body, null, _xmlDoc);
    }

    /// <summary>
    /// Sets an expression body for the conversion operator.
    /// </summary>
    /// <param name="expression">The expression (e.g., "new MyType(value)", "source.Value").</param>
    public ConversionOperatorBuilder WithExpressionBody(string expression)
    {
        return new ConversionOperatorBuilder(_isExplicit, _targetType, _sourceType, _parameterName,
            _attributes, _usings, null, expression, _xmlDoc);
    }

    #endregion

    #region XML Documentation

    /// <summary>
    /// Sets XML documentation for the conversion operator.
    /// </summary>
    public ConversionOperatorBuilder WithXmlDoc(Func<XmlDocumentationBuilder, XmlDocumentationBuilder> configure)
    {
        var xmlDoc = configure(XmlDocumentationBuilder.Create());
        return new ConversionOperatorBuilder(_isExplicit, _targetType, _sourceType, _parameterName,
            _attributes, _usings, _body, _expressionBody, xmlDoc);
    }

    /// <summary>
    /// Sets XML documentation with a simple summary.
    /// </summary>
    public ConversionOperatorBuilder WithXmlDoc(string summary)
    {
        var xmlDoc = XmlDocumentationBuilder.WithSummary(summary);
        return new ConversionOperatorBuilder(_isExplicit, _targetType, _sourceType, _parameterName,
            _attributes, _usings, _body, _expressionBody, xmlDoc);
    }

    #endregion

    #region Attributes

    /// <summary>
    /// Adds an attribute to the conversion operator.
    /// </summary>
    public ConversionOperatorBuilder WithAttribute(string name)
    {
        var attribute = AttributeBuilder.For(name);
        return new ConversionOperatorBuilder(_isExplicit, _targetType, _sourceType, _parameterName,
            _attributes.Add(attribute), _usings, _body, _expressionBody, _xmlDoc);
    }

    /// <summary>
    /// Adds an attribute with configuration.
    /// </summary>
    public ConversionOperatorBuilder WithAttribute(string name, Func<AttributeBuilder, AttributeBuilder> configure)
    {
        var attribute = configure(AttributeBuilder.For(name));
        return new ConversionOperatorBuilder(_isExplicit, _targetType, _sourceType, _parameterName,
            _attributes.Add(attribute), _usings, _body, _expressionBody, _xmlDoc);
    }

    #endregion

    #region Usings

    /// <summary>
    /// Adds a using directive.
    /// </summary>
    public ConversionOperatorBuilder AddUsing(string @namespace)
    {
        return new ConversionOperatorBuilder(_isExplicit, _targetType, _sourceType, _parameterName,
            _attributes, _usings.Add(@namespace), _body, _expressionBody, _xmlDoc);
    }

    /// <summary>
    /// Gets the using directives for this operator.
    /// </summary>
    internal ImmutableArray<string> Usings => _usings;

    #endregion

    #region Building

    /// <summary>
    /// Builds the conversion operator as a syntax node.
    /// </summary>
    internal ConversionOperatorDeclarationSyntax Build()
    {
        var implicitOrExplicit = _isExplicit
            ? SyntaxFactory.Token(SyntaxKind.ExplicitKeyword)
            : SyntaxFactory.Token(SyntaxKind.ImplicitKeyword);

        var op = SyntaxFactory.ConversionOperatorDeclaration(
            implicitOrExplicit,
            SyntaxFactory.ParseTypeName(_targetType));

        // Add modifiers (public static)
        op = op.WithModifiers(SyntaxFactory.TokenList(
            SyntaxFactory.Token(SyntaxKind.PublicKeyword),
            SyntaxFactory.Token(SyntaxKind.StaticKeyword)));

        // Add parameter
        var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier(_parameterName))
            .WithType(SyntaxFactory.ParseTypeName(_sourceType));
        op = op.WithParameterList(SyntaxFactory.ParameterList(
            SyntaxFactory.SingletonSeparatedList(parameter)));

        // Add attributes
        if (_attributes.Length > 0)
        {
            var attributeLists = _attributes.Select(a => a.BuildList()).ToArray();
            op = op.WithAttributeLists(SyntaxFactory.List(attributeLists));
        }

        // Add body or expression body
        if (_expressionBody != null)
        {
            var arrowExpression = SyntaxFactory.ArrowExpressionClause(
                SyntaxFactory.ParseExpression(_expressionBody));
            op = op
                .WithExpressionBody(arrowExpression)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }
        else if (_body.HasValue)
        {
            op = op.WithBody(_body.Value.Build());
        }
        else
        {
            // Empty body
            op = op.WithBody(SyntaxFactory.Block());
        }

        // Add XML documentation
        if (_xmlDoc.HasValue && _xmlDoc.Value.HasContent)
        {
            var trivia = _xmlDoc.Value.Build();
            op = op.WithLeadingTrivia(trivia);
        }

        return op;
    }

    /// <summary>
    /// Gets whether this is an explicit conversion (true) or implicit (false).
    /// </summary>
    public bool IsExplicit => _isExplicit;

    /// <summary>
    /// Gets the target type.
    /// </summary>
    public string TargetType => _targetType;

    /// <summary>
    /// Gets the source type.
    /// </summary>
    public string SourceType => _sourceType;

    #endregion
}

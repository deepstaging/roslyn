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
public record struct ConversionOperatorBuilder
{
    /// <summary>Gets whether this is an explicit conversion (true) or implicit (false).</summary>
    public bool IsExplicit { get; init; }

    /// <summary>Gets the target type.</summary>
    public string TargetType { get; init; }

    /// <summary>Gets the source type.</summary>
    public string SourceType { get; init; }

    /// <summary>Gets the parameter name.</summary>
    public string ParameterName { get; init; }

    /// <summary>Gets the attributes applied to the operator.</summary>
    public ImmutableArray<AttributeBuilder> Attributes { get; init; }

    /// <summary>Gets the using directives.</summary>
    public ImmutableArray<string> Usings { get; init; }

    /// <summary>Gets the body builder.</summary>
    public BodyBuilder? Body { get; init; }

    /// <summary>Gets the expression body.</summary>
    public string? ExpressionBody { get; init; }

    /// <summary>Gets the XML documentation builder.</summary>
    public XmlDocumentationBuilder? XmlDoc { get; init; }

    /// <summary>Gets the preprocessor directive condition for conditional compilation.</summary>
    public Directive? Condition { get; init; }

    /// <summary>Gets the region name for grouping this member in a #region block.</summary>
    public string? Region { get; init; }

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

        return new ConversionOperatorBuilder
        {
            IsExplicit = true,
            TargetType = targetType,
            SourceType = sourceType,
            ParameterName = parameterName,
        };
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

        return new ConversionOperatorBuilder
        {
            IsExplicit = false,
            TargetType = targetType,
            SourceType = sourceType,
            ParameterName = parameterName,
        };
    }

    #endregion

    #region Body

    /// <summary>
    /// Sets a statement body for the conversion operator.
    /// </summary>
    public ConversionOperatorBuilder WithBody(Func<BodyBuilder, BodyBuilder> configure)
    {
        var body = configure(BodyBuilder.Empty());
        return this with { Body = body, ExpressionBody = null };
    }

    /// <summary>
    /// Sets an expression body for the conversion operator.
    /// </summary>
    /// <param name="expression">The expression (e.g., "new MyType(value)", "source.Value").</param>
    public ConversionOperatorBuilder WithExpressionBody(string expression)
    {
        return this with { ExpressionBody = expression, Body = null };
    }

    /// <summary>
    /// Wraps this conversion operator in a preprocessor directive (#if/#endif).
    /// </summary>
    /// <param name="directive">The directive condition (e.g., Directives.Net6OrGreater).</param>
    public ConversionOperatorBuilder When(Directive directive) =>
        this with { Condition = directive };

    /// <summary>
    /// Assigns this conversion operator to a named region for grouping in #region/#endregion blocks.
    /// </summary>
    /// <param name="regionName">The region name (e.g., "Conversions").</param>
    public ConversionOperatorBuilder InRegion(string regionName) =>
        this with { Region = regionName };

    #endregion

    #region XML Documentation

    /// <summary>
    /// Sets XML documentation for the conversion operator.
    /// </summary>
    public ConversionOperatorBuilder WithXmlDoc(Func<XmlDocumentationBuilder, XmlDocumentationBuilder> configure)
    {
        var xmlDoc = configure(XmlDocumentationBuilder.Create());
        return this with { XmlDoc = xmlDoc };
    }

    /// <summary>
    /// Sets XML documentation with a simple summary.
    /// </summary>
    public ConversionOperatorBuilder WithXmlDoc(string summary)
    {
        var xmlDoc = XmlDocumentationBuilder.ForSummary(summary);
        return this with { XmlDoc = xmlDoc };
    }

    #endregion

    #region Attributes

    /// <summary>
    /// Adds an attribute to the conversion operator.
    /// </summary>
    public ConversionOperatorBuilder WithAttribute(string name)
    {
        var attribute = AttributeBuilder.For(name);
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(attribute) };
    }

    /// <summary>
    /// Adds an attribute with configuration.
    /// </summary>
    public ConversionOperatorBuilder WithAttribute(string name, Func<AttributeBuilder, AttributeBuilder> configure)
    {
        var attribute = configure(AttributeBuilder.For(name));
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(attribute) };
    }

    #endregion

    #region Usings

    /// <summary>
    /// Adds a using directive.
    /// </summary>
    public ConversionOperatorBuilder AddUsing(string @namespace)
    {
        var usings = Usings.IsDefault ? [] : Usings;
        return this with { Usings = usings.Add(@namespace) };
    }

    #endregion

    #region Building

    /// <summary>
    /// Builds the conversion operator as a syntax node.
    /// </summary>
    internal ConversionOperatorDeclarationSyntax Build()
    {
        var implicitOrExplicit = IsExplicit
            ? SyntaxFactory.Token(SyntaxKind.ExplicitKeyword)
            : SyntaxFactory.Token(SyntaxKind.ImplicitKeyword);

        var op = SyntaxFactory.ConversionOperatorDeclaration(
            implicitOrExplicit,
            SyntaxFactory.ParseTypeName(TargetType));

        // Add modifiers (public static)
        op = op.WithModifiers(SyntaxFactory.TokenList(
            SyntaxFactory.Token(SyntaxKind.PublicKeyword),
            SyntaxFactory.Token(SyntaxKind.StaticKeyword)));

        // Add parameter
        var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier(ParameterName))
            .WithType(SyntaxFactory.ParseTypeName(SourceType));
        op = op.WithParameterList(SyntaxFactory.ParameterList(
            SyntaxFactory.SingletonSeparatedList(parameter)));

        // Add attributes
        var attributes = Attributes.IsDefault ? [] : Attributes;
        if (attributes.Length > 0)
        {
            var attributeLists = attributes.Select(a => a.BuildList()).ToArray();
            op = op.WithAttributeLists(SyntaxFactory.List(attributeLists));
        }

        // Add body or expression body
        if (ExpressionBody != null)
        {
            var arrowExpression = SyntaxFactory.ArrowExpressionClause(
                SyntaxFactory.ParseExpression(ExpressionBody));
            op = op
                .WithExpressionBody(arrowExpression)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }
        else if (Body.HasValue)
        {
            op = op.WithBody(Body.Value.Build());
        }
        else
        {
            // Empty body
            op = op.WithBody(SyntaxFactory.Block());
        }

        // Add XML documentation
        if (XmlDoc.HasValue && XmlDoc.Value.HasContent)
        {
            var trivia = XmlDoc.Value.Build();
            op = op.WithLeadingTrivia(trivia);
        }

        // Wrap in preprocessor directive if specified
        if (Condition.HasValue)
        {
            op = DirectiveHelper.WrapInDirective(op, Condition.Value);
        }

        return op;
    }

    #endregion
}

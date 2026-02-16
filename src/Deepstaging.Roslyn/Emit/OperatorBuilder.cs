// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for operator overloads (binary and unary).
/// Immutable - each method returns a new instance.
/// </summary>
/// <example>
/// <code>
/// // Equality operator
/// OperatorBuilder.Equality("CustomerId")
///     .WithExpressionBody("left.Equals(right)");
///
/// // Binary operator with custom types
/// OperatorBuilder.Binary("+", "Money", "Money", "Money")
///     .WithExpressionBody("new Money(left.Amount + right.Amount)");
///
/// // Unary operator
/// OperatorBuilder.UnaryMinus("Money")
///     .WithExpressionBody("new Money(-operand.Amount)");
/// </code>
/// </example>
public record struct OperatorBuilder
{
    /// <summary>Gets the operator symbol.</summary>
    public string Operator { get; init; }

    /// <summary>Gets whether this is a unary operator.</summary>
    public bool IsUnary { get; init; }

    /// <summary>Gets the operand type (left operand for binary).</summary>
    public string OperandType { get; init; }

    /// <summary>Gets the right operand type for binary operators.</summary>
    public string? RightType { get; init; }

    /// <summary>Gets the return type.</summary>
    public string ReturnType { get; init; }

    /// <summary>Gets the left parameter name.</summary>
    public string LeftParameterName { get; init; }

    /// <summary>Gets the right parameter name.</summary>
    public string RightParameterName { get; init; }

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

    #region Factory Methods - Comparison Operators

    /// <summary>
    /// Creates an equality operator (==).
    /// </summary>
    /// <param name="type">The type to compare.</param>
    /// <param name="leftParameterName">Left parameter name (default: "left").</param>
    /// <param name="rightParameterName">Right parameter name (default: "right").</param>
    public static OperatorBuilder Equality(
        string type,
        string leftParameterName = "left",
        string rightParameterName = "right")
        => Binary("==", type, type, "bool", leftParameterName, rightParameterName);

    /// <summary>
    /// Creates an inequality operator (!=).
    /// </summary>
    /// <param name="type">The type to compare.</param>
    /// <param name="leftParameterName">Left parameter name (default: "left").</param>
    /// <param name="rightParameterName">Right parameter name (default: "right").</param>
    public static OperatorBuilder Inequality(
        string type,
        string leftParameterName = "left",
        string rightParameterName = "right")
        => Binary("!=", type, type, "bool", leftParameterName, rightParameterName);

    /// <summary>
    /// Creates a less-than operator (&lt;).
    /// </summary>
    /// <param name="type">The type to compare.</param>
    /// <param name="leftParameterName">Left parameter name (default: "left").</param>
    /// <param name="rightParameterName">Right parameter name (default: "right").</param>
    public static OperatorBuilder LessThan(
        string type,
        string leftParameterName = "left",
        string rightParameterName = "right")
        => Binary("<", type, type, "bool", leftParameterName, rightParameterName);

    /// <summary>
    /// Creates a greater-than operator (&gt;).
    /// </summary>
    /// <param name="type">The type to compare.</param>
    /// <param name="leftParameterName">Left parameter name (default: "left").</param>
    /// <param name="rightParameterName">Right parameter name (default: "right").</param>
    public static OperatorBuilder GreaterThan(
        string type,
        string leftParameterName = "left",
        string rightParameterName = "right")
        => Binary(">", type, type, "bool", leftParameterName, rightParameterName);

    /// <summary>
    /// Creates a less-than-or-equal operator (&lt;=).
    /// </summary>
    /// <param name="type">The type to compare.</param>
    /// <param name="leftParameterName">Left parameter name (default: "left").</param>
    /// <param name="rightParameterName">Right parameter name (default: "right").</param>
    public static OperatorBuilder LessThanOrEqual(
        string type,
        string leftParameterName = "left",
        string rightParameterName = "right")
        => Binary("<=", type, type, "bool", leftParameterName, rightParameterName);

    /// <summary>
    /// Creates a greater-than-or-equal operator (&gt;=).
    /// </summary>
    /// <param name="type">The type to compare.</param>
    /// <param name="leftParameterName">Left parameter name (default: "left").</param>
    /// <param name="rightParameterName">Right parameter name (default: "right").</param>
    public static OperatorBuilder GreaterThanOrEqual(
        string type,
        string leftParameterName = "left",
        string rightParameterName = "right")
        => Binary(">=", type, type, "bool", leftParameterName, rightParameterName);

    #endregion

    #region Factory Methods - Arithmetic Operators

    /// <summary>
    /// Creates an addition operator (+).
    /// </summary>
    /// <param name="type">The operand and return type.</param>
    /// <param name="leftParameterName">Left parameter name (default: "left").</param>
    /// <param name="rightParameterName">Right parameter name (default: "right").</param>
    public static OperatorBuilder Addition(
        string type,
        string leftParameterName = "left",
        string rightParameterName = "right")
        => Binary("+", type, type, type, leftParameterName, rightParameterName);

    /// <summary>
    /// Creates a subtraction operator (-).
    /// </summary>
    /// <param name="type">The operand and return type.</param>
    /// <param name="leftParameterName">Left parameter name (default: "left").</param>
    /// <param name="rightParameterName">Right parameter name (default: "right").</param>
    public static OperatorBuilder Subtraction(
        string type,
        string leftParameterName = "left",
        string rightParameterName = "right")
        => Binary("-", type, type, type, leftParameterName, rightParameterName);

    /// <summary>
    /// Creates a multiplication operator (*).
    /// </summary>
    /// <param name="type">The operand and return type.</param>
    /// <param name="leftParameterName">Left parameter name (default: "left").</param>
    /// <param name="rightParameterName">Right parameter name (default: "right").</param>
    public static OperatorBuilder Multiplication(
        string type,
        string leftParameterName = "left",
        string rightParameterName = "right")
        => Binary("*", type, type, type, leftParameterName, rightParameterName);

    /// <summary>
    /// Creates a division operator (/).
    /// </summary>
    /// <param name="type">The operand and return type.</param>
    /// <param name="leftParameterName">Left parameter name (default: "left").</param>
    /// <param name="rightParameterName">Right parameter name (default: "right").</param>
    public static OperatorBuilder Division(
        string type,
        string leftParameterName = "left",
        string rightParameterName = "right")
        => Binary("/", type, type, type, leftParameterName, rightParameterName);

    /// <summary>
    /// Creates a modulus operator (%).
    /// </summary>
    /// <param name="type">The operand and return type.</param>
    /// <param name="leftParameterName">Left parameter name (default: "left").</param>
    /// <param name="rightParameterName">Right parameter name (default: "right").</param>
    public static OperatorBuilder Modulus(
        string type,
        string leftParameterName = "left",
        string rightParameterName = "right")
        => Binary("%", type, type, type, leftParameterName, rightParameterName);

    #endregion

    #region Factory Methods - Unary Operators

    /// <summary>
    /// Creates a unary plus operator (+).
    /// </summary>
    /// <param name="type">The operand and return type.</param>
    /// <param name="parameterName">Parameter name (default: "operand").</param>
    public static OperatorBuilder UnaryPlus(string type, string parameterName = "operand")
        => Unary("+", type, type, parameterName);

    /// <summary>
    /// Creates a unary minus/negation operator (-).
    /// </summary>
    /// <param name="type">The operand and return type.</param>
    /// <param name="parameterName">Parameter name (default: "operand").</param>
    public static OperatorBuilder UnaryMinus(string type, string parameterName = "operand")
        => Unary("-", type, type, parameterName);

    /// <summary>
    /// Creates a logical negation operator (!).
    /// </summary>
    /// <param name="type">The operand type.</param>
    /// <param name="returnType">The return type (default: "bool").</param>
    /// <param name="parameterName">Parameter name (default: "operand").</param>
    public static OperatorBuilder LogicalNot(string type, string returnType = "bool", string parameterName = "operand")
        => Unary("!", type, returnType, parameterName);

    /// <summary>
    /// Creates a bitwise complement operator (~).
    /// </summary>
    /// <param name="type">The operand and return type.</param>
    /// <param name="parameterName">Parameter name (default: "operand").</param>
    public static OperatorBuilder BitwiseComplement(string type, string parameterName = "operand")
        => Unary("~", type, type, parameterName);

    /// <summary>
    /// Creates a bitwise AND operator (&amp;).
    /// </summary>
    /// <param name="type">The operand and return type.</param>
    /// <param name="leftParameterName">Left parameter name (default: "left").</param>
    /// <param name="rightParameterName">Right parameter name (default: "right").</param>
    public static OperatorBuilder BitwiseAnd(
        string type,
        string leftParameterName = "left",
        string rightParameterName = "right")
        => Binary("&", type, type, type, leftParameterName, rightParameterName);

    /// <summary>
    /// Creates a bitwise OR operator (|).
    /// </summary>
    /// <param name="type">The operand and return type.</param>
    /// <param name="leftParameterName">Left parameter name (default: "left").</param>
    /// <param name="rightParameterName">Right parameter name (default: "right").</param>
    public static OperatorBuilder BitwiseOr(
        string type,
        string leftParameterName = "left",
        string rightParameterName = "right")
        => Binary("|", type, type, type, leftParameterName, rightParameterName);

    /// <summary>
    /// Creates a bitwise XOR operator (^).
    /// </summary>
    /// <param name="type">The operand and return type.</param>
    /// <param name="leftParameterName">Left parameter name (default: "left").</param>
    /// <param name="rightParameterName">Right parameter name (default: "right").</param>
    public static OperatorBuilder ExclusiveOr(
        string type,
        string leftParameterName = "left",
        string rightParameterName = "right")
        => Binary("^", type, type, type, leftParameterName, rightParameterName);

    /// <summary>
    /// Creates a left shift operator (&lt;&lt;).
    /// </summary>
    /// <param name="type">The operand and return type.</param>
    /// <param name="leftParameterName">Left parameter name (default: "left").</param>
    /// <param name="rightParameterName">Right parameter name (default: "right").</param>
    public static OperatorBuilder LeftShift(
        string type,
        string leftParameterName = "left",
        string rightParameterName = "right")
        => Binary("<<", type, "int", type, leftParameterName, rightParameterName);

    /// <summary>
    /// Creates a right shift operator (&gt;&gt;).
    /// </summary>
    /// <param name="type">The operand and return type.</param>
    /// <param name="leftParameterName">Left parameter name (default: "left").</param>
    /// <param name="rightParameterName">Right parameter name (default: "right").</param>
    public static OperatorBuilder RightShift(
        string type,
        string leftParameterName = "left",
        string rightParameterName = "right")
        => Binary(">>", type, "int", type, leftParameterName, rightParameterName);

    /// <summary>
    /// Creates an unsigned right shift operator (&gt;&gt;&gt;).
    /// </summary>
    /// <param name="type">The operand and return type.</param>
    /// <param name="leftParameterName">Left parameter name (default: "left").</param>
    /// <param name="rightParameterName">Right parameter name (default: "right").</param>
    public static OperatorBuilder UnsignedRightShift(
        string type,
        string leftParameterName = "left",
        string rightParameterName = "right")
        => Binary(">>>", type, "int", type, leftParameterName, rightParameterName);

    /// <summary>
    /// Creates an increment operator (++).
    /// </summary>
    /// <param name="type">The operand and return type.</param>
    /// <param name="parameterName">Parameter name (default: "operand").</param>
    public static OperatorBuilder Increment(string type, string parameterName = "operand")
        => Unary("++", type, type, parameterName);

    /// <summary>
    /// Creates a decrement operator (--).
    /// </summary>
    /// <param name="type">The operand and return type.</param>
    /// <param name="parameterName">Parameter name (default: "operand").</param>
    public static OperatorBuilder Decrement(string type, string parameterName = "operand")
        => Unary("--", type, type, parameterName);

    /// <summary>
    /// Creates a true operator.
    /// </summary>
    /// <param name="type">The operand type.</param>
    /// <param name="parameterName">Parameter name (default: "operand").</param>
    public static OperatorBuilder True(string type, string parameterName = "operand")
        => Unary("true", type, "bool", parameterName);

    /// <summary>
    /// Creates a false operator.
    /// </summary>
    /// <param name="type">The operand type.</param>
    /// <param name="parameterName">Parameter name (default: "operand").</param>
    public static OperatorBuilder False(string type, string parameterName = "operand")
        => Unary("false", type, "bool", parameterName);

    #endregion

    #region Factory Methods - Generic

    /// <summary>
    /// Creates a binary operator with custom types.
    /// </summary>
    /// <param name="operator">The operator symbol (e.g., "+", "==", "&lt;&lt;").</param>
    /// <param name="leftType">The left operand type.</param>
    /// <param name="rightType">The right operand type.</param>
    /// <param name="returnType">The return type.</param>
    /// <param name="leftParameterName">Left parameter name (default: "left").</param>
    /// <param name="rightParameterName">Right parameter name (default: "right").</param>
    public static OperatorBuilder Binary(
        string @operator,
        string leftType,
        string rightType,
        string returnType,
        string leftParameterName = "left",
        string rightParameterName = "right")
    {
        ValidateOperator(@operator);
        ValidateType(leftType, nameof(leftType));
        ValidateType(rightType, nameof(rightType));
        ValidateType(returnType, nameof(returnType));

        return new OperatorBuilder
        {
            Operator = @operator,
            IsUnary = false,
            OperandType = leftType,
            RightType = rightType,
            ReturnType = returnType,
            LeftParameterName = leftParameterName,
            RightParameterName = rightParameterName
        };
    }

    /// <summary>
    /// Creates a unary operator.
    /// </summary>
    /// <param name="operator">The operator symbol (e.g., "-", "!", "++").</param>
    /// <param name="operandType">The operand type.</param>
    /// <param name="returnType">The return type.</param>
    /// <param name="parameterName">Parameter name (default: "operand").</param>
    public static OperatorBuilder Unary(
        string @operator,
        string operandType,
        string returnType,
        string parameterName = "operand")
    {
        ValidateOperator(@operator);
        ValidateType(operandType, nameof(operandType));
        ValidateType(returnType, nameof(returnType));

        return new OperatorBuilder
        {
            Operator = @operator,
            IsUnary = true,
            OperandType = operandType,
            ReturnType = returnType,
            LeftParameterName = parameterName,
            RightParameterName = string.Empty
        };
    }

    private static void ValidateOperator(string @operator)
    {
        if (string.IsNullOrWhiteSpace(@operator))
            throw new ArgumentException("Operator cannot be null or empty.", nameof(@operator));
    }

    private static void ValidateType(string type, string paramName)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Type cannot be null or empty.", paramName);
    }

    #endregion

    #region Body

    /// <summary>
    /// Sets a statement body for the operator.
    /// </summary>
    public OperatorBuilder WithBody(Func<BodyBuilder, BodyBuilder> configure)
    {
        var body = configure(BodyBuilder.Empty());
        return this with { Body = body, ExpressionBody = null };
    }

    /// <summary>
    /// Sets an expression body for the operator.
    /// </summary>
    /// <param name="expression">The expression (e.g., "left.Equals(right)", "new Money(-operand.Amount)").</param>
    public OperatorBuilder WithExpressionBody(string expression) =>
        this with { ExpressionBody = expression, Body = null };

    /// <summary>
    /// Wraps this operator in a preprocessor directive (#if/#endif).
    /// </summary>
    /// <param name="directive">The directive condition (e.g., Directives.Net6OrGreater).</param>
    public OperatorBuilder When(Directive directive) =>
        this with { Condition = directive };

    /// <summary>
    /// Assigns this operator to a named region for grouping in #region/#endregion blocks.
    /// </summary>
    /// <param name="regionName">The region name (e.g., "Operators").</param>
    public OperatorBuilder InRegion(string regionName) =>
        this with { Region = regionName };

    #endregion

    #region XML Documentation

    /// <summary>
    /// Sets XML documentation for the operator.
    /// </summary>
    public OperatorBuilder WithXmlDoc(Func<XmlDocumentationBuilder, XmlDocumentationBuilder> configure)
    {
        var xmlDoc = configure(XmlDocumentationBuilder.Create());
        return this with { XmlDoc = xmlDoc };
    }

    /// <summary>
    /// Sets XML documentation with a simple summary.
    /// </summary>
    public OperatorBuilder WithXmlDoc(string summary)
    {
        var xmlDoc = XmlDocumentationBuilder.ForSummary(summary);
        return this with { XmlDoc = xmlDoc };
    }

    /// <summary>
    /// Sets the XML documentation for the operator using inheritdoc.
    /// </summary>
    /// <param name="cref">Optional cref attribute for the inheritdoc element.</param>
    public OperatorBuilder WithInheritDoc(string? cref = null)
    {
        var xmlDoc = XmlDocumentationBuilder.ForInheritDoc(cref);
        return this with { XmlDoc = xmlDoc };
    }

    #endregion

    #region Attributes

    /// <summary>
    /// Adds an attribute to the operator.
    /// </summary>
    public OperatorBuilder WithAttribute(string name)
    {
        var attribute = AttributeBuilder.For(name);
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(attribute) };
    }

    /// <summary>
    /// Adds an attribute with configuration.
    /// </summary>
    public OperatorBuilder WithAttribute(string name, Func<AttributeBuilder, AttributeBuilder> configure)
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
    public OperatorBuilder AddUsing(string @namespace)
    {
        var usings = Usings.IsDefault ? [] : Usings;
        return this with { Usings = usings.Add(@namespace) };
    }

    #endregion

    #region Building

    /// <summary>
    /// Builds the operator as a syntax node.
    /// </summary>
    internal OperatorDeclarationSyntax Build()
    {
        var operatorToken = GetOperatorToken();

        var op = SyntaxFactory.OperatorDeclaration(
            SyntaxFactory.ParseTypeName(ReturnType),
            operatorToken);

        // Add modifiers (public static)
        op = op.WithModifiers(SyntaxFactory.TokenList(
            SyntaxFactory.Token(SyntaxKind.PublicKeyword),
            SyntaxFactory.Token(SyntaxKind.StaticKeyword)));

        // Add parameters
        var parameters = new List<ParameterSyntax>();

        var leftParam = SyntaxFactory.Parameter(SyntaxFactory.Identifier(LeftParameterName))
            .WithType(SyntaxFactory.ParseTypeName(OperandType));

        parameters.Add(leftParam);

        if (!IsUnary && RightType != null)
        {
            var rightParam = SyntaxFactory.Parameter(SyntaxFactory.Identifier(RightParameterName))
                .WithType(SyntaxFactory.ParseTypeName(RightType));

            parameters.Add(rightParam);
        }

        op = op.WithParameterList(SyntaxFactory.ParameterList(
            SyntaxFactory.SeparatedList(parameters)));

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
        if (Condition.HasValue) op = DirectiveHelper.WrapInDirective(op, Condition.Value);

        return op;
    }

    private SyntaxToken GetOperatorToken() =>
        Operator switch
        {
            "+" => SyntaxFactory.Token(SyntaxKind.PlusToken),
            "-" => SyntaxFactory.Token(SyntaxKind.MinusToken),
            "*" => SyntaxFactory.Token(SyntaxKind.AsteriskToken),
            "/" => SyntaxFactory.Token(SyntaxKind.SlashToken),
            "%" => SyntaxFactory.Token(SyntaxKind.PercentToken),
            "&" => SyntaxFactory.Token(SyntaxKind.AmpersandToken),
            "|" => SyntaxFactory.Token(SyntaxKind.BarToken),
            "^" => SyntaxFactory.Token(SyntaxKind.CaretToken),
            "<<" => SyntaxFactory.Token(SyntaxKind.LessThanLessThanToken),
            ">>" => SyntaxFactory.Token(SyntaxKind.GreaterThanGreaterThanToken),
            ">>>" => SyntaxFactory.Token(SyntaxKind.GreaterThanGreaterThanGreaterThanToken),
            "==" => SyntaxFactory.Token(SyntaxKind.EqualsEqualsToken),
            "!=" => SyntaxFactory.Token(SyntaxKind.ExclamationEqualsToken),
            "<" => SyntaxFactory.Token(SyntaxKind.LessThanToken),
            ">" => SyntaxFactory.Token(SyntaxKind.GreaterThanToken),
            "<=" => SyntaxFactory.Token(SyntaxKind.LessThanEqualsToken),
            ">=" => SyntaxFactory.Token(SyntaxKind.GreaterThanEqualsToken),
            "!" => SyntaxFactory.Token(SyntaxKind.ExclamationToken),
            "~" => SyntaxFactory.Token(SyntaxKind.TildeToken),
            "++" => SyntaxFactory.Token(SyntaxKind.PlusPlusToken),
            "--" => SyntaxFactory.Token(SyntaxKind.MinusMinusToken),
            "true" => SyntaxFactory.Token(SyntaxKind.TrueKeyword),
            "false" => SyntaxFactory.Token(SyntaxKind.FalseKeyword),
            _ => throw new InvalidOperationException($"Unsupported operator: {Operator}")
        };

    #endregion
}
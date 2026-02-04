// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for methods.
/// Supports instance and static methods, async methods, and custom bodies.
/// Immutable - each method returns a new instance.
/// </summary>
public readonly struct MethodBuilder
{
    private readonly string _name;
    private readonly string? _returnType;
    private readonly Accessibility _accessibility;
    private readonly bool _isStatic;
    private readonly bool _isVirtual;
    private readonly bool _isOverride;
    private readonly bool _isAbstract;
    private readonly bool _isAsync;
    private readonly ImmutableArray<TypeParameterBuilder> _typeParameters;
    private readonly ImmutableArray<ParameterBuilder> _parameters;
    private readonly ImmutableArray<AttributeBuilder> _attributes;
    private readonly ImmutableArray<string> _usings;
    private readonly BodyBuilder? _body;
    private readonly string? _expressionBody;
    private readonly XmlDocumentationBuilder? _xmlDoc;

    private MethodBuilder(
        string name,
        string? returnType,
        Accessibility accessibility,
        bool isStatic,
        bool isVirtual,
        bool isOverride,
        bool isAbstract,
        bool isAsync,
        ImmutableArray<TypeParameterBuilder> typeParameters,
        ImmutableArray<ParameterBuilder> parameters,
        ImmutableArray<AttributeBuilder> attributes,
        ImmutableArray<string> usings,
        BodyBuilder? body,
        string? expressionBody,
        XmlDocumentationBuilder? xmlDoc)
    {
        _name = name;
        _returnType = returnType;
        _accessibility = accessibility;
        _isStatic = isStatic;
        _isVirtual = isVirtual;
        _isOverride = isOverride;
        _isAbstract = isAbstract;
        _isAsync = isAsync;
        _typeParameters = typeParameters.IsDefault ? ImmutableArray<TypeParameterBuilder>.Empty : typeParameters;
        _parameters = parameters.IsDefault ? ImmutableArray<ParameterBuilder>.Empty : parameters;
        _attributes = attributes.IsDefault ? ImmutableArray<AttributeBuilder>.Empty : attributes;
        _usings = usings.IsDefault ? ImmutableArray<string>.Empty : usings;
        _body = body;
        _expressionBody = expressionBody;
        _xmlDoc = xmlDoc;
    }

    #region Factory Methods

    /// <summary>
    /// Creates a method builder for the specified method.
    /// </summary>
    /// <param name="name">The method name (e.g., "GetById", "Process").</param>
    public static MethodBuilder For(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Method name cannot be null or empty.", nameof(name));

        return new MethodBuilder(
            name,
            returnType: "void",
            Accessibility.Public,
            isStatic: false,
            isVirtual: false,
            isOverride: false,
            isAbstract: false,
            isAsync: false,
            ImmutableArray<TypeParameterBuilder>.Empty,
            ImmutableArray<ParameterBuilder>.Empty,
            ImmutableArray<AttributeBuilder>.Empty,
            ImmutableArray<string>.Empty,
            body: null,
            expressionBody: null,
            xmlDoc: null);
    }

    /// <summary>
    /// Creates a method builder by parsing a C# method signature.
    /// </summary>
    /// <param name="signature">The method signature (e.g., "public string GetName(int id)").</param>
    /// <returns>A configured MethodBuilder with parsed modifiers, return type, and parameters.</returns>
    /// <exception cref="ArgumentException">Thrown when the signature cannot be parsed.</exception>
    /// <example>
    /// <code>
    /// // Simple method
    /// var builder = MethodBuilder.Parse("public string GetName(int id)");
    /// 
    /// // Async method with default parameter
    /// var builder = MethodBuilder.Parse("public async Task&lt;bool&gt; ProcessAsync(string input, CancellationToken ct = default)");
    /// 
    /// // Generic method with constraints
    /// var builder = MethodBuilder.Parse("public T Convert&lt;T&gt;(object value) where T : class");
    /// </code>
    /// </example>
    public static MethodBuilder Parse(string signature) => SignatureParser.ParseMethod(signature);

    #endregion

    #region Return Type

    /// <summary>
    /// Sets the return type of the method.
    /// </summary>
    /// <param name="returnType">The return type (e.g., "void", "string", "Task&lt;int&gt;").</param>
    public MethodBuilder WithReturnType(string returnType)
    {
        return new MethodBuilder(_name, returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters, _parameters, _attributes, _usings, _body, _expressionBody, _xmlDoc);
    }

    #endregion

    #region Accessibility & Modifiers

    /// <summary>
    /// Sets the accessibility of the method.
    /// </summary>
    public MethodBuilder WithAccessibility(Accessibility accessibility)
    {
        return new MethodBuilder(_name, _returnType, accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters, _parameters, _attributes, _usings, _body, _expressionBody, _xmlDoc);
    }

    /// <summary>
    /// Marks the method as static.
    /// </summary>
    public MethodBuilder AsStatic()
    {
        return new MethodBuilder(_name, _returnType, _accessibility, true, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters, _parameters, _attributes, _usings, _body, _expressionBody, _xmlDoc);
    }

    /// <summary>
    /// Marks the method as virtual.
    /// </summary>
    public MethodBuilder AsVirtual()
    {
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, true, _isOverride,
            _isAbstract, _isAsync, _typeParameters, _parameters, _attributes, _usings, _body, _expressionBody, _xmlDoc);
    }

    /// <summary>
    /// Marks the method as override.
    /// </summary>
    public MethodBuilder AsOverride()
    {
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, true,
            _isAbstract, _isAsync, _typeParameters, _parameters, _attributes, _usings, _body, _expressionBody, _xmlDoc);
    }

    /// <summary>
    /// Marks the method as abstract.
    /// </summary>
    public MethodBuilder AsAbstract()
    {
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            true, _isAsync, _typeParameters, _parameters, _attributes, _usings, _body, _expressionBody, _xmlDoc);
    }

    /// <summary>
    /// Marks the method as async.
    /// </summary>
    public MethodBuilder Async()
    {
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, true, _typeParameters, _parameters, _attributes, _usings, _body, _expressionBody, _xmlDoc);
    }

    #endregion

    #region Type Parameters

    /// <summary>
    /// Adds a type parameter to the method.
    /// </summary>
    /// <param name="name">The type parameter name (e.g., "T", "RT").</param>
    public MethodBuilder AddTypeParameter(string name)
    {
        var typeParameter = TypeParameterBuilder.For(name);
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters.Add(typeParameter), _parameters, _attributes, _usings, _body,
            _expressionBody, _xmlDoc);
    }

    /// <summary>
    /// Adds a type parameter to the method with configuration.
    /// </summary>
    /// <param name="name">The type parameter name (e.g., "T", "RT").</param>
    /// <param name="configure">Configuration callback for the type parameter.</param>
    public MethodBuilder AddTypeParameter(string name, Func<TypeParameterBuilder, TypeParameterBuilder> configure)
    {
        var typeParameter = configure(TypeParameterBuilder.For(name));
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters.Add(typeParameter), _parameters, _attributes, _usings, _body,
            _expressionBody, _xmlDoc);
    }

    /// <summary>
    /// Adds a pre-configured type parameter to the method.
    /// </summary>
    public MethodBuilder AddTypeParameter(TypeParameterBuilder typeParameter)
    {
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters.Add(typeParameter), _parameters, _attributes, _usings, _body,
            _expressionBody, _xmlDoc);
    }

    #endregion

    #region Parameters

    /// <summary>
    /// Adds a parameter to the method.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="type">The parameter type.</param>
    public MethodBuilder AddParameter(string name, string type)
    {
        var parameter = ParameterBuilder.For(name, type);
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters, _parameters.Add(parameter), _attributes, _usings, _body,
            _expressionBody, _xmlDoc);
    }

    /// <summary>
    /// Adds a parameter to the method with configuration.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="type">The parameter type.</param>
    /// <param name="configure">Configuration callback for the parameter.</param>
    public MethodBuilder AddParameter(string name, string type, Func<ParameterBuilder, ParameterBuilder> configure)
    {
        var parameter = configure(ParameterBuilder.For(name, type));
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters, _parameters.Add(parameter), _attributes, _usings, _body,
            _expressionBody, _xmlDoc);
    }

    /// <summary>
    /// Adds a pre-configured parameter to the method.
    /// </summary>
    public MethodBuilder AddParameter(ParameterBuilder parameter)
    {
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters, _parameters.Add(parameter), _attributes, _usings, _body,
            _expressionBody, _xmlDoc);
    }

    #endregion

    #region Body

    /// <summary>
    /// Sets the method body using a body builder configuration.
    /// </summary>
    public MethodBuilder WithBody(Func<BodyBuilder, BodyBuilder> configure)
    {
        var body = configure(BodyBuilder.Empty());
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters, _parameters, _attributes, _usings, body, null, _xmlDoc);
    }

    /// <summary>
    /// Sets the method body as an expression (arrow expression syntax).
    /// Use this for expression-bodied methods like: public int GetValue() => 42;
    /// </summary>
    /// <param name="expression">The expression (e.g., "42", "x + y", "liftEff(...)").</param>
    public MethodBuilder WithExpressionBody(string expression)
    {
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters, _parameters, _attributes, _usings, null, expression, _xmlDoc);
    }

    /// <summary>
    /// Appends to the existing expression body.
    /// Useful for chaining method calls like: .WithExpressionBody("liftEff(...)").AppendExpressionBody(".WithActivity(...)")
    /// </summary>
    /// <param name="suffix">The text to append to the expression body.</param>
    /// <exception cref="InvalidOperationException">Thrown when no expression body has been set.</exception>
    public MethodBuilder AppendExpressionBody(string suffix)
    {
        if (_expressionBody is null)
            throw new InvalidOperationException(
                "Cannot append to expression body when no expression body has been set.");

        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters, _parameters, _attributes, _usings, null, _expressionBody + suffix,
            _xmlDoc);
    }

    #endregion

    #region XML Documentation

    /// <summary>
    /// Sets the XML documentation for the method.
    /// </summary>
    /// <param name="configure">Configuration callback for the XML documentation.</param>
    public MethodBuilder WithXmlDoc(Func<XmlDocumentationBuilder, XmlDocumentationBuilder> configure)
    {
        var xmlDoc = configure(XmlDocumentationBuilder.Create());
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters, _parameters, _attributes, _usings, _body, _expressionBody, xmlDoc);
    }

    /// <summary>
    /// Sets the XML documentation for the method with a simple summary.
    /// </summary>
    /// <param name="summary">The summary text.</param>
    public MethodBuilder WithXmlDoc(string summary)
    {
        var xmlDoc = XmlDocumentationBuilder.WithSummary(summary);
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters, _parameters, _attributes, _usings, _body, _expressionBody, xmlDoc);
    }

    /// <summary>
    /// Sets the XML documentation for the method from parsed XmlDocumentation.
    /// </summary>
    /// <param name="documentation">The parsed XML documentation to copy.</param>
    public MethodBuilder WithXmlDoc(XmlDocumentation documentation)
    {
        if (documentation.IsEmpty)
            return this;

        var xmlDoc = XmlDocumentationBuilder.From(documentation);
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters, _parameters, _attributes, _usings, _body, _expressionBody, xmlDoc);
    }

    #endregion

    #region Attributes

    /// <summary>
    /// Adds an attribute to the method.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "Obsolete", "MethodImpl").</param>
    public MethodBuilder WithAttribute(string name)
    {
        var attribute = AttributeBuilder.For(name);
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters, _parameters, _attributes.Add(attribute), _usings, _body,
            _expressionBody, _xmlDoc);
    }

    /// <summary>
    /// Adds an attribute to the method with configuration.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "Obsolete", "MethodImpl").</param>
    /// <param name="configure">Configuration callback for the attribute.</param>
    public MethodBuilder WithAttribute(string name, Func<AttributeBuilder, AttributeBuilder> configure)
    {
        var attribute = configure(AttributeBuilder.For(name));
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters, _parameters, _attributes.Add(attribute), _usings, _body,
            _expressionBody, _xmlDoc);
    }

    /// <summary>
    /// Adds a pre-configured attribute to the method.
    /// </summary>
    public MethodBuilder WithAttribute(AttributeBuilder attribute)
    {
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters, _parameters, _attributes.Add(attribute), _usings, _body,
            _expressionBody, _xmlDoc);
    }

    #endregion

    #region Usings

    /// <summary>
    /// Adds a using directive that will be collected by the containing TypeBuilder.
    /// </summary>
    /// <param name="namespace">The namespace to add (e.g., "System.Linq", "static System.Math").</param>
    public MethodBuilder AddUsing(string @namespace)
    {
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters, _parameters, _attributes, _usings.Add(@namespace), _body,
            _expressionBody, _xmlDoc);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="namespaces"></param>
    /// <returns></returns>
    public MethodBuilder AddUsings(params string[] namespaces)
    {
        return new MethodBuilder(_name, _returnType, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _isAsync, _typeParameters, _parameters, _attributes, _usings.AddRange(namespaces), _body,
            _expressionBody, _xmlDoc);
    }

    /// <summary>
    /// Gets the using directives for this method.
    /// </summary>
    internal ImmutableArray<string> Usings => _usings;

    #endregion

    #region Building

    /// <summary>
    /// Builds the method as a method declaration syntax node.
    /// </summary>
    internal MethodDeclarationSyntax Build()
    {
        var method = SyntaxFactory.MethodDeclaration(
            SyntaxFactory.ParseTypeName(_returnType ?? "void"),
            SyntaxFactory.Identifier(_name));

        // Add attributes
        if (_attributes.Length > 0)
        {
            var attributeLists = _attributes.Select(a => a.BuildList()).ToArray();
            method = method.WithAttributeLists(SyntaxFactory.List(attributeLists));
        }

        // Add modifiers
        var modifiers = new List<SyntaxKind>();
        modifiers.Add(AccessibilityToSyntaxKind(_accessibility));
        if (_isStatic) modifiers.Add(SyntaxKind.StaticKeyword);
        if (_isAsync) modifiers.Add(SyntaxKind.AsyncKeyword);
        if (_isVirtual) modifiers.Add(SyntaxKind.VirtualKeyword);
        if (_isOverride) modifiers.Add(SyntaxKind.OverrideKeyword);
        if (_isAbstract) modifiers.Add(SyntaxKind.AbstractKeyword);

        method = method.WithModifiers(
            SyntaxFactory.TokenList(modifiers.Select(SyntaxFactory.Token)));

        // Add type parameters
        if (_typeParameters.Length > 0)
        {
            var typeParameterList = SyntaxFactory.TypeParameterList(
                SyntaxFactory.SeparatedList(_typeParameters.Select(tp => tp.Build())));
            method = method.WithTypeParameterList(typeParameterList);

            // Add constraint clauses
            var constraintClauses = _typeParameters
                .Select(tp => tp.BuildConstraintClause())
                .Where(c => c != null)
                .Cast<TypeParameterConstraintClauseSyntax>()
                .ToList();

            if (constraintClauses.Count > 0)
            {
                method = method.WithConstraintClauses(SyntaxFactory.List(constraintClauses));
            }
        }

        // Add parameters
        var parameterList = SyntaxFactory.ParameterList(
            SyntaxFactory.SeparatedList(_parameters.Select(p => p.Build())));
        method = method.WithParameterList(parameterList);

        // Add body or expression body (if not abstract)
        if (!_isAbstract)
        {
            if (_expressionBody != null)
            {
                // Expression-bodied method: => expression;
                var arrowExpression = SyntaxFactory.ArrowExpressionClause(
                    SyntaxFactory.ParseExpression(_expressionBody));
                method = method
                    .WithExpressionBody(arrowExpression)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            }
            else if (_body.HasValue)
            {
                method = method.WithBody(_body.Value.Build());
            }
            else
            {
                // Empty body
                method = method.WithBody(SyntaxFactory.Block());
            }
        }
        else
        {
            // Abstract method - add semicolon instead of body
            method = method.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        // Add XML documentation
        if (_xmlDoc.HasValue && _xmlDoc.Value.HasContent)
        {
            var trivia = _xmlDoc.Value.Build();
            method = method.WithLeadingTrivia(trivia);
        }

        return method;
    }

    private static SyntaxKind AccessibilityToSyntaxKind(Accessibility accessibility) =>
        accessibility switch
        {
            Accessibility.Public => SyntaxKind.PublicKeyword,
            Accessibility.Private => SyntaxKind.PrivateKeyword,
            Accessibility.Protected => SyntaxKind.ProtectedKeyword,
            Accessibility.Internal => SyntaxKind.InternalKeyword,
            _ => SyntaxKind.PublicKeyword
        };

    /// <summary>
    /// Gets the method name.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Gets the return type.
    /// </summary>
    public string? ReturnType => _returnType;

    /// <summary>
    /// Gets the extension method target type, if this is an extension method.
    /// Returns null if the method is not an extension method.
    /// </summary>
    public string? ExtensionTargetType =>
        _parameters.FirstOrDefault(p => p.IsExtensionTarget) is { } param ? param.Type : null;

    #endregion
}
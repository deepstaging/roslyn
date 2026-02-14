// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Patterns;

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for methods.
/// Supports instance and static methods, async methods, and custom bodies.
/// Immutable - each method returns a new instance.
/// </summary>
public record struct MethodBuilder
{
    /// <summary>Gets the method name.</summary>
    public string Name { get; init; }

    /// <summary>Gets the return type.</summary>
    public string? ReturnType { get; init; }

    /// <summary>Gets the accessibility level.</summary>
    public Accessibility Accessibility { get; init; }

    /// <summary>Gets whether the method is static.</summary>
    public bool IsStatic { get; init; }

    /// <summary>Gets whether the method is virtual.</summary>
    public bool IsVirtual { get; init; }

    /// <summary>Gets whether the method is an override.</summary>
    public bool IsOverride { get; init; }

    /// <summary>Gets whether the method is abstract.</summary>
    public bool IsAbstract { get; init; }

    /// <summary>Gets whether the method is async.</summary>
    public bool IsAsync { get; init; }

    /// <summary>Gets the type parameters for the method.</summary>
    public ImmutableArray<TypeParameterBuilder> TypeParameters { get; init; }

    /// <summary>Gets the parameters for the method.</summary>
    public ImmutableArray<ParameterBuilder> Parameters { get; init; }

    /// <summary>Gets the attributes applied to the method.</summary>
    public ImmutableArray<AttributeBuilder> Attributes { get; init; }

    /// <summary>Gets the using directives for this method.</summary>
    public ImmutableArray<string> Usings { get; init; }

    /// <summary>Gets the method body builder.</summary>
    public BodyBuilder? Body { get; init; }

    /// <summary>Gets the expression body.</summary>
    public string? ExpressionBody { get; init; }

    /// <summary>Gets the XML documentation builder.</summary>
    public XmlDocumentationBuilder? XmlDoc { get; init; }

    /// <summary>Gets the preprocessor directive condition for conditional compilation.</summary>
    public Directive? Condition { get; init; }

    /// <summary>Gets the region name for grouping this member in a #region block.</summary>
    public string? Region { get; init; }

    /// <summary>Gets user-defined metadata that does not affect code generation.</summary>
    public ImmutableDictionary<string, object?>? Metadata { get; init; }

    #region Factory Methods

    /// <summary>
    /// Creates a method builder for the specified method.
    /// </summary>
    /// <param name="name">The method name (e.g., "GetById", "Process").</param>
    public static MethodBuilder For(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Method name cannot be null or empty.", nameof(name));

        return new MethodBuilder
        {
            Name = name,
            ReturnType = "void",
            Accessibility = Accessibility.Public
        };
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
    public static MethodBuilder Parse(string signature)
    {
        return SignatureParser.ParseMethod(signature);
    }

    #endregion

    #region Return Type

    /// <summary>
    /// Sets the return type of the method.
    /// </summary>
    /// <param name="returnType">The return type (e.g., "void", "string", "Task&lt;int&gt;").</param>
    public MethodBuilder WithReturnType(string returnType)
    {
        return this with { ReturnType = returnType };
    }

    /// <summary>
    /// Sets the return type using a symbol's globally qualified name.
    /// </summary>
    public MethodBuilder WithReturnType<T>(ValidSymbol<T> returnType) where T : class, ITypeSymbol
        => WithReturnType(returnType.GloballyQualifiedName);

    #endregion

    #region Accessibility & Modifiers

    /// <summary>
    /// Sets the accessibility of the method.
    /// </summary>
    public MethodBuilder WithAccessibility(Accessibility accessibility)
    {
        return this with { Accessibility = accessibility };
    }

    /// <summary>
    /// Sets the accessibility of the method from a keyword string (e.g., "public", "internal").
    /// Accepts the same values produced by <see cref="ValidSymbol{TSymbol}.AccessibilityString"/>
    /// and <see cref="SymbolSnapshot.AccessibilityString"/>.
    /// </summary>
    public MethodBuilder WithAccessibility(string accessibilityKeyword) =>
        WithAccessibility(AccessibilityHelper.Parse(accessibilityKeyword));

    /// <summary>
    /// Marks the method as static.
    /// </summary>
    public MethodBuilder AsStatic()
    {
        return this with { IsStatic = true };
    }

    /// <summary>
    /// Marks the method as virtual.
    /// </summary>
    public MethodBuilder AsVirtual()
    {
        return this with { IsVirtual = true };
    }

    /// <summary>
    /// Marks the method as override.
    /// </summary>
    public MethodBuilder AsOverride()
    {
        return this with { IsOverride = true };
    }

    /// <summary>
    /// Marks the method as abstract.
    /// </summary>
    public MethodBuilder AsAbstract()
    {
        return this with { IsAbstract = true };
    }

    /// <summary>
    /// Marks the method as async.
    /// </summary>
    public MethodBuilder Async()
    {
        return this with { IsAsync = true };
    }

    /// <summary>
    /// Wraps this method in a preprocessor directive (#if/#endif).
    /// </summary>
    /// <param name="directive">The directive condition (e.g., Directives.Net6OrGreater).</param>
    /// <example>
    /// <code>
    /// builder.AddMethod("Parse", m => m
    ///     .When(Directives.Net7OrGreater)
    ///     .WithParameter("input", "string")
    ///     .WithBody(b => b.AddReturn("new()")));
    /// </code>
    /// </example>
    public MethodBuilder When(Directive directive)
    {
        return this with { Condition = directive };
    }

    /// <summary>
    /// Assigns this method to a named region for grouping in #region/#endregion blocks.
    /// </summary>
    /// <param name="regionName">The region name (e.g., "Methods", "Helpers").</param>
    public MethodBuilder InRegion(string regionName)
    {
        return this with { Region = regionName };
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
        var typeParams = TypeParameters.IsDefault ? [] : TypeParameters;
        return this with { TypeParameters = typeParams.Add(typeParameter) };
    }

    /// <summary>
    /// Adds a type parameter to the method with configuration.
    /// </summary>
    /// <param name="name">The type parameter name (e.g., "T", "RT").</param>
    /// <param name="configure">Configuration callback for the type parameter.</param>
    public MethodBuilder AddTypeParameter(string name, Func<TypeParameterBuilder, TypeParameterBuilder> configure)
    {
        var typeParameter = configure(TypeParameterBuilder.For(name));
        var typeParams = TypeParameters.IsDefault ? [] : TypeParameters;
        return this with { TypeParameters = typeParams.Add(typeParameter) };
    }

    /// <summary>
    /// Adds a pre-configured type parameter to the method.
    /// </summary>
    public MethodBuilder AddTypeParameter(TypeParameterBuilder typeParameter)
    {
        var typeParams = TypeParameters.IsDefault ? [] : TypeParameters;
        return this with { TypeParameters = typeParams.Add(typeParameter) };
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
        var parameters = Parameters.IsDefault ? [] : Parameters;
        return this with { Parameters = parameters.Add(parameter) };
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
        var parameters = Parameters.IsDefault ? [] : Parameters;
        return this with { Parameters = parameters.Add(parameter) };
    }

    /// <summary>
    /// Adds a parameter using a symbol's globally qualified name as the type.
    /// </summary>
    public MethodBuilder AddParameter<T>(string name, ValidSymbol<T> type) where T : class, ITypeSymbol
        => AddParameter(name, type.GloballyQualifiedName);

    /// <summary>
    /// Adds a parameter using a symbol's globally qualified name, with configuration.
    /// </summary>
    public MethodBuilder AddParameter<T>(string name, ValidSymbol<T> type, Func<ParameterBuilder, ParameterBuilder> configure) where T : class, ITypeSymbol
        => AddParameter(name, type.GloballyQualifiedName, configure);

    /// <summary>
    /// Adds a pre-configured parameter to the method.
    /// </summary>
    public MethodBuilder AddParameter(ParameterBuilder parameter)
    {
        var parameters = Parameters.IsDefault ? [] : Parameters;
        return this with { Parameters = parameters.Add(parameter) };
    }

    /// <summary>
    /// Configures an existing parameter by name.
    /// Useful for adding attributes to parameters created via <see cref="Parse"/>.
    /// </summary>
    /// <param name="name">The name of the parameter to configure.</param>
    /// <param name="configure">Configuration callback for the parameter.</param>
    /// <exception cref="ArgumentException">Thrown when no parameter with the specified name exists.</exception>
    public MethodBuilder ConfigureParameter(string name, Func<ParameterBuilder, ParameterBuilder> configure)
    {
        var parameters = Parameters.IsDefault ? [] : Parameters;

        for (var i = 0; i < parameters.Length; i++)
        {
            if (parameters[i].Name == name)
            {
                var updated = configure(parameters[i]);
                return this with { Parameters = parameters.SetItem(i, updated) };
            }
        }

        throw new ArgumentException($"Parameter '{name}' not found. Available parameters: {string.Join(", ", parameters.Select(p => p.Name))}", nameof(name));
    }

    #endregion

    #region Body

    /// <summary>
    /// Sets the method body using a body builder configuration.
    /// </summary>
    public MethodBuilder WithBody(Func<BodyBuilder, BodyBuilder> configure)
    {
        var body = configure(BodyBuilder.Empty());
        return this with { Body = body, ExpressionBody = null };
    }

    /// <summary>
    /// Sets the method body as an expression (arrow expression syntax).
    /// Use this for expression-bodied methods like: public int GetValue() => 42;
    /// Trailing semicolons are automatically stripped.
    /// </summary>
    /// <param name="expression">The expression (e.g., "42", "x + y", "liftEff(...)").</param>
    public MethodBuilder WithExpressionBody(string expression)
    {
        // Auto-strip trailing semicolon - expression bodies don't need them
        var trimmed = expression.TrimEnd();
        if (trimmed.EndsWith(";"))
            trimmed = trimmed[..^1].TrimEnd();

        return this with { ExpressionBody = trimmed, Body = null };
    }

    /// <summary>
    /// Appends to the existing expression body.
    /// Useful for chaining method calls like: .WithExpressionBody("liftEff(...)").AppendExpressionBody(".WithActivity(...)")
    /// </summary>
    /// <param name="suffix">The text to append to the expression body.</param>
    /// <exception cref="InvalidOperationException">Thrown when no expression body has been set.</exception>
    public MethodBuilder AppendExpressionBody(string suffix)
    {
        if (ExpressionBody is null)
            throw new InvalidOperationException(
                "Cannot append to expression body when no expression body has been set.");

        return this with { ExpressionBody = ExpressionBody + suffix, Body = null };
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
        return this with { XmlDoc = xmlDoc };
    }

    /// <summary>
    /// Sets the XML documentation for the method with a simple summary.
    /// </summary>
    /// <param name="summary">The summary text.</param>
    public MethodBuilder WithXmlDoc(string summary)
    {
        var xmlDoc = XmlDocumentationBuilder.ForSummary(summary);
        return this with { XmlDoc = xmlDoc };
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
        return this with { XmlDoc = xmlDoc };
    }

    /// <summary>
    /// Sets the XML documentation for the method using inheritdoc.
    /// </summary>
    /// <param name="cref">Optional cref attribute for the inheritdoc element.</param>
    public MethodBuilder WithInheritDoc(string? cref = null)
    {
        var xmlDoc = XmlDocumentationBuilder.ForInheritDoc(cref);
        return this with { XmlDoc = xmlDoc };
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
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(attribute) };
    }

    /// <summary>
    /// Adds an attribute to the method with configuration.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "Obsolete", "MethodImpl").</param>
    /// <param name="configure">Configuration callback for the attribute.</param>
    public MethodBuilder WithAttribute(string name, Func<AttributeBuilder, AttributeBuilder> configure)
    {
        var attribute = configure(AttributeBuilder.For(name));
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(attribute) };
    }

    /// <summary>
    /// Adds a pre-configured attribute to the method.
    /// </summary>
    public MethodBuilder WithAttribute(AttributeBuilder attribute)
    {
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(attribute) };
    }

    #endregion

    #region Usings

    /// <summary>
    /// Adds a using directive that will be collected by the containing TypeBuilder.
    /// </summary>
    /// <param name="namespace">The namespace to add (e.g., "System.Linq", "static System.Math").</param>
    public MethodBuilder AddUsing(string @namespace)
    {
        var usings = Usings.IsDefault ? [] : Usings;
        return this with { Usings = usings.Add(@namespace) };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="namespaces"></param>
    /// <returns></returns>
    public MethodBuilder AddUsings(params string[] namespaces)
    {
        var usings = Usings.IsDefault ? [] : Usings;
        return this with { Usings = usings.AddRange(namespaces) };
    }

    #endregion

    #region Metadata

    /// <summary>
    /// Attaches a metadata entry to this builder. Metadata does not affect code generation.
    /// </summary>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The metadata value.</param>
    public MethodBuilder WithMetadata(string key, object? value) =>
        this with { Metadata = (Metadata ?? ImmutableDictionary<string, object?>.Empty).SetItem(key, value) };

    /// <summary>
    /// Retrieves a metadata entry by key.
    /// </summary>
    /// <typeparam name="T">The expected metadata type.</typeparam>
    /// <param name="key">The metadata key.</param>
    /// <returns>The metadata value cast to <typeparamref name="T"/>.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the key is not found.</exception>
    /// <exception cref="InvalidCastException">Thrown when the value is not of the expected type.</exception>
    public T GetMetadata<T>(string key) where T : class =>
        (T)(Metadata ?? throw new KeyNotFoundException($"Metadata key '{key}' not found."))[key]!;

    #endregion

    #region Building

    /// <summary>
    /// Builds the method as a method declaration syntax node.
    /// </summary>
    internal MethodDeclarationSyntax Build()
    {
        var method = SyntaxFactory.MethodDeclaration(
            SyntaxFactory.ParseTypeName(ReturnType ?? "void"),
            SyntaxFactory.Identifier(Name));

        // Add attributes
        var attributes = Attributes.IsDefault ? [] : Attributes;
        if (attributes.Length > 0)
        {
            var attributeLists = attributes.Select(a => a.BuildList()).ToArray();
            method = method.WithAttributeLists(SyntaxFactory.List(attributeLists));
        }

        // Add modifiers
        var modifiers = new List<SyntaxKind>();
        modifiers.Add(AccessibilityToSyntaxKind(Accessibility));
        if (IsStatic) modifiers.Add(SyntaxKind.StaticKeyword);
        if (IsAsync) modifiers.Add(SyntaxKind.AsyncKeyword);
        if (IsVirtual) modifiers.Add(SyntaxKind.VirtualKeyword);
        if (IsOverride) modifiers.Add(SyntaxKind.OverrideKeyword);
        if (IsAbstract) modifiers.Add(SyntaxKind.AbstractKeyword);

        method = method.WithModifiers(
            SyntaxFactory.TokenList(modifiers.Select(SyntaxFactory.Token)));

        // Add type parameters
        var typeParameters = TypeParameters.IsDefault ? [] : TypeParameters;
        if (typeParameters.Length > 0)
        {
            var typeParameterList = SyntaxFactory.TypeParameterList(
                SyntaxFactory.SeparatedList(typeParameters.Select(tp => tp.Build())));
            method = method.WithTypeParameterList(typeParameterList);

            // Add constraint clauses
            var constraintClauses = typeParameters
                .Select(tp => tp.BuildConstraintClause())
                .Where(c => c != null)
                .Cast<TypeParameterConstraintClauseSyntax>()
                .ToList();

            if (constraintClauses.Count > 0)
                method = method.WithConstraintClauses(SyntaxFactory.List(constraintClauses));
        }

        // Add parameters
        var parameters = Parameters.IsDefault ? [] : Parameters;
        var parameterList = SyntaxFactory.ParameterList(
            SyntaxFactory.SeparatedList(parameters.Select(p => p.Build())));
        method = method.WithParameterList(parameterList);

        // Add body or expression body (if not abstract)
        if (!IsAbstract)
        {
            if (ExpressionBody != null)
            {
                // Expression-bodied method: => expression;
                var arrowExpression = SyntaxFactory.ArrowExpressionClause(
                    SyntaxFactory.ParseExpression(ExpressionBody));
                method = method
                    .WithExpressionBody(arrowExpression)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            }
            else if (Body.HasValue || HasParameterValidations())
            {
                method = method.WithBody(BuildBodyWithValidations());
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
        if (XmlDoc.HasValue && XmlDoc.Value.HasContent)
        {
            var trivia = XmlDoc.Value.Build();
            method = method.WithLeadingTrivia(trivia);
        }

        // Wrap in preprocessor directive if specified
        if (Condition.HasValue)
        {
            method = DirectiveHelper.WrapInDirective(method, Condition.Value);
        }

        return method;
    }

    private static SyntaxKind AccessibilityToSyntaxKind(Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Public => SyntaxKind.PublicKeyword,
            Accessibility.Private => SyntaxKind.PrivateKeyword,
            Accessibility.Protected => SyntaxKind.ProtectedKeyword,
            Accessibility.Internal => SyntaxKind.InternalKeyword,
            _ => SyntaxKind.PublicKeyword
        };
    }

    private bool HasParameterValidations()
    {
        var parameters = Parameters.IsDefault ? [] : Parameters;
        return parameters.Any(p => !p.Validations.IsDefaultOrEmpty || p.AssignsToMember is not null);
    }

    private BlockSyntax BuildBodyWithValidations()
    {
        var bodyBuilder = BodyBuilder.Empty();
        var parameters = Parameters.IsDefault ? [] : Parameters;

        // 1. Add validation statements from parameters (at the start)
        foreach (var param in parameters)
        {
            foreach (var statement in param.GetValidationStatements())
            {
                bodyBuilder = bodyBuilder.AddStatements(statement);
            }
        }

        // 2. Add existing body statements
        if (Body.HasValue)
        {
            var existingStatements = Body.Value.Statements.IsDefault ? [] : Body.Value.Statements;
            foreach (var statement in existingStatements)
            {
                var statements = bodyBuilder.Statements.IsDefault ? [] : bodyBuilder.Statements;
                bodyBuilder = bodyBuilder with { Statements = statements.Add(statement) };
            }
        }

        // 3. Add assignment statements from parameters (at the end)
        foreach (var param in parameters)
        {
            var assignment = param.GetAssignmentStatement();
            if (assignment is not null)
            {
                bodyBuilder = bodyBuilder.AddStatement(assignment);
            }
        }

        return bodyBuilder.Build();
    }

    /// <summary>
    /// Gets the extension method target type, if this is an extension method.
    /// Returns null if the method is not an extension method.
    /// </summary>
    public string? ExtensionTargetType
    {
        get
        {
            var parameters = Parameters.IsDefault ? [] : Parameters;
            return parameters.FirstOrDefault(p => p.IsExtensionTarget) is { } param ? param.Type : null;
        }
    }

    #endregion
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Patterns;

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for constructors.
/// Supports instance and static constructors with parameters and bodies.
/// Immutable - each method returns a new instance.
/// </summary>
public record struct ConstructorBuilder
{
    /// <summary>Gets the type name for the constructor.</summary>
    public string TypeName { get; init; }
    /// <summary>Gets the accessibility of the constructor.</summary>
    public Accessibility Accessibility { get; init; }
    /// <summary>Gets whether the constructor is static.</summary>
    public bool IsStatic { get; init; }
    /// <summary>Gets whether this is a primary constructor.</summary>
    public bool IsPrimary { get; init; }

    /// <summary>Gets the parameters for the constructor.</summary>
    public ImmutableArray<ParameterBuilder> Parameters { get; init; }

    /// <summary>Gets the attributes applied to the constructor.</summary>
    public ImmutableArray<AttributeBuilder> Attributes { get; init; }

    /// <summary>Gets the using directives for the constructor.</summary>
    public ImmutableArray<string> Usings { get; init; }

    /// <summary>Gets the body of the constructor.</summary>
    public BodyBuilder? Body { get; init; }
    /// <summary>Gets the constructor initializer (this or base call).</summary>
    public ConstructorInitializer? Initializer { get; init; }
    /// <summary>Gets the XML documentation for the constructor.</summary>
    public XmlDocumentationBuilder? XmlDoc { get; init; }
    /// <summary>Gets the preprocessor directive condition for conditional compilation.</summary>
    public Directive? Condition { get; init; }

    #region Factory Methods

    /// <summary>
    /// Creates a constructor builder for the specified type.
    /// </summary>
    /// <param name="typeName">The type name (e.g., "Customer", "Repository").</param>
    public static ConstructorBuilder For(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            throw new ArgumentException("Type name cannot be null or empty.", nameof(typeName));

        return new ConstructorBuilder
        {
            TypeName = typeName,
            Accessibility = Accessibility.Public,
        };
    }

    #endregion

    #region Accessibility & Modifiers

    /// <summary>
    /// Sets the accessibility of the constructor.
    /// </summary>
    public ConstructorBuilder WithAccessibility(Accessibility accessibility)
    {
        return this with { Accessibility = accessibility };
    }

    /// <summary>
    /// Marks the constructor as static.
    /// Note: Static constructors cannot have parameters or accessibility modifiers.
    /// </summary>
    public ConstructorBuilder AsStatic()
    {
        return this with { Accessibility = Accessibility.NotApplicable, IsStatic = true };
    }

    /// <summary>
    /// Marks the constructor as a primary constructor.
    /// Primary constructors are declared in the type declaration itself (e.g., "class Person(string name)").
    /// Note: Primary constructors cannot have bodies or initializers.
    /// </summary>
    public ConstructorBuilder AsPrimary()
    {
        return this with { IsPrimary = true, Body = null, Initializer = null };
    }

    /// <summary>
    /// Wraps this constructor in a preprocessor directive (#if/#endif).
    /// </summary>
    /// <param name="directive">The directive condition (e.g., Directives.Net6OrGreater).</param>
    public ConstructorBuilder When(Directive directive) =>
        this with { Condition = directive };

    #endregion

    #region Parameters

    /// <summary>
    /// Adds a parameter to the constructor.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="type">The parameter type.</param>
    public ConstructorBuilder AddParameter(string name, string type)
    {
        var parameter = ParameterBuilder.For(name, type);
        var parameters = Parameters.IsDefault ? [] : Parameters;
        return this with { Parameters = parameters.Add(parameter) };
    }

    /// <summary>
    /// Adds a parameter to the constructor with configuration.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="type">The parameter type.</param>
    /// <param name="configure">Configuration callback for the parameter.</param>
    public ConstructorBuilder AddParameter(string name, string type, Func<ParameterBuilder, ParameterBuilder> configure)
    {
        var parameter = configure(ParameterBuilder.For(name, type));
        var parameters = Parameters.IsDefault ? [] : Parameters;
        return this with { Parameters = parameters.Add(parameter) };
    }

    /// <summary>
    /// Adds a pre-configured parameter to the constructor.
    /// </summary>
    public ConstructorBuilder AddParameter(ParameterBuilder parameter)
    {
        var parameters = Parameters.IsDefault ? [] : Parameters;
        return this with { Parameters = parameters.Add(parameter) };
    }

    #endregion

    #region Body

    /// <summary>
    /// Sets the constructor body using a body builder configuration.
    /// </summary>
    public ConstructorBuilder WithBody(Func<BodyBuilder, BodyBuilder> configure)
    {
        var body = configure(BodyBuilder.Empty());
        return this with { Body = body };
    }

    #endregion

    #region Constructor Chaining

    /// <summary>
    /// Adds a "this(...)" initializer for constructor chaining.
    /// </summary>
    /// <param name="arguments">The arguments to pass to the other constructor (e.g., "id", "\"default\"").</param>
    public ConstructorBuilder CallsThis(params string[] arguments)
    {
        var initializer = new ConstructorInitializer(ConstructorInitializerKind.This, arguments);
        return this with { Initializer = initializer };
    }

    /// <summary>
    /// Adds a "base(...)" initializer for calling the base class constructor.
    /// </summary>
    /// <param name="arguments">The arguments to pass to the base constructor.</param>
    public ConstructorBuilder CallsBase(params string[] arguments)
    {
        var initializer = new ConstructorInitializer(ConstructorInitializerKind.Base, arguments);
        return this with { Initializer = initializer };
    }

    #endregion

    #region XML Documentation

    /// <summary>
    /// Sets the XML documentation for the constructor.
    /// </summary>
    /// <param name="configure">Configuration callback for the XML documentation.</param>
    public ConstructorBuilder WithXmlDoc(Func<XmlDocumentationBuilder, XmlDocumentationBuilder> configure)
    {
        var xmlDoc = configure(XmlDocumentationBuilder.Create());
        return this with { XmlDoc = xmlDoc };
    }

    /// <summary>
    /// Sets the XML documentation for the constructor with a simple summary.
    /// </summary>
    /// <param name="summary">The summary text.</param>
    public ConstructorBuilder WithXmlDoc(string summary)
    {
        var xmlDoc = XmlDocumentationBuilder.ForSummary(summary);
        return this with { XmlDoc = xmlDoc };
    }

    /// <summary>
    /// Sets the XML documentation for the constructor from parsed XmlDocumentation.
    /// </summary>
    /// <param name="documentation">The parsed XML documentation to copy.</param>
    public ConstructorBuilder WithXmlDoc(XmlDocumentation documentation)
    {
        if (documentation.IsEmpty)
            return this;

        var xmlDoc = XmlDocumentationBuilder.From(documentation);
        return this with { XmlDoc = xmlDoc };
    }

    #endregion

    #region Attributes

    /// <summary>
    /// Adds an attribute to the constructor.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "Obsolete").</param>
    public ConstructorBuilder WithAttribute(string name)
    {
        var attribute = AttributeBuilder.For(name);
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(attribute) };
    }

    /// <summary>
    /// Adds an attribute to the constructor with configuration.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "Obsolete").</param>
    /// <param name="configure">Configuration callback for the attribute.</param>
    public ConstructorBuilder WithAttribute(string name, Func<AttributeBuilder, AttributeBuilder> configure)
    {
        var attribute = configure(AttributeBuilder.For(name));
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(attribute) };
    }

    /// <summary>
    /// Adds a pre-configured attribute to the constructor.
    /// </summary>
    public ConstructorBuilder WithAttribute(AttributeBuilder attribute)
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
    public ConstructorBuilder AddUsing(string @namespace)
    {
        var usings = Usings.IsDefault ? [] : Usings;
        return this with { Usings = usings.Add(@namespace) };
    }

    #endregion

    #region Building

    /// <summary>
    /// Builds the constructor as a constructor declaration syntax node.
    /// </summary>
    internal ConstructorDeclarationSyntax Build()
    {
        var constructor = SyntaxFactory.ConstructorDeclaration(
            SyntaxFactory.Identifier(TypeName));

        // Add attributes
        var attributes = Attributes.IsDefault ? [] : Attributes;
        if (attributes.Length > 0)
        {
            var attributeLists = attributes.Select(a => a.BuildList()).ToArray();
            constructor = constructor.WithAttributeLists(SyntaxFactory.List(attributeLists));
        }

        // Add modifiers (unless static)
        if (!IsStatic)
        {
            var modifiers = new List<SyntaxKind>();
            if (Accessibility != Accessibility.NotApplicable) modifiers.Add(AccessibilityToSyntaxKind(Accessibility));

            if (modifiers.Any())
                constructor = constructor.WithModifiers(
                    SyntaxFactory.TokenList(modifiers.Select(SyntaxFactory.Token)));
        }
        else
        {
            // Static constructor
            constructor = constructor.WithModifiers(
                SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.StaticKeyword)));
        }

        // Add parameters (not allowed for static constructors)
        if (!IsStatic)
        {
            var parameters = Parameters.IsDefault ? [] : Parameters;
            var parameterList = SyntaxFactory.ParameterList(
                SyntaxFactory.SeparatedList(parameters.Select(p => p.Build())));
            constructor = constructor.WithParameterList(parameterList);
        }
        else
        {
            constructor = constructor.WithParameterList(SyntaxFactory.ParameterList());
        }

        // Add initializer (this/base call)
        if (Initializer.HasValue)
        {
            var kind = Initializer.Value.Kind == ConstructorInitializerKind.This
                ? SyntaxKind.ThisConstructorInitializer
                : SyntaxKind.BaseConstructorInitializer;

            var arguments = Initializer.Value.Arguments
                .Select(arg => SyntaxFactory.Argument(SyntaxFactory.ParseExpression(arg)));

            var initializerSyntax = SyntaxFactory.ConstructorInitializer(
                kind,
                SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

            constructor = constructor.WithInitializer(initializerSyntax);
        }

        // Add body (including validation and assignments from parameters)
        var finalBody = BuildBodyWithValidations();
        constructor = constructor.WithBody(finalBody);

        // Add XML documentation
        if (XmlDoc.HasValue && XmlDoc.Value.HasContent)
        {
            var trivia = XmlDoc.Value.Build();
            constructor = constructor.WithLeadingTrivia(trivia);
        }

        // Wrap in preprocessor directive if specified
        if (Condition.HasValue)
        {
            constructor = DirectiveHelper.WrapInDirective(constructor, Condition.Value);
        }

        return constructor;
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

    #endregion
}

/// <summary>
/// Represents a constructor initializer (this or base call).
/// </summary>
public readonly struct ConstructorInitializer
{
    /// <summary>Gets the kind of initializer (this or base).</summary>
    public ConstructorInitializerKind Kind { get; }
    /// <summary>Gets the arguments passed to the initializer.</summary>
    public ImmutableArray<string> Arguments { get; }

    /// <summary>
    /// Creates a new constructor initializer.
    /// </summary>
    /// <param name="kind">The kind of initializer.</param>
    /// <param name="arguments">The arguments to pass.</param>
    public ConstructorInitializer(ConstructorInitializerKind kind, params string[] arguments)
    {
        Kind = kind;
        Arguments = arguments.ToImmutableArray();
    }
}

/// <summary>
/// Specifies the kind of constructor initializer.
/// </summary>
public enum ConstructorInitializerKind
{
    /// <summary>Initializer calls this().</summary>
    This,
    /// <summary>Initializer calls base().</summary>
    Base
}
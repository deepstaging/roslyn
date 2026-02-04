// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for constructors.
/// Supports instance and static constructors with parameters and bodies.
/// Immutable - each method returns a new instance.
/// </summary>
public readonly struct ConstructorBuilder
{
    private readonly string _typeName;
    private readonly Accessibility _accessibility;
    private readonly bool _isStatic;
    private readonly bool _isPrimary;
    private readonly ImmutableArray<ParameterBuilder> _parameters;
    private readonly ImmutableArray<AttributeBuilder> _attributes;
    private readonly ImmutableArray<string> _usings;
    private readonly BodyBuilder? _body;
    private readonly ConstructorInitializer? _initializer;
    private readonly XmlDocumentationBuilder? _xmlDoc;

    private ConstructorBuilder(
        string typeName,
        Accessibility accessibility,
        bool isStatic,
        bool isPrimary,
        ImmutableArray<ParameterBuilder> parameters,
        ImmutableArray<AttributeBuilder> attributes,
        ImmutableArray<string> usings,
        BodyBuilder? body,
        ConstructorInitializer? initializer,
        XmlDocumentationBuilder? xmlDoc)
    {
        _typeName = typeName;
        _accessibility = accessibility;
        _isStatic = isStatic;
        _isPrimary = isPrimary;
        _parameters = parameters.IsDefault ? ImmutableArray<ParameterBuilder>.Empty : parameters;
        _attributes = attributes.IsDefault ? ImmutableArray<AttributeBuilder>.Empty : attributes;
        _usings = usings.IsDefault ? ImmutableArray<string>.Empty : usings;
        _body = body;
        _initializer = initializer;
        _xmlDoc = xmlDoc;
    }

    #region Factory Methods

    /// <summary>
    /// Creates a constructor builder for the specified type.
    /// </summary>
    /// <param name="typeName">The type name (e.g., "Customer", "Repository").</param>
    public static ConstructorBuilder For(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            throw new ArgumentException("Type name cannot be null or empty.", nameof(typeName));

        return new ConstructorBuilder(
            typeName,
            Accessibility.Public,
            isStatic: false,
            isPrimary: false,
            ImmutableArray<ParameterBuilder>.Empty,
            ImmutableArray<AttributeBuilder>.Empty,
            ImmutableArray<string>.Empty,
            body: null,
            initializer: null,
            xmlDoc: null);
    }

    #endregion

    #region Accessibility & Modifiers

    /// <summary>
    /// Sets the accessibility of the constructor.
    /// </summary>
    public ConstructorBuilder WithAccessibility(Accessibility accessibility)
    {
        return new ConstructorBuilder(_typeName, accessibility, _isStatic, _isPrimary, _parameters, _attributes, _usings, _body, _initializer, _xmlDoc);
    }

    /// <summary>
    /// Marks the constructor as static.
    /// Note: Static constructors cannot have parameters or accessibility modifiers.
    /// </summary>
    public ConstructorBuilder AsStatic()
    {
        return new ConstructorBuilder(_typeName, Accessibility.NotApplicable, true, _isPrimary, _parameters, _attributes, _usings, _body, _initializer, _xmlDoc);
    }

    /// <summary>
    /// Marks the constructor as a primary constructor.
    /// Primary constructors are declared in the type declaration itself (e.g., "class Person(string name)").
    /// Note: Primary constructors cannot have bodies or initializers.
    /// </summary>
    public ConstructorBuilder AsPrimary()
    {
        return new ConstructorBuilder(_typeName, _accessibility, _isStatic, true, _parameters, _attributes, _usings, body: null, initializer: null, _xmlDoc);
    }

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
        return new ConstructorBuilder(_typeName, _accessibility, _isStatic, _isPrimary, _parameters.Add(parameter), _attributes, _usings, _body, _initializer, _xmlDoc);
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
        return new ConstructorBuilder(_typeName, _accessibility, _isStatic, _isPrimary, _parameters.Add(parameter), _attributes, _usings, _body, _initializer, _xmlDoc);
    }

    /// <summary>
    /// Adds a pre-configured parameter to the constructor.
    /// </summary>
    public ConstructorBuilder AddParameter(ParameterBuilder parameter)
    {
        return new ConstructorBuilder(_typeName, _accessibility, _isStatic, _isPrimary, _parameters.Add(parameter), _attributes, _usings, _body, _initializer, _xmlDoc);
    }

    #endregion

    #region Body

    /// <summary>
    /// Sets the constructor body using a body builder configuration.
    /// </summary>
    public ConstructorBuilder WithBody(Func<BodyBuilder, BodyBuilder> configure)
    {
        var body = configure(BodyBuilder.Empty());
        return new ConstructorBuilder(_typeName, _accessibility, _isStatic, _isPrimary, _parameters, _attributes, _usings, body, _initializer, _xmlDoc);
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
        return new ConstructorBuilder(_typeName, _accessibility, _isStatic, _isPrimary, _parameters, _attributes, _usings, _body, initializer, _xmlDoc);
    }

    /// <summary>
    /// Adds a "base(...)" initializer for calling the base class constructor.
    /// </summary>
    /// <param name="arguments">The arguments to pass to the base constructor.</param>
    public ConstructorBuilder CallsBase(params string[] arguments)
    {
        var initializer = new ConstructorInitializer(ConstructorInitializerKind.Base, arguments);
        return new ConstructorBuilder(_typeName, _accessibility, _isStatic, _isPrimary, _parameters, _attributes, _usings, _body, initializer, _xmlDoc);
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
        return new ConstructorBuilder(_typeName, _accessibility, _isStatic, _isPrimary, _parameters, _attributes, _usings, _body, _initializer, xmlDoc);
    }

    /// <summary>
    /// Sets the XML documentation for the constructor with a simple summary.
    /// </summary>
    /// <param name="summary">The summary text.</param>
    public ConstructorBuilder WithXmlDoc(string summary)
    {
        var xmlDoc = XmlDocumentationBuilder.WithSummary(summary);
        return new ConstructorBuilder(_typeName, _accessibility, _isStatic, _isPrimary, _parameters, _attributes, _usings, _body, _initializer, xmlDoc);
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
        return new ConstructorBuilder(_typeName, _accessibility, _isStatic, _isPrimary, _parameters, _attributes, _usings, _body, _initializer, xmlDoc);
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
        return new ConstructorBuilder(_typeName, _accessibility, _isStatic, _isPrimary, _parameters, _attributes.Add(attribute), _usings, _body, _initializer, _xmlDoc);
    }

    /// <summary>
    /// Adds an attribute to the constructor with configuration.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "Obsolete").</param>
    /// <param name="configure">Configuration callback for the attribute.</param>
    public ConstructorBuilder WithAttribute(string name, Func<AttributeBuilder, AttributeBuilder> configure)
    {
        var attribute = configure(AttributeBuilder.For(name));
        return new ConstructorBuilder(_typeName, _accessibility, _isStatic, _isPrimary, _parameters, _attributes.Add(attribute), _usings, _body, _initializer, _xmlDoc);
    }

    /// <summary>
    /// Adds a pre-configured attribute to the constructor.
    /// </summary>
    public ConstructorBuilder WithAttribute(AttributeBuilder attribute)
    {
        return new ConstructorBuilder(_typeName, _accessibility, _isStatic, _isPrimary, _parameters, _attributes.Add(attribute), _usings, _body, _initializer, _xmlDoc);
    }

    #endregion

    #region Usings

    /// <summary>
    /// Adds a using directive that will be collected by the containing TypeBuilder.
    /// </summary>
    /// <param name="namespace">The namespace to add (e.g., "System.Linq", "static System.Math").</param>
    public ConstructorBuilder AddUsing(string @namespace)
    {
        return new ConstructorBuilder(_typeName, _accessibility, _isStatic, _isPrimary, _parameters, _attributes, _usings.Add(@namespace), _body, _initializer, _xmlDoc);
    }

    /// <summary>
    /// Gets the using directives for this constructor.
    /// </summary>
    internal ImmutableArray<string> Usings => _usings;

    /// <summary>
    /// Gets whether this is a primary constructor.
    /// </summary>
    internal bool IsPrimary => _isPrimary;

    /// <summary>
    /// Gets the parameters for this constructor.
    /// </summary>
    internal ImmutableArray<ParameterBuilder> Parameters => _parameters;

    #endregion

    #region Building

    /// <summary>
    /// Builds the constructor as a constructor declaration syntax node.
    /// </summary>
    internal ConstructorDeclarationSyntax Build()
    {
        var constructor = SyntaxFactory.ConstructorDeclaration(
            SyntaxFactory.Identifier(_typeName));

        // Add attributes
        if (_attributes.Length > 0)
        {
            var attributeLists = _attributes.Select(a => a.BuildList()).ToArray();
            constructor = constructor.WithAttributeLists(SyntaxFactory.List(attributeLists));
        }

        // Add modifiers (unless static)
        if (!_isStatic)
        {
            var modifiers = new List<SyntaxKind>();
            if (_accessibility != Accessibility.NotApplicable)
            {
                modifiers.Add(AccessibilityToSyntaxKind(_accessibility));
            }

            if (modifiers.Any())
            {
                constructor = constructor.WithModifiers(
                    SyntaxFactory.TokenList(modifiers.Select(SyntaxFactory.Token)));
            }
        }
        else
        {
            // Static constructor
            constructor = constructor.WithModifiers(
                SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.StaticKeyword)));
        }

        // Add parameters (not allowed for static constructors)
        if (!_isStatic)
        {
            var parameterList = SyntaxFactory.ParameterList(
                SyntaxFactory.SeparatedList(_parameters.Select(p => p.Build())));
            constructor = constructor.WithParameterList(parameterList);
        }
        else
        {
            constructor = constructor.WithParameterList(SyntaxFactory.ParameterList());
        }

        // Add initializer (this/base call)
        if (_initializer.HasValue)
        {
            var kind = _initializer.Value.Kind == ConstructorInitializerKind.This
                ? SyntaxKind.ThisConstructorInitializer
                : SyntaxKind.BaseConstructorInitializer;

            var arguments = _initializer.Value.Arguments
                .Select(arg => SyntaxFactory.Argument(SyntaxFactory.ParseExpression(arg)));

            var initializerSyntax = SyntaxFactory.ConstructorInitializer(
                kind,
                SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

            constructor = constructor.WithInitializer(initializerSyntax);
        }

        // Add body
        if (_body.HasValue)
        {
            constructor = constructor.WithBody(_body.Value.Build());
        }
        else
        {
            // Empty body
            constructor = constructor.WithBody(SyntaxFactory.Block());
        }

        // Add XML documentation
        if (_xmlDoc.HasValue && _xmlDoc.Value.HasContent)
        {
            var trivia = _xmlDoc.Value.Build();
            constructor = constructor.WithLeadingTrivia(trivia);
        }

        return constructor;
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

    #endregion
}

/// <summary>
/// Represents a constructor initializer (this or base call).
/// </summary>
internal readonly struct ConstructorInitializer
{
    public ConstructorInitializerKind Kind { get; }
    public ImmutableArray<string> Arguments { get; }

    public ConstructorInitializer(ConstructorInitializerKind kind, params string[] arguments)
    {
        Kind = kind;
        Arguments = arguments.ToImmutableArray();
    }
}

internal enum ConstructorInitializerKind
{
    This,
    Base
}

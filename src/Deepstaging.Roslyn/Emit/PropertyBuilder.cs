// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for properties.
/// Supports auto-properties, expression-bodied properties, and properties with backing fields.
/// Immutable - each method returns a new instance.
/// </summary>
public readonly struct PropertyBuilder
{
    private readonly string _name;
    private readonly string _type;
    private readonly Accessibility _accessibility;
    private readonly bool _isStatic;
    private readonly bool _isVirtual;
    private readonly bool _isOverride;
    private readonly bool _isAbstract;
    private readonly PropertyAccessorStyle _accessorStyle;
    private readonly string? _getterBody;
    private readonly string? _setterBody;
    private readonly string? _initializer;
    private readonly string? _backingFieldName;
    private readonly ImmutableArray<AttributeBuilder> _attributes;
    private readonly ImmutableArray<string> _usings;
    private readonly XmlDocumentationBuilder? _xmlDoc;

    private PropertyBuilder(
        string name,
        string type,
        Accessibility accessibility,
        bool isStatic,
        bool isVirtual,
        bool isOverride,
        bool isAbstract,
        PropertyAccessorStyle accessorStyle,
        string? getterBody,
        string? setterBody,
        string? initializer,
        string? backingFieldName,
        ImmutableArray<AttributeBuilder> attributes,
        ImmutableArray<string> usings,
        XmlDocumentationBuilder? xmlDoc)
    {
        _name = name;
        _type = type;
        _accessibility = accessibility;
        _isStatic = isStatic;
        _isVirtual = isVirtual;
        _isOverride = isOverride;
        _isAbstract = isAbstract;
        _accessorStyle = accessorStyle;
        _getterBody = getterBody;
        _setterBody = setterBody;
        _initializer = initializer;
        _backingFieldName = backingFieldName;
        _attributes = attributes.IsDefault ? ImmutableArray<AttributeBuilder>.Empty : attributes;
        _usings = usings.IsDefault ? ImmutableArray<string>.Empty : usings;
        _xmlDoc = xmlDoc;
    }

    #region Factory Methods

    /// <summary>
    /// Creates a property builder for the specified property.
    /// </summary>
    /// <param name="name">The property name (e.g., "UserId", "Name").</param>
    /// <param name="type">The property type (e.g., "string", "int", "List&lt;T&gt;").</param>
    public static PropertyBuilder For(string name, string type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Property name cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Property type cannot be null or empty.", nameof(type));

        return new PropertyBuilder(
            name,
            type,
            Accessibility.Public,
            isStatic: false,
            isVirtual: false,
            isOverride: false,
            isAbstract: false,
            PropertyAccessorStyle.None,
            getterBody: null,
            setterBody: null,
            initializer: null,
            backingFieldName: null,
            ImmutableArray<AttributeBuilder>.Empty,
            ImmutableArray<string>.Empty,
            xmlDoc: null);
    }

    /// <summary>
    /// Creates a property builder by parsing a C# property signature.
    /// </summary>
    /// <param name="signature">The property signature (e.g., "public string Name { get; set; }").</param>
    /// <returns>A configured PropertyBuilder with parsed modifiers, type, and accessors.</returns>
    /// <exception cref="ArgumentException">Thrown when the signature cannot be parsed.</exception>
    /// <example>
    /// <code>
    /// // Auto-property
    /// var builder = PropertyBuilder.Parse("public string Name { get; set; }");
    /// 
    /// // Get-only property
    /// var builder = PropertyBuilder.Parse("public int Count { get; }");
    /// 
    /// // With initializer
    /// var builder = PropertyBuilder.Parse("public List&lt;string&gt; Items { get; set; } = new()");
    /// </code>
    /// </example>
    public static PropertyBuilder Parse(string signature) => SignatureParser.ParseProperty(signature);

    #endregion

    #region Accessibility & Modifiers

    /// <summary>
    /// Sets the accessibility of the property.
    /// </summary>
    public PropertyBuilder WithAccessibility(Accessibility accessibility)
    {
        return new PropertyBuilder(_name, _type, accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _accessorStyle, _getterBody, _setterBody, _initializer, _backingFieldName, _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Marks the property as static.
    /// </summary>
    public PropertyBuilder AsStatic()
    {
        return new PropertyBuilder(_name, _type, _accessibility, true, _isVirtual, _isOverride,
            _isAbstract, _accessorStyle, _getterBody, _setterBody, _initializer, _backingFieldName, _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Marks the property as virtual.
    /// </summary>
    public PropertyBuilder AsVirtual()
    {
        return new PropertyBuilder(_name, _type, _accessibility, _isStatic, true, _isOverride,
            _isAbstract, _accessorStyle, _getterBody, _setterBody, _initializer, _backingFieldName, _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Marks the property as override.
    /// </summary>
    public PropertyBuilder AsOverride()
    {
        return new PropertyBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, true,
            _isAbstract, _accessorStyle, _getterBody, _setterBody, _initializer, _backingFieldName, _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Marks the property as abstract.
    /// </summary>
    public PropertyBuilder AsAbstract()
    {
        return new PropertyBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride,
            true, _accessorStyle, _getterBody, _setterBody, _initializer, _backingFieldName, _attributes, _usings, _xmlDoc);
    }

    #endregion

    #region Accessors

    /// <summary>
    /// Configures the property as an auto-property with get and set accessors.
    /// Example: public string Name { get; set; }
    /// </summary>
    public PropertyBuilder WithAutoPropertyAccessors()
    {
        return new PropertyBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, PropertyAccessorStyle.Auto, null, null, _initializer, _backingFieldName, _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Configures the property with an expression-bodied getter.
    /// Example: public string Name => _name;
    /// </summary>
    /// <param name="expression">The getter expression (e.g., "=> _name", "_name").</param>
    public PropertyBuilder WithGetter(string expression)
    {
        var cleanExpression = expression.TrimStart().StartsWith("=>")
            ? expression.TrimStart().Substring(2).Trim()
            : expression.Trim();

        return new PropertyBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, PropertyAccessorStyle.ExpressionBodied, cleanExpression, _setterBody,
            _initializer, _backingFieldName, _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Configures the property with a setter using a body builder.
    /// </summary>
    public PropertyBuilder WithSetter(Func<BodyBuilder, BodyBuilder> configure)
    {
        var body = configure(BodyBuilder.Empty());
        return new PropertyBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, PropertyAccessorStyle.BlockBodied, _getterBody, 
            body.Build().ToFullString(), _initializer, _backingFieldName, _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Configures the property with a getter body builder.
    /// </summary>
    public PropertyBuilder WithGetter(Func<BodyBuilder, BodyBuilder> configure)
    {
        var body = configure(BodyBuilder.Empty());
        return new PropertyBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, PropertyAccessorStyle.BlockBodied, body.Build().ToFullString(),
            _setterBody, _initializer, _backingFieldName, _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Marks the property as read-only (get accessor only).
    /// Must be used with WithGetter() or WithAutoPropertyAccessors().
    /// </summary>
    public PropertyBuilder AsReadOnly()
    {
        return new PropertyBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, PropertyAccessorStyle.GetterOnly, _getterBody, null, _initializer, _backingFieldName, _attributes, _usings, _xmlDoc);
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Sets an initializer for the property.
    /// Example: = new();
    /// </summary>
    /// <param name="initializer">The initializer expression (e.g., "new()", "default", "\"value\"").</param>
    public PropertyBuilder WithInitializer(string initializer)
    {
        return new PropertyBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _accessorStyle, _getterBody, _setterBody, initializer, _backingFieldName, _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Specifies a backing field name for the property.
    /// Note: The backing field must be added separately via FieldBuilder.
    /// </summary>
    /// <param name="fieldName">The backing field name (e.g., "_name").</param>
    public PropertyBuilder WithBackingField(string fieldName)
    {
        return new PropertyBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _accessorStyle, _getterBody, _setterBody, _initializer, fieldName, _attributes, _usings, _xmlDoc);
    }

    #endregion

    #region XML Documentation

    /// <summary>
    /// Sets the XML documentation for the property.
    /// </summary>
    /// <param name="configure">Configuration callback for the XML documentation.</param>
    public PropertyBuilder WithXmlDoc(Func<XmlDocumentationBuilder, XmlDocumentationBuilder> configure)
    {
        var xmlDoc = configure(XmlDocumentationBuilder.Create());
        return new PropertyBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _accessorStyle, _getterBody, _setterBody, _initializer, _backingFieldName, _attributes, _usings, xmlDoc);
    }

    /// <summary>
    /// Sets the XML documentation for the property with a simple summary.
    /// </summary>
    /// <param name="summary">The summary text.</param>
    public PropertyBuilder WithXmlDoc(string summary)
    {
        var xmlDoc = XmlDocumentationBuilder.WithSummary(summary);
        return new PropertyBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _accessorStyle, _getterBody, _setterBody, _initializer, _backingFieldName, _attributes, _usings, xmlDoc);
    }

    /// <summary>
    /// Sets the XML documentation for the property from parsed XmlDocumentation.
    /// </summary>
    /// <param name="documentation">The parsed XML documentation to copy.</param>
    public PropertyBuilder WithXmlDoc(XmlDocumentation documentation)
    {
        if (documentation.IsEmpty)
            return this;

        var xmlDoc = XmlDocumentationBuilder.From(documentation);
        return new PropertyBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _accessorStyle, _getterBody, _setterBody, _initializer, _backingFieldName, _attributes, _usings, xmlDoc);
    }

    #endregion

    #region Attributes

    /// <summary>
    /// Adds an attribute to the property.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "JsonProperty", "Required").</param>
    public PropertyBuilder WithAttribute(string name)
    {
        var attribute = AttributeBuilder.For(name);
        return new PropertyBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _accessorStyle, _getterBody, _setterBody, _initializer, _backingFieldName, _attributes.Add(attribute), _usings, _xmlDoc);
    }

    /// <summary>
    /// Adds an attribute to the property with configuration.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "JsonProperty", "Required").</param>
    /// <param name="configure">Configuration callback for the attribute.</param>
    public PropertyBuilder WithAttribute(string name, Func<AttributeBuilder, AttributeBuilder> configure)
    {
        var attribute = configure(AttributeBuilder.For(name));
        return new PropertyBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _accessorStyle, _getterBody, _setterBody, _initializer, _backingFieldName, _attributes.Add(attribute), _usings, _xmlDoc);
    }

    /// <summary>
    /// Adds a pre-configured attribute to the property.
    /// </summary>
    public PropertyBuilder WithAttribute(AttributeBuilder attribute)
    {
        return new PropertyBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _accessorStyle, _getterBody, _setterBody, _initializer, _backingFieldName, _attributes.Add(attribute), _usings, _xmlDoc);
    }

    #endregion

    #region Usings

    /// <summary>
    /// Adds a using directive that will be collected by the containing TypeBuilder.
    /// </summary>
    /// <param name="namespace">The namespace to add (e.g., "System.Linq", "static System.Math").</param>
    public PropertyBuilder AddUsing(string @namespace)
    {
        return new PropertyBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride,
            _isAbstract, _accessorStyle, _getterBody, _setterBody, _initializer, _backingFieldName, _attributes, _usings.Add(@namespace), _xmlDoc);
    }

    /// <summary>
    /// Gets the using directives for this property.
    /// </summary>
    internal ImmutableArray<string> Usings => _usings;

    #endregion

    #region Building

    /// <summary>
    /// Builds the property as a property declaration syntax node.
    /// </summary>
    internal PropertyDeclarationSyntax Build()
    {
        var property = SyntaxFactory.PropertyDeclaration(
            SyntaxFactory.ParseTypeName(_type),
            SyntaxFactory.Identifier(_name));

        // Add attributes
        if (_attributes.Length > 0)
        {
            var attributeLists = _attributes.Select(a => a.BuildList()).ToArray();
            property = property.WithAttributeLists(SyntaxFactory.List(attributeLists));
        }

        // Add modifiers
        var modifiers = new List<SyntaxKind>();
        modifiers.Add(AccessibilityToSyntaxKind(_accessibility));
        if (_isStatic) modifiers.Add(SyntaxKind.StaticKeyword);
        if (_isVirtual) modifiers.Add(SyntaxKind.VirtualKeyword);
        if (_isOverride) modifiers.Add(SyntaxKind.OverrideKeyword);
        if (_isAbstract) modifiers.Add(SyntaxKind.AbstractKeyword);

        property = property.WithModifiers(
            SyntaxFactory.TokenList(modifiers.Select(SyntaxFactory.Token)));

        // Add accessors based on style
        property = _accessorStyle switch
        {
            PropertyAccessorStyle.Auto => AddAutoAccessors(property),
            PropertyAccessorStyle.ExpressionBodied => AddExpressionBody(property),
            PropertyAccessorStyle.BlockBodied => AddBlockBodiedAccessors(property),
            PropertyAccessorStyle.GetterOnly => AddGetterOnly(property),
            _ => property
        };

        // Add initializer if specified
        if (_initializer != null)
        {
            property = property.WithInitializer(
                SyntaxFactory.EqualsValueClause(
                    SyntaxFactory.ParseExpression(_initializer)))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        // Add XML documentation
        if (_xmlDoc.HasValue && _xmlDoc.Value.HasContent)
        {
            var trivia = _xmlDoc.Value.Build();
            property = property.WithLeadingTrivia(trivia);
        }

        return property;
    }

    private PropertyDeclarationSyntax AddAutoAccessors(PropertyDeclarationSyntax property)
    {
        return property.WithAccessorList(
            SyntaxFactory.AccessorList(
                SyntaxFactory.List(new[]
                {
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                })));
    }

    private PropertyDeclarationSyntax AddGetterOnly(PropertyDeclarationSyntax property)
    {
        return property.WithAccessorList(
            SyntaxFactory.AccessorList(
                SyntaxFactory.SingletonList(
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))));
    }

    private PropertyDeclarationSyntax AddExpressionBody(PropertyDeclarationSyntax property)
    {
        if (_getterBody == null)
            return property;

        return property
            .WithExpressionBody(
                SyntaxFactory.ArrowExpressionClause(
                    SyntaxFactory.ParseExpression(_getterBody)))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    }

    private PropertyDeclarationSyntax AddBlockBodiedAccessors(PropertyDeclarationSyntax property)
    {
        var accessors = new List<AccessorDeclarationSyntax>();

        if (_getterBody != null)
        {
            var getter = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithBody(SyntaxFactory.ParseStatement(_getterBody) as BlockSyntax);
            accessors.Add(getter);
        }

        if (_setterBody != null)
        {
            var setter = SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .WithBody(SyntaxFactory.ParseStatement(_setterBody) as BlockSyntax);
            accessors.Add(setter);
        }

        return property.WithAccessorList(
            SyntaxFactory.AccessorList(SyntaxFactory.List(accessors)));
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
    /// Gets the property name.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Gets the property type.
    /// </summary>
    public string Type => _type;

    #endregion
}

internal enum PropertyAccessorStyle
{
    None,
    Auto,
    ExpressionBodied,
    BlockBodied,
    GetterOnly
}

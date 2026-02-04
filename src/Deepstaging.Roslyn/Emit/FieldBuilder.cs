// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for fields.
/// Supports private backing fields, constants, and readonly fields.
/// Immutable - each method returns a new instance.
/// </summary>
public readonly struct FieldBuilder
{
    private readonly string _name;
    private readonly string _type;
    private readonly Accessibility _accessibility;
    private readonly bool _isStatic;
    private readonly bool _isReadonly;
    private readonly bool _isConst;
    private readonly string? _initializer;
    private readonly ImmutableArray<AttributeBuilder> _attributes;
    private readonly ImmutableArray<string> _usings;
    private readonly XmlDocumentationBuilder? _xmlDoc;

    private FieldBuilder(
        string name,
        string type,
        Accessibility accessibility,
        bool isStatic,
        bool isReadonly,
        bool isConst,
        string? initializer,
        ImmutableArray<AttributeBuilder> attributes,
        ImmutableArray<string> usings,
        XmlDocumentationBuilder? xmlDoc)
    {
        _name = name;
        _type = type;
        _accessibility = accessibility;
        _isStatic = isStatic;
        _isReadonly = isReadonly;
        _isConst = isConst;
        _initializer = initializer;
        _attributes = attributes.IsDefault ? ImmutableArray<AttributeBuilder>.Empty : attributes;
        _usings = usings.IsDefault ? ImmutableArray<string>.Empty : usings;
        _xmlDoc = xmlDoc;
    }

    #region Factory Methods

    /// <summary>
    /// Creates a field builder for the specified field.
    /// </summary>
    /// <param name="name">The field name (e.g., "_value", "_name").</param>
    /// <param name="type">The field type (e.g., "string", "int", "IRepository").</param>
    public static FieldBuilder For(string name, string type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Field name cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Field type cannot be null or empty.", nameof(type));

        return new FieldBuilder(
            name,
            type,
            Accessibility.Private,
            isStatic: false,
            isReadonly: false,
            isConst: false,
            initializer: null,
            ImmutableArray<AttributeBuilder>.Empty,
            ImmutableArray<string>.Empty,
            xmlDoc: null);
    }

    /// <summary>
    /// Creates a field builder by parsing a C# field signature.
    /// </summary>
    /// <param name="signature">The field signature (e.g., "private readonly string _name").</param>
    /// <returns>A configured FieldBuilder with parsed modifiers and type.</returns>
    /// <exception cref="ArgumentException">Thrown when the signature cannot be parsed.</exception>
    /// <example>
    /// <code>
    /// // Readonly field
    /// var builder = FieldBuilder.Parse("private readonly string _name");
    /// 
    /// // Static field with initializer
    /// var builder = FieldBuilder.Parse("private static int _count = 0");
    /// 
    /// // Const field
    /// var builder = FieldBuilder.Parse("public const int MaxRetries = 3");
    /// </code>
    /// </example>
    public static FieldBuilder Parse(string signature) => SignatureParser.ParseField(signature);

    #endregion

    #region Accessibility & Modifiers

    /// <summary>
    /// Sets the accessibility of the field.
    /// </summary>
    public FieldBuilder WithAccessibility(Accessibility accessibility)
    {
        return new FieldBuilder(_name, _type, accessibility, _isStatic, _isReadonly, _isConst, _initializer, _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Marks the field as static.
    /// </summary>
    public FieldBuilder AsStatic()
    {
        return new FieldBuilder(_name, _type, _accessibility, true, _isReadonly, _isConst, _initializer, _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Marks the field as readonly.
    /// </summary>
    public FieldBuilder AsReadonly()
    {
        return new FieldBuilder(_name, _type, _accessibility, _isStatic, true, _isConst, _initializer, _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Marks the field as const.
    /// Note: const fields must have an initializer.
    /// </summary>
    public FieldBuilder AsConst()
    {
        return new FieldBuilder(_name, _type, _accessibility, _isStatic, _isReadonly, true, _initializer, _attributes, _usings, _xmlDoc);
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Sets an initializer for the field.
    /// </summary>
    /// <param name="initializer">The initializer expression (e.g., "new()", "null", "42").</param>
    public FieldBuilder WithInitializer(string initializer)
    {
        return new FieldBuilder(_name, _type, _accessibility, _isStatic, _isReadonly, _isConst, initializer, _attributes, _usings, _xmlDoc);
    }

    #endregion

    #region XML Documentation

    /// <summary>
    /// Sets the XML documentation for the field.
    /// </summary>
    /// <param name="configure">Configuration callback for the XML documentation.</param>
    public FieldBuilder WithXmlDoc(Func<XmlDocumentationBuilder, XmlDocumentationBuilder> configure)
    {
        var xmlDoc = configure(XmlDocumentationBuilder.Create());
        return new FieldBuilder(_name, _type, _accessibility, _isStatic, _isReadonly, _isConst, _initializer, _attributes, _usings, xmlDoc);
    }

    /// <summary>
    /// Sets the XML documentation for the field with a simple summary.
    /// </summary>
    /// <param name="summary">The summary text.</param>
    public FieldBuilder WithXmlDoc(string summary)
    {
        var xmlDoc = XmlDocumentationBuilder.WithSummary(summary);
        return new FieldBuilder(_name, _type, _accessibility, _isStatic, _isReadonly, _isConst, _initializer, _attributes, _usings, xmlDoc);
    }

    /// <summary>
    /// Sets the XML documentation for the field from parsed XmlDocumentation.
    /// </summary>
    /// <param name="documentation">The parsed XML documentation to copy.</param>
    public FieldBuilder WithXmlDoc(XmlDocumentation documentation)
    {
        if (documentation.IsEmpty)
            return this;

        var xmlDoc = XmlDocumentationBuilder.From(documentation);
        return new FieldBuilder(_name, _type, _accessibility, _isStatic, _isReadonly, _isConst, _initializer, _attributes, _usings, xmlDoc);
    }

    #endregion

    #region Attributes

    /// <summary>
    /// Adds an attribute to the field.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "NonSerialized", "ThreadStatic").</param>
    public FieldBuilder WithAttribute(string name)
    {
        var attribute = AttributeBuilder.For(name);
        return new FieldBuilder(_name, _type, _accessibility, _isStatic, _isReadonly, _isConst, _initializer, _attributes.Add(attribute), _usings, _xmlDoc);
    }

    /// <summary>
    /// Adds an attribute to the field with configuration.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "NonSerialized", "ThreadStatic").</param>
    /// <param name="configure">Configuration callback for the attribute.</param>
    public FieldBuilder WithAttribute(string name, Func<AttributeBuilder, AttributeBuilder> configure)
    {
        var attribute = configure(AttributeBuilder.For(name));
        return new FieldBuilder(_name, _type, _accessibility, _isStatic, _isReadonly, _isConst, _initializer, _attributes.Add(attribute), _usings, _xmlDoc);
    }

    /// <summary>
    /// Adds a pre-configured attribute to the field.
    /// </summary>
    public FieldBuilder WithAttribute(AttributeBuilder attribute)
    {
        return new FieldBuilder(_name, _type, _accessibility, _isStatic, _isReadonly, _isConst, _initializer, _attributes.Add(attribute), _usings, _xmlDoc);
    }

    #endregion

    #region Usings

    /// <summary>
    /// Adds a using directive that will be collected by the containing TypeBuilder.
    /// </summary>
    /// <param name="namespace">The namespace to add (e.g., "System.Linq", "static System.Math").</param>
    public FieldBuilder AddUsing(string @namespace)
    {
        return new FieldBuilder(_name, _type, _accessibility, _isStatic, _isReadonly, _isConst, _initializer, _attributes, _usings.Add(@namespace), _xmlDoc);
    }

    /// <summary>
    /// Gets the using directives for this field.
    /// </summary>
    internal ImmutableArray<string> Usings => _usings;

    #endregion

    #region Building

    /// <summary>
    /// Builds the field as a field declaration syntax node.
    /// </summary>
    internal FieldDeclarationSyntax Build()
    {
        var variable = SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(_name));

        // Add initializer if specified
        if (_initializer != null)
        {
            variable = variable.WithInitializer(
                SyntaxFactory.EqualsValueClause(
                    SyntaxFactory.ParseExpression(_initializer)));
        }

        var declaration = SyntaxFactory.VariableDeclaration(
            SyntaxFactory.ParseTypeName(_type))
            .WithVariables(SyntaxFactory.SingletonSeparatedList(variable));

        var field = SyntaxFactory.FieldDeclaration(declaration);

        // Add attributes
        if (_attributes.Length > 0)
        {
            var attributeLists = _attributes.Select(a => a.BuildList()).ToArray();
            field = field.WithAttributeLists(SyntaxFactory.List(attributeLists));
        }

        // Add modifiers
        var modifiers = new List<SyntaxKind>();
        modifiers.Add(AccessibilityToSyntaxKind(_accessibility));
        if (_isStatic) modifiers.Add(SyntaxKind.StaticKeyword);
        if (_isReadonly) modifiers.Add(SyntaxKind.ReadOnlyKeyword);
        if (_isConst) modifiers.Add(SyntaxKind.ConstKeyword);

        field = field.WithModifiers(
            SyntaxFactory.TokenList(modifiers.Select(SyntaxFactory.Token)));

        // Add XML documentation
        if (_xmlDoc.HasValue && _xmlDoc.Value.HasContent)
        {
            var trivia = _xmlDoc.Value.Build();
            field = field.WithLeadingTrivia(trivia);
        }

        return field;
    }

    private static SyntaxKind AccessibilityToSyntaxKind(Accessibility accessibility) =>
        accessibility switch
        {
            Accessibility.Public => SyntaxKind.PublicKeyword,
            Accessibility.Private => SyntaxKind.PrivateKeyword,
            Accessibility.Protected => SyntaxKind.ProtectedKeyword,
            Accessibility.Internal => SyntaxKind.InternalKeyword,
            _ => SyntaxKind.PrivateKeyword
        };

    /// <summary>
    /// Gets the field name.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Gets the field type.
    /// </summary>
    public string Type => _type;

    #endregion
}

// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for event declarations.
/// Supports simple field-like events and custom add/remove accessors.
/// Immutable - each method returns a new instance.
/// </summary>
public readonly struct EventBuilder
{
    private readonly string _name;
    private readonly string _type;
    private readonly Accessibility _accessibility;
    private readonly bool _isStatic;
    private readonly bool _isVirtual;
    private readonly bool _isOverride;
    private readonly bool _isAbstract;
    private readonly ImmutableArray<AttributeBuilder> _attributes;
    private readonly ImmutableArray<string> _usings;
    private readonly XmlDocumentationBuilder? _xmlDoc;

    private EventBuilder(
        string name,
        string type,
        Accessibility accessibility,
        bool isStatic,
        bool isVirtual,
        bool isOverride,
        bool isAbstract,
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
        _attributes = attributes.IsDefault ? ImmutableArray<AttributeBuilder>.Empty : attributes;
        _usings = usings.IsDefault ? ImmutableArray<string>.Empty : usings;
        _xmlDoc = xmlDoc;
    }

    #region Factory Methods

    /// <summary>
    /// Creates an event builder for the specified event.
    /// </summary>
    /// <param name="name">The event name (e.g., "PropertyChanged", "Clicked").</param>
    /// <param name="type">The event handler type (e.g., "EventHandler", "PropertyChangedEventHandler?").</param>
    public static EventBuilder For(string name, string type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Event name cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Event type cannot be null or empty.", nameof(type));

        return new EventBuilder(
            name,
            type,
            Accessibility.Public,
            isStatic: false,
            isVirtual: false,
            isOverride: false,
            isAbstract: false,
            ImmutableArray<AttributeBuilder>.Empty,
            ImmutableArray<string>.Empty,
            xmlDoc: null);
    }

    #endregion

    #region Accessibility & Modifiers

    /// <summary>
    /// Sets the accessibility of the event.
    /// </summary>
    public EventBuilder WithAccessibility(Accessibility accessibility)
    {
        return new EventBuilder(_name, _type, accessibility, _isStatic, _isVirtual, _isOverride, _isAbstract, _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Marks the event as static.
    /// </summary>
    public EventBuilder AsStatic()
    {
        return new EventBuilder(_name, _type, _accessibility, true, _isVirtual, _isOverride, _isAbstract, _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Marks the event as virtual.
    /// </summary>
    public EventBuilder AsVirtual()
    {
        return new EventBuilder(_name, _type, _accessibility, _isStatic, true, _isOverride, _isAbstract, _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Marks the event as override.
    /// </summary>
    public EventBuilder AsOverride()
    {
        return new EventBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, true, _isAbstract, _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Marks the event as abstract.
    /// </summary>
    public EventBuilder AsAbstract()
    {
        return new EventBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride, true, _attributes, _usings, _xmlDoc);
    }

    #endregion

    #region XML Documentation

    /// <summary>
    /// Sets the XML documentation for the event.
    /// </summary>
    /// <param name="configure">Configuration callback for the XML documentation.</param>
    public EventBuilder WithXmlDoc(Func<XmlDocumentationBuilder, XmlDocumentationBuilder> configure)
    {
        var xmlDoc = configure(XmlDocumentationBuilder.Create());
        return new EventBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride, _isAbstract, _attributes, _usings, xmlDoc);
    }

    /// <summary>
    /// Sets the XML documentation for the event with a simple summary.
    /// </summary>
    /// <param name="summary">The summary text.</param>
    public EventBuilder WithXmlDoc(string summary)
    {
        var xmlDoc = XmlDocumentationBuilder.WithSummary(summary);
        return new EventBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride, _isAbstract, _attributes, _usings, xmlDoc);
    }

    /// <summary>
    /// Sets the XML documentation for the event from parsed XmlDocumentation.
    /// </summary>
    /// <param name="documentation">The parsed XML documentation to copy.</param>
    public EventBuilder WithXmlDoc(XmlDocumentation documentation)
    {
        if (documentation.IsEmpty)
            return this;

        var xmlDoc = XmlDocumentationBuilder.From(documentation);
        return new EventBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride, _isAbstract, _attributes, _usings, xmlDoc);
    }

    #endregion

    #region Attributes

    /// <summary>
    /// Adds an attribute to the event.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "NonSerialized").</param>
    public EventBuilder WithAttribute(string name)
    {
        var attribute = AttributeBuilder.For(name);
        return new EventBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride, _isAbstract, _attributes.Add(attribute), _usings, _xmlDoc);
    }

    /// <summary>
    /// Adds an attribute to the event with configuration.
    /// </summary>
    /// <param name="name">The attribute name.</param>
    /// <param name="configure">Configuration callback for the attribute.</param>
    public EventBuilder WithAttribute(string name, Func<AttributeBuilder, AttributeBuilder> configure)
    {
        var attribute = configure(AttributeBuilder.For(name));
        return new EventBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride, _isAbstract, _attributes.Add(attribute), _usings, _xmlDoc);
    }

    /// <summary>
    /// Adds a pre-configured attribute to the event.
    /// </summary>
    public EventBuilder WithAttribute(AttributeBuilder attribute)
    {
        return new EventBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride, _isAbstract, _attributes.Add(attribute), _usings, _xmlDoc);
    }

    #endregion

    #region Usings

    /// <summary>
    /// Adds a using directive that will be collected by the containing TypeBuilder.
    /// </summary>
    /// <param name="namespace">The namespace to add (e.g., "System.ComponentModel").</param>
    public EventBuilder AddUsing(string @namespace)
    {
        return new EventBuilder(_name, _type, _accessibility, _isStatic, _isVirtual, _isOverride, _isAbstract, _attributes, _usings.Add(@namespace), _xmlDoc);
    }

    /// <summary>
    /// Gets the using directives for this event.
    /// </summary>
    internal ImmutableArray<string> Usings => _usings;

    #endregion

    #region Building

    /// <summary>
    /// Builds the event as an event field declaration syntax node.
    /// </summary>
    internal EventFieldDeclarationSyntax Build()
    {
        var variable = SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(_name));

        var declaration = SyntaxFactory.VariableDeclaration(
            SyntaxFactory.ParseTypeName(_type))
            .WithVariables(SyntaxFactory.SingletonSeparatedList(variable));

        var eventDecl = SyntaxFactory.EventFieldDeclaration(declaration);

        // Add attributes
        if (_attributes.Length > 0)
        {
            var attributeLists = _attributes.Select(a => a.BuildList()).ToArray();
            eventDecl = eventDecl.WithAttributeLists(SyntaxFactory.List(attributeLists));
        }

        // Add modifiers
        var modifiers = new List<SyntaxKind>();
        modifiers.Add(AccessibilityToSyntaxKind(_accessibility));
        if (_isStatic) modifiers.Add(SyntaxKind.StaticKeyword);
        if (_isAbstract) modifiers.Add(SyntaxKind.AbstractKeyword);
        if (_isVirtual) modifiers.Add(SyntaxKind.VirtualKeyword);
        if (_isOverride) modifiers.Add(SyntaxKind.OverrideKeyword);

        eventDecl = eventDecl.WithModifiers(
            SyntaxFactory.TokenList(modifiers.Select(SyntaxFactory.Token)));

        // Add XML documentation
        if (_xmlDoc.HasValue && _xmlDoc.Value.HasContent)
        {
            var trivia = _xmlDoc.Value.Build();
            eventDecl = eventDecl.WithLeadingTrivia(trivia);
        }

        return eventDecl;
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
    /// Gets the event name.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Gets the event type.
    /// </summary>
    public string Type => _type;

    #endregion
}

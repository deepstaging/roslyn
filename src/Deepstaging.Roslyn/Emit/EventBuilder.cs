// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for event declarations.
/// Supports simple field-like events and custom add/remove accessors.
/// Immutable - each method returns a new instance.
/// </summary>
public record struct EventBuilder
{
    /// <summary>The event name.</summary>
    public string Name { get; init; }

    /// <summary>The event type.</summary>
    public string Type { get; init; }

    /// <summary>The accessibility of the event.</summary>
    public Accessibility Accessibility { get; init; }

    /// <summary>Whether the event is static.</summary>
    public bool IsStatic { get; init; }

    /// <summary>Whether the event is virtual.</summary>
    public bool IsVirtual { get; init; }

    /// <summary>Whether the event is override.</summary>
    public bool IsOverride { get; init; }

    /// <summary>Whether the event is abstract.</summary>
    public bool IsAbstract { get; init; }

    /// <summary>The attributes on the event.</summary>
    public ImmutableArray<AttributeBuilder> Attributes { get; init; }

    /// <summary>The using directives for this event.</summary>
    public ImmutableArray<string> Usings { get; init; }

    /// <summary>The XML documentation for the event.</summary>
    public XmlDocumentationBuilder? XmlDoc { get; init; }

    /// <summary>Gets the preprocessor directive condition for conditional compilation.</summary>
    public Directive? Condition { get; init; }

    /// <summary>Gets the region name for grouping this member in a #region block.</summary>
    public string? Region { get; init; }

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

        return new EventBuilder
        {
            Name = name,
            Type = type,
            Accessibility = Accessibility.Public
        };
    }

    /// <summary>
    /// Creates an event builder using a symbol's globally qualified name as the type.
    /// </summary>
    /// <param name="name">The event name (e.g., "PropertyChanged", "Clicked").</param>
    /// <param name="type">The event handler type symbol.</param>
    public static EventBuilder For<T>(string name, ValidSymbol<T> type) where T : class, ITypeSymbol
        => For(name, type.GloballyQualifiedName);

    #endregion

    #region Accessibility & Modifiers

    /// <summary>
    /// Sets the accessibility of the event.
    /// </summary>
    public readonly EventBuilder WithAccessibility(Accessibility accessibility) =>
        this with { Accessibility = accessibility };

    /// <summary>
    /// Sets the accessibility of the event from a keyword string (e.g., "public", "internal").
    /// Accepts the same values produced by <see cref="ValidSymbol{TSymbol}.AccessibilityString"/>
    /// and <see cref="SymbolSnapshot.AccessibilityString"/>.
    /// </summary>
    public readonly EventBuilder WithAccessibility(string accessibilityKeyword) =>
        WithAccessibility(AccessibilityHelper.Parse(accessibilityKeyword));

    /// <summary>
    /// Marks the event as static.
    /// </summary>
    public readonly EventBuilder AsStatic() =>
        this with { IsStatic = true };

    /// <summary>
    /// Marks the event as virtual.
    /// </summary>
    public readonly EventBuilder AsVirtual() =>
        this with { IsVirtual = true };

    /// <summary>
    /// Marks the event as override.
    /// </summary>
    public readonly EventBuilder AsOverride() =>
        this with { IsOverride = true };

    /// <summary>
    /// Marks the event as abstract.
    /// </summary>
    public readonly EventBuilder AsAbstract() =>
        this with { IsAbstract = true };

    /// <summary>
    /// Wraps this event in a preprocessor directive (#if/#endif).
    /// </summary>
    /// <param name="directive">The directive condition (e.g., Directives.Net6OrGreater).</param>
    public readonly EventBuilder When(Directive directive) =>
        this with { Condition = directive };

    /// <summary>
    /// Assigns this event to a named region for grouping in #region/#endregion blocks.
    /// </summary>
    /// <param name="regionName">The region name (e.g., "Events").</param>
    public readonly EventBuilder InRegion(string regionName) =>
        this with { Region = regionName };

    #endregion

    #region XML Documentation

    /// <summary>
    /// Sets the XML documentation for the event.
    /// </summary>
    /// <param name="configure">Configuration callback for the XML documentation.</param>
    public readonly EventBuilder WithXmlDoc(Func<XmlDocumentationBuilder, XmlDocumentationBuilder> configure) =>
        this with { XmlDoc = configure(XmlDocumentationBuilder.Create()) };

    /// <summary>
    /// Sets the XML documentation for the event with a simple summary.
    /// </summary>
    /// <param name="summary">The summary text.</param>
    public readonly EventBuilder WithXmlDoc(string summary) =>
        this with { XmlDoc = XmlDocumentationBuilder.ForSummary(summary) };

    /// <summary>
    /// Sets the XML documentation for the event from a pipeline-safe <see cref="DocumentationSnapshot"/>.
    /// </summary>
    /// <param name="snapshot">The documentation snapshot to copy.</param>
    public readonly EventBuilder WithXmlDoc(DocumentationSnapshot snapshot) =>
        snapshot.HasValue || snapshot.Params.Count > 0 || snapshot.TypeParams.Count > 0
            ? this with { XmlDoc = XmlDocumentationBuilder.From(snapshot) }
            : this;

    #endregion

    #region Attributes

    /// <summary>
    /// Adds an attribute to the event.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "NonSerialized").</param>
    public readonly EventBuilder WithAttribute(string name)
    {
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(AttributeBuilder.For(name)) };
    }

    /// <summary>
    /// Adds an attribute to the event with configuration.
    /// </summary>
    /// <param name="name">The attribute name.</param>
    /// <param name="configure">Configuration callback for the attribute.</param>
    public readonly EventBuilder WithAttribute(string name, Func<AttributeBuilder, AttributeBuilder> configure)
    {
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(configure(AttributeBuilder.For(name))) };
    }

    /// <summary>
    /// Adds a pre-configured attribute to the event.
    /// </summary>
    public readonly EventBuilder WithAttribute(AttributeBuilder attribute)
    {
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(attribute) };
    }

    #endregion

    #region Usings

    /// <summary>
    /// Adds a using directive that will be collected by the containing TypeBuilder.
    /// </summary>
    /// <param name="namespace">The namespace to add (e.g., "System.ComponentModel").</param>
    public readonly EventBuilder AddUsing(string @namespace)
    {
        var usings = Usings.IsDefault ? [] : Usings;
        return this with { Usings = usings.Add(@namespace) };
    }

    #endregion

    #region Building

    /// <summary>
    /// Builds the event as an event field declaration syntax node.
    /// </summary>
    internal readonly EventFieldDeclarationSyntax Build()
    {
        var variable = SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(Name));

        var declaration = SyntaxFactory.VariableDeclaration(
                SyntaxFactory.ParseTypeName(Type))
            .WithVariables(SyntaxFactory.SingletonSeparatedList(variable));

        var eventDecl = SyntaxFactory.EventFieldDeclaration(declaration);

        // Add attributes
        var attributes = Attributes.IsDefault ? [] : Attributes;

        if (attributes.Length > 0)
        {
            var attributeLists = attributes.Select(a => a.BuildList()).ToArray();
            eventDecl = eventDecl.WithAttributeLists(SyntaxFactory.List(attributeLists));
        }

        // Add modifiers
        var modifiers = new List<SyntaxKind>();
        modifiers.Add(AccessibilityToSyntaxKind(Accessibility));
        if (IsStatic) modifiers.Add(SyntaxKind.StaticKeyword);
        if (IsAbstract) modifiers.Add(SyntaxKind.AbstractKeyword);
        if (IsVirtual) modifiers.Add(SyntaxKind.VirtualKeyword);
        if (IsOverride) modifiers.Add(SyntaxKind.OverrideKeyword);

        eventDecl = eventDecl.WithModifiers(
            SyntaxFactory.TokenList(modifiers.Select(SyntaxFactory.Token)));

        // Add XML documentation
        if (XmlDoc.HasValue && XmlDoc.Value.HasContent)
        {
            var trivia = XmlDoc.Value.Build();
            eventDecl = eventDecl.WithLeadingTrivia(trivia);
        }

        // Wrap in preprocessor directive if specified
        if (Condition.HasValue) eventDecl = DirectiveHelper.WrapInDirective(eventDecl, Condition.Value);

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

    #endregion
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for fields.
/// Supports private backing fields, constants, and readonly fields.
/// Immutable - each method returns a new instance.
/// </summary>
public record struct FieldBuilder
{
    /// <summary>
    /// Gets the field name.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the field type.
    /// </summary>
    public string Type { get; init; }

    /// <summary>
    /// Gets the accessibility of the field.
    /// </summary>
    public Accessibility Accessibility { get; init; }

    /// <summary>
    /// Gets whether the field is static.
    /// </summary>
    public bool IsStatic { get; init; }

    /// <summary>
    /// Gets whether the field is readonly.
    /// </summary>
    public bool IsReadonly { get; init; }

    /// <summary>
    /// Gets whether the field is const.
    /// </summary>
    public bool IsConst { get; init; }

    /// <summary>
    /// Gets the initializer expression for the field.
    /// </summary>
    public string? Initializer { get; init; }

    /// <summary>
    /// Gets the attributes applied to the field.
    /// </summary>
    public ImmutableArray<AttributeBuilder> Attributes { get; init; }

    /// <summary>
    /// Gets the using directives for this field.
    /// </summary>
    internal ImmutableArray<string> Usings { get; init; }

    /// <summary>
    /// Gets the XML documentation for the field.
    /// </summary>
    public XmlDocumentationBuilder? XmlDoc { get; init; }

    /// <summary>Gets the preprocessor directive condition for conditional compilation.</summary>
    public Directive? Condition { get; init; }

    /// <summary>Gets the region name for grouping this member in a #region block.</summary>
    public string? Region { get; init; }

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

        return new FieldBuilder
        {
            Name = name,
            Type = type,
            Accessibility = Accessibility.Private
        };
    }

    /// <summary>
    /// Creates a field builder using a symbol's globally qualified name as the type.
    /// </summary>
    /// <param name="name">The field name (e.g., "_value", "_name").</param>
    /// <param name="type">The field type symbol.</param>
    public static FieldBuilder For<T>(string name, ValidSymbol<T> type) where T : class, ITypeSymbol
        => For(name, type.GloballyQualifiedName);

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
    public static FieldBuilder Parse(string signature)
    {
        return SignatureParser.ParseField(signature);
    }

    #endregion

    #region Accessibility & Modifiers

    /// <summary>
    /// Sets the accessibility of the field.
    /// </summary>
    public FieldBuilder WithAccessibility(Accessibility accessibility) =>
        this with { Accessibility = accessibility };

    /// <summary>
    /// Marks the field as static.
    /// </summary>
    public FieldBuilder AsStatic() =>
        this with { IsStatic = true };

    /// <summary>
    /// Marks the field as readonly.
    /// </summary>
    public FieldBuilder AsReadonly() =>
        this with { IsReadonly = true };

    /// <summary>
    /// Marks the field as const.
    /// Note: const fields must have an initializer.
    /// </summary>
    public FieldBuilder AsConst() =>
        this with { IsConst = true };

    /// <summary>
    /// Wraps this field in a preprocessor directive (#if/#endif).
    /// </summary>
    /// <param name="directive">The directive condition (e.g., Directives.Net6OrGreater).</param>
    public FieldBuilder When(Directive directive) =>
        this with { Condition = directive };

    /// <summary>
    /// Assigns this field to a named region for grouping in #region/#endregion blocks.
    /// </summary>
    /// <param name="regionName">The region name (e.g., "Fields", "Constants").</param>
    public FieldBuilder InRegion(string regionName) =>
        this with { Region = regionName };

    #endregion

    #region Initialization

    /// <summary>
    /// Sets an initializer for the field.
    /// </summary>
    /// <param name="initializer">The initializer expression (e.g., "new()", "null", "42").</param>
    public FieldBuilder WithInitializer(string initializer) =>
        this with { Initializer = initializer };

    #endregion

    #region XML Documentation

    /// <summary>
    /// Sets the XML documentation for the field.
    /// </summary>
    /// <param name="configure">Configuration callback for the XML documentation.</param>
    public FieldBuilder WithXmlDoc(Func<XmlDocumentationBuilder, XmlDocumentationBuilder> configure)
    {
        var xmlDoc = configure(XmlDocumentationBuilder.Create());
        return this with { XmlDoc = xmlDoc };
    }

    /// <summary>
    /// Sets the XML documentation for the field with a simple summary.
    /// </summary>
    /// <param name="summary">The summary text.</param>
    public FieldBuilder WithXmlDoc(string summary)
    {
        var xmlDoc = XmlDocumentationBuilder.ForSummary(summary);
        return this with { XmlDoc = xmlDoc };
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
        return this with { XmlDoc = xmlDoc };
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
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(attribute) };
    }

    /// <summary>
    /// Adds an attribute to the field with configuration.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "NonSerialized", "ThreadStatic").</param>
    /// <param name="configure">Configuration callback for the attribute.</param>
    public FieldBuilder WithAttribute(string name, Func<AttributeBuilder, AttributeBuilder> configure)
    {
        var attribute = configure(AttributeBuilder.For(name));
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(attribute) };
    }

    /// <summary>
    /// Adds a pre-configured attribute to the field.
    /// </summary>
    public FieldBuilder WithAttribute(AttributeBuilder attribute)
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
    public FieldBuilder AddUsing(string @namespace)
    {
        var usings = Usings.IsDefault ? [] : Usings;
        return this with { Usings = usings.Add(@namespace) };
    }

    #endregion

    #region Building

    /// <summary>
    /// Builds the field as a field declaration syntax node.
    /// </summary>
    internal FieldDeclarationSyntax Build()
    {
        var variable = SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(Name));

        // Add initializer if specified
        if (Initializer != null)
            variable = variable.WithInitializer(
                SyntaxFactory.EqualsValueClause(
                    SyntaxFactory.ParseExpression(Initializer)));

        var declaration = SyntaxFactory.VariableDeclaration(
                SyntaxFactory.ParseTypeName(Type))
            .WithVariables(SyntaxFactory.SingletonSeparatedList(variable));

        var field = SyntaxFactory.FieldDeclaration(declaration);

        // Add attributes
        var attributes = Attributes.IsDefault ? [] : Attributes;
        if (attributes.Length > 0)
        {
            var attributeLists = attributes.Select(a => a.BuildList()).ToArray();
            field = field.WithAttributeLists(SyntaxFactory.List(attributeLists));
        }

        // Add modifiers
        var modifiers = new List<SyntaxKind>();
        modifiers.Add(AccessibilityToSyntaxKind(Accessibility));
        if (IsStatic) modifiers.Add(SyntaxKind.StaticKeyword);
        if (IsReadonly) modifiers.Add(SyntaxKind.ReadOnlyKeyword);
        if (IsConst) modifiers.Add(SyntaxKind.ConstKeyword);

        field = field.WithModifiers(
            SyntaxFactory.TokenList(modifiers.Select(SyntaxFactory.Token)));

        // Add XML documentation
        if (XmlDoc.HasValue && XmlDoc.Value.HasContent)
        {
            var trivia = XmlDoc.Value.Build();
            field = field.WithLeadingTrivia(trivia);
        }

        // Wrap in preprocessor directive if specified
        if (Condition.HasValue)
        {
            field = DirectiveHelper.WrapInDirective(field, Condition.Value);
        }

        return field;
    }

    private static SyntaxKind AccessibilityToSyntaxKind(Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Public => SyntaxKind.PublicKeyword,
            Accessibility.Private => SyntaxKind.PrivateKeyword,
            Accessibility.Protected => SyntaxKind.ProtectedKeyword,
            Accessibility.Internal => SyntaxKind.InternalKeyword,
            _ => SyntaxKind.PrivateKeyword
        };
    }

    #endregion
}
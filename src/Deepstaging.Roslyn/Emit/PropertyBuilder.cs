// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for properties.
/// Supports auto-properties, expression-bodied properties, and properties with backing fields.
/// Immutable - each method returns a new instance.
/// </summary>
public record struct PropertyBuilder
{
    /// <summary>Gets the property name.</summary>
    public string Name { get; init; }

    /// <summary>Gets the property type.</summary>
    public string Type { get; init; }

    /// <summary>Gets the accessibility level.</summary>
    public Accessibility Accessibility { get; init; }

    /// <summary>Gets whether the property is static.</summary>
    public bool IsStatic { get; init; }

    /// <summary>Gets whether the property is virtual.</summary>
    public bool IsVirtual { get; init; }

    /// <summary>Gets whether the property is an override.</summary>
    public bool IsOverride { get; init; }

    /// <summary>Gets whether the property is abstract.</summary>
    public bool IsAbstract { get; init; }

    /// <summary>Gets whether the property is sealed.</summary>
    public bool IsSealed { get; init; }

    /// <summary>Gets whether the property is required.</summary>
    public bool IsRequired { get; init; }

    /// <summary>Gets whether the property has an init-only setter.</summary>
    public bool HasInitSetter { get; init; }

    /// <summary>Gets whether the property has an individual auto getter.</summary>
    public bool HasAutoGetter { get; init; }

    /// <summary>Gets whether the property has an individual auto setter.</summary>
    public bool HasAutoSetter { get; init; }

    /// <summary>Gets the accessor style.</summary>
    internal PropertyAccessorStyle AccessorStyle { get; init; }

    /// <summary>Gets the getter body expression or statement.</summary>
    public string? GetterBody { get; init; }

    /// <summary>Gets the setter body statement.</summary>
    public string? SetterBody { get; init; }

    /// <summary>Gets the property initializer expression.</summary>
    public string? Initializer { get; init; }

    /// <summary>Gets the backing field name.</summary>
    public string? BackingFieldName { get; init; }

    /// <summary>Gets the attributes for this property.</summary>
    public ImmutableArray<AttributeBuilder> Attributes { get; init; }

    /// <summary>Gets the using directives for this property.</summary>
    public ImmutableArray<string> Usings { get; init; }

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

        return new PropertyBuilder
        {
            Name = name,
            Type = type,
            Accessibility = Accessibility.Public
        };
    }

    /// <summary>
    /// Creates a property builder using a symbol's globally qualified name as the type.
    /// </summary>
    /// <param name="name">The property name (e.g., "UserId", "Name").</param>
    /// <param name="type">The property type symbol.</param>
    public static PropertyBuilder For<T>(string name, ValidSymbol<T> type) where T : class, ITypeSymbol
        => For(name, type.GloballyQualifiedName);

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
    public readonly PropertyBuilder WithAccessibility(Accessibility accessibility) =>
        this with { Accessibility = accessibility };

    /// <summary>
    /// Sets the accessibility of the property from a keyword string (e.g., "public", "internal").
    /// Accepts the same values produced by <see cref="ValidSymbol{TSymbol}.AccessibilityString"/>
    /// and <see cref="SymbolSnapshot.AccessibilityString"/>.
    /// </summary>
    public readonly PropertyBuilder WithAccessibility(string accessibilityKeyword) =>
        WithAccessibility(AccessibilityHelper.Parse(accessibilityKeyword));

    /// <summary>
    /// Marks the property as static.
    /// </summary>
    public readonly PropertyBuilder AsStatic() =>
        this with { IsStatic = true };

    /// <summary>
    /// Marks the property as virtual.
    /// </summary>
    public readonly PropertyBuilder AsVirtual() =>
        this with { IsVirtual = true };

    /// <summary>
    /// Marks the property as override.
    /// </summary>
    public readonly PropertyBuilder AsOverride() =>
        this with { IsOverride = true };

    /// <summary>
    /// Marks the property as abstract.
    /// </summary>
    public readonly PropertyBuilder AsAbstract() =>
        this with { IsAbstract = true };

    /// <summary>
    /// Marks the property as sealed. Only valid when used with override.
    /// </summary>
    public readonly PropertyBuilder AsSealed() =>
        this with { IsSealed = true };

    /// <summary>
    /// Marks the property as required (C# 11+). Requires an init or set accessor.
    /// </summary>
    public readonly PropertyBuilder AsRequired() =>
        this with { IsRequired = true };

    #endregion

    #region Accessors

    /// <summary>
    /// Configures the property as an auto-property with get and set accessors.
    /// Example: public string Name { get; set; }
    /// </summary>
    public readonly PropertyBuilder WithAutoPropertyAccessors() =>
        this with { AccessorStyle = PropertyAccessorStyle.Auto, GetterBody = null, SetterBody = null };

    /// <summary>
    /// Adds an auto getter to the property. Can be composed with <see cref="WithAutoSetter"/> or <see cref="WithAutoInitSetter"/>.
    /// Example: <c>prop.WithAutoGetter()</c> → <c>{ get; }</c>
    /// Example: <c>prop.WithAutoGetter().WithAutoSetter()</c> → <c>{ get; set; }</c>
    /// </summary>
    public readonly PropertyBuilder WithAutoGetter() =>
        this with { HasAutoGetter = true };

    /// <summary>
    /// Adds an auto setter to the property. Can be composed with <see cref="WithAutoGetter"/>.
    /// Example: <c>prop.WithAutoGetter().WithAutoSetter()</c> → <c>{ get; set; }</c>
    /// </summary>
    public readonly PropertyBuilder WithAutoSetter() =>
        this with { HasAutoSetter = true };

    /// <summary>
    /// Adds an auto init-only setter to the property. Can be composed with <see cref="WithAutoGetter"/>.
    /// Example: <c>prop.WithAutoGetter().WithAutoInitSetter()</c> → <c>{ get; init; }</c>
    /// </summary>
    public readonly PropertyBuilder WithAutoInitSetter() =>
        this with { HasAutoSetter = true, HasInitSetter = true };

    /// <summary>
    /// Configures the property with an expression-bodied getter.
    /// Example: public string Name => _name;
    /// </summary>
    /// <param name="expression">The getter expression (e.g., "=> _name", "_name").</param>
    public readonly PropertyBuilder WithGetter(string expression)
    {
        var cleanExpression = expression.TrimStart().StartsWith("=>")
            ? expression.TrimStart().Substring(2).Trim()
            : expression.Trim();

        return this with { AccessorStyle = PropertyAccessorStyle.ExpressionBodied, GetterBody = cleanExpression };
    }

    /// <summary>
    /// Configures the property with a setter using a body builder.
    /// </summary>
    public readonly PropertyBuilder WithSetter(Func<BodyBuilder, BodyBuilder> configure)
    {
        var body = configure(BodyBuilder.Empty());

        return this with
        {
            AccessorStyle = PropertyAccessorStyle.BlockBodied, SetterBody = body.Build().ToFullString()
        };
    }

    /// <summary>
    /// Configures the property with a getter body builder.
    /// </summary>
    public readonly PropertyBuilder WithGetter(Func<BodyBuilder, BodyBuilder> configure)
    {
        var body = configure(BodyBuilder.Empty());

        return this with
        {
            AccessorStyle = PropertyAccessorStyle.BlockBodied, GetterBody = body.Build().ToFullString()
        };
    }

    /// <summary>
    /// Marks the property as read-only (get accessor only).
    /// Must be used with WithGetter() or WithAutoPropertyAccessors().
    /// </summary>
    public readonly PropertyBuilder AsReadOnly() =>
        this with { AccessorStyle = PropertyAccessorStyle.GetterOnly, SetterBody = null };

    /// <summary>
    /// Configures the property to use an init-only setter instead of a set accessor.
    /// Example: public string Name { get; init; }
    /// </summary>
    public readonly PropertyBuilder WithInitOnlySetter() =>
        this with { HasInitSetter = true };

    /// <summary>
    /// Wraps this property in a preprocessor directive (#if/#endif).
    /// </summary>
    /// <param name="directive">The directive condition (e.g., Directives.Net6OrGreater).</param>
    public readonly PropertyBuilder When(Directive directive) =>
        this with { Condition = directive };

    /// <summary>
    /// Assigns this property to a named region for grouping in #region/#endregion blocks.
    /// </summary>
    /// <param name="regionName">The region name (e.g., "Properties", "Configuration").</param>
    public readonly PropertyBuilder InRegion(string regionName) =>
        this with { Region = regionName };

    #endregion

    #region Initialization

    /// <summary>
    /// Sets an initializer for the property.
    /// Example: = new();
    /// </summary>
    /// <param name="initializer">The initializer expression (e.g., "new()", "default", "\"value\"").</param>
    public readonly PropertyBuilder WithInitializer(string initializer) =>
        this with { Initializer = initializer };

    /// <summary>
    /// Specifies a backing field name for the property.
    /// Note: The backing field must be added separately via FieldBuilder.
    /// </summary>
    /// <param name="fieldName">The backing field name (e.g., "_name").</param>
    public readonly PropertyBuilder WithBackingField(string fieldName) =>
        this with { BackingFieldName = fieldName };

    #endregion

    #region XML Documentation

    /// <summary>
    /// Sets the XML documentation for the property.
    /// </summary>
    /// <param name="configure">Configuration callback for the XML documentation.</param>
    public readonly PropertyBuilder WithXmlDoc(Func<XmlDocumentationBuilder, XmlDocumentationBuilder> configure)
    {
        var xmlDoc = configure(XmlDocumentationBuilder.Create());
        return this with { XmlDoc = xmlDoc };
    }

    /// <summary>
    /// Sets the XML documentation for the property with a simple summary.
    /// </summary>
    /// <param name="summary">The summary text.</param>
    public readonly PropertyBuilder WithXmlDoc(string summary)
    {
        var xmlDoc = XmlDocumentationBuilder.ForSummary(summary);
        return this with { XmlDoc = xmlDoc };
    }

    /// <summary>
    /// Sets the XML documentation for the property from a pipeline-safe <see cref="DocumentationSnapshot"/>.
    /// </summary>
    /// <param name="snapshot">The documentation snapshot to copy.</param>
    public readonly PropertyBuilder WithXmlDoc(DocumentationSnapshot snapshot)
    {
        if (!snapshot.HasValue && snapshot.Params.Count == 0 && snapshot.TypeParams.Count == 0)
            return this;

        var xmlDoc = XmlDocumentationBuilder.From(snapshot);
        return this with { XmlDoc = xmlDoc };
    }

    /// <summary>
    /// Sets the XML documentation for the property using inheritdoc.
    /// </summary>
    /// <param name="cref">Optional cref attribute for the inheritdoc element.</param>
    public readonly PropertyBuilder WithInheritDoc(string? cref = null)
    {
        var xmlDoc = XmlDocumentationBuilder.ForInheritDoc(cref);
        return this with { XmlDoc = xmlDoc };
    }

    #endregion

    #region Attributes

    /// <summary>
    /// Adds an attribute to the property.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "JsonProperty", "Required").</param>
    public readonly PropertyBuilder WithAttribute(string name)
    {
        var attribute = AttributeBuilder.For(name);
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(attribute) };
    }

    /// <summary>
    /// Adds an attribute to the property with configuration.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "JsonProperty", "Required").</param>
    /// <param name="configure">Configuration callback for the attribute.</param>
    public readonly PropertyBuilder WithAttribute(string name, Func<AttributeBuilder, AttributeBuilder> configure)
    {
        var attribute = configure(AttributeBuilder.For(name));
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(attribute) };
    }

    /// <summary>
    /// Adds a pre-configured attribute to the property.
    /// </summary>
    public readonly PropertyBuilder WithAttribute(AttributeBuilder attribute)
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
    public readonly PropertyBuilder AddUsing(string @namespace)
    {
        var usings = Usings.IsDefault ? [] : Usings;
        return this with { Usings = usings.Add(@namespace) };
    }

    #endregion

    #region Metadata

    /// <summary>
    /// Attaches a metadata entry to this builder. Metadata does not affect code generation.
    /// </summary>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The metadata value.</param>
    public readonly PropertyBuilder WithMetadata(string key, object? value) =>
        this with { Metadata = (Metadata ?? ImmutableDictionary<string, object?>.Empty).SetItem(key, value) };

    /// <summary>
    /// Retrieves a metadata entry by key.
    /// </summary>
    /// <typeparam name="T">The expected metadata type.</typeparam>
    /// <param name="key">The metadata key.</param>
    /// <returns>The metadata value cast to <typeparamref name="T"/>.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the key is not found.</exception>
    /// <exception cref="InvalidCastException">Thrown when the value is not of the expected type.</exception>
    public readonly T GetMetadata<T>(string key) where T : class =>
        (T)(Metadata ?? throw new KeyNotFoundException($"Metadata key '{key}' not found."))[key]!;

    #endregion

    #region Building

    /// <summary>
    /// Builds the property as a property declaration syntax node.
    /// </summary>
    internal readonly PropertyDeclarationSyntax Build()
    {
        var property = SyntaxFactory.PropertyDeclaration(
            SyntaxFactory.ParseTypeName(Type),
            SyntaxFactory.Identifier(Name));

        // Add attributes
        var attributes = Attributes.IsDefault ? [] : Attributes;

        if (attributes.Length > 0)
        {
            var attributeLists = attributes.Select(a => a.BuildList()).ToArray();
            property = property.WithAttributeLists(SyntaxFactory.List(attributeLists));
        }

        // Add modifiers (order matters for valid C#)
        var modifiers = new List<SyntaxKind>();
        modifiers.Add(AccessibilityToSyntaxKind(Accessibility));
        if (IsStatic) modifiers.Add(SyntaxKind.StaticKeyword);
        if (IsSealed) modifiers.Add(SyntaxKind.SealedKeyword);
        if (IsVirtual) modifiers.Add(SyntaxKind.VirtualKeyword);
        if (IsOverride) modifiers.Add(SyntaxKind.OverrideKeyword);
        if (IsAbstract) modifiers.Add(SyntaxKind.AbstractKeyword);
        if (IsRequired) modifiers.Add(SyntaxKind.RequiredKeyword);

        property = property.WithModifiers(
            SyntaxFactory.TokenList(modifiers.Select(SyntaxFactory.Token)));

        // Add accessors based on style (composed auto accessors take priority)
        property = (HasAutoGetter || HasAutoSetter) switch
        {
            true => AddComposedAutoAccessors(property),
            false => AccessorStyle switch
            {
                PropertyAccessorStyle.Auto => AddAutoAccessors(property),
                PropertyAccessorStyle.ExpressionBodied => AddExpressionBody(property),
                PropertyAccessorStyle.BlockBodied => AddBlockBodiedAccessors(property),
                PropertyAccessorStyle.GetterOnly => AddGetterOnly(property),
                _ => property
            }
        };

        // Add initializer if specified
        if (Initializer != null)
            property = property.WithInitializer(
                    SyntaxFactory.EqualsValueClause(
                        SyntaxFactory.ParseExpression(Initializer)))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        // Add XML documentation
        if (XmlDoc.HasValue && XmlDoc.Value.HasContent)
        {
            var trivia = XmlDoc.Value.Build();
            property = property.WithLeadingTrivia(trivia);
        }

        // Wrap in preprocessor directive if specified
        if (Condition.HasValue) property = DirectiveHelper.WrapInDirective(property, Condition.Value);

        return property;
    }

    private readonly PropertyDeclarationSyntax AddAutoAccessors(PropertyDeclarationSyntax property)
    {
        var setterKind = HasInitSetter ? SyntaxKind.InitAccessorDeclaration : SyntaxKind.SetAccessorDeclaration;

        return property.WithAccessorList(
            SyntaxFactory.AccessorList(
                SyntaxFactory.List(new[]
                {
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(setterKind)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                })));
    }

    private readonly PropertyDeclarationSyntax AddComposedAutoAccessors(PropertyDeclarationSyntax property)
    {
        var accessors = new List<AccessorDeclarationSyntax>();

        if (HasAutoGetter)
        {
            accessors.Add(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
        }

        if (HasAutoSetter)
        {
            var setterKind = HasInitSetter ? SyntaxKind.InitAccessorDeclaration : SyntaxKind.SetAccessorDeclaration;

            accessors.Add(SyntaxFactory.AccessorDeclaration(setterKind)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
        }

        return property.WithAccessorList(
            SyntaxFactory.AccessorList(SyntaxFactory.List(accessors)));
    }

    private readonly PropertyDeclarationSyntax AddGetterOnly(PropertyDeclarationSyntax property) =>
        property.WithAccessorList(
            SyntaxFactory.AccessorList(
                SyntaxFactory.SingletonList(
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))));

    private readonly PropertyDeclarationSyntax AddExpressionBody(PropertyDeclarationSyntax property)
    {
        if (GetterBody == null)
            return property;

        return property
            .WithExpressionBody(
                SyntaxFactory.ArrowExpressionClause(
                    SyntaxFactory.ParseExpression(GetterBody)))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    }

    private readonly PropertyDeclarationSyntax AddBlockBodiedAccessors(PropertyDeclarationSyntax property)
    {
        var accessors = new List<AccessorDeclarationSyntax>();

        if (GetterBody != null)
        {
            var getter = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithBody(SyntaxFactory.ParseStatement(GetterBody) as BlockSyntax);

            accessors.Add(getter);
        }

        if (SetterBody != null)
        {
            var setterKind = HasInitSetter ? SyntaxKind.InitAccessorDeclaration : SyntaxKind.SetAccessorDeclaration;

            var setter = SyntaxFactory.AccessorDeclaration(setterKind)
                .WithBody(SyntaxFactory.ParseStatement(SetterBody) as BlockSyntax);

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

    #endregion
}

/// <summary>
/// Specifies the accessor style for a property.
/// </summary>
internal enum PropertyAccessorStyle
{
    None,
    Auto,
    ExpressionBodied,
    BlockBodied,
    GetterOnly
}
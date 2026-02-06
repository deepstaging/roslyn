// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for indexers.
/// Supports expression-bodied and block-bodied indexers.
/// Immutable - each method returns a new instance.
/// </summary>
public readonly struct IndexerBuilder
{
    private readonly string _type;
    private readonly Accessibility _accessibility;
    private readonly bool _isVirtual;
    private readonly bool _isOverride;
    private readonly bool _isAbstract;
    private readonly bool _isSealed;
    private readonly bool _hasInitSetter;
    private readonly IndexerAccessorStyle _accessorStyle;
    private readonly string? _getterBody;
    private readonly string? _setterBody;
    private readonly ImmutableArray<ParameterBuilder> _parameters;
    private readonly ImmutableArray<AttributeBuilder> _attributes;
    private readonly ImmutableArray<string> _usings;
    private readonly XmlDocumentationBuilder? _xmlDoc;

    private IndexerBuilder(
        string type,
        Accessibility accessibility,
        bool isVirtual,
        bool isOverride,
        bool isAbstract,
        bool isSealed,
        bool hasInitSetter,
        IndexerAccessorStyle accessorStyle,
        string? getterBody,
        string? setterBody,
        ImmutableArray<ParameterBuilder> parameters,
        ImmutableArray<AttributeBuilder> attributes,
        ImmutableArray<string> usings,
        XmlDocumentationBuilder? xmlDoc)
    {
        _type = type;
        _accessibility = accessibility;
        _isVirtual = isVirtual;
        _isOverride = isOverride;
        _isAbstract = isAbstract;
        _isSealed = isSealed;
        _hasInitSetter = hasInitSetter;
        _accessorStyle = accessorStyle;
        _getterBody = getterBody;
        _setterBody = setterBody;
        _parameters = parameters.IsDefault ? ImmutableArray<ParameterBuilder>.Empty : parameters;
        _attributes = attributes.IsDefault ? ImmutableArray<AttributeBuilder>.Empty : attributes;
        _usings = usings.IsDefault ? ImmutableArray<string>.Empty : usings;
        _xmlDoc = xmlDoc;
    }

    #region Factory Methods

    /// <summary>
    /// Creates an indexer builder for the specified return type.
    /// </summary>
    /// <param name="type">The indexer return type (e.g., "string", "T", "int").</param>
    public static IndexerBuilder For(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Indexer type cannot be null or empty.", nameof(type));

        return new IndexerBuilder(
            type,
            Accessibility.Public,
            false,
            false,
            false,
            false,
            false,
            IndexerAccessorStyle.None,
            null,
            null,
            ImmutableArray<ParameterBuilder>.Empty,
            ImmutableArray<AttributeBuilder>.Empty,
            ImmutableArray<string>.Empty,
            null);
    }

    #endregion

    #region Accessibility & Modifiers

    /// <summary>
    /// Sets the accessibility of the indexer.
    /// </summary>
    public IndexerBuilder WithAccessibility(Accessibility accessibility)
    {
        return new IndexerBuilder(_type, accessibility, _isVirtual, _isOverride,
            _isAbstract, _isSealed, _hasInitSetter, _accessorStyle, _getterBody, _setterBody, _parameters, _attributes,
            _usings, _xmlDoc);
    }

    /// <summary>
    /// Marks the indexer as virtual.
    /// </summary>
    public IndexerBuilder AsVirtual()
    {
        return new IndexerBuilder(_type, _accessibility, true, _isOverride,
            _isAbstract, _isSealed, _hasInitSetter, _accessorStyle, _getterBody, _setterBody, _parameters, _attributes,
            _usings, _xmlDoc);
    }

    /// <summary>
    /// Marks the indexer as override.
    /// </summary>
    public IndexerBuilder AsOverride()
    {
        return new IndexerBuilder(_type, _accessibility, _isVirtual, true,
            _isAbstract, _isSealed, _hasInitSetter, _accessorStyle, _getterBody, _setterBody, _parameters, _attributes,
            _usings, _xmlDoc);
    }

    /// <summary>
    /// Marks the indexer as abstract.
    /// </summary>
    public IndexerBuilder AsAbstract()
    {
        return new IndexerBuilder(_type, _accessibility, _isVirtual, _isOverride,
            true, _isSealed, _hasInitSetter, _accessorStyle, _getterBody, _setterBody, _parameters, _attributes,
            _usings, _xmlDoc);
    }

    /// <summary>
    /// Marks the indexer as sealed. Only valid when used with override.
    /// </summary>
    public IndexerBuilder AsSealed()
    {
        return new IndexerBuilder(_type, _accessibility, _isVirtual, _isOverride,
            _isAbstract, true, _hasInitSetter, _accessorStyle, _getterBody, _setterBody, _parameters, _attributes,
            _usings, _xmlDoc);
    }

    #endregion

    #region Parameters

    /// <summary>
    /// Adds an index parameter to the indexer.
    /// </summary>
    /// <param name="name">The parameter name (e.g., "index", "key").</param>
    /// <param name="type">The parameter type (e.g., "int", "string").</param>
    public IndexerBuilder AddParameter(string name, string type)
    {
        var parameter = ParameterBuilder.For(name, type);
        return new IndexerBuilder(_type, _accessibility, _isVirtual, _isOverride,
            _isAbstract, _isSealed, _hasInitSetter, _accessorStyle, _getterBody, _setterBody,
            _parameters.Add(parameter), _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Adds an index parameter to the indexer with configuration.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="type">The parameter type.</param>
    /// <param name="configure">Configuration callback for the parameter.</param>
    public IndexerBuilder AddParameter(string name, string type, Func<ParameterBuilder, ParameterBuilder> configure)
    {
        var parameter = configure(ParameterBuilder.For(name, type));
        return new IndexerBuilder(_type, _accessibility, _isVirtual, _isOverride,
            _isAbstract, _isSealed, _hasInitSetter, _accessorStyle, _getterBody, _setterBody,
            _parameters.Add(parameter), _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Adds a pre-configured parameter to the indexer.
    /// </summary>
    public IndexerBuilder AddParameter(ParameterBuilder parameter)
    {
        return new IndexerBuilder(_type, _accessibility, _isVirtual, _isOverride,
            _isAbstract, _isSealed, _hasInitSetter, _accessorStyle, _getterBody, _setterBody,
            _parameters.Add(parameter), _attributes, _usings, _xmlDoc);
    }

    #endregion

    #region Accessors

    /// <summary>
    /// Configures the indexer as an auto-indexer with get and set accessors.
    /// Note: Indexers cannot have auto-implemented accessors, so a body is typically required.
    /// </summary>
    public IndexerBuilder WithAutoAccessors()
    {
        return new IndexerBuilder(_type, _accessibility, _isVirtual, _isOverride,
            _isAbstract, _isSealed, _hasInitSetter, IndexerAccessorStyle.Auto, null, null, _parameters, _attributes,
            _usings, _xmlDoc);
    }

    /// <summary>
    /// Configures the indexer with an expression-bodied getter.
    /// Example: public string this[int index] => _items[index];
    /// </summary>
    /// <param name="expression">The getter expression (e.g., "=> _items[index]", "_items[index]").</param>
    public IndexerBuilder WithGetter(string expression)
    {
        var cleanExpression = expression.TrimStart().StartsWith("=>")
            ? expression.TrimStart().Substring(2).Trim()
            : expression.Trim();

        return new IndexerBuilder(_type, _accessibility, _isVirtual, _isOverride,
            _isAbstract, _isSealed, _hasInitSetter, IndexerAccessorStyle.ExpressionBodied, cleanExpression, _setterBody,
            _parameters, _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Configures the indexer with a getter body builder.
    /// </summary>
    public IndexerBuilder WithGetter(Func<BodyBuilder, BodyBuilder> configure)
    {
        var body = configure(BodyBuilder.Empty());
        return new IndexerBuilder(_type, _accessibility, _isVirtual, _isOverride,
            _isAbstract, _isSealed, _hasInitSetter, IndexerAccessorStyle.BlockBodied, body.Build().ToFullString(),
            _setterBody, _parameters, _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Configures the indexer with a setter body builder.
    /// </summary>
    public IndexerBuilder WithSetter(Func<BodyBuilder, BodyBuilder> configure)
    {
        var body = configure(BodyBuilder.Empty());
        return new IndexerBuilder(_type, _accessibility, _isVirtual, _isOverride,
            _isAbstract, _isSealed, _hasInitSetter, IndexerAccessorStyle.BlockBodied, _getterBody,
            body.Build().ToFullString(), _parameters, _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Marks the indexer as read-only (get accessor only).
    /// </summary>
    public IndexerBuilder AsReadOnly()
    {
        return new IndexerBuilder(_type, _accessibility, _isVirtual, _isOverride,
            _isAbstract, _isSealed, _hasInitSetter, IndexerAccessorStyle.GetterOnly, _getterBody, null, _parameters,
            _attributes, _usings, _xmlDoc);
    }

    /// <summary>
    /// Configures the indexer to use an init-only setter instead of a set accessor.
    /// Example: public string this[int index] { get; init; }
    /// </summary>
    public IndexerBuilder WithInitOnlySetter()
    {
        return new IndexerBuilder(_type, _accessibility, _isVirtual, _isOverride,
            _isAbstract, _isSealed, true, _accessorStyle, _getterBody, _setterBody, _parameters, _attributes, _usings,
            _xmlDoc);
    }

    #endregion

    #region XML Documentation

    /// <summary>
    /// Sets the XML documentation for the indexer.
    /// </summary>
    /// <param name="configure">Configuration callback for the XML documentation.</param>
    public IndexerBuilder WithXmlDoc(Func<XmlDocumentationBuilder, XmlDocumentationBuilder> configure)
    {
        var xmlDoc = configure(XmlDocumentationBuilder.Create());
        return new IndexerBuilder(_type, _accessibility, _isVirtual, _isOverride,
            _isAbstract, _isSealed, _hasInitSetter, _accessorStyle, _getterBody, _setterBody, _parameters, _attributes,
            _usings, xmlDoc);
    }

    /// <summary>
    /// Sets the XML documentation for the indexer with a simple summary.
    /// </summary>
    /// <param name="summary">The summary text.</param>
    public IndexerBuilder WithXmlDoc(string summary)
    {
        var xmlDoc = XmlDocumentationBuilder.WithSummary(summary);
        return new IndexerBuilder(_type, _accessibility, _isVirtual, _isOverride,
            _isAbstract, _isSealed, _hasInitSetter, _accessorStyle, _getterBody, _setterBody, _parameters, _attributes,
            _usings, xmlDoc);
    }

    #endregion

    #region Attributes

    /// <summary>
    /// Adds an attribute to the indexer.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "JsonIgnore").</param>
    public IndexerBuilder WithAttribute(string name)
    {
        var attribute = AttributeBuilder.For(name);
        return new IndexerBuilder(_type, _accessibility, _isVirtual, _isOverride,
            _isAbstract, _isSealed, _hasInitSetter, _accessorStyle, _getterBody, _setterBody, _parameters,
            _attributes.Add(attribute), _usings, _xmlDoc);
    }

    /// <summary>
    /// Adds an attribute to the indexer with configuration.
    /// </summary>
    /// <param name="name">The attribute name.</param>
    /// <param name="configure">Configuration callback for the attribute.</param>
    public IndexerBuilder WithAttribute(string name, Func<AttributeBuilder, AttributeBuilder> configure)
    {
        var attribute = configure(AttributeBuilder.For(name));
        return new IndexerBuilder(_type, _accessibility, _isVirtual, _isOverride,
            _isAbstract, _isSealed, _hasInitSetter, _accessorStyle, _getterBody, _setterBody, _parameters,
            _attributes.Add(attribute), _usings, _xmlDoc);
    }

    /// <summary>
    /// Adds a pre-configured attribute to the indexer.
    /// </summary>
    public IndexerBuilder WithAttribute(AttributeBuilder attribute)
    {
        return new IndexerBuilder(_type, _accessibility, _isVirtual, _isOverride,
            _isAbstract, _isSealed, _hasInitSetter, _accessorStyle, _getterBody, _setterBody, _parameters,
            _attributes.Add(attribute), _usings, _xmlDoc);
    }

    #endregion

    #region Usings

    /// <summary>
    /// Adds a using directive that will be collected by the containing TypeBuilder.
    /// </summary>
    /// <param name="namespace">The namespace to add (e.g., "System.Linq").</param>
    public IndexerBuilder AddUsing(string @namespace)
    {
        return new IndexerBuilder(_type, _accessibility, _isVirtual, _isOverride,
            _isAbstract, _isSealed, _hasInitSetter, _accessorStyle, _getterBody, _setterBody, _parameters, _attributes,
            _usings.Add(@namespace), _xmlDoc);
    }

    /// <summary>
    /// Gets the using directives for this indexer.
    /// </summary>
    internal ImmutableArray<string> Usings => _usings;

    #endregion

    #region Building

    /// <summary>
    /// Builds the indexer as an indexer declaration syntax node.
    /// </summary>
    internal IndexerDeclarationSyntax Build()
    {
        var indexer = SyntaxFactory.IndexerDeclaration(
            SyntaxFactory.ParseTypeName(_type));

        // Add attributes
        if (_attributes.Length > 0)
        {
            var attributeLists = _attributes.Select(a => a.BuildList()).ToArray();
            indexer = indexer.WithAttributeLists(SyntaxFactory.List(attributeLists));
        }

        // Add modifiers (order matters for valid C#)
        var modifiers = new List<SyntaxKind>();
        modifiers.Add(AccessibilityToSyntaxKind(_accessibility));
        if (_isSealed) modifiers.Add(SyntaxKind.SealedKeyword);
        if (_isVirtual) modifiers.Add(SyntaxKind.VirtualKeyword);
        if (_isOverride) modifiers.Add(SyntaxKind.OverrideKeyword);
        if (_isAbstract) modifiers.Add(SyntaxKind.AbstractKeyword);

        indexer = indexer.WithModifiers(
            SyntaxFactory.TokenList(modifiers.Select(SyntaxFactory.Token)));

        // Add parameters
        if (_parameters.Length > 0)
        {
            var parameterList = SyntaxFactory.BracketedParameterList(
                SyntaxFactory.SeparatedList(_parameters.Select(p => p.Build())));
            indexer = indexer.WithParameterList(parameterList);
        }

        // Add accessors based on style
        indexer = _accessorStyle switch
        {
            IndexerAccessorStyle.Auto => AddAutoAccessors(indexer),
            IndexerAccessorStyle.ExpressionBodied => AddExpressionBody(indexer),
            IndexerAccessorStyle.BlockBodied => AddBlockBodiedAccessors(indexer),
            IndexerAccessorStyle.GetterOnly => AddGetterOnly(indexer),
            _ => indexer
        };

        // Add XML documentation
        if (_xmlDoc.HasValue && _xmlDoc.Value.HasContent)
        {
            var trivia = _xmlDoc.Value.Build();
            indexer = indexer.WithLeadingTrivia(trivia);
        }

        return indexer;
    }

    private IndexerDeclarationSyntax AddAutoAccessors(IndexerDeclarationSyntax indexer)
    {
        var setterKind = _hasInitSetter ? SyntaxKind.InitAccessorDeclaration : SyntaxKind.SetAccessorDeclaration;
        return indexer.WithAccessorList(
            SyntaxFactory.AccessorList(
                SyntaxFactory.List(new[]
                {
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(setterKind)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                })));
    }

    private IndexerDeclarationSyntax AddGetterOnly(IndexerDeclarationSyntax indexer)
    {
        return indexer.WithAccessorList(
            SyntaxFactory.AccessorList(
                SyntaxFactory.SingletonList(
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))));
    }

    private IndexerDeclarationSyntax AddExpressionBody(IndexerDeclarationSyntax indexer)
    {
        if (_getterBody == null)
            return indexer;

        return indexer
            .WithExpressionBody(
                SyntaxFactory.ArrowExpressionClause(
                    SyntaxFactory.ParseExpression(_getterBody)))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    }

    private IndexerDeclarationSyntax AddBlockBodiedAccessors(IndexerDeclarationSyntax indexer)
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
            var setterKind = _hasInitSetter ? SyntaxKind.InitAccessorDeclaration : SyntaxKind.SetAccessorDeclaration;
            var setter = SyntaxFactory.AccessorDeclaration(setterKind)
                .WithBody(SyntaxFactory.ParseStatement(_setterBody) as BlockSyntax);
            accessors.Add(setter);
        }

        return indexer.WithAccessorList(
            SyntaxFactory.AccessorList(SyntaxFactory.List(accessors)));
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

    /// <summary>
    /// Gets the indexer return type.
    /// </summary>
    public string Type => _type;

    #endregion
}

internal enum IndexerAccessorStyle
{
    None,
    Auto,
    ExpressionBodied,
    BlockBodied,
    GetterOnly
}
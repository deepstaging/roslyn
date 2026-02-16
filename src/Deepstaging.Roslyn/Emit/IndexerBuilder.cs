// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for indexers.
/// Supports expression-bodied and block-bodied indexers.
/// Immutable - each method returns a new instance.
/// </summary>
public record struct IndexerBuilder()
{
    /// <summary>The return type of the indexer.</summary>
    public string Type { get; init; } = "";

    /// <summary>The accessibility level of the indexer.</summary>
    public Accessibility Accessibility { get; init; }

    /// <summary>Whether the indexer is virtual.</summary>
    public bool IsVirtual { get; init; }

    /// <summary>Whether the indexer is an override.</summary>
    public bool IsOverride { get; init; }

    /// <summary>Whether the indexer is abstract.</summary>
    public bool IsAbstract { get; init; }

    /// <summary>Whether the indexer is sealed.</summary>
    public bool IsSealed { get; init; }

    /// <summary>Whether the indexer has an init-only setter.</summary>
    public bool HasInitSetter { get; init; }

    /// <summary>The accessor style for the indexer.</summary>
    public IndexerAccessorStyle AccessorStyle { get; init; }

    /// <summary>The getter body expression or block.</summary>
    public string? GetterBody { get; init; }

    /// <summary>The setter body expression or block.</summary>
    public string? SetterBody { get; init; }

    /// <summary>The indexer parameters.</summary>
    public ImmutableArray<ParameterBuilder> Parameters { get; init; }

    /// <summary>The attributes applied to the indexer.</summary>
    public ImmutableArray<AttributeBuilder> Attributes { get; init; }

    /// <summary>The using directives for this indexer.</summary>
    public ImmutableArray<string> Usings { get; init; }

    /// <summary>The XML documentation for the indexer.</summary>
    public XmlDocumentationBuilder? XmlDoc { get; init; }

    /// <summary>Gets the preprocessor directive condition for conditional compilation.</summary>
    public Directive? Condition { get; init; }

    /// <summary>Gets the region name for grouping this member in a #region block.</summary>
    public string? Region { get; init; }

    #region Factory Methods

    /// <summary>
    /// Creates an indexer builder for the specified return type.
    /// </summary>
    /// <param name="type">The indexer return type (e.g., "string", "T", "int").</param>
    public static IndexerBuilder For(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Indexer type cannot be null or empty.", nameof(type));

        return new IndexerBuilder
        {
            Type = type,
            Accessibility = Accessibility.Public
        };
    }

    /// <summary>
    /// Creates an indexer builder using a symbol's globally qualified name as the return type.
    /// </summary>
    /// <param name="type">The indexer return type symbol.</param>
    public static IndexerBuilder For<T>(ValidSymbol<T> type) where T : class, ITypeSymbol
        => For(type.GloballyQualifiedName);

    #endregion

    #region Accessibility & Modifiers

    /// <summary>
    /// Sets the accessibility of the indexer.
    /// </summary>
    public IndexerBuilder WithAccessibility(Accessibility accessibility) =>
        this with { Accessibility = accessibility };

    /// <summary>
    /// Sets the accessibility of the indexer from a keyword string (e.g., "public", "internal").
    /// Accepts the same values produced by <see cref="ValidSymbol{TSymbol}.AccessibilityString"/>
    /// and <see cref="SymbolSnapshot.AccessibilityString"/>.
    /// </summary>
    public IndexerBuilder WithAccessibility(string accessibilityKeyword) =>
        WithAccessibility(AccessibilityHelper.Parse(accessibilityKeyword));

    /// <summary>
    /// Marks the indexer as virtual.
    /// </summary>
    public IndexerBuilder AsVirtual() =>
        this with { IsVirtual = true };

    /// <summary>
    /// Marks the indexer as override.
    /// </summary>
    public IndexerBuilder AsOverride() =>
        this with { IsOverride = true };

    /// <summary>
    /// Marks the indexer as abstract.
    /// </summary>
    public IndexerBuilder AsAbstract() =>
        this with { IsAbstract = true };

    /// <summary>
    /// Marks the indexer as sealed. Only valid when used with override.
    /// </summary>
    public IndexerBuilder AsSealed() =>
        this with { IsSealed = true };

    /// <summary>
    /// Wraps this indexer in a preprocessor directive (#if/#endif).
    /// </summary>
    /// <param name="directive">The directive condition (e.g., Directives.Net6OrGreater).</param>
    public IndexerBuilder When(Directive directive) =>
        this with { Condition = directive };

    /// <summary>
    /// Assigns this indexer to a named region for grouping in #region/#endregion blocks.
    /// </summary>
    /// <param name="regionName">The region name (e.g., "Indexers").</param>
    public IndexerBuilder InRegion(string regionName) =>
        this with { Region = regionName };

    #endregion

    #region Parameters

    /// <summary>
    /// Adds an index parameter to the indexer.
    /// </summary>
    /// <param name="name">The parameter name (e.g., "index", "key").</param>
    /// <param name="type">The parameter type (e.g., "int", "string").</param>
    public IndexerBuilder AddParameter(string name, string type)
    {
        var parameters = Parameters.IsDefault ? [] : Parameters;
        return this with { Parameters = parameters.Add(ParameterBuilder.For(name, type)) };
    }

    /// <summary>
    /// Adds an index parameter to the indexer with configuration.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="type">The parameter type.</param>
    /// <param name="configure">Configuration callback for the parameter.</param>
    public IndexerBuilder AddParameter(string name, string type, Func<ParameterBuilder, ParameterBuilder> configure)
    {
        var parameters = Parameters.IsDefault ? [] : Parameters;
        return this with { Parameters = parameters.Add(configure(ParameterBuilder.For(name, type))) };
    }

    /// <summary>
    /// Adds an index parameter using a symbol's globally qualified name as the type.
    /// </summary>
    public IndexerBuilder AddParameter<T>(string name, ValidSymbol<T> type) where T : class, ITypeSymbol
        => AddParameter(name, type.GloballyQualifiedName);

    /// <summary>
    /// Adds an index parameter using a symbol's globally qualified name, with configuration.
    /// </summary>
    public IndexerBuilder AddParameter<T>(
        string name,
        ValidSymbol<T> type,
        Func<ParameterBuilder, ParameterBuilder> configure) where T : class, ITypeSymbol
        => AddParameter(name, type.GloballyQualifiedName, configure);

    /// <summary>
    /// Adds a pre-configured parameter to the indexer.
    /// </summary>
    public IndexerBuilder AddParameter(ParameterBuilder parameter)
    {
        var parameters = Parameters.IsDefault ? [] : Parameters;
        return this with { Parameters = parameters.Add(parameter) };
    }

    #endregion

    #region Accessors

    /// <summary>
    /// Configures the indexer as an auto-indexer with get and set accessors.
    /// Note: Indexers cannot have auto-implemented accessors, so a body is typically required.
    /// </summary>
    public IndexerBuilder WithAutoAccessors() =>
        this with { AccessorStyle = IndexerAccessorStyle.Auto, GetterBody = null, SetterBody = null };

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

        return this with { AccessorStyle = IndexerAccessorStyle.ExpressionBodied, GetterBody = cleanExpression };
    }

    /// <summary>
    /// Configures the indexer with a getter body builder.
    /// </summary>
    public IndexerBuilder WithGetter(Func<BodyBuilder, BodyBuilder> configure)
    {
        var body = configure(BodyBuilder.Empty());
        return this with { AccessorStyle = IndexerAccessorStyle.BlockBodied, GetterBody = body.Build().ToFullString() };
    }

    /// <summary>
    /// Configures the indexer with a setter body builder.
    /// </summary>
    public IndexerBuilder WithSetter(Func<BodyBuilder, BodyBuilder> configure)
    {
        var body = configure(BodyBuilder.Empty());
        return this with { AccessorStyle = IndexerAccessorStyle.BlockBodied, SetterBody = body.Build().ToFullString() };
    }

    /// <summary>
    /// Marks the indexer as read-only (get accessor only).
    /// </summary>
    public IndexerBuilder AsReadOnly() =>
        this with { AccessorStyle = IndexerAccessorStyle.GetterOnly, SetterBody = null };

    /// <summary>
    /// Configures the indexer to use an init-only setter instead of a set accessor.
    /// Example: public string this[int index] { get; init; }
    /// </summary>
    public IndexerBuilder WithInitOnlySetter() =>
        this with { HasInitSetter = true };

    #endregion

    #region XML Documentation

    /// <summary>
    /// Sets the XML documentation for the indexer.
    /// </summary>
    /// <param name="configure">Configuration callback for the XML documentation.</param>
    public IndexerBuilder WithXmlDoc(Func<XmlDocumentationBuilder, XmlDocumentationBuilder> configure) =>
        this with { XmlDoc = configure(XmlDocumentationBuilder.Create()) };

    /// <summary>
    /// Sets the XML documentation for the indexer with a simple summary.
    /// </summary>
    /// <param name="summary">The summary text.</param>
    public IndexerBuilder WithXmlDoc(string summary) =>
        this with { XmlDoc = XmlDocumentationBuilder.ForSummary(summary) };

    #endregion

    #region Attributes

    /// <summary>
    /// Adds an attribute to the indexer.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "JsonIgnore").</param>
    public IndexerBuilder WithAttribute(string name)
    {
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(AttributeBuilder.For(name)) };
    }

    /// <summary>
    /// Adds an attribute to the indexer with configuration.
    /// </summary>
    /// <param name="name">The attribute name.</param>
    /// <param name="configure">Configuration callback for the attribute.</param>
    public IndexerBuilder WithAttribute(string name, Func<AttributeBuilder, AttributeBuilder> configure)
    {
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(configure(AttributeBuilder.For(name))) };
    }

    /// <summary>
    /// Adds a pre-configured attribute to the indexer.
    /// </summary>
    public IndexerBuilder WithAttribute(AttributeBuilder attribute)
    {
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(attribute) };
    }

    #endregion

    #region Usings

    /// <summary>
    /// Adds a using directive that will be collected by the containing TypeBuilder.
    /// </summary>
    /// <param name="namespace">The namespace to add (e.g., "System.Linq").</param>
    public IndexerBuilder AddUsing(string @namespace)
    {
        var usings = Usings.IsDefault ? [] : Usings;
        return this with { Usings = usings.Add(@namespace) };
    }

    #endregion

    #region Building

    /// <summary>
    /// Builds the indexer as an indexer declaration syntax node.
    /// </summary>
    internal IndexerDeclarationSyntax Build()
    {
        var indexer = SyntaxFactory.IndexerDeclaration(
            SyntaxFactory.ParseTypeName(Type));

        // Add attributes
        var attributes = Attributes.IsDefault ? [] : Attributes;

        if (attributes.Length > 0)
        {
            var attributeLists = attributes.Select(a => a.BuildList()).ToArray();
            indexer = indexer.WithAttributeLists(SyntaxFactory.List(attributeLists));
        }

        // Add modifiers (order matters for valid C#)
        var modifiers = new List<SyntaxKind>();
        modifiers.Add(AccessibilityToSyntaxKind(Accessibility));
        if (IsSealed) modifiers.Add(SyntaxKind.SealedKeyword);
        if (IsVirtual) modifiers.Add(SyntaxKind.VirtualKeyword);
        if (IsOverride) modifiers.Add(SyntaxKind.OverrideKeyword);
        if (IsAbstract) modifiers.Add(SyntaxKind.AbstractKeyword);

        indexer = indexer.WithModifiers(
            SyntaxFactory.TokenList(modifiers.Select(SyntaxFactory.Token)));

        // Add parameters
        var parameters = Parameters.IsDefault ? [] : Parameters;

        if (parameters.Length > 0)
        {
            var parameterList = SyntaxFactory.BracketedParameterList(
                SyntaxFactory.SeparatedList(parameters.Select(p => p.Build())));

            indexer = indexer.WithParameterList(parameterList);
        }

        // Add accessors based on style
        indexer = AccessorStyle switch
        {
            IndexerAccessorStyle.Auto => AddAutoAccessors(indexer),
            IndexerAccessorStyle.ExpressionBodied => AddExpressionBody(indexer),
            IndexerAccessorStyle.BlockBodied => AddBlockBodiedAccessors(indexer),
            IndexerAccessorStyle.GetterOnly => AddGetterOnly(indexer),
            _ => indexer
        };

        // Add XML documentation
        if (XmlDoc.HasValue && XmlDoc.Value.HasContent)
        {
            var trivia = XmlDoc.Value.Build();
            indexer = indexer.WithLeadingTrivia(trivia);
        }

        // Wrap in preprocessor directive if specified
        if (Condition.HasValue) indexer = DirectiveHelper.WrapInDirective(indexer, Condition.Value);

        return indexer;
    }

    private IndexerDeclarationSyntax AddAutoAccessors(IndexerDeclarationSyntax indexer)
    {
        var setterKind = HasInitSetter ? SyntaxKind.InitAccessorDeclaration : SyntaxKind.SetAccessorDeclaration;

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

    private static IndexerDeclarationSyntax AddGetterOnly(IndexerDeclarationSyntax indexer) => indexer.WithAccessorList(
        SyntaxFactory.AccessorList(
            SyntaxFactory.SingletonList(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))));

    private IndexerDeclarationSyntax AddExpressionBody(IndexerDeclarationSyntax indexer)
    {
        if (GetterBody == null)
            return indexer;

        return indexer
            .WithExpressionBody(
                SyntaxFactory.ArrowExpressionClause(
                    SyntaxFactory.ParseExpression(GetterBody)))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    }

    private IndexerDeclarationSyntax AddBlockBodiedAccessors(IndexerDeclarationSyntax indexer)
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

        return indexer.WithAccessorList(
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

/// <summary>Specifies the accessor style for an indexer.</summary>
public enum IndexerAccessorStyle
{
    /// <summary>No accessor style specified.</summary>
    None,

    /// <summary>Auto-implemented accessors.</summary>
    Auto,

    /// <summary>Expression-bodied accessor.</summary>
    ExpressionBodied,

    /// <summary>Block-bodied accessors.</summary>
    BlockBodied,

    /// <summary>Getter-only accessor.</summary>
    GetterOnly
}
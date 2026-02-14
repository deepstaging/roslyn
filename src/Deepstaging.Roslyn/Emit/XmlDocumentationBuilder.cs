// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for XML documentation comments.
/// Supports summary, remarks, params, returns, exceptions, and more.
/// Immutable - each method returns a new instance.
/// </summary>
public record struct XmlDocumentationBuilder
{
    /// <summary>The summary text.</summary>
    public string? Summary { get; init; }

    /// <summary>The remarks text.</summary>
    public string? Remarks { get; init; }

    /// <summary>The returns description.</summary>
    public string? Returns { get; init; }

    /// <summary>The value description (for properties).</summary>
    public string? Value { get; init; }

    /// <summary>The example code.</summary>
    public string? Example { get; init; }

    /// <summary>The parameter documentation entries.</summary>
    public ImmutableArray<(string Name, string Description)> Params { get; init; }

    /// <summary>The type parameter documentation entries.</summary>
    public ImmutableArray<(string Name, string Description)> TypeParams { get; init; }

    /// <summary>The exception documentation entries.</summary>
    public ImmutableArray<(string Type, string Description)> Exceptions { get; init; }

    /// <summary>The seealso cref references.</summary>
    public ImmutableArray<string> SeeAlso { get; init; }

    /// <summary>The inheritdoc cref reference (null = no inheritdoc, empty = inheritdoc without cref).</summary>
    public string? InheritDoc { get; init; }

    #region Factory Methods

    /// <summary>
    /// Creates an empty XML documentation builder.
    /// </summary>
    public static XmlDocumentationBuilder Create() => new();

    /// <summary>
    /// Creates an XML documentation builder with a summary.
    /// </summary>
    /// <param name="summary">The summary text.</param>
    public static XmlDocumentationBuilder ForSummary(string summary) => new() { Summary = summary };

    /// <summary>
    /// Creates an XML documentation builder with an inheritdoc element.
    /// </summary>
    /// <param name="cref">Optional cref attribute for the inheritdoc element.</param>
    public static XmlDocumentationBuilder ForInheritDoc(string? cref = null) => new() { InheritDoc = cref ?? string.Empty };

    /// <summary>
    /// Creates a builder pre-populated from a pipeline-safe <see cref="DocumentationSnapshot"/>.
    /// </summary>
    /// <param name="snapshot">The documentation snapshot to copy from.</param>
    public static XmlDocumentationBuilder From(DocumentationSnapshot snapshot)
    {
        if (!snapshot.HasValue && snapshot.Params.Count == 0 && snapshot.TypeParams.Count == 0)
            return new();

        return new()
        {
            Summary = snapshot.Summary,
            Remarks = snapshot.Remarks,
            Returns = snapshot.Returns,
            Value = snapshot.Value,
            Example = snapshot.Example,
            Params = [..snapshot.Params.Select(p => (p.Name, p.Description))],
            TypeParams = [..snapshot.TypeParams.Select(p => (p.Name, p.Description))],
            Exceptions = [..snapshot.Exceptions.Select(e => (e.Type, e.Description))],
            SeeAlso = [..snapshot.SeeAlso],
        };
    }

    #endregion

    #region Content

    /// <summary>
    /// Sets the summary element.
    /// </summary>
    /// <param name="text">The summary text.</param>
    public readonly XmlDocumentationBuilder WithSummary(string text) => this with { Summary = text };

    /// <summary>
    /// Sets the remarks element.
    /// </summary>
    /// <param name="text">The remarks text.</param>
    public readonly XmlDocumentationBuilder WithRemarks(string text) => this with { Remarks = text };

    /// <summary>
    /// Sets the returns element.
    /// </summary>
    /// <param name="text">The returns description.</param>
    public readonly XmlDocumentationBuilder WithReturns(string text) => this with { Returns = text };

    /// <summary>
    /// Sets the value element (for properties).
    /// </summary>
    /// <param name="text">The value description.</param>
    public readonly XmlDocumentationBuilder WithValue(string text) => this with { Value = text };

    /// <summary>
    /// Adds a param element.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="description">The parameter description.</param>
    public readonly XmlDocumentationBuilder AddParam(string name, string description)
    {
        var @params = Params.IsDefault ? [] : Params;
        return this with { Params = @params.Add((name, description)) };
    }

    /// <summary>
    /// Adds a typeparam element.
    /// </summary>
    /// <param name="name">The type parameter name.</param>
    /// <param name="description">The type parameter description.</param>
    public readonly XmlDocumentationBuilder AddTypeParam(string name, string description)
    {
        var typeParams = TypeParams.IsDefault ? [] : TypeParams;
        return this with { TypeParams = typeParams.Add((name, description)) };
    }

    /// <summary>
    /// Adds an exception element.
    /// </summary>
    /// <param name="exceptionType">The exception type (e.g., "ArgumentNullException").</param>
    /// <param name="description">When the exception is thrown.</param>
    public readonly XmlDocumentationBuilder AddException(string exceptionType, string description)
    {
        var exceptions = Exceptions.IsDefault ? [] : Exceptions;
        return this with { Exceptions = exceptions.Add((exceptionType, description)) };
    }

    /// <summary>
    /// Adds a seealso element.
    /// </summary>
    /// <param name="cref">The cref reference (e.g., "MyClass", "MyMethod").</param>
    public readonly XmlDocumentationBuilder AddSeeAlso(string cref)
    {
        var seeAlso = SeeAlso.IsDefault ? [] : SeeAlso;
        return this with { SeeAlso = seeAlso.Add(cref) };
    }

    /// <summary>
    /// Sets the example element.
    /// </summary>
    /// <param name="code">The example code.</param>
    public readonly XmlDocumentationBuilder WithExample(string code) => this with { Example = code };

    /// <summary>
    /// Sets the inheritdoc element. When set, this replaces the summary.
    /// </summary>
    /// <param name="cref">Optional cref attribute for the inheritdoc element.</param>
    public readonly XmlDocumentationBuilder WithInheritDoc(string? cref = null) => this with { InheritDoc = cref ?? string.Empty };

    #endregion

    #region Building

    /// <summary>
    /// Returns true if the builder has any content.
    /// </summary>
    public readonly bool HasContent
    {
        get
        {
            var @params = Params.IsDefault ? [] : Params;
            var typeParams = TypeParams.IsDefault ? [] : TypeParams;
            var exceptions = Exceptions.IsDefault ? [] : Exceptions;
            var seeAlso = SeeAlso.IsDefault ? [] : SeeAlso;
            return Summary != null ||
                   InheritDoc != null ||
                   Remarks != null ||
                   Returns != null ||
                   Value != null ||
                   @params.Length > 0 ||
                   typeParams.Length > 0 ||
                   exceptions.Length > 0 ||
                   seeAlso.Length > 0 ||
                   Example != null;
        }
    }

    /// <summary>
    /// Builds the XML documentation as a documentation comment trivia.
    /// </summary>
    internal readonly SyntaxTriviaList Build()
    {
        if (!HasContent)
            return SyntaxTriviaList.Empty;

        var lines = new List<string>();

        // InheritDoc takes precedence over Summary when both are set
        if (InheritDoc != null)
        {
            if (string.IsNullOrEmpty(InheritDoc))
                lines.Add("/// <inheritdoc/>");
            else
                lines.Add($"/// <inheritdoc cref=\"{InheritDoc}\"/>");
        }
        else if (Summary != null)
        {
            lines.Add("/// <summary>");
            foreach (var line in SplitLines(Summary)) lines.Add($"/// {line}");
            lines.Add("/// </summary>");
        }

        // Type params
        var typeParams = TypeParams.IsDefault ? [] : TypeParams;
        foreach (var (name, description) in typeParams)
            lines.Add($"/// <typeparam name=\"{EscapeXml(name)}\">{EscapeXml(description)}</typeparam>");

        // Params
        var @params = Params.IsDefault ? [] : Params;
        foreach (var (name, description) in @params)
            lines.Add($"/// <param name=\"{EscapeXml(name)}\">{EscapeXml(description)}</param>");

        // Returns
        if (Returns != null) lines.Add($"/// <returns>{EscapeXml(Returns)}</returns>");

        // Value
        if (Value != null) lines.Add($"/// <value>{EscapeXml(Value)}</value>");

        // Exceptions
        var exceptions = Exceptions.IsDefault ? [] : Exceptions;
        foreach (var (type, description) in exceptions)
            lines.Add($"/// <exception cref=\"{EscapeXml(type)}\">{EscapeXml(description)}</exception>");

        // Remarks
        if (Remarks != null)
        {
            lines.Add("/// <remarks>");
            foreach (var line in SplitLines(Remarks)) lines.Add($"/// {line}");
            lines.Add("/// </remarks>");
        }

        // Example
        if (Example != null)
        {
            lines.Add("/// <example>");
            lines.Add("/// <code>");
            foreach (var line in SplitLines(Example)) lines.Add($"/// {line}");
            lines.Add("/// </code>");
            lines.Add("/// </example>");
        }

        // SeeAlso
        var seeAlso = SeeAlso.IsDefault ? [] : SeeAlso;
        foreach (var cref in seeAlso) lines.Add($"/// <seealso cref=\"{EscapeXml(cref)}\"/>");

        // Build trivia
        var triviaList = new List<SyntaxTrivia>();
        foreach (var line in lines)
        {
            triviaList.Add(SyntaxFactory.Comment(line));
            triviaList.Add(SyntaxFactory.EndOfLine("\n"));
        }

        return SyntaxFactory.TriviaList(triviaList);
    }

    private static string[] SplitLines(string text)
    {
        return text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
    }

    private static string EscapeXml(string text)
    {
        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;");
    }

    #endregion
}
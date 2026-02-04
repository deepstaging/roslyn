// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for XML documentation comments.
/// Supports summary, remarks, params, returns, exceptions, and more.
/// Immutable - each method returns a new instance.
/// </summary>
public readonly struct XmlDocumentationBuilder
{
    private readonly string? _summary;
    private readonly string? _remarks;
    private readonly string? _returns;
    private readonly string? _value;
    private readonly ImmutableArray<(string Name, string Description)> _params;
    private readonly ImmutableArray<(string Name, string Description)> _typeParams;
    private readonly ImmutableArray<(string Type, string Description)> _exceptions;
    private readonly ImmutableArray<string> _seeAlso;
    private readonly string? _example;

    private XmlDocumentationBuilder(
        string? summary,
        string? remarks,
        string? returns,
        string? value,
        ImmutableArray<(string Name, string Description)> @params,
        ImmutableArray<(string Name, string Description)> typeParams,
        ImmutableArray<(string Type, string Description)> exceptions,
        ImmutableArray<string> seeAlso,
        string? example)
    {
        _summary = summary;
        _remarks = remarks;
        _returns = returns;
        _value = value;
        _params = @params.IsDefault ? ImmutableArray<(string, string)>.Empty : @params;
        _typeParams = typeParams.IsDefault ? ImmutableArray<(string, string)>.Empty : typeParams;
        _exceptions = exceptions.IsDefault ? ImmutableArray<(string, string)>.Empty : exceptions;
        _seeAlso = seeAlso.IsDefault ? ImmutableArray<string>.Empty : seeAlso;
        _example = example;
    }

    #region Factory Methods

    /// <summary>
    /// Creates an empty XML documentation builder.
    /// </summary>
    public static XmlDocumentationBuilder Create()
    {
        return new XmlDocumentationBuilder(
            summary: null,
            remarks: null,
            returns: null,
            value: null,
            ImmutableArray<(string, string)>.Empty,
            ImmutableArray<(string, string)>.Empty,
            ImmutableArray<(string, string)>.Empty,
            ImmutableArray<string>.Empty,
            example: null);
    }

    /// <summary>
    /// Creates an XML documentation builder with a summary.
    /// </summary>
    /// <param name="summary">The summary text.</param>
    public static XmlDocumentationBuilder WithSummary(string summary)
    {
        return Create().Summary(summary);
    }

    /// <summary>
    /// Creates an XML documentation builder from parsed XmlDocumentation.
    /// </summary>
    /// <param name="documentation">The parsed XML documentation to copy from.</param>
    public static XmlDocumentationBuilder From(XmlDocumentation documentation)
    {
        if (documentation.IsEmpty)
            return Create();

        var builder = Create();

        if (documentation.Summary != null)
            builder = builder.Summary(documentation.Summary);

        if (documentation.Remarks != null)
            builder = builder.Remarks(documentation.Remarks);

        if (documentation.Returns != null)
            builder = builder.Returns(documentation.Returns);

        if (documentation.Value != null)
            builder = builder.Value(documentation.Value);

        if (documentation.Example != null)
            builder = builder.Example(documentation.Example);

        foreach (var (name, description) in documentation.Params)
            builder = builder.Param(name, description);

        foreach (var (name, description) in documentation.TypeParams)
            builder = builder.TypeParam(name, description);

        foreach (var (type, description) in documentation.Exceptions)
            builder = builder.Exception(type, description);

        foreach (var cref in documentation.SeeAlso)
            builder = builder.SeeAlso(cref);

        return builder;
    }

    #endregion

    #region Content

    /// <summary>
    /// Sets the summary element.
    /// </summary>
    /// <param name="text">The summary text.</param>
    public XmlDocumentationBuilder Summary(string text)
    {
        return new XmlDocumentationBuilder(text, _remarks, _returns, _value, _params, _typeParams, _exceptions, _seeAlso, _example);
    }

    /// <summary>
    /// Sets the remarks element.
    /// </summary>
    /// <param name="text">The remarks text.</param>
    public XmlDocumentationBuilder Remarks(string text)
    {
        return new XmlDocumentationBuilder(_summary, text, _returns, _value, _params, _typeParams, _exceptions, _seeAlso, _example);
    }

    /// <summary>
    /// Sets the returns element.
    /// </summary>
    /// <param name="text">The returns description.</param>
    public XmlDocumentationBuilder Returns(string text)
    {
        return new XmlDocumentationBuilder(_summary, _remarks, text, _value, _params, _typeParams, _exceptions, _seeAlso, _example);
    }

    /// <summary>
    /// Sets the value element (for properties).
    /// </summary>
    /// <param name="text">The value description.</param>
    public XmlDocumentationBuilder Value(string text)
    {
        return new XmlDocumentationBuilder(_summary, _remarks, _returns, text, _params, _typeParams, _exceptions, _seeAlso, _example);
    }

    /// <summary>
    /// Adds a param element.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="description">The parameter description.</param>
    public XmlDocumentationBuilder Param(string name, string description)
    {
        return new XmlDocumentationBuilder(_summary, _remarks, _returns, _value, _params.Add((name, description)), _typeParams, _exceptions, _seeAlso, _example);
    }

    /// <summary>
    /// Adds a typeparam element.
    /// </summary>
    /// <param name="name">The type parameter name.</param>
    /// <param name="description">The type parameter description.</param>
    public XmlDocumentationBuilder TypeParam(string name, string description)
    {
        return new XmlDocumentationBuilder(_summary, _remarks, _returns, _value, _params, _typeParams.Add((name, description)), _exceptions, _seeAlso, _example);
    }

    /// <summary>
    /// Adds an exception element.
    /// </summary>
    /// <param name="exceptionType">The exception type (e.g., "ArgumentNullException").</param>
    /// <param name="description">When the exception is thrown.</param>
    public XmlDocumentationBuilder Exception(string exceptionType, string description)
    {
        return new XmlDocumentationBuilder(_summary, _remarks, _returns, _value, _params, _typeParams, _exceptions.Add((exceptionType, description)), _seeAlso, _example);
    }

    /// <summary>
    /// Adds a seealso element.
    /// </summary>
    /// <param name="cref">The cref reference (e.g., "MyClass", "MyMethod").</param>
    public XmlDocumentationBuilder SeeAlso(string cref)
    {
        return new XmlDocumentationBuilder(_summary, _remarks, _returns, _value, _params, _typeParams, _exceptions, _seeAlso.Add(cref), _example);
    }

    /// <summary>
    /// Sets the example element.
    /// </summary>
    /// <param name="code">The example code.</param>
    public XmlDocumentationBuilder Example(string code)
    {
        return new XmlDocumentationBuilder(_summary, _remarks, _returns, _value, _params, _typeParams, _exceptions, _seeAlso, code);
    }

    #endregion

    #region Building

    /// <summary>
    /// Returns true if the builder has any content.
    /// </summary>
    public bool HasContent =>
        _summary != null ||
        _remarks != null ||
        _returns != null ||
        _value != null ||
        _params.Length > 0 ||
        _typeParams.Length > 0 ||
        _exceptions.Length > 0 ||
        _seeAlso.Length > 0 ||
        _example != null;

    /// <summary>
    /// Builds the XML documentation as a documentation comment trivia.
    /// </summary>
    internal SyntaxTriviaList Build()
    {
        if (!HasContent)
            return SyntaxTriviaList.Empty;

        var lines = new List<string>();

        // Summary
        if (_summary != null)
        {
            lines.Add("/// <summary>");
            foreach (var line in SplitLines(_summary))
            {
                lines.Add($"/// {line}");
            }
            lines.Add("/// </summary>");
        }

        // Type params
        foreach (var (name, description) in _typeParams)
        {
            lines.Add($"/// <typeparam name=\"{EscapeXml(name)}\">{EscapeXml(description)}</typeparam>");
        }

        // Params
        foreach (var (name, description) in _params)
        {
            lines.Add($"/// <param name=\"{EscapeXml(name)}\">{EscapeXml(description)}</param>");
        }

        // Returns
        if (_returns != null)
        {
            lines.Add($"/// <returns>{EscapeXml(_returns)}</returns>");
        }

        // Value
        if (_value != null)
        {
            lines.Add($"/// <value>{EscapeXml(_value)}</value>");
        }

        // Exceptions
        foreach (var (type, description) in _exceptions)
        {
            lines.Add($"/// <exception cref=\"{EscapeXml(type)}\">{EscapeXml(description)}</exception>");
        }

        // Remarks
        if (_remarks != null)
        {
            lines.Add("/// <remarks>");
            foreach (var line in SplitLines(_remarks))
            {
                lines.Add($"/// {line}");
            }
            lines.Add("/// </remarks>");
        }

        // Example
        if (_example != null)
        {
            lines.Add("/// <example>");
            lines.Add("/// <code>");
            foreach (var line in SplitLines(_example))
            {
                lines.Add($"/// {line}");
            }
            lines.Add("/// </code>");
            lines.Add("/// </example>");
        }

        // SeeAlso
        foreach (var cref in _seeAlso)
        {
            lines.Add($"/// <seealso cref=\"{EscapeXml(cref)}\"/>");
        }

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

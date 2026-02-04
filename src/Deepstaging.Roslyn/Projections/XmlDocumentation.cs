// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.Xml.Linq;

namespace Deepstaging.Roslyn;

/// <summary>
/// Parsed XML documentation from a symbol.
/// Provides access to common documentation elements like summary, remarks, returns, etc.
/// </summary>
public readonly struct XmlDocumentation
{
    private readonly XElement? _member;

    private XmlDocumentation(XElement? member)
    {
        _member = member;
    }

    /// <summary>
    /// Creates an XmlDocumentation from raw XML documentation string.
    /// </summary>
    private static XmlDocumentation Parse(string? xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
            return new XmlDocumentation(null);

        try
        {
            var doc = XElement.Parse(xml);
            return new XmlDocumentation(doc);
        }
        catch
        {
            return new XmlDocumentation(null);
        }
    }

    /// <summary>
    /// Creates an XmlDocumentation from a symbol.
    /// </summary>
    public static XmlDocumentation FromSymbol(ISymbol? symbol)
    {
        return Parse(symbol?.GetDocumentationCommentXml());
    }

    /// <summary>
    /// Gets a value indicating whether documentation exists.
    /// </summary>
    public bool HasValue => _member != null;

    /// <summary>
    /// Gets a value indicating whether documentation is empty.
    /// </summary>
    public bool IsEmpty => _member == null;

    /// <summary>
    /// Gets the summary text, or null if not present.
    /// </summary>
    public string? Summary => GetElementText("summary");

    /// <summary>
    /// Gets the remarks text, or null if not present.
    /// </summary>
    public string? Remarks => GetElementText("remarks");

    /// <summary>
    /// Gets the returns text, or null if not present.
    /// </summary>
    public string? Returns => GetElementText("returns");

    /// <summary>
    /// Gets the value text (for properties), or null if not present.
    /// </summary>
    public string? Value => GetElementText("value");

    /// <summary>
    /// Gets the example text, or null if not present.
    /// </summary>
    public string? Example => GetElementText("example");

    /// <summary>
    /// Gets all param elements as name-description pairs.
    /// </summary>
    public IEnumerable<(string Name, string Description)> Params
    {
        get
        {
            if (_member == null)
                yield break;

            foreach (var param in _member.Elements("param"))
            {
                var name = param.Attribute("name")?.Value;
                if (name != null)
                    yield return (name, NormalizeWhitespace(param.Value));
            }
        }
    }

    /// <summary>
    /// Gets the description for a specific parameter.
    /// </summary>
    public string? GetParam(string name)
    {
        if (_member == null)
            return null;

        var param = _member.Elements("param")
            .FirstOrDefault(p => p.Attribute("name")?.Value == name);

        return param != null ? NormalizeWhitespace(param.Value) : null;
    }

    /// <summary>
    /// Gets all typeparam elements as name-description pairs.
    /// </summary>
    public IEnumerable<(string Name, string Description)> TypeParams
    {
        get
        {
            if (_member == null)
                yield break;

            foreach (var param in _member.Elements("typeparam"))
            {
                var name = param.Attribute("name")?.Value;
                if (name != null)
                    yield return (name, NormalizeWhitespace(param.Value));
            }
        }
    }

    /// <summary>
    /// Gets the description for a specific type parameter.
    /// </summary>
    public string? GetTypeParam(string name)
    {
        if (_member == null)
            return null;

        var param = _member.Elements("typeparam")
            .FirstOrDefault(p => p.Attribute("name")?.Value == name);

        return param != null ? NormalizeWhitespace(param.Value) : null;
    }

    /// <summary>
    /// Gets all exception elements as type-description pairs.
    /// </summary>
    public IEnumerable<(string Type, string Description)> Exceptions
    {
        get
        {
            if (_member == null)
                yield break;

            foreach (var ex in _member.Elements("exception"))
            {
                var cref = ex.Attribute("cref")?.Value;
                if (cref != null)
                    yield return (cref, NormalizeWhitespace(ex.Value));
            }
        }
    }

    /// <summary>
    /// Gets all seealso cref values.
    /// </summary>
    public IEnumerable<string> SeeAlso
    {
        get
        {
            if (_member == null)
                yield break;

            foreach (var see in _member.Elements("seealso"))
            {
                var cref = see.Attribute("cref")?.Value;
                if (cref != null)
                    yield return cref;
            }
        }
    }

    /// <summary>
    /// Gets the raw XML string.
    /// </summary>
    public string? RawXml => _member?.ToString();

    private string? GetElementText(string elementName)
    {
        var element = _member?.Element(elementName);
        return element != null ? NormalizeWhitespace(element.Value) : null;
    }

    private static string NormalizeWhitespace(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        // Trim each line and join with single spaces
        var lines = text.Split('\n')
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrEmpty(line));

        return string.Join(" ", lines);
    }
}

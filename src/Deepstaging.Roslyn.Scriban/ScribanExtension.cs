// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Scriban;

/// <summary>
/// Represents a validated Scriban template file extension.
/// Only well-known extensions are accepted, ensuring consistency across the framework.
/// </summary>
public readonly struct ScribanExtension : IEquatable<ScribanExtension>
{
    /// <summary>
    /// All well-known Scriban template file extensions.
    /// </summary>
    private static readonly string[] KnownExtensions =
    [
        ".scriban", ".sbn",
        ".scriban-html", ".scriban-htm", ".sbn-html", ".sbn-htm", ".sbnhtml", ".sbnhtm",
        ".scriban-txt", ".sbn-txt", ".sbntxt",
        ".scriban-cs", ".sbn-cs", ".sbncs",
        ".liquid"
    ];

    /// <summary>Mixed Scriban and C# (<c>.scriban-cs</c>).</summary>
    public static readonly ScribanExtension CSharp = new(".scriban-cs");

    /// <summary>Plain Scriban script (<c>.scriban</c>).</summary>
    public static readonly ScribanExtension Scriban = new(".scriban");

    /// <summary>Mixed Scriban and HTML (<c>.scriban-html</c>).</summary>
    public static readonly ScribanExtension Html = new(".scriban-html");

    /// <summary>Mixed Scriban and text (<c>.scriban-txt</c>).</summary>
    public static readonly ScribanExtension Text = new(".scriban-txt");

    /// <summary>Liquid template (<c>.liquid</c>).</summary>
    public static readonly ScribanExtension Liquid = new(".liquid");

    /// <summary>
    /// Gets the file extension string (e.g., <c>".scriban-cs"</c>).
    /// </summary>
    public string Value { get; }

    private ScribanExtension(string value) => Value = value;

    /// <summary>
    /// Creates a <see cref="ScribanExtension"/> from a string, validating it against known extensions.
    /// </summary>
    /// <param name="extension">The file extension (e.g., <c>".scriban-cs"</c>).</param>
    /// <exception cref="ArgumentException">Thrown when the extension is not a known Scriban extension.</exception>
    public static ScribanExtension From(string extension)
    {
        if (!KnownExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException(
                $"'{extension}' is not a known Scriban extension. " +
                $"Valid extensions: {string.Join(", ", KnownExtensions)}",
                nameof(extension));

        return new ScribanExtension(extension);
    }

    /// <summary>
    /// Checks whether the given extension string is a known Scriban template extension.
    /// </summary>
    public static bool IsKnown(string extension) =>
        KnownExtensions.Any(ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Checks whether a filename or resource name ends with a known Scriban template extension.
    /// </summary>
    public static bool IsKnownSuffix(string name) =>
        KnownExtensions.Any(ext => name.EndsWith(ext, StringComparison.OrdinalIgnoreCase));

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <inheritdoc />
    public bool Equals(ScribanExtension other) =>
        string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ScribanExtension other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    /// <summary>Equality operator.</summary>
    public static bool operator ==(ScribanExtension left, ScribanExtension right) => left.Equals(right);

    /// <summary>Inequality operator.</summary>
    public static bool operator !=(ScribanExtension left, ScribanExtension right) => !left.Equals(right);
}
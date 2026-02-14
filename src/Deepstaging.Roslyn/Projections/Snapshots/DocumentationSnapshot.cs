// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Pipeline-safe snapshot of XML documentation extracted from a symbol.
/// All data is stored as strings, safe for use in incremental generator pipeline models.
/// </summary>
public sealed record DocumentationSnapshot : IEquatable<DocumentationSnapshot>
{
    /// <summary>
    /// Gets an empty documentation snapshot.
    /// </summary>
    public static DocumentationSnapshot Empty { get; } = new();

    /// <summary>
    /// Gets the summary text, or null if not present.
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// Gets the remarks text, or null if not present.
    /// </summary>
    public string? Remarks { get; init; }

    /// <summary>
    /// Gets the returns text, or null if not present.
    /// </summary>
    public string? Returns { get; init; }

    /// <summary>
    /// Gets the value text (for properties), or null if not present.
    /// </summary>
    public string? Value { get; init; }

    /// <summary>
    /// Gets the example text, or null if not present.
    /// </summary>
    public string? Example { get; init; }

    /// <summary>
    /// Gets all param elements as name-description pairs.
    /// </summary>
    public EquatableArray<ParamDocumentation> Params { get; init; } = [];

    /// <summary>
    /// Gets all typeparam elements as name-description pairs.
    /// </summary>
    public EquatableArray<ParamDocumentation> TypeParams { get; init; } = [];

    /// <summary>
    /// Gets all exception elements as type-description pairs.
    /// </summary>
    public EquatableArray<ExceptionDocumentation> Exceptions { get; init; } = [];

    /// <summary>
    /// Gets all seealso cref values.
    /// </summary>
    public EquatableArray<string> SeeAlso { get; init; } = [];

    /// <summary>
    /// Gets a value indicating whether documentation exists.
    /// </summary>
    public bool HasValue => Summary != null || Remarks != null || Returns != null;
}

/// <summary>
/// A parameter or type parameter documentation entry.
/// </summary>
public sealed record ParamDocumentation(string Name, string Description) : IEquatable<ParamDocumentation>;

/// <summary>
/// An exception documentation entry.
/// </summary>
public sealed record ExceptionDocumentation(string Type, string Description) : IEquatable<ExceptionDocumentation>;

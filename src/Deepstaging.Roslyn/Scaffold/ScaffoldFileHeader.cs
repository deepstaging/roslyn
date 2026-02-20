// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Scaffold;

/// <summary>
/// Represents the provenance header embedded at the top of scaffolded files.
/// Provides a standard format for tracking which package, version, and model hash
/// produced the file, enabling staleness detection and upgrade notifications.
/// </summary>
/// <remarks>
/// <para>
/// The header format is two lines at the top of the file:
/// <code>
/// // @deepstaging-web v0.1.0 hash:a3f8c2d1e5b7
/// // scaffold: api-client
/// </code>
/// </para>
/// <para>
/// Satellite libraries (e.g., Deepstaging.Web, Deepstaging.Web.Svelte) embed this header
/// when scaffolding user-owned files. The <see cref="Analyzers.ScaffoldFileAnalyzer"/> base class
/// uses <see cref="Parse"/> to extract the hash for staleness comparison.
/// </para>
/// </remarks>
public sealed record ScaffoldFileHeader
{
    /// <summary>
    /// The package identifier (e.g., "deepstaging-web", "deepstaging-web-svelte").
    /// </summary>
    public required string Package { get; init; }

    /// <summary>
    /// The library version at scaffold time (e.g., "0.1.0", "1.2.3-alpha.1").
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// The SHA-256 hash of the model that produced this file.
    /// </summary>
    public required string Hash { get; init; }

    /// <summary>
    /// The scaffold name identifying which scaffold produced this file (e.g., "api-types", "api-client").
    /// </summary>
    public required string ScaffoldName { get; init; }

    private const string Prefix = "// @";
    private const string HashMarker = "hash:";
    private const string ScaffoldMarker = "// scaffold: ";

    /// <summary>
    /// Formats the header as two lines to be prepended to scaffolded file content.
    /// </summary>
    public string Format() =>
        $"{Prefix}{Package} v{Version} {HashMarker}{Hash}\n{ScaffoldMarker}{ScaffoldName}";

    /// <summary>
    /// Attempts to parse a <see cref="ScaffoldFileHeader"/> from the beginning of file content.
    /// Returns <c>null</c> if the content does not contain a valid scaffold header.
    /// </summary>
    /// <param name="content">The full text content of a scaffolded file.</param>
    /// <returns>The parsed header, or <c>null</c> if parsing fails.</returns>
    public static ScaffoldFileHeader? Parse(string content)
    {
        if (string.IsNullOrEmpty(content))
            return null;

        // Split into first two lines
        var firstNewline = content.IndexOf('\n');
        if (firstNewline < 0)
            return null;

        var line1 = content.Substring(0, firstNewline).TrimEnd('\r');
        var rest = content.Substring(firstNewline + 1);
        var secondNewline = rest.IndexOf('\n');
        var line2 = secondNewline >= 0
            ? rest.Substring(0, secondNewline).TrimEnd('\r')
            : rest.TrimEnd('\r');

        // Line 1: // @{package} v{version} hash:{hash}
        if (!line1.StartsWith(Prefix, StringComparison.Ordinal))
            return null;

        var afterPrefix = line1.Substring(Prefix.Length);

        var hashIdx = afterPrefix.IndexOf(HashMarker, StringComparison.Ordinal);
        if (hashIdx < 0)
            return null;

        var hash = afterPrefix.Substring(hashIdx + HashMarker.Length).Trim();
        var beforeHash = afterPrefix.Substring(0, hashIdx).Trim();

        // beforeHash = "{package} v{version}"
        var vIdx = beforeHash.LastIndexOf(" v", StringComparison.Ordinal);
        if (vIdx < 0)
            return null;

        var package = beforeHash.Substring(0, vIdx);
        var version = beforeHash.Substring(vIdx + 2);

        if (string.IsNullOrEmpty(package) || string.IsNullOrEmpty(version) || string.IsNullOrEmpty(hash))
            return null;

        // Line 2: // scaffold: {name}
        if (!line2.StartsWith(ScaffoldMarker, StringComparison.Ordinal))
            return null;

        var scaffoldName = line2.Substring(ScaffoldMarker.Length).Trim();
        if (string.IsNullOrEmpty(scaffoldName))
            return null;

        return new ScaffoldFileHeader
        {
            Package = package,
            Version = version,
            Hash = hash,
            ScaffoldName = scaffoldName
        };
    }

    /// <summary>
    /// Extracts just the hash value from file content, or <c>null</c> if no valid header is found.
    /// Convenience method for <see cref="Analyzers.TrackedFileTypeAnalyzer"/> integration.
    /// </summary>
    /// <param name="content">The full text content of a scaffolded file.</param>
    /// <returns>The hash string, or <c>null</c>.</returns>
    public static string? ExtractHash(string content) => Parse(content)?.Hash;
}

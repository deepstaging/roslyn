// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Analyzers;

/// <summary>
/// Discovers and indexes additional files from the compiler, extracting embedded hashes for staleness tracking.
/// Used by <see cref="TrackedFileTypeAnalyzer"/> to check whether tracked files exist and whether they are stale.
/// </summary>
public sealed class TrackedFiles
{
    private readonly Dictionary<string, string?> _hashes;

    private TrackedFiles(Dictionary<string, string?> hashes) => _hashes = hashes;

    /// <summary>
    /// Creates a <see cref="TrackedFiles"/> instance by scanning additional texts for tracked files.
    /// </summary>
    /// <param name="additionalTexts">The additional texts provided by the compiler.</param>
    /// <param name="isTracked">Predicate that determines whether a file path is a tracked file.</param>
    /// <param name="extractHash">Function that extracts an embedded hash from file content, or returns <c>null</c> if none is found.</param>
    public static TrackedFiles Discover(
        ImmutableArray<AdditionalText> additionalTexts,
        Func<string, bool> isTracked,
        Func<string, string?> extractHash)
    {
        var hashes = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (var text in additionalTexts)
        {
            if (!isTracked(text.Path))
                continue;

            var fileName = Path.GetFileName(text.Path);
            var content = text.GetText()?.ToString();
            var hash = content is not null ? extractHash(content) : null;

            hashes[fileName] = hash;
        }

        return new TrackedFiles(hashes);
    }

    /// <summary>
    /// Whether a tracked file with the given name exists.
    /// </summary>
    public bool HasFile(string fileName) => _hashes.ContainsKey(fileName);

    /// <summary>
    /// Gets the embedded hash from a tracked file, or <c>null</c> if the file
    /// doesn't exist or has no embedded hash.
    /// </summary>
    public string? GetHash(string fileName) =>
        _hashes.TryGetValue(fileName, out var hash) ? hash : null;

    /// <summary>
    /// Whether any tracked files were found.
    /// </summary>
    public bool HasAny => _hashes.Count > 0;
}
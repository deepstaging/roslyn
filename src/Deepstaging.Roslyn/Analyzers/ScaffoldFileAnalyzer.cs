// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Analyzers;

using Scaffold;

/// <summary>
/// Base analyzer for tracking scaffolded files that use <see cref="ScaffoldFileHeader"/> provenance headers.
/// Extends <see cref="TrackedFileTypeAnalyzer"/> with automatic header parsing, so subclasses only need to
/// provide the package name, relevance check, hash computation, and expected file names.
/// </summary>
/// <remarks>
/// <para>
/// Use this as the base class for satellite scaffold analyzers. A typical implementation is ~15 lines:
/// </para>
/// <example>
/// <code>
/// [DiagnosticAnalyzer(LanguageNames.CSharp)]
/// [TracksFiles("DSWEB01",
///     MissingTitle = "Frontend assets can be scaffolded",
///     MissingMessage = "Class '{0}' can scaffold frontend assets: {1}",
///     StaleTitle = "Frontend assets may be out of date",
///     StaleMessage = "Frontend assets for '{0}' may be out of date ({1})")]
/// public sealed class WebAppScaffoldAnalyzer : ScaffoldFileAnalyzer
/// {
///     public const string DiagnosticId = "DSWEB01";
///     protected override string PackageName => "deepstaging-web";
///     protected override bool IsRelevant(ValidSymbol&lt;INamedTypeSymbol&gt; s) =>
///         s.HasAttribute&lt;WebAppAttribute&gt;();
///     protected override string ComputeHash(ValidSymbol&lt;INamedTypeSymbol&gt; s) =>
///         WebScaffoldHash.Compute(s);
///     protected override IEnumerable&lt;string&gt; GetExpectedFileNames(ValidSymbol&lt;INamedTypeSymbol&gt; s) =>
///         ["api-types.ts", "api-client.ts"];
/// }
/// </code>
/// </example>
/// </remarks>
public abstract class ScaffoldFileAnalyzer : TrackedFileTypeAnalyzer
{
    /// <summary>
    /// Gets the package identifier used in scaffold file headers (e.g., "deepstaging-web").
    /// Used to filter tracked files to only those belonging to this satellite.
    /// </summary>
    protected abstract string PackageName { get; }

    /// <summary>
    /// Determines whether a file name (not full path) is one this analyzer tracks.
    /// Override to customize file name matching beyond the default expected file names check.
    /// </summary>
    /// <param name="fileName">The file name (without directory path).</param>
    /// <returns><c>true</c> if this analyzer should track the file.</returns>
    protected virtual bool IsTrackedFileName(string fileName) => false;

    /// <inheritdoc />
    protected sealed override bool IsTrackedFile(string filePath)
    {
        var fileName = Path.GetFileName(filePath);

        if (IsTrackedFileName(fileName))
            return true;

        // Parse the file content isn't available here — match by file name convention.
        // TrackedFileTypeAnalyzer calls this to filter AdditionalTexts, so we need a
        // name-based check. We accept all files and let hash comparison handle the rest,
        // or subclasses can override IsTrackedFileName for efficient filtering.
        // Default: check if this looks like a scaffold file by its header once content is available.
        return HasScaffoldExtension(fileName);
    }

    /// <inheritdoc />
    protected sealed override string? ExtractHash(string fileContent) =>
        ScaffoldFileHeader.ExtractHash(fileContent);

    private static bool HasScaffoldExtension(string fileName) =>
        fileName.EndsWith(".ts", StringComparison.OrdinalIgnoreCase) ||
        fileName.EndsWith(".svelte", StringComparison.OrdinalIgnoreCase) ||
        fileName.EndsWith(".tsx", StringComparison.OrdinalIgnoreCase) ||
        fileName.EndsWith(".jsx", StringComparison.OrdinalIgnoreCase) ||
        fileName.EndsWith(".vue", StringComparison.OrdinalIgnoreCase) ||
        fileName.EndsWith(".css", StringComparison.OrdinalIgnoreCase);
}

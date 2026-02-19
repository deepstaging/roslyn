// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Reflection;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Roslyn.Analyzers;

/// <summary>
/// Base class for analyzers that track additional files associated with a type symbol,
/// reporting "missing" or "stale" diagnostics based on file existence and embedded hash comparison.
/// </summary>
/// <remarks>
/// <para>
/// Uses declarative configuration via <see cref="TracksFilesAttribute"/> to define the diagnostic metadata.
/// Both the "missing" and "stale" diagnostics share the same diagnostic ID so a single code fix can handle either.
/// </para>
/// <para>
/// The analyzer follows this pipeline:
/// <list type="number">
/// <item>On compilation start, discovers tracked files from <c>AdditionalTexts</c></item>
/// <item>Collects all <c>build_property.Deepstaging*</c> MSBuild properties from global options</item>
/// <item>For each type symbol, checks if it is relevant (e.g., has a specific attribute)</item>
/// <item>If any expected file is missing → reports the "missing" diagnostic</item>
/// <item>If all files are present but any has a stale hash → reports the "stale" diagnostic</item>
/// </list>
/// </para>
/// <para>
/// All <c>build_property.Deepstaging*</c> values are automatically forwarded into
/// <see cref="Diagnostic.Properties"/> (with the <c>build_property.</c> prefix stripped),
/// so code fixes can read them without additional wiring. To expose a new property,
/// add <c>&lt;CompilerVisibleProperty Include="DeepstagingMyProperty"/&gt;</c> to
/// the NuGet <c>.props</c> file.
/// </para>
/// </remarks>
public abstract class TrackedFileTypeAnalyzer : DiagnosticAnalyzer
{
    private readonly DiagnosticDescriptor _missingRule;
    private readonly DiagnosticDescriptor _staleRule;

    /// <summary>
    /// Gets the diagnostic descriptor for the "missing files" rule.
    /// </summary>
    protected DiagnosticDescriptor MissingRule => _missingRule;

    /// <summary>
    /// Gets the diagnostic descriptor for the "stale files" rule.
    /// </summary>
    protected DiagnosticDescriptor StaleRule => _staleRule;

    /// <inheritdoc />
    public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [_missingRule, _staleRule];

    /// <summary>
    /// Initializes a new instance of the <see cref="TrackedFileTypeAnalyzer"/> class.
    /// </summary>
    protected TrackedFileTypeAnalyzer()
    {
        var attr = GetType().GetCustomAttribute<TracksFilesAttribute>() ??
                   throw new InvalidOperationException(
                       $"Analyzer {GetType().Name} must have a [TracksFiles] attribute.");

        _missingRule = attr.ToMissingDescriptor();
        _staleRule = attr.ToStaleDescriptor();
    }

    /// <inheritdoc />
    public sealed override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        var files = TrackedFiles.Discover(
            [..context.Options.AdditionalFiles],
            IsTrackedFile,
            ExtractHash);

        var buildProperties = context.Options.AnalyzerConfigOptionsProvider.GlobalOptions
            .DiscoverBuildProperties();

        context.RegisterSymbolAction(
            ctx => AnalyzeSymbol(ctx, files, buildProperties),
            SymbolKind.NamedType);
    }

    private void AnalyzeSymbol(
        SymbolAnalysisContext context,
        TrackedFiles files,
        ImmutableDictionary<string, string?> buildProperties)
    {
        if (context.Symbol is not INamedTypeSymbol type)
            return;

        var valid = ValidSymbol<INamedTypeSymbol>.From(type);

        if (!IsRelevant(valid))
            return;

        var expectedFiles = GetExpectedFileNames(valid);
        var allPresent = expectedFiles.All(files.HasFile);

        if (!allPresent)
        {
            var location = GetLocation(valid);
            var messageArgs = GetMessageArgs(valid);
            context.ReportDiagnostic(Diagnostic.Create(_missingRule, location, buildProperties, messageArgs));
            return;
        }

        var currentHash = ComputeHash(valid);
        var anyStale = expectedFiles.Any(f => files.HasFile(f) && files.GetHash(f) != currentHash);

        if (anyStale)
        {
            var location = GetLocation(valid);
            var messageArgs = GetMessageArgs(valid);
            context.ReportDiagnostic(Diagnostic.Create(_staleRule, location, buildProperties, messageArgs));
        }
    }

    /// <summary>
    /// Determines whether the given file path represents a tracked file.
    /// Called during file discovery to filter <c>AdditionalTexts</c>.
    /// </summary>
    /// <param name="filePath">The full file path of an additional text.</param>
    /// <returns><c>true</c> if the file should be tracked; otherwise, <c>false</c>.</returns>
    protected abstract bool IsTrackedFile(string filePath);

    /// <summary>
    /// Determines whether the given type symbol should be analyzed for tracked files.
    /// Typically checks for the presence of a specific attribute.
    /// </summary>
    /// <param name="symbol">The validated type symbol.</param>
    /// <returns><c>true</c> if the symbol is relevant; otherwise, <c>false</c>.</returns>
    protected abstract bool IsRelevant(ValidSymbol<INamedTypeSymbol> symbol);

    /// <summary>
    /// Computes the current hash for the given symbol's model.
    /// This hash is compared against the embedded hash in existing files to detect staleness.
    /// </summary>
    /// <param name="symbol">The validated type symbol.</param>
    /// <returns>The computed hash string.</returns>
    protected abstract string ComputeHash(ValidSymbol<INamedTypeSymbol> symbol);

    /// <summary>
    /// Extracts an embedded hash from file content.
    /// Called during file discovery to read the hash from each tracked file.
    /// </summary>
    /// <param name="fileContent">The text content of a tracked file.</param>
    /// <returns>The extracted hash, or <c>null</c> if no valid hash is found.</returns>
    protected abstract string? ExtractHash(string fileContent);

    /// <summary>
    /// Returns the file names expected to exist for the given symbol.
    /// If any expected file is missing, the "missing" diagnostic is reported.
    /// If all files are present but any has a stale hash, the "stale" diagnostic is reported.
    /// </summary>
    /// <param name="symbol">The validated type symbol.</param>
    /// <returns>The expected file names (e.g., <c>["appsettings.schema.json", "secrets.schema.json"]</c>).</returns>
    protected abstract IEnumerable<string> GetExpectedFileNames(ValidSymbol<INamedTypeSymbol> symbol);

    /// <summary>
    /// Gets the message format arguments for the diagnostic.
    /// Default returns the symbol name as the first argument.
    /// </summary>
    /// <param name="symbol">The validated type symbol.</param>
    /// <returns>The message format arguments.</returns>
    protected virtual object[] GetMessageArgs(ValidSymbol<INamedTypeSymbol> symbol) => [symbol.Name];

    /// <summary>
    /// Gets the location for the diagnostic.
    /// Default returns the primary location of the symbol.
    /// </summary>
    /// <param name="symbol">The validated type symbol.</param>
    /// <returns>The diagnostic location.</returns>
    protected virtual Location GetLocation(ValidSymbol<INamedTypeSymbol> symbol) => symbol.Location;
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Deepstaging.Roslyn.Scaffold;

/// <summary>
/// Base class for scaffold code fix providers that write user-owned files with
/// <see cref="ScaffoldFileHeader"/> provenance headers.
/// </summary>
/// <remarks>
/// <para>
/// Extends <see cref="ProjectCodeFix{TSymbol}"/> with scaffold-specific helpers.
/// Use <see cref="ScaffoldFileActionsExtensions.WriteScaffold"/> in your file action chain
/// to automatically prepend the header with package name, version, and hash.
/// </para>
/// <example>
/// <code>
/// [Shared]
/// [CodeFix("DSWEB01")]
/// [ExportCodeFixProvider(LanguageNames.CSharp)]
/// public sealed class ScaffoldFrontendAssetsCodeFix : ScaffoldCodeFix&lt;INamedTypeSymbol&gt;
/// {
///     protected override string PackageName => "deepstaging-web";
///
///     protected override CodeAction? CreateFix(
///         Project project, ValidSymbol&lt;INamedTypeSymbol&gt; symbol, Diagnostic diagnostic)
///     {
///         var hash = WebScaffoldHash.Compute(symbol);
///         return project.FileActions("Scaffold frontend assets")
///             .WriteScaffold(PackageName, PackageVersion, "api-types.ts", "api-types", typesCode, hash)
///             .WriteScaffold(PackageName, PackageVersion, "api-client.ts", "api-client", clientCode, hash)
///             .ToCodeAction();
///     }
/// }
/// </code>
/// </example>
/// </remarks>
/// <typeparam name="TSymbol">The expected symbol type at the diagnostic location.</typeparam>
public abstract class ScaffoldCodeFix<TSymbol> : ProjectCodeFix<TSymbol>
    where TSymbol : class, ISymbol
{
    /// <summary>
    /// Gets the package identifier used in scaffold file headers (e.g., "deepstaging-web").
    /// </summary>
    protected abstract string PackageName { get; }

    /// <summary>
    /// Gets the package version from the assembly's <see cref="AssemblyInformationalVersionAttribute"/>.
    /// Falls back to <see cref="AssemblyName.Version"/> if the informational version is not set.
    /// </summary>
    protected string PackageVersion { get; } = ResolveVersion();

    private static string ResolveVersion()
    {
        var asm = Assembly.GetCallingAssembly();

        var infoVersion = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        if (infoVersion is { Length: > 0 })
        {
            // Strip metadata after '+' (e.g., "1.0.0+abc123" → "1.0.0")
            var plusIdx = infoVersion.IndexOf('+');
            return plusIdx >= 0 ? infoVersion.Substring(0, plusIdx) : infoVersion;
        }

        return asm.GetName().Version?.ToString(3) ?? "0.0.0";
    }
}

/// <summary>
/// Extension methods for <see cref="ProjectFileActionsBuilder"/> to support scaffold file writing.
/// </summary>
public static class ScaffoldFileActionsExtensions
{
    /// <summary>
    /// Writes a scaffolded file with a <see cref="ScaffoldFileHeader"/> prepended to the content.
    /// </summary>
    /// <param name="builder">The file actions builder.</param>
    /// <param name="packageName">The package identifier (e.g., "deepstaging-web").</param>
    /// <param name="packageVersion">The package version (e.g., "0.1.0").</param>
    /// <param name="relativePath">The relative file path (e.g., "api-types.ts").</param>
    /// <param name="scaffoldName">The scaffold identifier (e.g., "api-types").</param>
    /// <param name="content">The file content (without header).</param>
    /// <param name="hash">The computed model hash.</param>
    /// <returns>The builder for chaining.</returns>
    public static ProjectFileActionsBuilder WriteScaffold(
        this ProjectFileActionsBuilder builder,
        string packageName,
        string packageVersion,
        string relativePath,
        string scaffoldName,
        string content,
        string hash)
    {
        var header = new ScaffoldFileHeader
        {
            Package = packageName,
            Version = packageVersion,
            Hash = hash,
            ScaffoldName = scaffoldName
        };

        var fullContent = $"{header.Format()}\n\n{content}";

        return builder.Write(relativePath, fullContent);
    }
}

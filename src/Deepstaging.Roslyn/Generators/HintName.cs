// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Generators;

/// <summary>
/// Provides consistent hint name generation for source generation output files.
/// Hint names are used by Roslyn to uniquely identify generated files.
/// </summary>
/// <param name="root">The root namespace/path prefix for generated files.</param>
/// <param name="extension">The file extension for generated files (default is ".cs").</param>
public sealed class HintName(string root, string extension = ".cs")
{
    /// <summary>
    /// Creates a hint name by appending ".g.cs" to the name and prefixing with the root.
    /// </summary>
    /// <param name="name">The base name for the file.</param>
    /// <returns>A hint name in the format "{root}/{name}.g.cs".</returns>
    public string Filename(string name)
    {
        return $"{root}/{name}.g{extension}";
    }

    /// <summary>
    /// Creates a hint name with an additional path segment between root and name.
    /// </summary>
    /// <param name="append">Additional path segment to append after root.</param>
    /// <param name="name">The base name for the file.</param>
    /// <returns>A hint name in the format "{root}.{append}/{name}.g.cs".</returns>
    public string Filename(string append, string name)
    {
        var prefix = string.Join(".", new[] { root, append }.Where(x => x is not null));
        return $"{$"{prefix}/{name}".Replace(".g.cs", "")}.g{extension}";
    }
}

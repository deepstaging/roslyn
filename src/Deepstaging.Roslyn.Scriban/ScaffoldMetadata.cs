// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Microsoft.CodeAnalysis;

namespace Deepstaging.Roslyn.Scriban;

/// <summary>
/// Reads <see cref="ScaffoldInfo"/> entries from assembly metadata attributes in a compilation.
/// Used by analyzers and code fixes to discover available customizable templates.
/// </summary>
public static class ScaffoldMetadata
{
    /// <summary>
    /// Reads all scaffold metadata from the compilation's assembly attributes.
    /// </summary>
    /// <param name="compilation">The compilation to read from.</param>
    /// <returns>An array of discovered scaffold info entries.</returns>
    public static ImmutableArray<ScaffoldInfo> ReadFrom(Compilation compilation)
    {
        var assemblyMetadataType = compilation.GetTypeByMetadataName(
            "System.Reflection.AssemblyMetadataAttribute");
        if (assemblyMetadataType is null)
            return [];

        var builder = ImmutableArray.CreateBuilder<ScaffoldInfo>();
        var contentMap = new Dictionary<string, string>();
        var triggerMap = new Dictionary<string, string>();

        foreach (var attr in compilation.Assembly.GetAttributes())
        {
            if (!SymbolEqualityComparer.Default.Equals(attr.AttributeClass, assemblyMetadataType))
                continue;

            if (attr.ConstructorArguments.Length != 2)
                continue;

            var key = attr.ConstructorArguments[0].Value as string;
            var value = attr.ConstructorArguments[1].Value as string;
            if (key is null || value is null)
                continue;

            if (!key.StartsWith(ScaffoldEmitter.MetadataKeyPrefix, StringComparison.Ordinal))
                continue;

            var remainder = key.Substring(ScaffoldEmitter.MetadataKeyPrefix.Length);

            if (remainder.EndsWith(ScaffoldEmitter.ContentKeySuffix, StringComparison.Ordinal))
            {
                var templateName = remainder.Substring(
                    0, remainder.Length - ScaffoldEmitter.ContentKeySuffix.Length);
                contentMap[templateName] = value;
            }
            else
            {
                triggerMap[remainder] = value;
            }
        }

        foreach (var kvp in triggerMap)
        {
            contentMap.TryGetValue(kvp.Key, out var scaffold);
            builder.Add(new ScaffoldInfo(kvp.Key, kvp.Value, scaffold));
        }

        return builder.ToImmutable();
    }
}

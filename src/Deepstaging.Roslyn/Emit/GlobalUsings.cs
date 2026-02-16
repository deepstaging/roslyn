// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Emits a file containing <c>global using</c> directives.
/// </summary>
/// <example>
/// <code>
/// context.AddSource("MyGenerator.GlobalUsings.g.cs",
///     GlobalUsings.Emit(
///         JsonRefs.Namespace,
///         LoggingRefs.Namespace,
///         CollectionRefs.Namespace));
/// </code>
/// </example>
public static class GlobalUsings
{
    /// <summary>
    /// Emits <c>global using</c> directives for the specified namespaces.
    /// Supports static usings via the <c>"static Namespace"</c> convention (see <see cref="NamespaceRef.AsStatic"/>).
    /// </summary>
    /// <param name="namespaces">The namespaces to include as global usings.</param>
    public static string Emit(params string[] namespaces)
    {
        if (namespaces.Length == 0)
            return string.Empty;

        var builder = new StringBuilder();

        foreach (var ns in namespaces)
        {
            if (string.IsNullOrWhiteSpace(ns))
                continue;

            builder.AppendLine(ns.StartsWith("static ", StringComparison.Ordinal)
                ? $"global using static {ns.Substring(7)};"
                : $"global using {ns};");
        }

        return builder.ToString();
    }

    /// <summary>
    /// Emits <c>global using</c> directives for the specified namespace references.
    /// </summary>
    /// <param name="namespaces">The namespace references to include as global usings.</param>
    public static string Emit(params NamespaceRef[] namespaces)
    {
        var strings = new string[namespaces.Length];

        for (var i = 0; i < namespaces.Length; i++)
            strings[i] = namespaces[i].Value;

        return Emit(strings);
    }
}
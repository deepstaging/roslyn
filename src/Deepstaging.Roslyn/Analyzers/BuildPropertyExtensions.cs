// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Roslyn.Analyzers;

/// <summary>
/// Extension methods for reading MSBuild build properties from <see cref="AnalyzerConfigOptions"/>.
/// Handles the <c>build_property.</c> prefix automatically.
/// </summary>
/// <remarks>
/// <para>
/// MSBuild properties exposed via <c>&lt;CompilerVisibleProperty Include="MyProp"/&gt;</c>
/// appear in <see cref="AnalyzerConfigOptions"/> with a <c>build_property.</c> prefix.
/// These extensions strip that prefix from the caller's perspective:
/// </para>
/// <code>
/// // Instead of:
/// options.TryGetValue("build_property.DeepstagingDataDirectory", out var value);
///
/// // Write:
/// var value = options.GetBuildProperty("DeepstagingDataDirectory", ".config");
/// </code>
/// <para>
/// Use <c>DiscoverBuildProperties</c> to collect all
/// properties matching a prefix into an <see cref="ImmutableDictionary{TKey,TValue}"/>
/// suitable for forwarding via <see cref="Diagnostic.Properties"/>.
/// </para>
/// </remarks>
public static class BuildPropertyExtensions
{
    private const string Prefix = "build_property.";

    extension(AnalyzerConfigOptions options)
    {
        /// <summary>
        /// Gets a build property value, returning a fallback if the property is not set or empty.
        /// </summary>
        /// <param name="name">The property name without the <c>build_property.</c> prefix.</param>
        /// <param name="fallback">The value to return if the property is not set or empty.</param>
        /// <returns>The property value, or <paramref name="fallback"/>.</returns>
        public string GetBuildProperty(string name, string fallback)
        {
            options.TryGetValue(Prefix + name, out var value);
            return string.IsNullOrEmpty(value) ? fallback : value!;
        }

        /// <summary>
        /// Gets a build property value parsed as <typeparamref name="T"/>,
        /// returning a fallback if the property is not set, empty, or cannot be parsed.
        /// </summary>
        /// <typeparam name="T">
        /// The target type. Supported: <see cref="bool"/>, <see cref="int"/>,
        /// <see cref="long"/>, <see cref="double"/>.
        /// </typeparam>
        /// <param name="name">The property name without the <c>build_property.</c> prefix.</param>
        /// <param name="fallback">The value to return if the property is not set, empty, or cannot be parsed.</param>
        /// <returns>The parsed property value, or <paramref name="fallback"/>.</returns>
        public T GetBuildProperty<T>(string name, T fallback) where T : struct
        {
            options.TryGetValue(Prefix + name, out var raw);

            if (string.IsNullOrEmpty(raw))
                return fallback;

            return TryParse<T>(raw!, out var result) ? result : fallback;
        }

        /// <summary>
        /// Attempts to get a build property value.
        /// </summary>
        /// <param name="name">The property name without the <c>build_property.</c> prefix.</param>
        /// <param name="value">The property value if found; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the property exists and is non-empty; otherwise, <c>false</c>.</returns>
        public bool TryGetBuildProperty(string name, out string? value)
        {
            options.TryGetValue(Prefix + name, out value);
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Discovers all build properties whose names start with the specified prefix,
        /// returning them as an <see cref="ImmutableDictionary{TKey,TValue}"/> with the
        /// <c>build_property.</c> prefix stripped.
        /// </summary>
        /// <param name="prefix">
        /// The property name prefix to filter by (e.g., <c>"Deepstaging"</c>).
        /// Defaults to <c>"Deepstaging"</c>.
        /// </param>
        /// <returns>
        /// A dictionary mapping property names (without <c>build_property.</c> prefix)
        /// to their values. Suitable for forwarding via <see cref="Diagnostic.Properties"/>.
        /// </returns>
        public ImmutableDictionary<string, string?> DiscoverBuildProperties(string prefix = "Deepstaging")
        {
            var builder = ImmutableDictionary.CreateBuilder<string, string?>();

            foreach (var key in options.Keys)
            {
                if (!key.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                var propertyName = key.Substring(Prefix.Length);

                if (!propertyName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                options.TryGetValue(key, out var value);
                builder[propertyName] = value;
            }

            return builder.ToImmutable();
        }
    }

    private static bool TryParse<T>(string raw, out T result) where T : struct
    {
        if (typeof(T) == typeof(bool))
        {
            if (bool.TryParse(raw, out var b))
            {
                result = (T)(object)b;
                return true;
            }
        }
        else if (typeof(T) == typeof(int))
        {
            if (int.TryParse(raw, out var i))
            {
                result = (T)(object)i;
                return true;
            }
        }
        else if (typeof(T) == typeof(long))
        {
            if (long.TryParse(raw, out var l))
            {
                result = (T)(object)l;
                return true;
            }
        }
        else if (typeof(T) == typeof(double))
        {
            if (double.TryParse(raw, out var d))
            {
                result = (T)(object)d;
                return true;
            }
        }

        result = default;
        return false;
    }
}
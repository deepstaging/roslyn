// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Deepstaging.Roslyn.Testing;

/// <summary>
///     Extension methods for generic object manipulation and fluent configuration.
/// </summary>
public static class ObjectExtensions
{
    /// <param name="obj">The object to serialize.</param>
    /// <typeparam name="T">The type of the object.</typeparam>
    extension<T>(T obj)
    {
        /// <summary>
        ///     Serializes an object to a JSON string with customizable formatting and naming policies.
        ///     Provides a fluent interface for JSON serialization with sensible defaults and optional configuration.
        /// </summary>
        /// <param name="indented">Whether to format the JSON with indentation for readability. Default is true.</param>
        /// <param name="namingPolicy">
        ///     The naming policy to apply to property names. Default is CamelCase. Pass null to use
        ///     CamelCase.
        /// </param>
        /// <param name="configure">Optional action to further customize the JsonSerializerOptions before serialization.</param>
        /// <returns>A JSON string representation of the object.</returns>
        /// <example>
        ///     <code>
        /// var person = new { Name = "Alice", Age = 30 };
        /// 
        /// // Basic usage with defaults (camelCase, indented)
        /// var json = person.AsJsonString();
        /// // Output: "{\n  \"name\": \"Alice\",\n  \"age\": 30\n}"
        /// 
        /// // Single-line JSON without indentation
        /// var compact = person.AsJsonString(indented: false);
        /// // Output: "{\"name\":\"Alice\",\"age\":30}"
        /// 
        /// // Preserve original casing
        /// var pascalCase = person.AsJsonString(namingPolicy: null);
        /// 
        /// // Custom serialization options
        /// var custom = person.AsJsonString(configure: opts =>
        /// {
        ///     opts.WriteIndented = true;
        ///     opts.PropertyNameCaseInsensitive = true;
        /// });
        /// </code>
        /// </example>
        public string AsJsonString(bool indented = true, JsonNamingPolicy? namingPolicy = null,
            Action<JsonSerializerOptions>? configure = null)
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = indented,
                PropertyNamingPolicy = namingPolicy ?? JsonNamingPolicy.CamelCase,
                IgnoreReadOnlyProperties = true,
                IncludeFields = false
            };
            configure?.Invoke(options);
            return JsonSerializer.Serialize(obj, options);
        }

        /// <summary>
        ///     Serializes an object to a JSON string using the provided JsonTypeInfo.
        /// </summary>
        /// <param name="info">The JsonTypeInfo to use for serialization.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public string AsJson(JsonTypeInfo<T> info)
        {
            return JsonSerializer.Serialize(obj, info);
        }
    }
}
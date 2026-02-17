// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Text.Json;
using System.Text.Json.Nodes;

namespace Deepstaging.Roslyn;

/// <summary>
/// Provides deep merge operations for JSON documents.
/// Merges a template into an existing document, adding missing keys while preserving existing values.
/// </summary>
public static class JsonMerge
{
    private static readonly JsonDocumentOptions ParseOptions = new() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip };

    /// <summary>
    /// Deep merges a template JSON into an existing JSON document.
    /// Missing keys from the template are added; existing values are preserved.
    /// </summary>
    /// <param name="existing">The existing JSON content (user-edited file).</param>
    /// <param name="template">The template JSON content (generated structure with defaults).</param>
    /// <returns>The merged JSON string with indented formatting.</returns>
    public static string Apply(string existing, string template)
    {
        var existingNode = JsonNode.Parse(existing, documentOptions: ParseOptions);
        var templateNode = JsonNode.Parse(template);

        if (existingNode is JsonObject existingObj && templateNode is JsonObject templateObj)
        {
            MergeObjects(existingObj, templateObj);
            return Serialize(existingObj);
        }

        // If existing isn't a JSON object, return template as-is
        return template;
    }

    /// <summary>
    /// Synchronizes an existing JSON document with a template: adds missing keys, preserves existing values
    /// for matching keys, and removes keys that are not present in the template.
    /// Keys starting with <c>$</c> (e.g. <c>$schema</c>) are always preserved.
    /// </summary>
    /// <param name="existing">The existing JSON content (user-edited file).</param>
    /// <param name="template">The template JSON content (authoritative structure with defaults).</param>
    /// <returns>The synchronized JSON string with indented formatting.</returns>
    public static string Sync(string existing, string template)
    {
        var existingNode = JsonNode.Parse(existing, documentOptions: ParseOptions);
        var templateNode = JsonNode.Parse(template);

        if (existingNode is JsonObject existingObj && templateNode is JsonObject templateObj)
        {
            SyncObjects(existingObj, templateObj);
            return Serialize(existingObj);
        }

        return template;
    }

    private static void MergeObjects(JsonObject target, JsonObject source)
    {
        foreach (var property in source)
        {
            if (target.ContainsKey(property.Key))
            {
                // Existing key — recurse into nested objects, preserve everything else
                if (target[property.Key] is JsonObject targetChild && property.Value is JsonObject sourceChild)
                    MergeObjects(targetChild, sourceChild);
            }
            else
            {
                // Missing key — add from template
                target[property.Key] = property.Value?.DeepClone();
            }
        }
    }

    private static void SyncObjects(JsonObject target, JsonObject source)
    {
        // Remove keys not in template (except $-prefixed metadata keys)
        var keysToRemove = target
            .Select(p => p.Key)
            .Where(key => !key.StartsWith("$") && !source.ContainsKey(key))
            .ToList();

        foreach (var key in keysToRemove)
            target.Remove(key);

        // Add missing keys, recurse into nested objects
        foreach (var property in source)
        {
            if (target.ContainsKey(property.Key))
            {
                if (target[property.Key] is JsonObject targetChild && property.Value is JsonObject sourceChild)
                    SyncObjects(targetChild, sourceChild);
            }
            else
            {
                target[property.Key] = property.Value?.DeepClone();
            }
        }
    }

    private static string Serialize(JsonNode node) =>
        node.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
}

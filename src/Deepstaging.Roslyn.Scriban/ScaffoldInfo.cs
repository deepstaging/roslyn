// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Scriban;

/// <summary>
/// Represents a customizable template discovered from assembly metadata.
/// </summary>
/// <param name="TemplateName">The namespaced template name (e.g., "Deepstaging.Ids/StrongId").</param>
/// <param name="TriggerAttributeName">
/// The fully qualified name of the trigger attribute (e.g., "Deepstaging.Ids.StrongIdAttribute").
/// </param>
/// <param name="Scaffold">The scaffold template content, or null if not available.</param>
public readonly record struct ScaffoldInfo(
    string TemplateName,
    string TriggerAttributeName,
    string? Scaffold);
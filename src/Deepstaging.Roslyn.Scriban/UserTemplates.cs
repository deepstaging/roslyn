// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Scriban;

/// <summary>
/// Provides access to user-defined Scriban templates discovered from AdditionalTexts.
/// Templates are keyed by their namespaced path (e.g., "Deepstaging.Ids/StrongId").
/// </summary>
public sealed class UserTemplates
{
    /// <summary>
    /// An empty instance with no user templates.
    /// </summary>
    public static readonly UserTemplates Empty = new(ImmutableDictionary<string, string>.Empty);

    private readonly ImmutableDictionary<string, string> _templates;

    private UserTemplates(ImmutableDictionary<string, string> templates)
    {
        _templates = templates;
    }

    /// <summary>
    /// Creates a <see cref="UserTemplates"/> instance from AdditionalTexts provided by the compiler.
    /// Discovers templates following the convention: Templates/{ProjectPrefix}/{Name}.scriban-cs
    /// </summary>
    /// <param name="additionalTexts">Additional texts from the incremental generator pipeline.</param>
    public static UserTemplates From(ImmutableArray<AdditionalText> additionalTexts)
    {
        var builder = ImmutableDictionary.CreateBuilder<string, string>();

        foreach (var text in additionalTexts)
        {
            if (!text.Path.EndsWith(".scriban-cs", StringComparison.OrdinalIgnoreCase))
                continue;

            var name = ExtractTemplateName(text.Path);
            if (name is null)
                continue;

            var content = text.GetText()?.ToString();
            if (content is not null)
                builder[name] = content;
        }

        return new UserTemplates(builder.ToImmutable());
    }

    /// <summary>
    /// Attempts to render a user template with the given name and model.
    /// Returns null if no matching user template exists.
    /// </summary>
    /// <param name="templateName">The namespaced template name (e.g., "Deepstaging.Ids/StrongId").</param>
    /// <param name="model">The model object to pass to the Scriban template.</param>
    public RenderResult? TryRender(string templateName, object? model)
    {
        if (!_templates.TryGetValue(templateName, out var templateText))
            return null;

        return Template.RenderFromText(templateText, templateName, model);
    }

    /// <summary>
    /// Checks whether a user template exists for the given name.
    /// </summary>
    public bool HasTemplate(string templateName) => _templates.ContainsKey(templateName);

    /// <summary>
    /// Extracts the namespaced template name from a file path.
    /// Convention: Templates/{ProjectPrefix}/{Name}.scriban-cs → "{ProjectPrefix}/{Name}"
    /// </summary>
    private static string? ExtractTemplateName(string path)
    {
        // Normalize separators
        var normalized = path.Replace('\\', '/');

        const string marker = "Templates/";
        var idx = normalized.LastIndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
            return null;

        var relative = normalized[(idx + marker.Length)..];

        // Strip extension (.scriban-cs)
        const string extension = ".scriban-cs";
        if (relative.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            relative = relative[..^extension.Length];

        return relative.Length > 0 ? relative : null;
    }
}

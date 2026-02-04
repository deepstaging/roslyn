// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.Collections.Concurrent;
using System.Reflection;

namespace Deepstaging.Roslyn.Scriban;

/// <summary>
///     Manages template loading, parsing, and rendering via Scriban.
///     Templates are loaded as embedded resources from the assembly with automatic error handling
///     and context preparation for snake_case property access.
///     
///     Performance: Uses caching for both template text loading and Scriban parsing to minimize
///     repeated work during source generation. Critical for IDE responsiveness.
/// </summary>
[DebuggerDisplay("{Name}")]
public sealed record Template(TemplateName Name, object? Context = null)
{
    #region Caching Infrastructure
    
    // Cache embedded resource text by (Assembly, ResourceName)
    // Avoids re-reading streams for the same template
    private static readonly ConcurrentDictionary<(Assembly, string), string> TemplateTextCache = new();
    
    // Cache parsed Scriban templates by template text
    // Avoids re-parsing the same template multiple times (5-10ms saved per parse)
    // Key is the template text itself (string.GetHashCode is fast and collision-resistant for code)
    private static readonly ConcurrentDictionary<string, global::Scriban.Template> ParsedTemplateCache = new();
    
    #endregion

    /// <summary>
    /// Gets the name of the template.
    /// </summary>
    public TemplateName Name { get; } = Name;
    
    /// <summary>
    /// Gets the optional context object to be used during template rendering.
    /// </summary>
    public object? Context { get; } = Context;
    
    /// <summary>
    /// Gets the template text, using cache if available
    /// </summary>
    public string Text => TemplateTextCache.GetOrAdd((Name.Assembly, Name.Value), key =>
    {
        var (assembly, resourceName) = key;
        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
            throw new TemplateNotFoundException(resourceName);

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    });

    #region Public API

    /// <summary>
    ///     Renders a template with the given name and optional context.
    /// </summary>
    /// <param name="named">The name of the template to render</param>
    /// <param name="context">Optional render context (will be converted to ScriptObject if needed)</param>
    /// <returns>A RenderResult indicating success or failure</returns>
    public static RenderResult RenderTemplate(TemplateName named, object? context = null)
    {
        return RenderTemplate(new Template(named, context));
    }

    #endregion

    #region Private Render Logic

    private static RenderResult RenderTemplate(Template template)
    {
        return Render(template.Name, template.Text, template.Context);
    }

    /// <summary>
    ///     Core rendering logic: parses Scriban template and renders with context.
    ///     Uses caching for both template text and parsed templates.
    /// </summary>
    private static RenderResult Render(TemplateName name, string text, object? context = null)
    {
        try
        {
            // Get or parse the template (cache hit avoids 5-10ms parse time)
            var scribanTemplate = ParsedTemplateCache.GetOrAdd(text, t => global::Scriban.Template.Parse(t));
            if (scribanTemplate.HasErrors)
                return CreateParseErrorResult(name, scribanTemplate);

            var scriptObject = DeepstagingTemplateObject.From(context);
            return new RenderResult.Success(scribanTemplate.Render(scriptObject), context) { TemplateName = name.Value };
        }
        catch (Exception e)
        {
            var diagnostic = Diagnostic.Create(
                Rules.TemplateRenderError,
                null, e.GetType().Name, e.Message);

            return new RenderResult.Failure(diagnostic) { TemplateName = name.Value };
        }
    }

    /// <summary>
    ///     Creates a diagnostic result for template parsing errors.
    /// </summary>
    private static RenderResult CreateParseErrorResult(TemplateName name, global::Scriban.Template template)
    {
        var diagnostic = Diagnostic.Create(
            Rules.TemplateRenderError,
            null, "ParsingError",
            $"'{name}' has errors during parsing. Errors: {string.Join(", ", template.Messages)}");

        return new RenderResult.Failure(diagnostic) { TemplateName = name.Value };
    }

    #endregion
}

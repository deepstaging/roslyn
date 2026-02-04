// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.Reflection;
using System.Text;

namespace Deepstaging.Roslyn.Scriban;

/// <summary>
/// Exception thrown when a requested template cannot be found as an embedded resource.
/// Provides detailed information about available templates to aid debugging.
/// </summary>
/// <param name="name">The name of the template that could not be found.</param>
public sealed class TemplateNotFoundException(string name) : Exception
{
    /// <summary>
    /// Canonical Scriban template file extensions supported by the framework.
    /// </summary>
    private static readonly string[] TemplateExtensions =
    [
        // Plain scriban scripts
        ".scriban", ".sbn",
        // Mixed scriban and HTML
        ".scriban-html", ".scriban-htm", ".sbn-html", ".sbn-htm", ".sbnhtml", ".sbnhtm",
        // Mixed scriban and text
        ".scriban-txt", ".sbn-txt", ".sbntxt",
        // Mixed scriban and C#
        ".scriban-cs", ".sbn-cs", ".sbncs",
        // Liquid templates
        ".liquid"
    ];

    /// <summary>
    /// Gets a detailed error message listing available Scriban templates from loaded assemblies.
    /// Filters for all canonical Scriban template extensions including plain (.scriban, .sbn),
    /// HTML (.scriban-html, .sbn-html, .scriban-htm, .sbn-htm, .sbnhtml, .sbnhtm),
    /// text (.scriban-txt, .sbn-txt, .sbntxt), C# (.scriban-cs, .sbn-cs, .sbncs),
    /// and Liquid (.liquid) templates.
    /// </summary>
    public override string Message => BuildErrorMessage();

    private string BuildErrorMessage()
    {
        var availableTemplates = FindAvailableTemplates();
        
        var message = new StringBuilder()
            .Append($"Template '{name}' not found as an embedded resource. ");
        
        if (availableTemplates.Length > 0)
        {
            message.Append($"Available templates ({availableTemplates.Length}): ")
                   .Append(string.Join(", ", availableTemplates));
        }
        else
        {
            message.Append("No Scriban templates found in loaded assemblies. ")
                   .Append("Ensure templates are marked as EmbeddedResource in your .csproj.");
        }

        return message.ToString();
    }

    private static string[] FindAvailableTemplates() =>
    [
        ..AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(GetTemplateResourcesFromAssembly)
            .OrderBy(resourceName => resourceName)
    ];

    private static IEnumerable<string> GetTemplateResourcesFromAssembly(Assembly assembly)
    {
        try
        {
            return assembly.GetManifestResourceNames()
                .Where(IsTemplateResource);
        }
        catch
        {
            // Some assemblies may not allow resource enumeration
            return [];
        }
    }

    private static bool IsTemplateResource(string resourceName) =>
        TemplateExtensions.Any(ext => resourceName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
}

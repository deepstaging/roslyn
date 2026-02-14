// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Scriban;

/// <summary>
/// Assembly-level attribute emitted by generators to advertise that a customizable template
/// is available. Analyzers and code fixes use this to offer template scaffolding to users.
/// </summary>
/// <remarks>
/// This attribute is emitted by generators that use <see cref="CustomizableEmit"/> and
/// <see cref="TemplateMap{TModel}"/>. It carries the template name, trigger attribute type,
/// and an optional scaffold template that reproduces the default emit output.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class CustomizableTemplateAttribute : Attribute
{
    /// <summary>
    /// Gets the namespaced template name (e.g., "Deepstaging.Ids/StrongId").
    /// Matches the filesystem convention: Templates/{TemplateName}.scriban-cs
    /// </summary>
    public string TemplateName { get; }

    /// <summary>
    /// Gets the attribute type that triggers the generator (e.g., <c>typeof(StrongIdAttribute)</c>).
    /// Used by analyzers to identify types that support customizable templates.
    /// </summary>
    public Type TriggerAttribute { get; }

    /// <summary>
    /// Gets or sets the scaffold template content — a Scriban template that reproduces the
    /// default emit output with model property placeholders (e.g., <c>{{ TypeName }}</c>).
    /// Used by code fixes to create a starting-point template file for the user.
    /// </summary>
    public string? Scaffold { get; set; }

    /// <summary>
    /// Initializes a new instance with the template name and trigger attribute type.
    /// </summary>
    /// <param name="templateName">The namespaced template name.</param>
    /// <param name="triggerAttribute">The attribute type that triggers the generator.</param>
    public CustomizableTemplateAttribute(string templateName, Type triggerAttribute)
    {
        TemplateName = templateName;
        TriggerAttribute = triggerAttribute;
    }
}

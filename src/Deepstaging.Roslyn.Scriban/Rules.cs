// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Scriban;

#pragma warning disable RS2008 // Generators don't need analyzer release tracking

/// <summary>
///     Diagnostic rules for Scriban template rendering.
/// </summary>
public static class Rules
{
    /// <summary>
    ///     DEEPCORE001 - Template Rendering Error
    ///     Occurs when a Scriban template fails to render during source code generation.
    /// </summary>
    public static readonly DiagnosticDescriptor TemplateRenderError = new(
        "DEEPCORE001",
        "Template Rendering Error",
        "{0} occurred while rendering a template: {1}",
        "Deepstaging.CodeGeneration",
        DiagnosticSeverity.Error,
        true,
        "A template failed to render during source code generation. Check the error message and fix the template or context object.");
}

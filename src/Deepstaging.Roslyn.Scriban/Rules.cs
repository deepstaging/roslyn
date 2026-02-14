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

    /// <summary>
    ///     DSRK006 - User Template Rendered Invalid C#
    ///     Occurs when a user-provided Scriban template renders successfully but produces C# with syntax errors.
    /// </summary>
    public static readonly DiagnosticDescriptor UserTemplateInvalidCSharp = new(
        "DSRK006",
        "User template rendered invalid C#",
        "User template '{0}' rendered invalid C#: {1}",
        "CodeGeneration",
        DiagnosticSeverity.Error,
        true,
        "A user-provided Scriban template rendered successfully but the output is not valid C#. " +
        "Check the template for missing or malformed code.");

    /// <summary>
    ///     DSRK007 - User Template Scriban Error
    ///     Occurs when a user-provided Scriban template has parse or render errors.
    /// </summary>
    public static readonly DiagnosticDescriptor UserTemplateScribanError = new(
        "DSRK007",
        "User template Scriban error",
        "User template '{0}' has errors: {1}",
        "CodeGeneration",
        DiagnosticSeverity.Error,
        true,
        "A user-provided Scriban template has syntax or rendering errors. " +
        "Check the Scriban template syntax and ensure all referenced model properties exist.");
}
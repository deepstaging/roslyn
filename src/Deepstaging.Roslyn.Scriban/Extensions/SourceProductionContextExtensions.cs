// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Microsoft.CodeAnalysis.CSharp;

namespace Deepstaging.Roslyn.Scriban;

/// <summary>
/// Extension methods for Roslyn's source generation contexts to support template-based code generation.
/// Provides convenient methods for rendering Scriban templates and adding the results as generated source files.
/// </summary>
public static class SourceProductionContextExtensions
{
    extension(IncrementalGeneratorPostInitializationContext ctx)
    {
        /// <summary>
        /// Renders a Scriban template and adds the result as a generated source file.
        /// Used during the post-initialization phase of incremental generation.
        /// </summary>
        /// <param name="template">The template to render.</param>
        /// <param name="hintName">Unique hint name for the generated file (must be unique within the generator).</param>
        /// <param name="context">Optional data context to pass to the template for rendering.</param>
        /// <param name="format">If true, formats the generated C# code using Roslyn's syntax tree normalization.</param>
        /// <exception cref="InvalidOperationException">Thrown if template rendering fails.</exception>
        public void AddFromTemplate(TemplateName template, string hintName, object? context = null, bool format = false)
        {
            switch (Template.RenderTemplate(template, context))
            {
                case RenderResult.Success s:
                    var code = format ? FormatCode(s.Text) : s.Text;
                    ctx.AddSource(hintName, code);
                    break;
                case RenderResult.Failure f:
                    throw new InvalidOperationException(f.Diagnostic.GetMessage());
            }
        }
    }

    extension(SourceProductionContext ctx)
    {
        /// <summary>
        /// Renders a Scriban template and adds the result as a generated source file.
        /// Used during the source production phase with diagnostic reporting support.
        /// </summary>
        /// <param name="template">The template to render.</param>
        /// <param name="hintName">Unique hint name for the generated file (must be unique within the generator).</param>
        /// <param name="context">Optional data context to pass to the template for rendering.</param>
        /// <param name="diagnostics">Optional diagnostics to report. If any are present, generation is skipped.</param>
        /// <param name="format">If true, formats the generated C# code using Roslyn's syntax tree normalization.</param>
        public void AddFromTemplate(TemplateName template, string hintName, object? context = null,
            IReadOnlyList<Diagnostic>? diagnostics = null, bool format = false)
        {
            if (diagnostics is not null && diagnostics.Any())
            {
                foreach (var diagnostic in diagnostics)
                    ctx.ReportDiagnostic(diagnostic);

                return;
            }

            switch (Template.RenderTemplate(template, context))
            {
                case RenderResult.Success s:
                    var code = format ? FormatCode(s.Text) : s.Text;
                    ctx.AddSource(hintName, code);
                    break;
                case RenderResult.Failure f:
                    ctx.ReportDiagnostic(f.Diagnostic);
                    break;
            }
        }
    }

    /// <summary>
    /// Formats generated C# code using Roslyn's syntax tree normalization.
    /// This ensures consistent formatting (indentation, spacing, line breaks).
    /// Note: Roslyn follows standard C# formatting conventions, which may collapse
    /// manually-added line breaks for things like interface lists.
    /// </summary>
    private static string FormatCode(string code)
    {
        try
        {
            return CSharpSyntaxTree
                .ParseText(code)
                .GetRoot()
                .NormalizeWhitespace(indentation: "    ", eol: "\n")
                .ToFullString();
        }
        catch
        {
            // If formatting fails (e.g., invalid syntax), return original
            return code;
        }
    }
}

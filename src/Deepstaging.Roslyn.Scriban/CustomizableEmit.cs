// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Deepstaging.Roslyn.Scriban;

/// <summary>
/// Bridges <see cref="OptionalEmit"/> with user-customizable Scriban templates.
/// Wraps a default emit result and a template definition, resolving to a user-provided
/// template when available or falling back to the default emit.
/// </summary>
public readonly struct CustomizableEmit
{
    /// <summary>
    /// Gets the default emit result produced by the generator's writer.
    /// </summary>
    public OptionalEmit DefaultEmit { get; }

    /// <summary>
    /// Gets the namespaced template name (e.g., "Deepstaging.Ids/StrongId").
    /// </summary>
    public string TemplateName { get; }

    /// <summary>
    /// Gets the model object used for both the default emit and template rendering.
    /// </summary>
    public object? Model { get; }

    /// <summary>
    /// Gets the explicit property bindings recorded by a <see cref="TemplateMap{TModel}"/>.
    /// These define which model property values appear in the emit output and should become
    /// Scriban template placeholders when scaffolding.
    /// </summary>
    public IReadOnlyList<TemplateBinding> Bindings { get; }

    internal CustomizableEmit(
        OptionalEmit defaultEmit,
        string templateName,
        object? model,
        IReadOnlyList<TemplateBinding>? bindings = null)
    {
        DefaultEmit = defaultEmit;
        TemplateName = templateName;
        Model = model;
        Bindings = bindings ?? [];
    }

    /// <summary>
    /// Resolves this customizable emit against user templates.
    /// If a matching user template exists, renders it with the model and validates the output.
    /// Otherwise, returns the default emit unchanged.
    /// </summary>
    /// <param name="templates">User templates discovered from AdditionalTexts.</param>
    public OptionalEmit ResolveFrom(UserTemplates templates)
    {
        // Default emit must be valid — if not, report those diagnostics
        if (DefaultEmit.IsNotValid(out _))
            return DefaultEmit;

        var rendered = templates.TryRender(TemplateName, Model);

        if (rendered is null)
            return DefaultEmit;

        var location = CreateTemplateLocation(templates);

        return rendered switch
        {
            RenderResult.Success s => ValidateRenderedOutput(s.Text, location),
            RenderResult.Failure f => CreateScribanErrorResult(f, location),
            _ => DefaultEmit
        };
    }

    /// <summary>
    /// Resolves this customizable emit and adds the result as generated source.
    /// Convenience method combining <see cref="ResolveFrom"/> and AddSourceTo.
    /// </summary>
    /// <param name="ctx">The source production context.</param>
    /// <param name="filename">Unique hint name for the generated file.</param>
    /// <param name="templates">User templates discovered from AdditionalTexts.</param>
    public void AddSourceTo(SourceProductionContext ctx, string filename, UserTemplates templates)
    {
        var resolved = ResolveFrom(templates);

        if (resolved.IsNotValid(out var validCode))
        {
            foreach (var diagnostic in resolved.Diagnostics)
                ctx.ReportDiagnostic(diagnostic);

            return;
        }

        ctx.AddSource(filename, validCode.Code);
    }

    /// <summary>
    /// Validates rendered Scriban output as C# source code.
    /// Produces a DSRK006 diagnostic with the template name and aggregated C# errors
    /// instead of raw CS* diagnostics.
    /// </summary>
    private OptionalEmit ValidateRenderedOutput(string code, Location? templateLocation)
    {
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetCompilationUnitRoot();

        var errors = root.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToImmutableArray();

        if (!errors.Any())
            return OptionalEmit.FromSuccess(root, code);

        var errorSummary = string.Join("; ", errors.Select(d =>
            $"{d.Id} at {d.Location.GetLineSpan().StartLinePosition}: {d.GetMessage()}"));

        var diagnostic = Diagnostic.Create(
            Rules.UserTemplateInvalidCSharp,
            templateLocation ?? Location.None,
            TemplateName,
            errorSummary);

        return OptionalEmit.FromFailure([diagnostic]);
    }

    /// <summary>
    /// Creates a DSRK007 diagnostic for Scriban parse/render errors in user templates.
    /// </summary>
    private OptionalEmit CreateScribanErrorResult(RenderResult.Failure failure, Location? templateLocation)
    {
        var diagnostic = Diagnostic.Create(
            Rules.UserTemplateScribanError,
            templateLocation ?? Location.None,
            TemplateName,
            failure.Diagnostic.GetMessage());

        return OptionalEmit.FromFailure([diagnostic]);
    }

    /// <summary>
    /// Creates a <see cref="Location"/> pointing to the user template file, if the file path is known.
    /// </summary>
    private Location? CreateTemplateLocation(UserTemplates templates)
    {
        var filePath = templates.GetFilePath(TemplateName);

        if (filePath is null)
            return null;

        return Location.Create(
            filePath,
            TextSpan.FromBounds(0, 0),
            new LinePositionSpan(LinePosition.Zero, LinePosition.Zero));
    }
}
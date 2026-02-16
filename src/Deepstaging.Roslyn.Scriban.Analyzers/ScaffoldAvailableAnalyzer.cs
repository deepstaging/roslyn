// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Scriban;

/// <summary>
/// Reports an info diagnostic when a type uses a trigger attribute that has a customizable
/// template available, but no user template file has been created yet.
/// </summary>
/// <remarks>
/// <para>
/// This analyzer reads <c>[assembly: AssemblyMetadata("Deepstaging.Scaffold:...", "...")]</c>
/// attributes emitted by generators via <see cref="ScaffoldEmitter"/>. It then checks
/// <see cref="AnalyzerOptions.AdditionalFiles"/> for matching template files.
/// </para>
/// <para>
/// When a type has the trigger attribute but no template file exists, it reports
/// <c>DSRK005</c> as an informational diagnostic.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ScaffoldAvailableAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        ScaffoldDiagnostics.ScaffoldAvailable,
        "Customizable template available",
        "Type '{0}' supports a customizable template '{1}'. Add a template to override the default generated code.",
        "CodeGeneration",
        DiagnosticSeverity.Info,
        true,
        "A source generator for this type supports customizable templates via Scriban. " +
        "Create a template file to customize the generated output.");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        var scaffolds = ScaffoldMetadata.ReadFrom(context.Compilation);
        if (scaffolds.IsEmpty) return;

        // Determine which templates already have user files
        var userTemplates = UserTemplates.From(
            [..context.Options.AdditionalFiles]);

        // Only care about scaffolds that don't have user templates yet
        var uncovered = scaffolds
            .Where(s => !userTemplates.HasTemplate(s.TemplateName))
            .ToArray();

        if (uncovered.Length == 0) return;

        // Build a lookup from trigger attribute name → scaffold info
        var triggerLookup = new Dictionary<string, ScaffoldInfo>(StringComparer.Ordinal);

        foreach (var scaffold in uncovered)
            triggerLookup[scaffold.TriggerAttributeName] = scaffold;

        context.RegisterSymbolAction(ctx => AnalyzeSymbol(ctx, triggerLookup), SymbolKind.NamedType);
    }

    private static void AnalyzeSymbol(
        SymbolAnalysisContext context,
        Dictionary<string, ScaffoldInfo> triggerLookup)
    {
        // context.Symbol.AsNamedType()
        //     .GetAttributes()
        //     .Where(attr =>
        //     {
        //         
        //         return attr.AttributeClass.Name == "TemplateAttribute";
        //     })

        var type = (INamedTypeSymbol)context.Symbol;

        foreach (var attr in type.GetAttributes())
        {
            if (attr.AttributeClass is null) continue;

            var attrFullName = attr.AttributeClass.ToDisplayString(
                SymbolDisplayFormat.FullyQualifiedFormat);

            // Strip "global::" prefix for matching
            if (attrFullName.StartsWith("global::"))
                attrFullName = attrFullName.Substring("global::".Length);

            if (triggerLookup.TryGetValue(attrFullName, out var scaffold))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    type.Locations.FirstOrDefault() ?? Location.None,
                    ImmutableDictionary.CreateRange(
                    [
                        new KeyValuePair<string, string?>("TemplateName", scaffold.TemplateName),
                        new KeyValuePair<string, string?>("TriggerAttributeName", scaffold.TriggerAttributeName)
                    ]),
                    type.Name,
                    scaffold.TemplateName);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Reflection;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Roslyn.Analyzers;

/// <summary>
/// Base class for analyzers that scan assembly-level attributes and report diagnostics
/// for each extracted item. Handles <c>RegisterCompilationAction</c> boilerplate,
/// attribute filtering by metadata name, and <see cref="ValidAttribute"/> projection.
/// </summary>
/// <typeparam name="TItem">The model type extracted from each matching attribute.</typeparam>
/// <remarks>
/// <para>
/// Subclasses annotate with <see cref="ReportsAttribute"/> (one or more) to declare diagnostics,
/// then implement:
/// </para>
/// <list type="bullet">
/// <item><see cref="AttributeFullyQualifiedName"/> — the attribute to scan for</item>
/// <item><see cref="TryExtractItem"/> — extract a model from each matching attribute</item>
/// <item><see cref="Analyze"/> — evaluate each item and report diagnostics</item>
/// </list>
/// </remarks>
public abstract class AssemblyAttributeAnalyzer<TItem> : DiagnosticAnalyzer
{
    private readonly ImmutableArray<DiagnosticDescriptor> _rules;

    /// <inheritdoc />
    public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => _rules;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyAttributeAnalyzer{TItem}"/> class.
    /// </summary>
    protected AssemblyAttributeAnalyzer()
    {
        var reportsAttrs = GetType().GetCustomAttributes<ReportsAttribute>(false);
        var builder = ImmutableArray.CreateBuilder<DiagnosticDescriptor>();
        foreach (var attr in reportsAttrs)
            builder.Add(attr.ToDescriptor());

        if (builder.Count == 0)
            throw new InvalidOperationException(
                $"Analyzer {GetType().Name} must have at least one [Reports] attribute.");

        _rules = builder.ToImmutable();
    }

    /// <inheritdoc />
    public sealed override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationAction(AnalyzeCompilation);
    }

    private void AnalyzeCompilation(CompilationAnalysisContext context)
    {
        var attributeSymbol = context.Compilation.GetTypeByMetadataName(AttributeFullyQualifiedName);
        if (attributeSymbol == null)
            return;

        var items = ImmutableArray.CreateBuilder<TItem>();

        foreach (var attrData in context.Compilation.Assembly.GetAttributes())
        {
            if (!SymbolEqualityComparer.Default.Equals(attrData.AttributeClass, attributeSymbol))
                continue;

            var attr = ValidAttribute.From(attrData);
            var location = attrData.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? Location.None;

            if (TryExtractItem(attr, location, out var item))
                items.Add(item);
        }

        if (items.Count == 0)
            return;

        Analyze(context, items.ToImmutable());
    }

    /// <summary>
    /// Gets the fully qualified metadata name of the assembly attribute to scan for
    /// (e.g., <c>"MyApp.RequiresToolAttribute"</c>).
    /// </summary>
    protected abstract string AttributeFullyQualifiedName { get; }

    /// <summary>
    /// Extracts a model item from a matching assembly attribute.
    /// Return <c>false</c> to skip the attribute (e.g., if required data is missing).
    /// </summary>
    /// <param name="attribute">The validated attribute projection.</param>
    /// <param name="location">The source location of the attribute declaration.</param>
    /// <param name="item">The extracted model item.</param>
    /// <returns><c>true</c> if the item was extracted successfully; otherwise, <c>false</c>.</returns>
    protected abstract bool TryExtractItem(ValidAttribute attribute, Location location, out TItem item);

    /// <summary>
    /// Analyzes all extracted items and reports diagnostics.
    /// Access <see cref="CompilationAnalysisContext.Options"/> for additional files, global options, etc.
    /// Use the descriptors from <see cref="SupportedDiagnostics"/> indexed by position to match
    /// the <see cref="ReportsAttribute"/> declarations.
    /// </summary>
    /// <param name="context">The compilation analysis context for reporting diagnostics.</param>
    /// <param name="items">The extracted items from all matching assembly attributes.</param>
    protected abstract void Analyze(CompilationAnalysisContext context, ImmutableArray<TItem> items);

    /// <summary>
    /// Gets a supported diagnostic descriptor by index (matching the order of <see cref="ReportsAttribute"/> declarations).
    /// </summary>
    /// <param name="index">The zero-based index.</param>
    /// <returns>The diagnostic descriptor.</returns>
    protected DiagnosticDescriptor GetRule(int index) => _rules[index];
}

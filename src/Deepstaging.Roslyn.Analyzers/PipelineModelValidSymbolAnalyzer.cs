// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Analyzers;

/// <summary>
/// Reports <b>DSRK002</b> when a <c>[PipelineModel]</c> property uses <c>ValidSymbol&lt;T&gt;</c>,
/// which retains the entire Compilation and prevents garbage collection across edits.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(DiagnosticId, "Pipeline model property uses ValidSymbol<T>",
    Message = "Property '{0}' on pipeline model '{1}' uses ValidSymbol<T> which retains the Compilation — use a snapshot type instead",
    Category = "PipelineModel",
    Description = "ValidSymbol<T> holds a reference to ISymbol which retains the entire Compilation, preventing garbage collection across edits. Use TypeSnapshot, MethodSnapshot, or other snapshot types.")]
public sealed class PipelineModelValidSymbolAnalyzer : MultiDiagnosticTypeAnalyzer<ValidSymbol<IPropertySymbol>>
{
    /// <summary>Diagnostic ID for ValidSymbol usage in pipeline models.</summary>
    public const string DiagnosticId = "DSRK002";

    /// <inheritdoc />
    protected override IEnumerable<ValidSymbol<IPropertySymbol>> GetDiagnosticItems(ValidSymbol<INamedTypeSymbol> type)
    {
        if (type.LacksAttribute<PipelineModelAttribute>())
            yield break;

        var properties = type.QueryProperties()
            .ThatAreInstance()
            .Where(x => x.Type.IsValidSymbolType());

        foreach (var property in properties.GetAll())
            yield return property;
    }

    /// <inheritdoc />
    protected override object[] GetMessageArgs(ValidSymbol<INamedTypeSymbol> symbol, ValidSymbol<IPropertySymbol> item)
        => [item.Name, symbol.Name];

    /// <inheritdoc />
    protected override Location GetLocation(ValidSymbol<INamedTypeSymbol> symbol, ValidSymbol<IPropertySymbol> item)
        => item.Location;
}
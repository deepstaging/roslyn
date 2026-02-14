// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Analyzers;

/// <summary>
/// Reports <b>DSRK003</b> when a <c>[PipelineModel]</c> property uses an <c>ISymbol</c> type,
/// which retains the entire Compilation and prevents garbage collection.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(DiagnosticId, "Pipeline model property uses ISymbol",
    Message = "Property '{0}' on pipeline model '{1}' uses an ISymbol type which retains the Compilation — extract data during the projection step",
    Category = "PipelineModel",
    Description = "ISymbol types retain the entire Compilation in memory, preventing garbage collection. Extract the needed data into plain types during the projection step.")]
public sealed class PipelineModelSymbolAnalyzer : MultiDiagnosticTypeAnalyzer<ValidSymbol<IPropertySymbol>>
{
    /// <summary>Diagnostic ID for ISymbol usage in pipeline models.</summary>
    public const string DiagnosticId = "DSRK003";

    /// <inheritdoc />
    protected override IEnumerable<ValidSymbol<IPropertySymbol>> GetDiagnosticItems(ValidSymbol<INamedTypeSymbol> type)
    {
        if (type.LacksAttribute<PipelineModelAttribute>())
            yield break;

        var properties = type.QueryProperties()
            .ThatAreInstance()
            .Where(x => x.Type.IsRoslynSymbolType());

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
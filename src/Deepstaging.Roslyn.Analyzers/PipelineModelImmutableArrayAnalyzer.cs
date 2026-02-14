// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Analyzers;

/// <summary>
/// Reports <b>DSRK001</b> when a <c>[PipelineModel]</c> property uses <c>ImmutableArray&lt;T&gt;</c>,
/// which has reference equality and breaks incremental generator caching.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(DiagnosticId, "Pipeline model property uses ImmutableArray<T>",
    Message =
        "Property '{0}' on pipeline model '{1}' uses ImmutableArray<T> which has reference equality — use EquatableArray<T> instead",
    Category = "PipelineModel",
    Description =
        "ImmutableArray<T> uses reference equality in record types, breaking incremental generator caching. Replace with EquatableArray<T> which implements sequence equality.")]
public sealed class PipelineModelImmutableArrayAnalyzer : MultiDiagnosticTypeAnalyzer<ValidSymbol<IPropertySymbol>>
{
    /// <summary>Diagnostic ID for ImmutableArray usage in pipeline models.</summary>
    public const string DiagnosticId = "DSRK001";

    /// <inheritdoc />
    protected override IEnumerable<ValidSymbol<IPropertySymbol>> GetDiagnosticItems(ValidSymbol<INamedTypeSymbol> type)
    {
        if (type.LacksAttribute<PipelineModelAttribute>())
            yield break;

        var properties = type.QueryProperties()
            .ThatAreInstance()
            .Where(x => x.Type.IsImmutableArrayType());
        
        foreach (var property in properties.GetAll())
            yield return property;
    }

    /// <inheritdoc />
    protected override object[] GetMessageArgs(ValidSymbol<INamedTypeSymbol> symbol, ValidSymbol<IPropertySymbol> item)
    {
        return [item.Name, symbol.Name];
    }

    /// <inheritdoc />
    protected override Location GetLocation(ValidSymbol<INamedTypeSymbol> symbol, ValidSymbol<IPropertySymbol> item)
    {
        return item.Location;
    }
}
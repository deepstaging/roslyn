// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Analyzers;

/// <summary>
/// Reports <b>DSRK004</b> when a <c>[PipelineModel]</c> property type does not implement
/// <c>IEquatable&lt;T&gt;</c>, which breaks record equality and incremental generator caching.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(DiagnosticId, "Pipeline model property type lacks IEquatable<T>",
    Category = "PipelineModel",
    Severity = DiagnosticSeverity.Warning,
    Description =
        """
        Pipeline model properties must have types that implement IEquatable<T> for correct record equality.
        Types without IEquatable<T> fall back to reference equality, breaking incremental generator caching.
        """,
    Message =
        "Property '{0}' on pipeline model '{1}' has type '{2}' which does not implement IEquatable<T> — equality will be broken"
)]
public sealed class PipelineModelNonEquatableAnalyzer : MultiDiagnosticTypeAnalyzer<ValidSymbol<IPropertySymbol>>
{
    /// <summary>Diagnostic ID for non-IEquatable property types in pipeline models.</summary>
    public const string DiagnosticId = "DSRK004";

    /// <inheritdoc />
    protected override IEnumerable<ValidSymbol<IPropertySymbol>> GetDiagnosticItems(ValidSymbol<INamedTypeSymbol> type)
    {
        if (type.LacksAttribute<PipelineModelAttribute>())
            yield break;

        var properties = type.QueryProperties()
            .ThatAreInstance()
            .Where(x =>
                // Exclude types already reported by their own dedicated analyzers
                !x.Type.IsImmutableArrayType() // DSRK001
                &&
                !x.Type.IsValidSymbolType() // DSRK002
                &&
                !x.Type.IsRoslynSymbolType() // DSRK003

                // Report if the type does not implement IEquatable<T>
                &&
                !x.Type.ImplementsIEquatable());

        foreach (var property in properties.GetAll())
            yield return property;
    }

    /// <inheritdoc />
    protected override object[] GetMessageArgs(ValidSymbol<INamedTypeSymbol> symbol, ValidSymbol<IPropertySymbol> item) =>
    [
        item.Name, symbol.Name, item.Type.Value.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
    ];

    /// <inheritdoc />
    protected override Location GetLocation(ValidSymbol<INamedTypeSymbol> symbol, ValidSymbol<IPropertySymbol> item) =>
        item.Location;
}
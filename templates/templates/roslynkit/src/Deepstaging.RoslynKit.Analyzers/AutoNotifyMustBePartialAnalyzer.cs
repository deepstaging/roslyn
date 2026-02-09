// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.RoslynKit.Analyzers;

/// <summary>
/// Analyzer that reports a diagnostic when a class with [AutoNotify]
/// is not declared as partial.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(Diagnostics.AutoNotifyMustBePartial, "Type with [AutoNotify] must be partial",
    Message = "Type '{0}' has [AutoNotify] attribute but is not declared as partial",
    Description = "Types decorated with [AutoNotify] must be declared as partial to allow source generation of INotifyPropertyChanged implementation.",
    Category = "Usage")]
public sealed class AutoNotifyMustBePartialAnalyzer : TypeAnalyzer
{
    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type)
    {
        return type.HasAttribute<AutoNotifyAttribute>() && type is { IsPartial: false };
    }
}
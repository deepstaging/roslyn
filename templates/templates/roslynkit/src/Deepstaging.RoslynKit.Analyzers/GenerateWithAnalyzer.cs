// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.RoslynKit.Analyzers;

/// <summary>
/// Analyzer that reports a diagnostic when a class/struct with [GenerateWith]
/// is not declared as partial.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(Diagnostics.GenerateWithMustBePartial, "Type with [GenerateWith] must be partial",
    Message = "Type '{0}' has [GenerateWith] attribute but is not declared as partial",
    Description =
        "Types decorated with [GenerateWith] must be declared as partial to allow source generation of With*() methods.",
    Category = "Usage")]
public sealed class GenerateWithAnalyzer : TypeAnalyzer
{
    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type)
    {
        return type.HasAttribute<GenerateWithAttribute>() && type is { IsPartial: false };
    }
}
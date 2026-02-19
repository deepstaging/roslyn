// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.RoslynKit.Analyzers;

using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

/// <summary>Reports RK001 when a class with [AutoNotify] is not partial.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports("RK001", "Type must be partial",
    Message = "Type '{0}' has [AutoNotify] but is not declared partial",
    Category = "Usage")]
public sealed class MustBePartialAnalyzer : SymbolAnalyzer<INamedTypeSymbol>
{
    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> symbol) =>
        symbol.HasAttribute<AutoNotifyAttribute>() && !symbol.IsPartial;
}

/// <summary>Reports RK002 when a field in an [AutoNotify] class is not private.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports("RK002", "Field must be private",
    Message = "Field '{0}' in [AutoNotify] type must be private",
    Category = "Usage")]
public sealed class FieldMustBePrivateAnalyzer : SymbolAnalyzer<IFieldSymbol>
{
    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<IFieldSymbol> symbol) =>
        symbol.ContainingType.HasAttribute<AutoNotifyAttribute>() && !symbol.IsPrivate;
}
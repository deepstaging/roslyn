// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Analyzers;
using Deepstaging.RoslynKit.Projection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.RoslynKit.Analyzers;

/// <summary>
/// Analyzer that reports a diagnostic when a backing field in an [AutoNotify] class
/// is not declared as private.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(Diagnostics.AutoNotifyFieldMustBePrivate, "AutoNotify backing field should be private",
    Message = "Field '{0}' in type with [AutoNotify] should be declared as private",
    Description =
        "Backing fields in types decorated with [AutoNotify] should be private to ensure proper encapsulation.",
    Category = "Usage",
    Severity = DiagnosticSeverity.Warning)]
public sealed class AutoNotifyFieldMustBePrivateAnalyzer : FieldAnalyzer
{
    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<IFieldSymbol> field)
    {
        // Check if containing type has [AutoNotify]
        if (field.ContainingType.LacksAttribute<AutoNotifyAttribute>())
            return false;

        // Check if field follows backing field naming convention
        return BackingFieldConventions.IsBackingFieldName(field.Name) && !field.IsPrivate;
    }
}
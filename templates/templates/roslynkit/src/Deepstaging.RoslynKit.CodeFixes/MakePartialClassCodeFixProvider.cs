// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Composition;
using Deepstaging.Roslyn;
using Deepstaging.RoslynKit.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.RoslynKit.CodeFixes;

/// <summary>
/// Code fix provider that adds the 'partial' modifier to class types
/// for generators that require partial declarations.
/// </summary>
[Shared]
[CodeFix(Diagnostics.GenerateWithMustBePartial)]
[CodeFix(Diagnostics.AutoNotifyMustBePartial)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MakePartialClassCodeFixProvider))]
public sealed class MakePartialClassCodeFixProvider : ClassCodeFix
{
    /// <inheritdoc />
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax)
    {
        return document.AddPartialModifierAction(syntax);
    }
}
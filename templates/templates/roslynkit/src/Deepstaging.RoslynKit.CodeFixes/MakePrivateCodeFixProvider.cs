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
/// Code fix provider that changes field accessibility to private
/// for backing fields in [AutoNotify] classes.
/// </summary>
[Shared]
[CodeFix(Diagnostics.AutoNotifyFieldMustBePrivate)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MakePrivateCodeFixProvider))]
public sealed class MakePrivateCodeFixProvider : FieldCodeFix
{
    /// <inheritdoc />
    protected override CodeAction CreateFix(Document document, ValidSyntax<FieldDeclarationSyntax> syntax)
    {
        return document.MakeFieldPrivateAction(syntax);
    }
}
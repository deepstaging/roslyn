// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.RoslynKit.CodeFixes;

using Deepstaging.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>Adds the partial modifier to a class reported by RK001.</summary>
[CodeFix("RK001")]
public sealed class MakePartialCodeFix : SyntaxCodeFix<ClassDeclarationSyntax>
{
    /// <inheritdoc />
    protected override CodeAction? CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.AddPartialModifierAction(syntax);
}

/// <summary>Makes a field private when reported by RK002.</summary>
[CodeFix("RK002")]
public sealed class MakePrivateCodeFix : SyntaxCodeFix<FieldDeclarationSyntax>
{
    /// <inheritdoc />
    protected override CodeAction? CreateFix(Document document, ValidSyntax<FieldDeclarationSyntax> syntax) =>
        document.MakeFieldPrivateAction(syntax);
}
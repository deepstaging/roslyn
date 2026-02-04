// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Deepstaging.Roslyn;

/// <summary>
/// Base class for code fix providers that use declarative configuration via <see cref="CodeFixAttribute"/>.
/// </summary>
/// <typeparam name="TSyntax">The syntax node type this code fix operates on.</typeparam>
public abstract class SyntaxCodeFix<TSyntax> : CodeFixProvider
    where TSyntax : SyntaxNode
{
    /// <inheritdoc />
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; }

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <summary>
    /// Initializes a new instance of the <see cref="SyntaxCodeFix{TSyntax}"/> class.
    /// </summary>
    protected SyntaxCodeFix()
    {
        var codeFixAttrs = GetType()
            .GetCustomAttributes<CodeFixAttribute>()
            .ToArray();

        if (codeFixAttrs.Length == 0)
            throw new InvalidOperationException(
                $"CodeFix {GetType().Name} must have at least one [CodeFix] attribute.");

        FixableDiagnosticIds = [..codeFixAttrs.Select(a => a.DiagnosticId)];
    }

    /// <inheritdoc />
    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var result = await context.FindDeclaration<TSyntax>().ConfigureAwait(false);
        if (result.IsNotValid(out var syntax))
            return;

        var codeAction = CreateFix(context.Document, syntax);
        if (codeAction is null)
            return;

        context.RegisterCodeFix(codeAction, context.Diagnostics[0]);
    }

    /// <summary>
    /// Creates the code action that fixes the diagnostic.
    /// </summary>
    /// <param name="document">The document containing the diagnostic.</param>
    /// <param name="syntax">The validated syntax node at the diagnostic location.</param>
    /// <returns>A code action to fix the diagnostic, or null to skip.</returns>
    protected abstract CodeAction? CreateFix(Document document, ValidSyntax<TSyntax> syntax);
}

// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.Roslyn;

/// <summary>
/// Code fix action helpers for document-level operations (using directives, pragma, etc.).
/// </summary>
public static class DocumentCodeFixActions
{
    #region Using Directive Helpers

    /// <summary>
    /// Creates a code action that adds a using directive to the document.
    /// </summary>
    /// <param name="document">The document to modify.</param>
    /// <param name="namespaceName">The namespace to add (e.g., "System.Linq").</param>
    public static CodeAction AddUsingAction(this Document document, string namespaceName)
    {
        var title = $"Add 'using {namespaceName};'";

        return CodeAction.Create(
            title,
            ct => AddUsingDirectiveAsync(document, namespaceName, ct),
            title);
    }

    #endregion

    #region Pragma Suppression Helpers

    /// <summary>
    /// Creates a code action that suppresses a diagnostic with a pragma directive.
    /// </summary>
    /// <param name="document">The document to modify.</param>
    /// <param name="diagnostic">The diagnostic to suppress.</param>
    public static CodeAction SuppressWithPragmaAction(this Document document, Diagnostic diagnostic)
    {
        var title = $"Suppress '{diagnostic.Id}' with #pragma";

        return CodeAction.Create(
            title,
            ct => SuppressWithPragmaAsync(document, diagnostic, ct),
            title);
    }

    #endregion

    #region Private Helper Methods

    private static async Task<Document> AddUsingDirectiveAsync(
        Document document,
        string namespaceName,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        if (root is not CompilationUnitSyntax compilationUnit)
            return document;

        var usingDirective = SyntaxFactory.UsingDirective(
                SyntaxFactory.ParseName(namespaceName))
            .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        // Check if using already exists
        if (compilationUnit.Usings.Any(u => u.Name?.ToString() == namespaceName))
            return document;

        var newRoot = compilationUnit.AddUsings(usingDirective);
        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> SuppressWithPragmaAsync(
        Document document,
        Diagnostic diagnostic,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        if (root is null)
            return document;

        var diagnosticSpan = diagnostic.Location.SourceSpan;
        var token = root.FindToken(diagnosticSpan.Start);
        var node = token.Parent;

        // Find the statement or member containing the diagnostic
        var targetNode = node?.AncestorsAndSelf()
            .FirstOrDefault(n => n is StatementSyntax or MemberDeclarationSyntax);

        if (targetNode is null)
            return document;

        var leadingTrivia = targetNode.GetLeadingTrivia();
        var indentation = leadingTrivia.LastOrDefault(t => t.IsKind(SyntaxKind.WhitespaceTrivia));

        var pragmaDisable = SyntaxFactory.Trivia(
            SyntaxFactory.PragmaWarningDirectiveTrivia(
                SyntaxFactory.Token(SyntaxKind.DisableKeyword),
                SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                    SyntaxFactory.IdentifierName(diagnostic.Id)),
                true));

        var pragmaRestore = SyntaxFactory.Trivia(
            SyntaxFactory.PragmaWarningDirectiveTrivia(
                SyntaxFactory.Token(SyntaxKind.RestoreKeyword),
                SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                    SyntaxFactory.IdentifierName(diagnostic.Id)),
                true));

        var newLeadingTrivia = leadingTrivia
            .Add(pragmaDisable)
            .Add(SyntaxFactory.CarriageReturnLineFeed)
            .Add(indentation);

        var trailingTrivia = targetNode.GetTrailingTrivia()
            .Add(SyntaxFactory.CarriageReturnLineFeed)
            .Add(indentation)
            .Add(pragmaRestore);

        var newNode = targetNode
            .WithLeadingTrivia(newLeadingTrivia)
            .WithTrailingTrivia(trailingTrivia);

        var newRoot = root.ReplaceNode(targetNode, newNode);
        return document.WithSyntaxRoot(newRoot);
    }

    #endregion
}
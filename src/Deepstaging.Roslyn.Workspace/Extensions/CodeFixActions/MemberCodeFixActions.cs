// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.Roslyn;

/// <summary>
/// Code fix action helpers for member declarations (attributes, etc.).
/// </summary>
public static class MemberCodeFixActions
{
    extension(Document document)
    {
        #region Attribute Helpers

        /// <summary>
        /// Creates a code action that adds an attribute to a member declaration.
        /// </summary>
        /// <param name="memberDecl">The validated member declaration syntax.</param>
        /// <param name="attributeName">The attribute name (e.g., "Obsolete" or "System.Obsolete").</param>
        /// <param name="attributeArguments">Optional attribute arguments (e.g., "\"Use NewMethod instead\"").</param>
        public CodeAction AddAttributeAction<T>(
            ValidSyntax<T> memberDecl,
            string attributeName,
            string? attributeArguments = null)
            where T : MemberDeclarationSyntax
        {
            var shortName = attributeName.Contains('.')
                ? attributeName.Substring(attributeName.LastIndexOf('.') + 1)
                : attributeName;
            var title = $"Add [{shortName}] attribute";
            return CodeAction.Create(
                title,
                ct => AddAttributeAsync(document, memberDecl, attributeName, attributeArguments, ct),
                title);
        }

        /// <summary>
        /// Creates a code action that removes an attribute from a member declaration.
        /// </summary>
        /// <param name="memberDecl">The validated member declaration syntax.</param>
        /// <param name="attributeName">The attribute name to remove.</param>
        public CodeAction RemoveAttributeAction<T>(
            ValidSyntax<T> memberDecl,
            string attributeName)
            where T : MemberDeclarationSyntax
        {
            var shortName = attributeName.Contains('.')
                ? attributeName.Substring(attributeName.LastIndexOf('.') + 1)
                : attributeName;
            var title = $"Remove [{shortName}] attribute";
            return CodeAction.Create(
                title,
                ct => RemoveAttributeAsync(document, memberDecl, attributeName, ct),
                title);
        }

        #endregion
    }

    #region Private Helper Methods

    private static async Task<Document> AddAttributeAsync<T>(
        Document document,
        ValidSyntax<T> memberDecl,
        string attributeName,
        string? attributeArguments,
        CancellationToken cancellationToken)
        where T : MemberDeclarationSyntax
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
            return document;

        var node = memberDecl.Node;

        AttributeSyntax attribute;
        if (attributeArguments is not null)
        {
            attribute = SyntaxFactory.Attribute(
                SyntaxFactory.ParseName(attributeName),
                SyntaxFactory.ParseAttributeArgumentList($"({attributeArguments})"));
        }
        else
        {
            attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(attributeName));
        }

        // Get leading trivia from first existing attribute or the member itself
        var leadingTrivia = node.AttributeLists.Count > 0
            ? node.AttributeLists[0].GetLeadingTrivia()
            : node.GetLeadingTrivia();

        var attributeList = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(attribute))
            .WithLeadingTrivia(leadingTrivia)
            .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        MemberDeclarationSyntax newNode;
        if (node.AttributeLists.Count > 0)
        {
            // Remove leading trivia from first existing attribute to avoid duplication
            var firstAttr = node.AttributeLists[0].WithLeadingTrivia(SyntaxFactory.TriviaList());
            var updatedLists = node.AttributeLists.Replace(node.AttributeLists[0], firstAttr);
            newNode = node.WithAttributeLists(updatedLists.Insert(0, attributeList));
        }
        else
        {
            newNode = node
                .WithLeadingTrivia(SyntaxFactory.TriviaList())
                .WithAttributeLists(node.AttributeLists.Add(attributeList));
        }

        var newRoot = root.ReplaceNode(node, newNode);
        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> RemoveAttributeAsync<T>(
        Document document,
        ValidSyntax<T> memberDecl,
        string attributeName,
        CancellationToken cancellationToken)
        where T : MemberDeclarationSyntax
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
            return document;

        var node = memberDecl.Node;
        var shortName = attributeName.Contains('.')
            ? attributeName.Substring(attributeName.LastIndexOf('.') + 1)
            : attributeName;

        // Also match without "Attribute" suffix
        var nameWithoutSuffix = shortName.EndsWith("Attribute")
            ? shortName.Substring(0, shortName.Length - 9)
            : shortName;

        var newAttributeLists = new List<AttributeListSyntax>();
        foreach (var attrList in node.AttributeLists)
        {
            var remainingAttrs = attrList.Attributes
                .Where(a =>
                {
                    var name = a.Name.ToString();
                    return name != shortName &&
                           name != attributeName &&
                           name != nameWithoutSuffix;
                })
                .ToList();

            if (remainingAttrs.Count > 0)
            {
                newAttributeLists.Add(attrList.WithAttributes(SyntaxFactory.SeparatedList(remainingAttrs)));
            }
        }

        var newNode = node.WithAttributeLists(SyntaxFactory.List(newAttributeLists));
        var newRoot = root.ReplaceNode(node, newNode);
        return document.WithSyntaxRoot(newRoot);
    }

    #endregion
}

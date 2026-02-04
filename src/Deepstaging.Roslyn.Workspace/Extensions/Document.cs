// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Microsoft.CodeAnalysis;

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for Document - provides helpers for common code fix operations.
/// </summary>
public static class DocumentExtensions
{
    extension(Document document)
    {
        /// <summary>
        /// Replaces a syntax node in the document and returns the updated document.
        /// </summary>
        /// <typeparam name="TSyntax">The type of syntax node being replaced.</typeparam>
        /// <param name="oldNode">The node to replace.</param>
        /// <param name="newNode">The replacement node.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The updated document with the node replaced.</returns>
        public async Task<Document> ReplaceNode<TSyntax>(
            TSyntax oldNode,
            TSyntax newNode,
            CancellationToken token = default) where TSyntax : SyntaxNode
        {
            var root = await document.GetSyntaxRootAsync(token).ConfigureAwait(false);
            if (root is null)
                return document;

            var newRoot = root.ReplaceNode(oldNode, newNode);
            return document.WithSyntaxRoot(newRoot);
        }

        /// <summary>
        /// Replaces a syntax node in the document synchronously (when root is already available).
        /// </summary>
        /// <typeparam name="TSyntax">The type of syntax node being replaced.</typeparam>
        /// <param name="root">The syntax root of the document.</param>
        /// <param name="oldNode">The node to replace.</param>
        /// <param name="newNode">The replacement node.</param>
        /// <returns>The updated document with the node replaced.</returns>
        public Document ReplaceNode<TSyntax>(
            SyntaxNode root,
            TSyntax oldNode,
            TSyntax newNode) where TSyntax : SyntaxNode
        {
            var newRoot = root.ReplaceNode(oldNode, newNode);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}

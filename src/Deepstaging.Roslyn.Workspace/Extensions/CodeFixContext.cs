// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for CodeFixContext - provides syntax finding helpers for code fixes.
/// </summary>
public static class CodeFixContextExtensions
{
    extension(CodeFixContext context)
    {
        #region Generic Find Methods

        /// <summary>
        /// Finds a declaration syntax of the specified type from the first diagnostic's location.
        /// Searches the token at the diagnostic span and its ancestors.
        /// </summary>
        /// <typeparam name="TSyntax">The type of syntax node to find.</typeparam>
        /// <returns>An OptionalSyntax containing the found declaration, or empty if not found.</returns>
        public async Task<OptionalSyntax<TSyntax>> FindDeclaration<TSyntax>() where TSyntax : SyntaxNode
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root is null)
                return OptionalSyntax<TSyntax>.Empty();

            var span = context.Diagnostics[0].Location.SourceSpan;
            var node = root.FindToken(span.Start)
                .Parent?
                .AncestorsAndSelf()
                .OfType<TSyntax>()
                .FirstOrDefault();

            return OptionalSyntax<TSyntax>.FromNullable(node);
        }

        /// <summary>
        /// Finds a type declaration syntax from the first diagnostic's location.
        /// </summary>
        /// <typeparam name="TSyntax">The type of type declaration syntax to find.</typeparam>
        /// <returns>An OptionalSyntax that can be validated to a ValidTypeSyntax.</returns>
        public async Task<OptionalSyntax<TSyntax>> FindTypeDeclaration<TSyntax>() where TSyntax : TypeDeclarationSyntax
        {
            return await context.FindDeclaration<TSyntax>().ConfigureAwait(false);
        }

        #endregion

        #region Type Declaration Helpers

        /// <summary>
        /// Finds a class declaration from the first diagnostic's location.
        /// </summary>
        public async Task<OptionalSyntax<ClassDeclarationSyntax>> FindClass()
        {
            return await context.FindDeclaration<ClassDeclarationSyntax>().ConfigureAwait(false);
        }

        /// <summary>
        /// Finds a struct declaration from the first diagnostic's location.
        /// </summary>
        public async Task<OptionalSyntax<StructDeclarationSyntax>> FindStruct()
        {
            return await context.FindDeclaration<StructDeclarationSyntax>().ConfigureAwait(false);
        }

        /// <summary>
        /// Finds an interface declaration from the first diagnostic's location.
        /// </summary>
        public async Task<OptionalSyntax<InterfaceDeclarationSyntax>> FindInterface()
        {
            return await context.FindDeclaration<InterfaceDeclarationSyntax>().ConfigureAwait(false);
        }

        /// <summary>
        /// Finds a record declaration from the first diagnostic's location.
        /// </summary>
        public async Task<OptionalSyntax<RecordDeclarationSyntax>> FindRecord()
        {
            return await context.FindDeclaration<RecordDeclarationSyntax>().ConfigureAwait(false);
        }

        /// <summary>
        /// Finds an enum declaration from the first diagnostic's location.
        /// </summary>
        public async Task<OptionalSyntax<EnumDeclarationSyntax>> FindEnum()
        {
            return await context.FindDeclaration<EnumDeclarationSyntax>().ConfigureAwait(false);
        }

        #endregion

        #region Member Declaration Helpers

        /// <summary>
        /// Finds a method declaration from the first diagnostic's location.
        /// </summary>
        public async Task<OptionalSyntax<MethodDeclarationSyntax>> FindMethod()
        {
            return await context.FindDeclaration<MethodDeclarationSyntax>().ConfigureAwait(false);
        }

        /// <summary>
        /// Finds a property declaration from the first diagnostic's location.
        /// </summary>
        public async Task<OptionalSyntax<PropertyDeclarationSyntax>> FindProperty()
        {
            return await context.FindDeclaration<PropertyDeclarationSyntax>().ConfigureAwait(false);
        }

        /// <summary>
        /// Finds a field declaration from the first diagnostic's location.
        /// </summary>
        public async Task<OptionalSyntax<FieldDeclarationSyntax>> FindField()
        {
            return await context.FindDeclaration<FieldDeclarationSyntax>().ConfigureAwait(false);
        }

        /// <summary>
        /// Finds a constructor declaration from the first diagnostic's location.
        /// </summary>
        public async Task<OptionalSyntax<ConstructorDeclarationSyntax>> FindConstructor()
        {
            return await context.FindDeclaration<ConstructorDeclarationSyntax>().ConfigureAwait(false);
        }

        /// <summary>
        /// Finds an event declaration from the first diagnostic's location.
        /// </summary>
        public async Task<OptionalSyntax<EventDeclarationSyntax>> FindEvent()
        {
            return await context.FindDeclaration<EventDeclarationSyntax>().ConfigureAwait(false);
        }

        /// <summary>
        /// Finds a parameter from the first diagnostic's location.
        /// </summary>
        public async Task<OptionalSyntax<ParameterSyntax>> FindParameter()
        {
            return await context.FindDeclaration<ParameterSyntax>().ConfigureAwait(false);
        }

        #endregion
    }
}
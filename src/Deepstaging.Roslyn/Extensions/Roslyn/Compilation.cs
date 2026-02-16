// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for Compilation to query types.
/// </summary>
public static class CompilationQueryExtensions
{
    extension(Compilation compilation)
    {
        /// <summary>
        /// Creates a TypeQuery to search for types in the compilation.
        /// </summary>
        public TypeQuery QueryTypes() => TypeQuery.From(compilation);

        /// <summary>
        /// Gets the declared symbol at the location of a diagnostic.
        /// Useful in code fix providers to resolve the symbol that triggered a diagnostic.
        /// </summary>
        /// <param name="diagnostic">The diagnostic whose location identifies the symbol.</param>
        /// <returns>An OptionalSymbol containing the declared symbol, or empty if not found.</returns>
        public OptionalSymbol<ISymbol> GetSymbolAtDiagnostic(Diagnostic diagnostic)
        {
            var location = diagnostic.Location;

            if (location.SourceTree is null)
                return OptionalSymbol<ISymbol>.Empty();

            var semanticModel = compilation.GetSemanticModel(location.SourceTree);
            var root = location.SourceTree.GetRoot();
            var node = root.FindNode(location.SourceSpan);

            return OptionalSymbol<ISymbol>.FromNullable(semanticModel.GetDeclaredSymbol(node));
        }
    }
}
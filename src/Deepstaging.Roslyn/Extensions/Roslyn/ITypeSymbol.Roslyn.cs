// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for identifying Roslyn and Deepstaging symbol types.
/// </summary>
public static class TypeSymbolRoslynExtensions
{
    /// <param name="typeSymbol">The type symbol to check.</param>
    extension(ITypeSymbol typeSymbol)
    {
        /// <summary>
        /// Checks if the type is <c>Deepstaging.Roslyn.ValidSymbol&lt;T&gt;</c>.
        /// </summary>
        public bool IsValidSymbolType()
        {
            return typeSymbol is INamedTypeSymbol { IsGenericType: true, Name: "ValidSymbol", Arity: 1 } named
                   && named.ContainingNamespace.ToDisplayString() == "Deepstaging.Roslyn";
        }

        /// <summary>
        /// Checks if the type is or implements <c>Microsoft.CodeAnalysis.ISymbol</c>.
        /// Types that implement ISymbol retain the entire Compilation in memory.
        /// </summary>
        public bool IsRoslynSymbolType()
        {
            if (typeSymbol is { Name: "ISymbol", TypeKind: TypeKind.Interface }
                && typeSymbol.ContainingNamespace.ToDisplayString() == "Microsoft.CodeAnalysis")
                return true;

            foreach (var iface in typeSymbol.AllInterfaces)
            {
                if (iface is { Name: "ISymbol", TypeKind: TypeKind.Interface }
                    && iface.ContainingNamespace.ToDisplayString() == "Microsoft.CodeAnalysis")
                    return true;
            }

            return false;
        }
    }
}

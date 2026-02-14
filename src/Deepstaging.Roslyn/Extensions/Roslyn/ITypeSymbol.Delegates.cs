// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for identifying delegate and functional types (Func, Action, Expression).
/// </summary>
public static class TypeSymbolDelegateExtensions
{
    /// <param name="typeSymbol">The type symbol to check.</param>
    extension(ITypeSymbol typeSymbol)
    {
        /// <summary>
        /// Checks if the type is <c>Func&lt;...&gt;</c>.
        /// </summary>
        public bool IsFuncType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "Func", IsGenericType: true };
        }

        /// <summary>
        /// Checks if the type is <c>Action</c> or <c>Action&lt;...&gt;</c>.
        /// </summary>
        public bool IsActionType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "Action" };
        }

        /// <summary>
        /// Checks if the type is a delegate type.
        /// </summary>
        public bool IsDelegateType()
        {
            return typeSymbol.TypeKind == TypeKind.Delegate;
        }

        /// <summary>
        /// Checks if the type is <c>Expression&lt;T&gt;</c>.
        /// </summary>
        public bool IsExpressionType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "Expression", IsGenericType: true, TypeArguments.Length: 1 } named
                   && named.ContainingNamespace.ToDisplayString() == "System.Linq.Expressions";
        }
    }
}

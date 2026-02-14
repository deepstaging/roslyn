// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for identifying Span and Memory types.
/// </summary>
public static class TypeSymbolSpanMemoryExtensions
{
    /// <param name="typeSymbol">The type symbol to check.</param>
    extension(ITypeSymbol typeSymbol)
    {
        /// <summary>
        /// Checks if the type is <c>Span&lt;T&gt;</c>.
        /// </summary>
        public bool IsSpanType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "Span", IsGenericType: true, TypeArguments.Length: 1 } named
                   && named.ContainingNamespace.ToDisplayString() == "System";
        }

        /// <summary>
        /// Checks if the type is <c>ReadOnlySpan&lt;T&gt;</c>.
        /// </summary>
        public bool IsReadOnlySpanType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "ReadOnlySpan", IsGenericType: true, TypeArguments.Length: 1 } named
                   && named.ContainingNamespace.ToDisplayString() == "System";
        }

        /// <summary>
        /// Checks if the type is <c>Memory&lt;T&gt;</c>.
        /// </summary>
        public bool IsMemoryType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "Memory", IsGenericType: true, TypeArguments.Length: 1 } named
                   && named.ContainingNamespace.ToDisplayString() == "System";
        }

        /// <summary>
        /// Checks if the type is <c>ReadOnlyMemory&lt;T&gt;</c>.
        /// </summary>
        public bool IsReadOnlyMemoryType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "ReadOnlyMemory", IsGenericType: true, TypeArguments.Length: 1 } named
                   && named.ContainingNamespace.ToDisplayString() == "System";
        }
    }
}

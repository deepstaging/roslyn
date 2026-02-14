// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for checking type inheritance and interface implementation.
/// </summary>
public static class TypeSymbolInheritanceExtensions
{
    /// <param name="typeSymbol">The type symbol to check.</param>
    extension(ITypeSymbol typeSymbol)
    {
        /// <summary>
        /// Checks if the type implements or inherits from the specified base type.
        /// </summary>
        public bool ImplementsOrInheritsFrom(ITypeSymbol baseType)
        {
            if (SymbolEqualityComparer.Default.Equals(typeSymbol, baseType))
                return true;

            var current = typeSymbol.BaseType;
            while (current != null)
            {
                if (SymbolEqualityComparer.Default.Equals(current, baseType))
                    return true;
                current = current.BaseType;
            }

            return typeSymbol.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, baseType));
        }

        /// <summary>
        /// Checks if the type is or inherits from a type with the specified name.
        /// </summary>
        public bool IsOrInheritsFrom(string typeName, string? containingNamespace = null)
        {
            var current = typeSymbol;
            while (current != null)
            {
                if (current.Name == typeName &&
                    (containingNamespace == null || current.ContainingNamespace?.ToString() == containingNamespace))
                    return true;
                current = current.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Checks if the type inherits from a type with the specified name (excludes the type itself).
        /// </summary>
        public bool InheritsFrom(string typeName, string? containingNamespace = null)
        {
            var current = typeSymbol.BaseType;
            while (current != null)
            {
                if (current.Name == typeName &&
                    (containingNamespace == null || current.ContainingNamespace?.ToString() == containingNamespace))
                    return true;
                current = current.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Gets the base type with the specified name from the inheritance chain.
        /// Returns null if no matching base type is found.
        /// </summary>
        public ITypeSymbol? GetBaseTypeByName(string typeName)
        {
            var current = typeSymbol.BaseType;
            while (current != null)
            {
                if (current.Name == typeName)
                    return current;
                current = current.BaseType;
            }

            return null;
        }

        /// <summary>
        /// Extracts the first type argument from a generic type with a single type parameter.
        /// Returns null if not a generic type or doesn't have exactly one type argument.
        /// </summary>
        public ITypeSymbol? GetSingleTypeArgument()
        {
            return typeSymbol is INamedTypeSymbol { IsGenericType: true, TypeArguments.Length: 1 } namedType
                ? namedType.TypeArguments[0]
                : null;
        }
    }
}

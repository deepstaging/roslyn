// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for OptionalSymbol to start fluent query builders.
/// Provides seamless transition from projections to queries.
/// </summary>
public static class OptionalSymbolQueryExtensions
{
    extension<TSymbol>(OptionalSymbol<TSymbol> symbol) where TSymbol : class, ISymbol
    {
        /// <summary>
        /// Starts a fluent query for methods on this type symbol.
        /// Returns empty results if symbol is not a type or is empty.
        /// </summary>
        public MethodQuery QueryMethods()
        {
            return symbol.OrNull() is ITypeSymbol typeSymbol
                ? MethodQuery.From(typeSymbol)
                : MethodQuery.From(null!);
        }

        /// <summary>
        /// Starts a fluent query for properties on this type symbol.
        /// Returns empty results if symbol is not a type or is empty.
        /// </summary>
        public PropertyQuery QueryProperties()
        {
            return symbol.OrNull() is ITypeSymbol typeSymbol
                ? PropertyQuery.From(typeSymbol)
                : PropertyQuery.From(null!);
        }

        /// <summary>
        /// Starts a fluent query for constructors on this type symbol.
        /// Returns empty results if symbol is not a type or is empty.
        /// </summary>
        public ConstructorQuery QueryConstructors()
        {
            return symbol.OrNull() is ITypeSymbol typeSymbol
                ? ConstructorQuery.From(typeSymbol)
                : ConstructorQuery.From(null!);
        }

        /// <summary>
        /// Starts a fluent query for fields on this type symbol.
        /// Returns empty results if symbol is not a type or is empty.
        /// </summary>
        public FieldQuery QueryFields()
        {
            return symbol.OrNull() is ITypeSymbol typeSymbol
                ? FieldQuery.From(typeSymbol)
                : FieldQuery.From(null!);
        }

        /// <summary>
        /// Starts a fluent query for events on this type symbol.
        /// Returns empty results if symbol is not a type or is empty.
        /// </summary>
        public EventQuery QueryEvents()
        {
            return symbol.OrNull() is ITypeSymbol typeSymbol
                ? EventQuery.From(typeSymbol)
                : EventQuery.From(null!);
        }

        /// <summary>
        /// Starts a fluent query for parameters on this method symbol.
        /// Returns empty results if symbol is not a method or is empty.
        /// </summary>
        public ParameterQuery QueryParameters()
        {
            return symbol.OrNull() is IMethodSymbol methodSymbol
                ? ParameterQuery.From(methodSymbol)
                : ParameterQuery.From(null!);
        }
    }
}
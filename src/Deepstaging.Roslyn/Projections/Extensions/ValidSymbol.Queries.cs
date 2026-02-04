// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for ValidSymbol to start fluent query builders.
/// Provides seamless transition from validated projections to queries.
/// </summary>
public static class ValidSymbolQueryExtensions
{
    extension<TSymbol>(ValidSymbol<TSymbol> symbol) where TSymbol : class, ISymbol
    {
        /// <summary>
        /// Starts a fluent query for methods on this type symbol.
        /// Only works if symbol is a type; throws otherwise.
        /// </summary>
        public MethodQuery QueryMethods()
        {
            return symbol.Value is ITypeSymbol typeSymbol
                ? MethodQuery.From(typeSymbol)
                : throw new InvalidOperationException("Symbol is not a type symbol");
        }

        /// <summary>
        /// Starts a fluent query for properties on this type symbol.
        /// Only works if symbol is a type; throws otherwise.
        /// </summary>
        public PropertyQuery QueryProperties()
        {
            return symbol.Value is ITypeSymbol typeSymbol
                ? PropertyQuery.From(typeSymbol)
                : throw new InvalidOperationException("Symbol is not a type symbol");
        }

        /// <summary>
        /// Starts a fluent query for constructors on this type symbol.
        /// Only works if symbol is a type; throws otherwise.
        /// </summary>
        public ConstructorQuery QueryConstructors()
        {
            return symbol.Value is ITypeSymbol typeSymbol
                ? ConstructorQuery.From(typeSymbol)
                : throw new InvalidOperationException("Symbol is not a type symbol");
        }

        /// <summary>
        /// Starts a fluent query for fields on this type symbol.
        /// Only works if symbol is a type; throws otherwise.
        /// </summary>
        public FieldQuery QueryFields()
        {
            return symbol.Value is ITypeSymbol typeSymbol
                ? FieldQuery.From(typeSymbol)
                : throw new InvalidOperationException("Symbol is not a type symbol");
        }

        /// <summary>
        /// Starts a fluent query for events on this type symbol.
        /// Only works if symbol is a type; throws otherwise.
        /// </summary>
        public EventQuery QueryEvents()
        {
            return symbol.Value is ITypeSymbol typeSymbol
                ? EventQuery.From(typeSymbol)
                : throw new InvalidOperationException("Symbol is not a type symbol");
        }

        /// <summary>
        /// Starts a fluent query for parameters on this method symbol.
        /// Only works if symbol is a method; throws otherwise.
        /// </summary>
        public ParameterQuery QueryParameters()
        {
            return symbol.Value is IMethodSymbol methodSymbol
                ? ParameterQuery.From(methodSymbol)
                : throw new InvalidOperationException("Symbol is not a method symbol");
        }
    }
}
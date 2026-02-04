// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Safe casting extension methods for OptionalSymbol.
/// Provides fluent type conversion between different symbol types.
/// </summary>
public static class OptionalSymbolCastingExtensions
{
    extension<TSymbol>(OptionalSymbol<TSymbol> optional)
        where TSymbol : class, ISymbol
    {
        /// <summary>
        /// Casts to IMethodSymbol.
        /// Returns Empty if symbol is not a method or optional is empty.
        /// </summary>
        public OptionalSymbol<IMethodSymbol> AsMethod()
        {
            return optional.HasValue && optional.OrThrow() is IMethodSymbol method
                ? OptionalSymbol<IMethodSymbol>.WithValue(method)
                : OptionalSymbol<IMethodSymbol>.Empty();
        }

        /// <summary>
        /// Casts to IPropertySymbol.
        /// Returns Empty if symbol is not a property or optional is empty.
        /// </summary>
        public OptionalSymbol<IPropertySymbol> AsProperty()
        {
            return optional.HasValue && optional.OrThrow() is IPropertySymbol property
                ? OptionalSymbol<IPropertySymbol>.WithValue(property)
                : OptionalSymbol<IPropertySymbol>.Empty();
        }

        /// <summary>
        /// Casts to IFieldSymbol.
        /// Returns Empty if symbol is not a field or optional is empty.
        /// </summary>
        public OptionalSymbol<IFieldSymbol> AsField()
        {
            return optional.HasValue && optional.OrThrow() is IFieldSymbol field
                ? OptionalSymbol<IFieldSymbol>.WithValue(field)
                : OptionalSymbol<IFieldSymbol>.Empty();
        }

        /// <summary>
        /// Casts to IParameterSymbol.
        /// Returns Empty if symbol is not a parameter or optional is empty.
        /// </summary>
        public OptionalSymbol<IParameterSymbol> AsParameter()
        {
            return optional.HasValue && optional.OrThrow() is IParameterSymbol parameter
                ? OptionalSymbol<IParameterSymbol>.WithValue(parameter)
                : OptionalSymbol<IParameterSymbol>.Empty();
        }

        /// <summary>
        /// Casts to INamedTypeSymbol.
        /// Returns Empty if symbol is not a named type or optional is empty.
        /// </summary>
        public OptionalSymbol<INamedTypeSymbol> AsNamedType()
        {
            return optional.HasValue && optional.OrThrow() is INamedTypeSymbol namedType
                ? OptionalSymbol<INamedTypeSymbol>.WithValue(namedType)
                : OptionalSymbol<INamedTypeSymbol>.Empty();
        }

        /// <summary>
        /// Casts to ITypeSymbol.
        /// Returns Empty if symbol is not a type or optional is empty.
        /// </summary>
        public OptionalSymbol<ITypeSymbol> AsType()
        {
            return optional.HasValue && optional.OrThrow() is ITypeSymbol type
                ? OptionalSymbol<ITypeSymbol>.WithValue(type)
                : OptionalSymbol<ITypeSymbol>.Empty();
        }

        /// <summary>
        /// Casts to IEventSymbol.
        /// Returns Empty if symbol is not an event or optional is empty.
        /// </summary>
        public OptionalSymbol<IEventSymbol> AsEvent()
        {
            return optional.HasValue && optional.OrThrow() is IEventSymbol eventSymbol
                ? OptionalSymbol<IEventSymbol>.WithValue(eventSymbol)
                : OptionalSymbol<IEventSymbol>.Empty();
        }

        /// <summary>
        /// Casts to INamespaceSymbol.
        /// Returns Empty if symbol is not a namespace or optional is empty.
        /// </summary>
        public OptionalSymbol<INamespaceSymbol> AsNamespace()
        {
            return optional.HasValue && optional.OrThrow() is INamespaceSymbol ns
                ? OptionalSymbol<INamespaceSymbol>.WithValue(ns)
                : OptionalSymbol<INamespaceSymbol>.Empty();
        }

        /// <summary>
        /// Casts to ITypeParameterSymbol.
        /// Returns Empty if symbol is not a type parameter or optional is empty.
        /// </summary>
        public OptionalSymbol<ITypeParameterSymbol> AsTypeParameter()
        {
            return optional.HasValue && optional.OrThrow() is ITypeParameterSymbol typeParam
                ? OptionalSymbol<ITypeParameterSymbol>.WithValue(typeParam)
                : OptionalSymbol<ITypeParameterSymbol>.Empty();
        }

        /// <summary>
        /// Casts to ILocalSymbol.
        /// Returns Empty if symbol is not a local variable or optional is empty.
        /// </summary>
        public OptionalSymbol<ILocalSymbol> AsLocal()
        {
            return optional.HasValue && optional.OrThrow() is ILocalSymbol local
                ? OptionalSymbol<ILocalSymbol>.WithValue(local)
                : OptionalSymbol<ILocalSymbol>.Empty();
        }

        /// <summary>
        /// Attempts to cast to a specific symbol type and validate.
        /// Returns true if cast succeeds and sets the validated symbol.
        /// </summary>
        public bool TryCastAndValidate<TDerived>(out ValidSymbol<TDerived> validated)
            where TDerived : class, ISymbol
        {
            if (optional.HasValue && optional.OrThrow() is TDerived derived)
            {
                validated = ValidSymbol<TDerived>.From(derived);
                return true;
            }

            validated = default;
            return false;
        }

        /// <summary>
        /// Chains a cast and validation in a single call.
        /// Returns the validated cast symbol or Empty if cast fails.
        /// </summary>
        public OptionalSymbol<TDerived> CastToValid<TDerived>()
            where TDerived : class, ISymbol
        {
            if (optional.HasValue && optional.OrThrow() is TDerived derived)
                return OptionalSymbol<TDerived>.WithValue(derived);
            return OptionalSymbol<TDerived>.Empty();
        }
    }
}
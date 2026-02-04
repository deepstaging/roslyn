// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for ValidSymbol&lt;IPropertySymbol&gt; exposing IPropertySymbol-specific functionality.
/// </summary>
public static class ValidPropertySymbolExtensions
{
    extension(ValidSymbol<IPropertySymbol> property)
    {
        /// <summary>
        /// Gets the type of the property.
        /// Properties always have a type in Roslyn's object model.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> ReturnType =>
            property.Value.Type.AsNamedType().ValidateOrThrow();

        /// <summary>
        /// Gets the getter method of the property, if it exists.
        /// Returns Empty if property has no getter.
        /// </summary>
        public OptionalSymbol<IMethodSymbol> GetMethod =>
            property.Value.GetMethod is not null
                ? OptionalSymbol<IMethodSymbol>.WithValue(property.Value.GetMethod)
                : OptionalSymbol<IMethodSymbol>.Empty();

        /// <summary>
        /// Gets the setter method of the property, if it exists.
        /// Returns Empty if property has no setter.
        /// </summary>
        public OptionalSymbol<IMethodSymbol> SetMethod =>
            property.Value.SetMethod is not null
                ? OptionalSymbol<IMethodSymbol>.WithValue(property.Value.SetMethod)
                : OptionalSymbol<IMethodSymbol>.Empty();
    }
}

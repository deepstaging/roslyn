// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for OptionalSymbol&lt;IPropertySymbol&gt; exposing IPropertySymbol-specific functionality.
/// Note: Most properties (IsReadOnly, IsVirtual, etc.) are accessible via .Symbol
/// These extensions focus on complex operations and type conversions.
/// </summary>
public static class ProjectedPropertySymbolExtensions
{
    extension(OptionalSymbol<IPropertySymbol> property)
    {
        /// <summary>
        /// Gets the type of the property as a projected symbol.
        /// Returns Empty if property symbol is not present, otherwise returns the property type (which is always present for valid properties).
        /// </summary>
        public OptionalSymbol<ITypeSymbol> ReturnType =>
            property.HasValue
                ? OptionalSymbol<ITypeSymbol>.WithValue(property.Symbol!.Type)
                : OptionalSymbol<ITypeSymbol>.Empty();

        /// <summary>
        /// Gets the getter method of the property, if it exists.
        /// Returns Empty if property has no getter.
        /// </summary>
        public OptionalSymbol<IMethodSymbol> GetMethod =>
            property is { HasValue: true, Symbol.GetMethod: not null }
                ? OptionalSymbol<IMethodSymbol>.WithValue(property.Symbol!.GetMethod)
                : OptionalSymbol<IMethodSymbol>.Empty();

        /// <summary>
        /// Gets the setter method of the property, if it exists.
        /// Returns Empty if property has no setter.
        /// </summary>
        public OptionalSymbol<IMethodSymbol> SetMethod =>
            property is { HasValue: true, Symbol.SetMethod: not null }
                ? OptionalSymbol<IMethodSymbol>.WithValue(property.Symbol!.SetMethod)
                : OptionalSymbol<IMethodSymbol>.Empty();

        /// <summary>
        /// Gets the property that is overridden by this property, if this is an override.
        /// Returns Empty if not an override or overridden property is null.
        /// </summary>
        public OptionalSymbol<IPropertySymbol> OverriddenProperty =>
            property is { HasValue: true, Symbol.OverriddenProperty: not null }
                ? OptionalSymbol<IPropertySymbol>.WithValue(property.Symbol!.OverriddenProperty)
                : OptionalSymbol<IPropertySymbol>.Empty();
    }
}
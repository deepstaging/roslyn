// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for ValidSymbol&lt;IParameterSymbol&gt; exposing IParameterSymbol-specific functionality.
/// </summary>
public static class ValidParameterSymbolExtensions
{
    extension(ValidSymbol<IParameterSymbol> parameter)
    {
        /// <summary>
        /// Gets the type of the parameter as a validated projected symbol.
        /// Parameters always have a type, so this returns a non-nullable ValidSymbol.
        /// </summary>
        public ValidSymbol<ITypeSymbol> Type =>
            ValidSymbol<ITypeSymbol>.From(parameter.Value.Type);

        /// <summary>
        /// Gets the default value of an optional parameter.
        /// Returns Empty if parameter has no explicit default value.
        /// </summary>
        public OptionalValue<object?> ExplicitDefaultValue =>
            parameter is { Value.HasExplicitDefaultValue: true }
                ? OptionalValue<object?>.WithValue(parameter.Value.ExplicitDefaultValue)
                : OptionalValue<object?>.Empty();
        
        /// <summary>
        /// 
        /// </summary>
        public bool HasExplicitDefaultValue =>
            parameter.Value.HasExplicitDefaultValue;
    }
}

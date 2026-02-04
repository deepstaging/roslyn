// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for OptionalSymbol&lt;IParameterSymbol&gt; exposing IParameterSymbol-specific functionality.
/// Note: Most properties (IsRef, IsOut, IsParams, IsOptional, etc.) are accessible via .Symbol
/// These extensions focus on getting default values and type conversions.
/// </summary>
public static class ProjectedParameterSymbolExtensions
{
    extension(OptionalSymbol<IParameterSymbol> parameter)
    {
        /// <summary>
        /// Gets the type of the parameter as a projected symbol.
        /// Returns Empty if parameter symbol is not present, otherwise returns the parameter type (which is always present for valid parameters).
        /// </summary>
        public OptionalSymbol<ITypeSymbol> Type =>
            parameter.HasValue
                ? OptionalSymbol<ITypeSymbol>.WithValue(parameter.Symbol!.Type)
                : OptionalSymbol<ITypeSymbol>.Empty();

        /// <summary>
        /// Gets the default value of an optional parameter.
        /// Returns Empty if parameter has no explicit default value.
        /// </summary>
        public OptionalArgument<object?> ExplicitDefaultValue =>
            parameter is { HasValue: true, Symbol.HasExplicitDefaultValue: true }
                ? OptionalArgument<object?>.WithValue(parameter.Symbol!.ExplicitDefaultValue)
                : OptionalArgument<object?>.Empty();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string? DefaultValueString() =>
            parameter.Symbol!.HasExplicitDefaultValue
                ? parameter.Symbol.ExplicitDefaultValue switch
                {
                    null => "default",
                    string s => $"\"{s}\"",
                    bool b => b ? "true" : "false",
                    var value => value.ToString()
                }
                : null;
    }
}
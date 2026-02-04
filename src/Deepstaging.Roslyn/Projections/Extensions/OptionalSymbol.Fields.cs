// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for OptionalSymbol&lt;IFieldSymbol&gt; exposing IFieldSymbol-specific functionality.
/// Note: Most properties (IsConst, IsReadOnly, IsVolatile, etc.) are accessible via .Symbol
/// These extensions focus on complex operations that aren't trivially accessible.
/// </summary>
public static class ProjectedFieldSymbolExtensions
{
    extension(OptionalSymbol<IFieldSymbol> field)
    {
        /// <summary>
        /// Gets the type of the field as a projected symbol.
        /// Returns Empty if field symbol is not present, otherwise returns the field type (which is always present for valid fields).
        /// </summary>
        public OptionalSymbol<ITypeSymbol> Type =>
            @field.HasValue
                ? OptionalSymbol<ITypeSymbol>.WithValue(@field.Symbol!.Type)
                : OptionalSymbol<ITypeSymbol>.Empty();

        /// <summary>
        /// Gets the type of the field as a projected symbol.
        /// Returns Empty if field symbol is not present, otherwise returns the field type (which is always present for valid fields).
        /// </summary>
        public OptionalSymbol<ITypeSymbol> ReturnType =>
            @field.HasValue
                ? OptionalSymbol<ITypeSymbol>.WithValue(@field.Symbol!.Type)
                : OptionalSymbol<ITypeSymbol>.Empty();

        /// <summary>
        /// Gets the constant value of a const field.
        /// Returns Empty if field is not const or has no value.
        /// </summary>
        public OptionalArgument<object?> ConstantValue =>
            @field is { HasValue: true, Symbol.HasConstantValue: true }
                ? OptionalArgument<object?>.WithValue(@field.Symbol!.ConstantValue)
                : OptionalArgument<object?>.Empty();

        /// <summary>
        /// Gets the associated symbol (property or event) for compiler-generated backing fields.
        /// Returns Empty if field has no associated symbol.
        /// </summary>
        public OptionalSymbol<ISymbol> AssociatedSymbol =>
            @field is { HasValue: true, Symbol.AssociatedSymbol: not null }
                ? OptionalSymbol<ISymbol>.WithValue(@field.Symbol!.AssociatedSymbol)
                : OptionalSymbol<ISymbol>.Empty();
    }
}
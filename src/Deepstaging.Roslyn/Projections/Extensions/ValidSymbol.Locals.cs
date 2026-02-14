// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for ValidSymbol&lt;ILocalSymbol&gt; exposing ILocalSymbol-specific functionality.
/// </summary>
public static class ValidLocalSymbolExtensions
{
    extension(ValidSymbol<ILocalSymbol> local)
    {
        /// <summary>
        /// Gets the type of the local variable as a validated projected symbol.
        /// Locals always have a type, so this returns a non-nullable ValidSymbol.
        /// </summary>
        public ValidSymbol<ITypeSymbol> Type =>
            ValidSymbol<ITypeSymbol>.From(local.Value.Type);
    }
}

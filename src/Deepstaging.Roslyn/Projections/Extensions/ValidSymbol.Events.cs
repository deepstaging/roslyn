// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for ValidSymbol&lt;IEventSymbol&gt; exposing IEventSymbol-specific functionality.
/// </summary>
public static class ValidEventSymbolExtensions
{
    extension(ValidSymbol<IEventSymbol> @event)
    {
        /// <summary>
        /// Gets the type of the event as a validated projected symbol.
        /// Events always have a type, so this returns a non-nullable ValidSymbol.
        /// </summary>
        public ValidSymbol<ITypeSymbol> Type =>
            ValidSymbol<ITypeSymbol>.From(@event.Value.Type);
    }
}
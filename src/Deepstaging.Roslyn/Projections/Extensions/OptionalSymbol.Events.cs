// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for OptionalSymbol&lt;IEventSymbol&gt; exposing IEventSymbol-specific functionality.
/// Note: Most properties (IsVirtual, IsOverride, etc.) are accessible via .Symbol
/// These extensions focus on getting accessor methods which require null checking.
/// </summary>
public static class ProjectedEventSymbolExtensions
{
    extension(OptionalSymbol<IEventSymbol> eventSymbol)
    {
        /// <summary>
        /// Gets the add method of the event.
        /// Returns Empty if add method is null.
        /// </summary>
        public OptionalSymbol<IMethodSymbol> GetAddMethod()
        {
            return eventSymbol is { HasValue: true, Symbol.AddMethod: not null }
                ? OptionalSymbol<IMethodSymbol>.WithValue(eventSymbol.Symbol.AddMethod)
                : OptionalSymbol<IMethodSymbol>.Empty();
        }

        /// <summary>
        /// Gets the remove method of the event.
        /// Returns Empty if remove method is null.
        /// </summary>
        public OptionalSymbol<IMethodSymbol> GetRemoveMethod()
        {
            return eventSymbol is { HasValue: true, Symbol.RemoveMethod: not null }
                ? OptionalSymbol<IMethodSymbol>.WithValue(eventSymbol.Symbol.RemoveMethod)
                : OptionalSymbol<IMethodSymbol>.Empty();
        }

        /// <summary>
        /// Gets the raise method of the event (rarely used in C#).
        /// Returns Empty if raise method is null.
        /// </summary>
        public OptionalSymbol<IMethodSymbol> GetRaiseMethod()
        {
            return eventSymbol is { HasValue: true, Symbol.RaiseMethod: not null }
                ? OptionalSymbol<IMethodSymbol>.WithValue(eventSymbol.Symbol.RaiseMethod)
                : OptionalSymbol<IMethodSymbol>.Empty();
        }

        /// <summary>
        /// Gets the event that is overridden by this event, if this is an override.
        /// Returns Empty if not an override or overridden event is null.
        /// </summary>
        public OptionalSymbol<IEventSymbol> GetOverriddenEvent()
        {
            return eventSymbol is { HasValue: true, Symbol.OverriddenEvent: not null }
                ? OptionalSymbol<IEventSymbol>.WithValue(eventSymbol.Symbol.OverriddenEvent)
                : OptionalSymbol<IEventSymbol>.Empty();
        }
    }
}
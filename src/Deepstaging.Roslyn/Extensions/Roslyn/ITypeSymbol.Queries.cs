// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for starting fluent query builders from an ITypeSymbol.
/// </summary>
public static class TypeSymbolQueryExtensions
{
    /// <param name="typeSymbol">The type symbol to query.</param>
    extension(ITypeSymbol typeSymbol)
    {
        /// <summary>
        /// Starts a fluent query for methods on this type.
        /// </summary>
        public MethodQuery QueryMethods() => MethodQuery.From(typeSymbol);

        /// <summary>
        /// Starts a fluent query for properties on this type.
        /// </summary>
        public PropertyQuery QueryProperties() => PropertyQuery.From(typeSymbol);

        /// <summary>
        /// Starts a fluent query for constructors on this type.
        /// </summary>
        public ConstructorQuery QueryConstructors() => ConstructorQuery.From(typeSymbol);

        /// <summary>
        /// Starts a fluent query for fields on this type.
        /// </summary>
        public FieldQuery QueryFields() => FieldQuery.From(typeSymbol);

        /// <summary>
        /// Starts a fluent query for events on this type.
        /// </summary>
        public EventQuery QueryEvents() => EventQuery.From(typeSymbol);

        /// <summary>
        /// Gets all attributes on this type as valid attributes.
        /// </summary>
        public ImmutableArray<ValidAttribute> QueryAttributes() =>
            [..typeSymbol.GetAttributes().Select(ValidAttribute.From)];
    }
}
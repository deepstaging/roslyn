// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.RoslynKit.Projection.Attributes;

namespace Deepstaging.RoslynKit.Projection;

/// <summary>
/// Extension methods for querying RoslynKit attributes on symbols.
/// </summary>
public static class Queries
{
    extension(ValidAttribute attribute)
    {
        /// <summary>
        /// Converts a <see cref="ValidAttribute"/> to an <see cref="AutoNotifyAttributeQuery"/>.
        /// </summary>
        public AutoNotifyAttributeQuery QueryAutoNotifyAttribute() =>
            attribute.AsQuery<AutoNotifyAttributeQuery>();

        /// <summary>
        /// Converts a <see cref="ValidAttribute"/> to an <see cref="AlsoNotifyAttributeQuery"/>.
        /// </summary>
        public AlsoNotifyAttributeQuery QueryAlsoNotifyAttribute() =>
            attribute.AsQuery<AlsoNotifyAttributeQuery>();
    }

    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        /// <summary>
        /// Gets the <see cref="RoslynKit.AutoNotifyAttribute"/> applied to this symbol as a queryable wrapper.
        /// </summary>
        /// <returns>An <see cref="AutoNotifyAttributeQuery"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the attribute is not found.</exception>
        public AutoNotifyAttributeQuery AutoNotifyAttribute() =>
            symbol.GetAttribute<AutoNotifyAttribute>()
                .Map(attr => attr.AsQuery<AutoNotifyAttributeQuery>())
                .OrThrow();
    }

    extension(ValidSymbol<IFieldSymbol> symbol)
    {
        /// <summary>
        /// Gets all <see cref="AlsoNotifyAttribute"/> instances applied to this symbol as queryable wrappers.
        /// </summary>
        /// <returns>An immutable array of <see cref="AlsoNotifyAttributeQuery"/> instances.</returns>
        public ImmutableArray<AlsoNotifyAttributeQuery> AlsoNotifyAttributes() =>
        [
            ..symbol.GetAttributes<AlsoNotifyAttribute>()
                .Select(attr => attr.AsQuery<AlsoNotifyAttributeQuery>())
        ];
    }
}
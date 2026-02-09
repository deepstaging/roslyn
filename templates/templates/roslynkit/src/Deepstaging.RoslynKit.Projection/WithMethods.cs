// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Deepstaging.RoslynKit.Projection.Models;

namespace Deepstaging.RoslynKit.Projection;

/// <summary>
/// Provides extension methods for querying and building "With" method models from type symbols.
/// </summary>
public static class WithMethods
{
    /// <summary>
    /// Extension block for <see cref="ValidSymbol{T}"/> of <see cref="INamedTypeSymbol"/>.
    /// </summary>
    /// <param name="symbol">The validated named type symbol to extend.</param>
    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        /// <summary>
        /// Queries the type symbol to build a model for generating "With" methods.
        /// Only supports classes and structs with public, instance, read-write properties.
        /// </summary>
        /// <returns>
        /// A <see cref="WithMethodsModel"/> containing the type information and properties,
        /// or <c>null</c> if the type is not a class/struct or has no eligible properties.
        /// </returns>
        public WithMethodsModel? QueryWithMethods()
        {
            // Only support classes and structs
            if (symbol is { IsClass: false, IsStruct: false })
                return null;

            // Get all public instance properties with getters and init/set accessors
            var props = symbol.QueryProperties()
                .ThatArePublic()
                .ThatAreInstance()
                .ThatAreReadWrite()
                .Select(p => new WithPropertyModel { Name = p.Name, TypeName = p.Type?.FullyQualifiedName! });

            if (props.IsDefaultOrEmpty)
                return null;

            return new WithMethodsModel
            {
                Namespace = symbol.Namespace ?? "",
                TypeName = symbol.Name,
                IsStruct = symbol.IsStruct,
                Properties = props
            };

        }
    }
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.RoslynKit.Projection.Models;

namespace Deepstaging.RoslynKit.Projection;

/// <summary>
/// Extension methods for building AutoNotify models from symbols decorated with <see cref="AutoNotifyAttribute"/>.
/// </summary>
public static class AutoNotify
{
    /// <param name="symbol">The type symbol to query.</param>
    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        /// <summary>
        /// Queries the <see cref="AutoNotifyAttribute"/> on this type and builds a model.
        /// Returns null if the type doesn't have the attribute or is otherwise invalid.
        /// </summary>
        /// <returns>An <see cref="AutoNotifyModel"/> or null if invalid.</returns>
        public AutoNotifyModel? QueryAutoNotify()
        {
            var properties = symbol.QueryNotifyProperties();
            if (properties.IsEmpty)
                return null;

            return new AutoNotifyModel
            {
                Namespace = symbol.Namespace ?? "",
                TypeName = symbol.Name,
                Accessibility = symbol.Accessibility,
                GenerateBaseImplementation = symbol.AutoNotifyAttribute().GenerateBaseImplementation,
                Properties = properties
            };
        }
    }

    /// <summary>
    /// Queries all instance fields that should become notify properties.
    /// A field is included if it follows the naming convention (_fieldName or m_fieldName).
    /// </summary>
    private static ImmutableArray<NotifyPropertyModel> QueryNotifyProperties(this ValidSymbol<INamedTypeSymbol> symbol)
    {
        return symbol.QueryFields()
            .ThatAreInstance()
            .Where(f => BackingFieldConventions.IsBackingFieldName(f.Name))
            .Select(field => new NotifyPropertyModel
            {
                PropertyName = field.Name
                    .TrimStart(BackingFieldConventions.HungarianPrefix)
                    .TrimStart(BackingFieldConventions.UnderscorePrefix)
                    .ToString()
                    .ToPascalCase(),
                FieldName = field.Name,
                TypeName = field.Type?.FullyQualifiedName!,
                IsFieldPrivate = field.IsPrivate,
                AlsoNotify =
                [
                    ..field.AlsoNotifyAttributes()
                        .SelectMany(attr => attr.PropertyNames)
                        .Distinct()
                ]
            });
    }
}
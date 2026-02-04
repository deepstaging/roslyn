// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for working with attributes in a fluent projection style.
/// </summary>
public static class OptionalAttributeExtensions
{
    extension(AttributeData? attribute)
    {
        /// <summary>
        /// Converts an AttributeData to an OptionalAttribute projection.
        /// </summary>
        public OptionalAttribute ToOptional()
        {
            return OptionalAttribute.FromNullable(attribute);
        }
    }

    extension(ITypeSymbol? type)
    {
        /// <summary>
        /// Gets all attributes applied to the type as optional projections.
        /// </summary>
        public IEnumerable<OptionalAttribute> QueryAttributes()
        {
            if (type == null) return [];
            return type.GetAttributes().Select(OptionalAttribute.WithValue);
        }

        /// <summary>
        /// Gets attributes with the specified name applied to the type.
        /// </summary>
        public IEnumerable<OptionalAttribute> QueryAttributes(string attributeName)
        {
            if (type == null) return [];

            var withAttribute = attributeName.EndsWith("Attribute") ? attributeName : attributeName + "Attribute";
            var withoutAttribute = attributeName.EndsWith("Attribute")
                ? attributeName.Substring(0, attributeName.Length - "Attribute".Length)
                : attributeName;

            return type.GetAttributes()
                .Where(attr => attr.AttributeClass?.Name == withAttribute ||
                               attr.AttributeClass?.Name == withoutAttribute)
                .Select(OptionalAttribute.WithValue);
        }
    }
}
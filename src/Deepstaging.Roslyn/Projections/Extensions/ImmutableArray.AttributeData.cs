// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// 
/// </summary>
public static class ImmutableArrayAttributeDataExtensions
{
    extension(ImmutableArray<AttributeData> attributes)
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<ValidAttribute> GetByType<T>() where T : Attribute
        {
            return attributes.GetByName(typeof(T).Name);
        }

        /// <summary>
        /// Gets attributes by System.Type, supporting open generic types.
        /// For generic attributes, use typeof(MyAttribute&lt;&gt;) to match any instantiation.
        /// </summary>
        /// <param name="attributeType">The attribute type. Can be an open generic like typeof(MyAttribute&lt;&gt;).</param>
        public IEnumerable<ValidAttribute> GetByType(Type attributeType)
        {
            // For generic types, get the name with arity (e.g., "HttpClientAttribute`1")
            var metadataName = attributeType.IsGenericType
                ? $"{GetNameWithoutGenericArity(attributeType.Name)}`{attributeType.GetGenericArguments().Length}"
                : attributeType.Name;

            return attributes.GetByMetadataName(metadataName);
        }

        /// <summary>
        /// Gets attribute by full name as OptionalSymbol.
        /// Returns Empty if not found.
        /// </summary>
        public IEnumerable<ValidAttribute> GetByName(string attributeName)
        {
            var withAttribute = attributeName.EndsWith("Attribute") ? attributeName : attributeName + "Attribute";
            var withoutAttribute = attributeName.EndsWith("Attribute")
                ? attributeName.Substring(0, attributeName.Length - "Attribute".Length)
                : attributeName;

            return attributes
                .Where(attr => attr.AttributeClass?.Name == withAttribute ||
                               attr.AttributeClass?.Name == withoutAttribute)
                .Select(ValidAttribute.From);
        }

        /// <summary>
        /// Gets attributes by metadata name, matching the attribute class or its original definition.
        /// Supports generic attributes with arity notation (e.g., "HttpClientAttribute`1").
        /// </summary>
        public IEnumerable<ValidAttribute> GetByMetadataName(string metadataName)
        {
            var withAttribute = metadataName.EndsWith("Attribute") || metadataName.Contains("`")
                ? metadataName
                : metadataName + "Attribute";

            // Extract base name without arity for matching non-generic or original definition
            var baseName = withAttribute.Contains("`")
                ? withAttribute.Substring(0, withAttribute.IndexOf('`'))
                : withAttribute;

            return attributes
                .Where(attr =>
                {
                    var attrClass = attr.AttributeClass;
                    if (attrClass == null) return false;

                    // Direct match on metadata name
                    if (attrClass.MetadataName == withAttribute) return true;

                    // For generic attributes, also check the original definition
                    if (attrClass.IsGenericType &&
                        attrClass.OriginalDefinition.MetadataName == withAttribute)
                        return true;

                    return false;
                })
                .Select(ValidAttribute.From);
        }

        private static string GetNameWithoutGenericArity(string name)
        {
            var index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }
    }
}
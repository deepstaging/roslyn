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
    }
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for AttributeData - provides bridge to OptionalAttribute/OptionalArgument.
/// For rich attribute querying, use .ToOptional() to wrap in OptionalAttribute.
/// </summary>
public static class AttributeDataExtensions
{
    extension(AttributeData attr)
    {
        /// <summary>
        /// Wraps AttributeData in an OptionalAttribute for fluent querying.
        /// </summary>
        public OptionalAttribute Query()
        {
            return OptionalAttribute.WithValue(attr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public bool Is<TAttribute>() where TAttribute : Attribute =>
            attr.AttributeClass?.ToDisplayString() == typeof(TAttribute).FullName;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public OptionalAttribute As<TAttribute>() where TAttribute : Attribute =>
            attr.Is<TAttribute>()
                ? OptionalAttribute.WithValue(attr)
                : OptionalAttribute.Empty();

        /// <summary>
        /// Gets a named argument value from an attribute.
        /// Returns an OptionalArgument that can be used with OrDefault/OrThrow/etc.
        /// For array/enumerable types, extracts values from TypedConstant.Values.
        /// </summary>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <param name="name">The name of the named argument.</param>
        public OptionalArgument<T> GetNamedArgument<T>(string name) =>
            GetArgument<T>(attr.NamedArguments.FirstOrDefault(kvp => kvp.Key == name).Value);

        /// <summary>
        /// Gets a constructor argument value at the specified index.
        /// Returns an OptionalArgument that can be used with OrDefault/OrThrow/etc.
        /// </summary>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <param name="index">The zero-based index of the constructor argument.</param>
        public OptionalArgument<T> GetConstructorArgument<T>(int index) =>
            attr.ConstructorArguments.Length > index
                ? GetArgument<T>(attr.ConstructorArguments[index])
                : OptionalArgument<T>.Empty();
    }

    private static OptionalArgument<T> GetArgument<T>(TypedConstant typedConstant)
    {
        // Check if T is an enumerable type (but not string)
        if (typeof(T) != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(typeof(T)))
        {
            if (typedConstant.IsNull || typedConstant.Values.IsDefaultOrEmpty)
                return OptionalArgument<T>.Empty();

            var elementType = typeof(T).IsArray
                ? typeof(T).GetElementType()!
                : typeof(T).GetGenericArguments().FirstOrDefault() ?? typeof(object);

            var values = typedConstant.Values
                .Where(v => v.Value is not null)
                .Select(v => v.Value)
                .ToArray();

            var typedArray = Array.CreateInstance(elementType, values.Length);
            for (int i = 0; i < values.Length; i++)
                typedArray.SetValue(values[i], i);

            if (typedArray is T result)
                return OptionalArgument<T>.WithValue(result);

            return OptionalArgument<T>.Empty();
        }

        return typedConstant.Value is T value
            ? OptionalArgument<T>.WithValue(value)
            : OptionalArgument<T>.Empty();
    }
}
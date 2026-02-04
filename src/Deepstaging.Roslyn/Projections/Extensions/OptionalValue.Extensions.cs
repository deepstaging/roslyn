// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for OptionalValue to support operations on collections and fluent querying.
/// </summary>
public static class ProjectedValueExtensions
{
    /// <summary>
    /// Creates an OptionalValue from a nullable value.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <param name="value">The nullable value.</param>
    /// <returns>OptionalValue with value if not null, Empty otherwise.</returns>
    public static OptionalValue<TSource> ToOptional<TSource>(this TSource? value) where TSource : class
    {
        return value != null ? OptionalValue<TSource>.WithValue(value) : OptionalValue<TSource>.Empty();
    }

    /// <summary>
    /// Creates an OptionalValue from a nullable struct.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <param name="value">The nullable struct.</param>
    /// <returns>OptionalValue with value if HasValue, Empty otherwise.</returns>
    public static OptionalValue<TSource> ToOptional<TSource>(this TSource? value) where TSource : struct
    {
        return value.HasValue ? OptionalValue<TSource>.WithValue(value.Value) : OptionalValue<TSource>.Empty();
    }


    /// <param name="source">The projected value containing an enumerable collection.</param>
    /// <typeparam name="TSource">The source element type.</typeparam>
    extension<TSource>(OptionalValue<IEnumerable<TSource>> source)
    {
        /// <summary>
        /// Maps each element in the projected collection and returns an ImmutableArray of results.
        /// Returns empty array if source is Empty.
        /// </summary>
        /// <typeparam name="TResult">The result element type.</typeparam>
        /// <param name="selector">Function that transforms each element.</param>
        /// <returns>ImmutableArray of transformed elements.</returns>
        public ImmutableArray<TResult> FlatMap<TResult>(Func<TSource, TResult> selector)
        {
            if (source.IsEmpty)
                return ImmutableArray<TResult>.Empty;

            var collection = source.OrNull();
            if (collection == null)
                return ImmutableArray<TResult>.Empty;

            return [..collection.Select(selector)];
        }
    }

    /// <param name="source">The projected value containing an ImmutableArray.</param>
    /// <typeparam name="TSource">The source element type.</typeparam>
    extension<TSource>(OptionalValue<ImmutableArray<TSource>> source)
    {
        /// <summary>
        /// Maps each element in the projected ImmutableArray and returns an ImmutableArray of results.
        /// Returns empty array if source is Empty.
        /// </summary>
        /// <typeparam name="TResult">The result element type.</typeparam>
        /// <param name="selector">Function that transforms each element.</param>
        /// <returns>ImmutableArray of transformed elements.</returns>
        public ImmutableArray<TResult> FlatMap<TResult>(Func<TSource, TResult> selector)
        {
            if (source.IsEmpty)
                return ImmutableArray<TResult>.Empty;

            var array = source.OrNull();
            if (array.IsDefaultOrEmpty)
                return ImmutableArray<TResult>.Empty;

            return [..array.Select(selector)];
        }
    }
}
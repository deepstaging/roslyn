// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for wrapping TypedConstant values in OptionalArgument for fluent querying.
/// </summary>
public static class ProjectedArgumentExtensions
{
    /// <summary>
    /// Wraps a TypedConstant value in an OptionalArgument for fluent querying.
    /// Returns Empty if the constant has no value or is of the wrong type.
    /// </summary>
    /// <typeparam name="T">The expected type of the constant value.</typeparam>
    /// <param name="constant">The typed constant to wrap.</param>
    /// <returns>An OptionalArgument containing the value, or Empty if null or wrong type.</returns>
    public static OptionalArgument<T> Query<T>(this TypedConstant constant)
    {
        return constant.Value is T typedValue
            ? OptionalArgument<T>.WithValue(typedValue)
            : OptionalArgument<T>.Empty();
    }

    /// <summary>
    /// Creates an OptionalArgument from a nullable value.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <param name="value">The nullable value.</param>
    /// <returns>OptionalArgument with value if not null, Empty otherwise.</returns>
    public static OptionalArgument<TSource> ToOptionalArgument<TSource>(this TSource? value) where TSource : class
    {
        return value != null ? OptionalArgument<TSource>.WithValue(value) : OptionalArgument<TSource>.Empty();
    }

    /// <summary>
    /// Creates an OptionalArgument from a nullable struct.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <param name="value">The nullable struct.</param>
    /// <returns>OptionalArgument with value if HasValue, Empty otherwise.</returns>
    public static OptionalArgument<TSource> ToOptionalArgument<TSource>(this TSource? value) where TSource : struct
    {
        return value.HasValue ? OptionalArgument<TSource>.WithValue(value.Value) : OptionalArgument<TSource>.Empty();
    }

    /// <summary>
    /// Maps each element in the projected collection and returns an ImmutableArray of results.
    /// Returns empty array if source is Empty.
    /// </summary>
    /// <typeparam name="TSource">The source element type.</typeparam>
    /// <typeparam name="TResult">The result element type.</typeparam>
    /// <param name="source">The projected argument containing an enumerable collection.</param>
    /// <param name="selector">Function that transforms each element.</param>
    /// <returns>ImmutableArray of transformed elements.</returns>
    public static ImmutableArray<TResult> FlatMap<TSource, TResult>(
        this OptionalArgument<IEnumerable<TSource>> source,
        Func<TSource, TResult> selector)
    {
        if (source.IsEmpty)
            return ImmutableArray<TResult>.Empty;

        var collection = source.OrNull();
        if (collection == null)
            return ImmutableArray<TResult>.Empty;

        return [..collection.Select(selector)];
    }

    /// <summary>
    /// Maps each element in the projected ImmutableArray and returns an ImmutableArray of results.
    /// Returns empty array if source is Empty.
    /// </summary>
    /// <typeparam name="TSource">The source element type.</typeparam>
    /// <typeparam name="TResult">The result element type.</typeparam>
    /// <param name="source">The projected argument containing an ImmutableArray.</param>
    /// <param name="selector">Function that transforms each element.</param>
    /// <returns>ImmutableArray of transformed elements.</returns>
    public static ImmutableArray<TResult> FlatMap<TSource, TResult>(
        this OptionalArgument<ImmutableArray<TSource>> source,
        Func<TSource, TResult> selector)
    {
        if (source.IsEmpty)
            return ImmutableArray<TResult>.Empty;

        var array = source.OrNull();
        if (array.IsDefaultOrEmpty)
            return ImmutableArray<TResult>.Empty;

        return [..array.Select(selector)];
    }
}
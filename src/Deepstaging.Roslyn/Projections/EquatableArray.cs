// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Collections;

namespace Deepstaging.Roslyn;

/// <summary>
/// A drop-in replacement for <see cref="ImmutableArray{T}"/> that provides structural equality.
/// Use this in pipeline models instead of <see cref="ImmutableArray{T}"/> to ensure
/// incremental generator caching works correctly.
/// </summary>
/// <typeparam name="T">The element type, which must implement <see cref="IEquatable{T}"/>.</typeparam>
public readonly struct EquatableArray<T>(ImmutableArray<T> array)
    : IEquatable<EquatableArray<T>>, IReadOnlyList<T>
    where T : IEquatable<T>
{
    /// <summary>
    /// Gets the underlying <see cref="ImmutableArray{T}"/>.
    /// Returns an empty array if the underlying array is default.
    /// </summary>
    public ImmutableArray<T> AsImmutableArray() => array.IsDefault ? [] : array;

    /// <summary>
    /// Gets an empty <see cref="EquatableArray{T}"/>.
    /// </summary>
    public static EquatableArray<T> Empty => new([]);

    /// <summary>
    /// Gets the number of elements in the array.
    /// </summary>
    public int Count => array.IsDefault ? 0 : array.Length;

    /// <summary>
    /// Gets the element at the specified index.
    /// </summary>
    public T this[int index] => array[index];

    /// <inheritdoc />
    public bool Equals(EquatableArray<T> other)
    {
        var left = AsImmutableArray();
        var right = other.AsImmutableArray();

        if (left.Length != right.Length)
            return false;

        for (var i = 0; i < left.Length; i++)
        {
            if (!left[i].Equals(right[i]))
                return false;
        }

        return true;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is EquatableArray<T> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var arr = AsImmutableArray();
        if (arr.IsEmpty)
            return 0;

        unchecked
        {
            var hash = 17;
            foreach (var item in arr)
                hash = hash * 31 + item.GetHashCode();
            return hash;
        }
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
        => ((IEnumerable<T>)AsImmutableArray()).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <summary>
    /// Converts an <see cref="ImmutableArray{T}"/> to an <see cref="EquatableArray{T}"/>.
    /// </summary>
    public static implicit operator EquatableArray<T>(ImmutableArray<T> array)
        => new(array);

    /// <summary>
    /// Converts an <see cref="EquatableArray{T}"/> to an <see cref="ImmutableArray{T}"/>.
    /// </summary>
    public static implicit operator ImmutableArray<T>(EquatableArray<T> array)
        => array.AsImmutableArray();

    /// <inheritdoc />
    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right)
        => left.Equals(right);

    /// <inheritdoc />
    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right)
        => !left.Equals(right);
}

/// <summary>
/// Extension methods for creating <see cref="EquatableArray{T}"/> instances.
/// </summary>
public static class EquatableArrayExtensions
{
    /// <summary>
    /// Converts an <see cref="IEnumerable{T}"/> to an <see cref="EquatableArray{T}"/>.
    /// </summary>
    public static EquatableArray<T> ToEquatableArray<T>(this IEnumerable<T> source) where T : IEquatable<T>
        => new(source.ToImmutableArray());

    /// <summary>
    /// Converts an <see cref="ImmutableArray{T}"/> to an <see cref="EquatableArray{T}"/>.
    /// </summary>
    public static EquatableArray<T> ToEquatableArray<T>(this ImmutableArray<T> source) where T : IEquatable<T>
        => new(source);

    /// <summary>
    /// Projects each element and wraps the result in an <see cref="EquatableArray{T}"/>.
    /// </summary>
    public static EquatableArray<TResult> SelectEquatable<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, TResult> selector) where TResult : IEquatable<TResult>
        => new(source.Select(selector).ToImmutableArray());

    /// <summary>
    /// Projects each element and wraps the result in an <see cref="EquatableArray{T}"/>.
    /// </summary>
    public static EquatableArray<TResult> SelectEquatable<TSource, TResult>(
        this ImmutableArray<TSource> source,
        Func<TSource, TResult> selector) where TResult : IEquatable<TResult>
        => new(source.Select(selector).ToImmutableArray());
}

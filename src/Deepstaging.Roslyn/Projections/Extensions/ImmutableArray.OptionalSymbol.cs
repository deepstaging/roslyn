// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for ImmutableArray of OptionalSymbol that return OptionalSymbol.
/// These override LINQ methods to provide safer, non-throwing alternatives.
/// </summary>
public static class ImmutableArrayOptionalSymbolExtensions
{
    extension<T>(ImmutableArray<OptionalSymbol<T>> source) where T : class, ISymbol
    {
        /// <summary>
        /// Returns the first element that has a value as an OptionalSymbol, or Empty if the array is empty or all elements are empty.
        /// </summary>
        public OptionalSymbol<T> First()
        {
            foreach (var item in source.Where(item => item.HasValue))
            {
                return item;
            }

            return OptionalSymbol<T>.Empty();
        }

        /// <summary>
        /// Returns the first element matching the predicate that has a value as an OptionalSymbol, or Empty if none found.
        /// </summary>
        public OptionalSymbol<T> First(Func<OptionalSymbol<T>, bool> predicate)
        {
            foreach (var item in source.Where(item => item.HasValue && predicate(item)))
            {
                return item;
            }

            return OptionalSymbol<T>.Empty();
        }

        /// <summary>
        /// Returns the only element that has a value as an OptionalSymbol, or Empty if the array doesn't contain exactly one element with a value.
        /// </summary>
        public OptionalSymbol<T> Single()
        {
            var result = OptionalSymbol<T>.Empty();
            bool found = false;

            foreach (var item in source)
            {
                if (item.HasValue)
                {
                    if (found)
                        return OptionalSymbol<T>.Empty(); // More than one
                    
                    result = item;
                    found = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the only element matching the predicate that has a value as an OptionalSymbol, or Empty if there isn't exactly one match.
        /// </summary>
        public OptionalSymbol<T> Single(Func<OptionalSymbol<T>, bool> predicate)
        {
            var result = OptionalSymbol<T>.Empty();
            bool found = false;

            foreach (var item in source)
            {
                if (item.HasValue && predicate(item))
                {
                    if (found)
                        return OptionalSymbol<T>.Empty(); // More than one match
                    
                    result = item;
                    found = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the last element that has a value as an OptionalSymbol, or Empty if the array is empty or all elements are empty.
        /// </summary>
        public OptionalSymbol<T> Last()
        {
            for (int i = source.Length - 1; i >= 0; i--)
            {
                if (source[i].HasValue)
                    return source[i];
            }
            return OptionalSymbol<T>.Empty();
        }

        /// <summary>
        /// Returns the last element matching the predicate that has a value as an OptionalSymbol, or Empty if none found.
        /// </summary>
        public OptionalSymbol<T> Last(Func<OptionalSymbol<T>, bool> predicate)
        {
            for (int i = source.Length - 1; i >= 0; i--)
            {
                if (source[i].HasValue && predicate(source[i]))
                    return source[i];
            }
            return OptionalSymbol<T>.Empty();
        }

        /// <summary>
        /// Returns the element at the specified index as an OptionalSymbol, or Empty if index is out of range or the element is empty.
        /// </summary>
        public OptionalSymbol<T> ElementAt(int index)
        {
            return index >= 0 && index < source.Length
                ? source[index]
                : OptionalSymbol<T>.Empty();
        }

        /// <summary>
        /// Returns the first element that has a value as an OptionalSymbol, or Empty if the array is empty or all elements are empty.
        /// Alias for First() to match LINQ naming conventions.
        /// </summary>
        public OptionalSymbol<T> FirstOrDefault()
        {
            return source.First();
        }

        /// <summary>
        /// Returns the first element matching the predicate that has a value as an OptionalSymbol, or Empty if none found.
        /// Alias for First(predicate) to match LINQ naming conventions.
        /// </summary>
        public OptionalSymbol<T> FirstOrDefault(Func<OptionalSymbol<T>, bool> predicate)
        {
            return source.First(predicate);
        }

        /// <summary>
        /// Returns the last element that has a value as an OptionalSymbol, or Empty if the array is empty or all elements are empty.
        /// Alias for Last() to match LINQ naming conventions.
        /// </summary>
        public OptionalSymbol<T> LastOrDefault()
        {
            return source.Last();
        }

        /// <summary>
        /// Returns the last element matching the predicate that has a value as an OptionalSymbol, or Empty if none found.
        /// Alias for Last(predicate) to match LINQ naming conventions.
        /// </summary>
        public OptionalSymbol<T> LastOrDefault(Func<OptionalSymbol<T>, bool> predicate)
        {
            return source.Last(predicate);
        }

        /// <summary>
        /// Returns the only element that has a value as an OptionalSymbol, or Empty if the array doesn't contain exactly one element with a value.
        /// Alias for Single() to match LINQ naming conventions.
        /// </summary>
        public OptionalSymbol<T> SingleOrDefault()
        {
            return source.Single();
        }

        /// <summary>
        /// Returns the only element matching the predicate that has a value as an OptionalSymbol, or Empty if there isn't exactly one match.
        /// Alias for Single(predicate) to match LINQ naming conventions.
        /// </summary>
        public OptionalSymbol<T> SingleOrDefault(Func<OptionalSymbol<T>, bool> predicate)
        {
            return source.Single(predicate);
        }

        /// <summary>
        /// Returns the element at the specified index as an OptionalSymbol, or Empty if index is out of range or the element is empty.
        /// Alias for ElementAt(index) to match LINQ naming conventions.
        /// </summary>
        public OptionalSymbol<T> ElementAtOrDefault(int index)
        {
            return source.ElementAt(index);
        }

        /// <summary>
        /// Provides indexer access to symbols by name.
        /// Usage: array.GetByName["PropertyName"] returns OptionalSymbol.
        /// Only searches elements that have a value.
        /// </summary>
        public SymbolNameIndexerForOptional<T> ByName => new(source);
    }
}

/// <summary>
/// Indexer wrapper that allows looking up optional symbols by name using [] syntax.
/// Usage: array.GetByName["PropertyName"] returns OptionalSymbol.
/// Only searches elements that have a value.
/// </summary>
public readonly struct SymbolNameIndexerForOptional<T> where T : class, ISymbol
{
    private readonly ImmutableArray<OptionalSymbol<T>> _source;

    internal SymbolNameIndexerForOptional(ImmutableArray<OptionalSymbol<T>> source)
    {
        _source = source;
    }

    /// <summary>
    /// Gets a symbol by name, returning OptionalSymbol.
    /// Returns Empty if not found, if multiple symbols have the same name, or if all matching elements are empty.
    /// Only searches elements that have a value.
    /// </summary>
    public OptionalSymbol<T> this[string name]
    {
        get
        {
            var result = OptionalSymbol<T>.Empty();
            bool found = false;

            foreach (var item in _source)
            {
                if (item.HasValue && item.Name == name)
                {
                    if (found)
                        return OptionalSymbol<T>.Empty(); // Multiple matches
                    
                    result = item;
                    found = true;
                }
            }

            return result;
        }
    }
}

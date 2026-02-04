// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
// ReSharper disable MemberCanBePrivate.Global
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for ImmutableArray of ValidSymbol that return OptionalSymbol.
/// These override LINQ methods to provide safer, non-throwing alternatives.
/// </summary>
public static class ImmutableArrayValidSymbolExtensions
{
    extension<T>(ImmutableArray<ValidSymbol<T>> source) where T : class, ISymbol
    {
        /// <summary>
        /// Returns the first element as an OptionalSymbol, or Empty if the array is empty.
        /// </summary>
        public OptionalSymbol<T> First()
        {
            return source.Length > 0
                ? OptionalSymbol<T>.WithValue(source[0].Value)
                : OptionalSymbol<T>.Empty();
        }

        /// <summary>
        /// Returns the first element matching the predicate as an OptionalSymbol, or Empty if none found.
        /// </summary>
        public OptionalSymbol<T> First(Func<ValidSymbol<T>, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                    return OptionalSymbol<T>.WithValue(item.Value);
            }
            return OptionalSymbol<T>.Empty();
        }

        /// <summary>
        /// Returns the only element as an OptionalSymbol, or Empty if the array doesn't contain exactly one element.
        /// </summary>
        public OptionalSymbol<T> Single()
        {
            return source.Length == 1
                ? OptionalSymbol<T>.WithValue(source[0].Value)
                : OptionalSymbol<T>.Empty();
        }

        /// <summary>
        /// Returns the only element matching the predicate as an OptionalSymbol, or Empty if there isn't exactly one match.
        /// </summary>
        public OptionalSymbol<T> Single(Func<ValidSymbol<T>, bool> predicate)
        {
            var result = OptionalSymbol<T>.Empty();
            bool found = false;

            foreach (var item in source)
            {
                if (predicate(item))
                {
                    if (found)
                        return OptionalSymbol<T>.Empty(); // More than one match
                
                    result = OptionalSymbol<T>.WithValue(item.Value);
                    found = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the last element as an OptionalSymbol, or Empty if the array is empty.
        /// </summary>
        public OptionalSymbol<T> Last()
        {
            return source.Length > 0
                ? OptionalSymbol<T>.WithValue(source[^1].Value)
                : OptionalSymbol<T>.Empty();
        }

        /// <summary>
        /// Returns the last element matching the predicate as an OptionalSymbol, or Empty if none found.
        /// </summary>
        public OptionalSymbol<T> Last(Func<ValidSymbol<T>, bool> predicate)
        {
            for (int i = source.Length - 1; i >= 0; i--)
            {
                if (predicate(source[i]))
                    return OptionalSymbol<T>.WithValue(source[i].Value);
            }
            return OptionalSymbol<T>.Empty();
        }

        /// <summary>
        /// Returns the element at the specified index as an OptionalSymbol, or Empty if index is out of range.
        /// </summary>
        public OptionalSymbol<T> ElementAt(int index)
        {
            return index >= 0 && index < source.Length
                ? OptionalSymbol<T>.WithValue(source[index].Value)
                : OptionalSymbol<T>.Empty();
        }

        /// <summary>
        /// Returns the first element as an OptionalSymbol, or Empty if the array is empty.
        /// Alias for First() to match LINQ naming conventions.
        /// </summary>
        public OptionalSymbol<T> FirstOrDefault()
        {
            return source.First();
        }

        /// <summary>
        /// Returns the first element matching the predicate as an OptionalSymbol, or Empty if none found.
        /// Alias for First(predicate) to match LINQ naming conventions.
        /// </summary>
        public OptionalSymbol<T> FirstOrDefault(Func<ValidSymbol<T>, bool> predicate)
        {
            return source.First(predicate);
        }

        /// <summary>
        /// Returns the last element as an OptionalSymbol, or Empty if the array is empty.
        /// Alias for Last() to match LINQ naming conventions.
        /// </summary>
        public OptionalSymbol<T> LastOrDefault()
        {
            return source.Last();
        }

        /// <summary>
        /// Returns the last element matching the predicate as an OptionalSymbol, or Empty if none found.
        /// Alias for Last(predicate) to match LINQ naming conventions.
        /// </summary>
        public OptionalSymbol<T> LastOrDefault(Func<ValidSymbol<T>, bool> predicate)
        {
            return source.Last(predicate);
        }

        /// <summary>
        /// Returns the only element as an OptionalSymbol, or Empty if the array doesn't contain exactly one element.
        /// Alias for Single() to match LINQ naming conventions.
        /// </summary>
        public OptionalSymbol<T> SingleOrDefault()
        {
            return source.Single();
        }

        /// <summary>
        /// Returns the only element matching the predicate as an OptionalSymbol, or Empty if there isn't exactly one match.
        /// Alias for Single(predicate) to match LINQ naming conventions.
        /// </summary>
        public OptionalSymbol<T> SingleOrDefault(Func<ValidSymbol<T>, bool> predicate)
        {
            return source.Single(predicate);
        }

        /// <summary>
        /// Returns the element at the specified index as an OptionalSymbol, or Empty if index is out of range.
        /// Alias for ElementAt(index) to match LINQ naming conventions.
        /// </summary>
        public OptionalSymbol<T> ElementAtOrDefault(int index)
        {
            return source.ElementAt(index);
        }

        /// <summary>
        /// Provides indexer access to symbols by name.
        /// Usage: array.GetByName["PropertyName"] returns OptionalSymbol.
        /// </summary>
        public SymbolNameIndexer<T> ByName => new(source);
    }
}

/// <summary>
/// Indexer wrapper that allows looking up symbols by name using [] syntax.
/// Usage: array.GetByName["PropertyName"] returns OptionalSymbol.
/// </summary>
public readonly struct SymbolNameIndexer<T> where T : class, ISymbol
{
    private readonly ImmutableArray<ValidSymbol<T>> _source;

    internal SymbolNameIndexer(ImmutableArray<ValidSymbol<T>> source)
    {
        _source = source;
    }

    /// <summary>
    /// Gets a symbol by name, returning OptionalSymbol.
    /// Returns Empty if not found or if multiple symbols have the same name.
    /// </summary>
    public OptionalSymbol<T> this[string name]
    {
        get
        {
            var result = OptionalSymbol<T>.Empty();
            bool found = false;

            foreach (var item in _source)
            {
                if (item.Value.Name == name)
                {
                    if (found)
                        return OptionalSymbol<T>.Empty(); // Multiple matches
                    
                    result = OptionalSymbol<T>.WithValue(item.Value);
                    found = true;
                }
            }

            return result;
        }
    }
}

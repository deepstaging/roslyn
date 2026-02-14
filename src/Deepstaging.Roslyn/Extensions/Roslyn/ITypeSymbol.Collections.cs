// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for identifying collection and immutable types.
/// </summary>
public static class TypeSymbolCollectionExtensions
{
    /// <param name="typeSymbol">The type symbol to check.</param>
    extension(ITypeSymbol typeSymbol)
    {
        // ── Mutable collection interfaces ────────────────────────────────

        /// <summary>
        /// Checks if the type is <c>IEnumerable&lt;T&gt;</c>.
        /// </summary>
        public bool IsEnumerableType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "IEnumerable", IsGenericType: true, TypeArguments.Length: 1 };
        }

        /// <summary>
        /// Checks if the type is <c>ICollection&lt;T&gt;</c>.
        /// </summary>
        public bool IsCollectionType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "ICollection", IsGenericType: true, TypeArguments.Length: 1 };
        }

        /// <summary>
        /// Checks if the type is <c>IList&lt;T&gt;</c>.
        /// </summary>
        public bool IsListType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "IList", IsGenericType: true, TypeArguments.Length: 1 };
        }

        /// <summary>
        /// Checks if the type is <c>IDictionary&lt;TKey, TValue&gt;</c>.
        /// </summary>
        public bool IsDictionaryType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "IDictionary", IsGenericType: true, TypeArguments.Length: 2 };
        }

        /// <summary>
        /// Checks if the type is <c>ISet&lt;T&gt;</c>.
        /// </summary>
        public bool IsSetType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "ISet", IsGenericType: true, TypeArguments.Length: 1 };
        }

        // ── Read-only collection interfaces ──────────────────────────────

        /// <summary>
        /// Checks if the type is <c>IReadOnlyList&lt;T&gt;</c>.
        /// </summary>
        public bool IsReadOnlyListType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "IReadOnlyList", IsGenericType: true, TypeArguments.Length: 1 };
        }

        /// <summary>
        /// Checks if the type is <c>IReadOnlyCollection&lt;T&gt;</c>.
        /// </summary>
        public bool IsReadOnlyCollectionType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "IReadOnlyCollection", IsGenericType: true, TypeArguments.Length: 1 };
        }

        /// <summary>
        /// Checks if the type is <c>IReadOnlyDictionary&lt;TKey, TValue&gt;</c>.
        /// </summary>
        public bool IsReadOnlyDictionaryType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "IReadOnlyDictionary", IsGenericType: true, TypeArguments.Length: 2 };
        }

        /// <summary>
        /// Checks if the type is <c>ReadOnlyCollection&lt;T&gt;</c>.
        /// </summary>
        public bool IsReadOnlyCollectionConcreteType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "ReadOnlyCollection", IsGenericType: true, TypeArguments.Length: 1 };
        }

        // ── Concrete collection types ────────────────────────────────────

        /// <summary>
        /// Checks if the type is <c>HashSet&lt;T&gt;</c>.
        /// </summary>
        public bool IsHashSetType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "HashSet", IsGenericType: true, TypeArguments.Length: 1 };
        }

        // ── Immutable collection types ───────────────────────────────────

        /// <summary>
        /// Checks if the type is <c>ImmutableArray&lt;T&gt;</c>.
        /// </summary>
        public bool IsImmutableArrayType()
        {
            return typeSymbol is INamedTypeSymbol { IsGenericType: true, Name: "ImmutableArray", Arity: 1 } named
                   && named.ContainingNamespace.ToDisplayString() == "System.Collections.Immutable";
        }

        /// <summary>
        /// Checks if the type is <c>ImmutableList&lt;T&gt;</c>.
        /// </summary>
        public bool IsImmutableListType()
        {
            return typeSymbol is INamedTypeSymbol { IsGenericType: true, Name: "ImmutableList", Arity: 1 } named
                   && named.ContainingNamespace.ToDisplayString() == "System.Collections.Immutable";
        }

        /// <summary>
        /// Checks if the type is <c>ImmutableDictionary&lt;TKey, TValue&gt;</c>.
        /// </summary>
        public bool IsImmutableDictionaryType()
        {
            return typeSymbol is INamedTypeSymbol { IsGenericType: true, Name: "ImmutableDictionary", Arity: 2 } named
                   && named.ContainingNamespace.ToDisplayString() == "System.Collections.Immutable";
        }

        /// <summary>
        /// Checks if the type is <c>ImmutableHashSet&lt;T&gt;</c>.
        /// </summary>
        public bool IsImmutableHashSetType()
        {
            return typeSymbol is INamedTypeSymbol { IsGenericType: true, Name: "ImmutableHashSet", Arity: 1 } named
                   && named.ContainingNamespace.ToDisplayString() == "System.Collections.Immutable";
        }

        // ── Query / Observable ───────────────────────────────────────────

        /// <summary>
        /// Checks if the type is <c>IQueryable&lt;T&gt;</c>.
        /// </summary>
        public bool IsQueryableType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "IQueryable", IsGenericType: true, TypeArguments.Length: 1 };
        }

        /// <summary>
        /// Checks if the type is <c>IObservable&lt;T&gt;</c>.
        /// </summary>
        public bool IsObservableType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "IObservable", IsGenericType: true, TypeArguments.Length: 1 };
        }
    }
}

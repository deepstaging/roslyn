// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deepstaging.Roslyn.TypeScript;

public readonly partial record struct TsTypeRef
{
    /// <summary>
    /// Maps a .NET <see cref="Type"/> to the corresponding TypeScript type reference.
    /// Handles primitives, common BCL types, arrays, nullables, generic collections, and <see cref="Task{TResult}"/>.
    /// </summary>
    /// <param name="type">The .NET type to map.</param>
    /// <returns>A <see cref="TsTypeRef"/> representing the TypeScript equivalent.</returns>
    /// <remarks>
    /// <para>Mapping rules (applied in order):</para>
    /// <list type="bullet">
    ///   <item><description><see cref="Nullable{T}"/> → <c>T | null</c></description></item>
    ///   <item><description>Arrays → <c>T[]</c> (<see cref="byte"/>[] → <c>Uint8Array</c>)</description></item>
    ///   <item><description><see cref="Task{TResult}"/> → <c>Promise&lt;T&gt;</c>, <see cref="Task"/> → <c>Promise&lt;void&gt;</c></description></item>
    ///   <item><description><see cref="Dictionary{TKey,TValue}"/> / <see cref="IDictionary{TKey,TValue}"/> / <see cref="IReadOnlyDictionary{TKey,TValue}"/> → <c>Record&lt;K, V&gt;</c></description></item>
    ///   <item><description><see cref="HashSet{T}"/> / <see cref="ISet{T}"/> → <c>Set&lt;T&gt;</c></description></item>
    ///   <item><description><see cref="List{T}"/> / <see cref="IEnumerable{T}"/> / <see cref="ICollection{T}"/> / <see cref="IList{T}"/> / <see cref="IReadOnlyList{T}"/> / <see cref="IReadOnlyCollection{T}"/> → <c>T[]</c></description></item>
    ///   <item><description>Numeric types → <c>number</c>, <see cref="string"/> → <c>string</c>, <see cref="bool"/> → <c>boolean</c></description></item>
    ///   <item><description><see cref="DateTime"/> / <see cref="DateTimeOffset"/> → <c>Date</c></description></item>
    ///   <item><description><see cref="Guid"/> / <see cref="Uri"/> → <c>string</c></description></item>
    ///   <item><description><see cref="object"/> → <c>unknown</c></description></item>
    ///   <item><description>Unknown types → type name as-is</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// TsTypeRef.From(typeof(int))                       // "number"
    /// TsTypeRef.From(typeof(string))                    // "string"
    /// TsTypeRef.From(typeof(Task&lt;string&gt;))        // "Promise&lt;string&gt;"
    /// TsTypeRef.From(typeof(Dictionary&lt;string, int&gt;)) // "Record&lt;string, number&gt;"
    /// TsTypeRef.From(typeof(int?))                      // "number | null"
    /// </code>
    /// </example>
    public static TsTypeRef From(Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        // Nullable<T> → T | null
        var underlying = System.Nullable.GetUnderlyingType(type);
        if (underlying != null)
            return From(underlying).Nullable();

        // byte[] → Uint8Array (before generic array handling)
        if (type == typeof(byte[]))
            return From("Uint8Array");

        // Arrays → T[]
        if (type.IsArray)
        {
            var elementType = type.GetElementType();
            return elementType != null ? From(elementType).Array() : From("unknown[]");
        }

        // Generic types
        if (type.IsGenericType)
        {
            var def = type.GetGenericTypeDefinition();
            var args = type.GetGenericArguments();

            // Task<T> → Promise<T>
            if (def == typeof(Task<>))
                return From("Promise").Of(From(args[0]));

            // Dictionary-like → Record<K, V>
            if (def == typeof(Dictionary<,>) ||
                def == typeof(IDictionary<,>) ||
                def == typeof(IReadOnlyDictionary<,>))
                return From("Record").Of(From(args[0]), From(args[1]));

            // Set-like → Set<T>
            if (def == typeof(HashSet<>) ||
                def == typeof(ISet<>))
                return From("Set").Of(From(args[0]));

            // Collection-like → T[]
            if (def == typeof(List<>) ||
                def == typeof(IList<>) ||
                def == typeof(ICollection<>) ||
                def == typeof(IEnumerable<>) ||
                def == typeof(IReadOnlyList<>) ||
                def == typeof(IReadOnlyCollection<>))
                return From(args[0]).Array();

            // KeyValuePair<K, V> → [K, V]
            if (def == typeof(KeyValuePair<,>))
                return Tuple(From(args[0]), From(args[1]));

            // Fallback: GenericType<T1, T2, ...>
            return From(StripGenericArity(def.Name)).Of(args.Select(From).ToArray());
        }

        // Task (non-generic) → Promise<void>
        if (type == typeof(Task))
            return From("Promise").Of(From("void"));

        // Primitives and well-known types
        return MapWellKnownType(type);
    }

    private static TsTypeRef MapWellKnownType(Type type)
    {
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Boolean:
                return From("boolean");

            case TypeCode.Char:
            case TypeCode.String:
                return From("string");

            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
                return From("number");

            case TypeCode.DateTime:
                return From("Date");

            default:
                break;
        }

        if (type == typeof(void))
            return From("void");

        if (type == typeof(DateTimeOffset))
            return From("Date");

        if (type == typeof(Guid) || type == typeof(Uri))
            return From("string");

        if (type == typeof(object))
            return From("unknown");

        if (type == typeof(TimeSpan))
            return From("number");

        // Fallback: use the type name
        return From(type.Name);
    }

    private static string StripGenericArity(string name)
    {
        var backtick = name.IndexOf('`');
        return backtick >= 0 ? name.Substring(0, backtick) : name;
    }
}

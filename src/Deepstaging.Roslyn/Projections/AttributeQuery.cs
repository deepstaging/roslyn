// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Linq.Expressions;
using System.Reflection;

namespace Deepstaging.Roslyn;

/// <summary>
/// Base record for strongly-typed attribute query wrappers.
/// Provides convenient access to constructor and named arguments with Option-style chaining.
/// </summary>
/// <param name="AttributeData">The underlying Roslyn attribute data.</param>
public abstract record AttributeQuery(AttributeData AttributeData)
{
    /// <summary>
    /// Gets a constructor argument at the specified index.
    /// </summary>
    protected OptionalArgument<T> ConstructorArg<T>(int index) =>
        AttributeData.GetConstructorArgument<T>(index);

    /// <summary>
    /// Gets a named argument (property value) from the attribute.
    /// </summary>
    protected OptionalArgument<T> NamedArg<T>(string name) =>
        AttributeData.GetNamedArgument<T>(name);

    /// <summary>
    /// Gets a type argument from a generic attribute at the specified index.
    /// </summary>
    protected OptionalSymbol<INamedTypeSymbol> TypeArg(int index) =>
        ValidAttribute.From(AttributeData).GetTypeArgument(index);
}

/// <summary>
/// Cached factory for creating AttributeQuery instances.
/// Uses compiled expression trees for performance equivalent to direct constructor calls.
/// </summary>
internal static class QueryFactory<TQuery> where TQuery : AttributeQuery
{
    /// <summary>
    /// Compiled factory delegate for creating TQuery instances.
    /// </summary>
    public static readonly Func<AttributeData, TQuery> Create = BuildFactory();

    private static Func<AttributeData, TQuery> BuildFactory()
    {
        var ctor = typeof(TQuery).GetConstructor([typeof(AttributeData)]) ??
                   throw new InvalidOperationException(
                       $"{typeof(TQuery).Name} must have a constructor that takes AttributeData.");

        var param = Expression.Parameter(typeof(AttributeData), "data");
        var newExpr = Expression.New(ctor, param);
        return Expression.Lambda<Func<AttributeData, TQuery>>(newExpr, param).Compile();
    }
}

/// <summary>
/// Extension methods for converting ValidAttribute to AttributeQuery types.
/// </summary>
public static class AttributeQueryExtensions
{
    extension(ValidAttribute attribute)
    {
        /// <summary>
        /// Converts this attribute to a strongly-typed query wrapper.
        /// </summary>
        /// <typeparam name="TQuery">The AttributeQuery type to create.</typeparam>
        /// <returns>A new instance of TQuery wrapping this attribute's data.</returns>
        public TQuery AsQuery<TQuery>() where TQuery : AttributeQuery
            => QueryFactory<TQuery>.Create(attribute.Value);
    }
}
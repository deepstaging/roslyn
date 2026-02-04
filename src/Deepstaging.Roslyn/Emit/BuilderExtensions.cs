// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Extension methods for fluent builder operations.
/// </summary>
public static class BuilderExtensions
{
    /// <summary>
    /// Applies a transformation for each item in the sequence, threading the builder through.
    /// </summary>
    /// <typeparam name="TBuilder">The builder type.</typeparam>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="builder">The builder instance.</param>
    /// <param name="items">The items to iterate over.</param>
    /// <param name="folder">The transformation to apply for each item.</param>
    /// <returns>The final builder after all transformations.</returns>
    public static TBuilder WithEach<TBuilder, TItem>(
        this TBuilder builder,
        IEnumerable<TItem> items,
        Func<TBuilder, TItem, TBuilder> folder) where TBuilder : struct
    {
        return items.Aggregate(builder, folder);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="condition"></param>
    /// <param name="transform"></param>
    /// <typeparam name="TBuilder"></typeparam>
    /// <returns></returns>
    public static TBuilder If<TBuilder>(
        this TBuilder builder,
        bool condition,
        Func<TBuilder, TBuilder> transform) where TBuilder : struct
    {
        return condition ? transform(builder) : builder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="condition"></param>
    /// <param name="transform"></param>
    /// <typeparam name="TBuilder"></typeparam>
    /// <returns></returns>
    public static TBuilder IfNot<TBuilder>(
        this TBuilder builder,
        bool condition,
        Func<TBuilder, TBuilder> transform) where TBuilder : struct
    {
        return !condition ? transform(builder) : builder;
    }
}

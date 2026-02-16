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
        Func<TBuilder, TItem, TBuilder> folder) where TBuilder : struct => items.Aggregate(builder, folder);

    /// <summary>
    /// Conditionally applies a transformation when the condition is true.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="then">The transformation to apply when condition is true.</param>
    /// <param name="else">Optional transformation to apply when condition is false.</param>
    /// <typeparam name="TBuilder">The builder type.</typeparam>
    /// <returns>The transformed builder.</returns>
    public static TBuilder If<TBuilder>(
        this TBuilder builder,
        bool condition,
        Func<TBuilder, TBuilder> then,
        Func<TBuilder, TBuilder>? @else = null) where TBuilder : struct =>
        condition ? then(builder) : @else?.Invoke(builder) ?? builder;

    /// <summary>
    /// Conditionally applies a transformation when the condition is false.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="then">The transformation to apply when condition is false.</param>
    /// <param name="else">Optional transformation to apply when condition is true.</param>
    /// <typeparam name="TBuilder">The builder type.</typeparam>
    /// <returns>The transformed builder.</returns>
    public static TBuilder IfNot<TBuilder>(
        this TBuilder builder,
        bool condition,
        Func<TBuilder, TBuilder> then,
        Func<TBuilder, TBuilder>? @else = null) where TBuilder : struct =>
        !condition ? then(builder) : @else?.Invoke(builder) ?? builder;
}
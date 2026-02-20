// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Generators;

/// <summary>
/// Multi-arity <c>Combine</c> overloads that flatten nested tuples and auto-collect plural providers.
/// <para>
/// Roslyn's built-in <c>Combine</c> only pairs two providers at a time, producing nested tuples like
/// <c>(((A, B), C), D)</c> that require painful destructuring. These overloads accept 2–4 additional
/// providers, call <c>.Collect()</c> internally, and return a flat tuple.
/// </para>
/// </summary>
/// <example>
/// <code>
/// // Before — nested tuples, manual .Collect() calls:
/// var combined = webApps
///     .Combine(dispatches.Collect())
///     .Combine(commands.Collect())
///     .Combine(queries.Collect())
///     .Select(static (tuple, _) =>
///     {
///         var (((webApp, dispatches), commands), queries) = tuple;
///         ...
///     });
///
/// // After — flat tuple, no .Collect() noise:
/// var combined = webApps
///     .Combine(dispatches, commands, queries)
///     .Select(static (tuple, _) =>
///     {
///         var (webApp, dispatches, commands, queries) = tuple;
///         ...
///     });
/// </code>
/// </example>
public static class CombineExtensions
{
    // ── IncrementalValuesProvider (plural) + 2 ──────────────────────────

    /// <summary>
    /// Combines a values provider with two additional values providers.
    /// The additional providers are collected into <see cref="ImmutableArray{T}"/> automatically.
    /// </summary>
    public static IncrementalValuesProvider<(T1, ImmutableArray<T2>, ImmutableArray<T3>)>
        Combine<T1, T2, T3>(
            this IncrementalValuesProvider<T1> source,
            IncrementalValuesProvider<T2> second,
            IncrementalValuesProvider<T3> third) =>
        source
            .Combine(second.Collect())
            .Combine(third.Collect())
            .Select(static (tuple, _) => (tuple.Left.Left, tuple.Left.Right, tuple.Right));

    // ── IncrementalValuesProvider (plural) + 3 ──────────────────────────

    /// <summary>
    /// Combines a values provider with three additional values providers.
    /// The additional providers are collected into <see cref="ImmutableArray{T}"/> automatically.
    /// </summary>
    public static IncrementalValuesProvider<(T1, ImmutableArray<T2>, ImmutableArray<T3>, ImmutableArray<T4>)>
        Combine<T1, T2, T3, T4>(
            this IncrementalValuesProvider<T1> source,
            IncrementalValuesProvider<T2> second,
            IncrementalValuesProvider<T3> third,
            IncrementalValuesProvider<T4> fourth) =>
        source
            .Combine(second.Collect())
            .Combine(third.Collect())
            .Combine(fourth.Collect())
            .Select(static (tuple, _) =>
                (tuple.Left.Left.Left, tuple.Left.Left.Right, tuple.Left.Right, tuple.Right));

    // ── IncrementalValueProvider (singular) + 2 ─────────────────────────

    /// <summary>
    /// Combines a single-value provider with two additional values providers.
    /// The additional providers are collected into <see cref="ImmutableArray{T}"/> automatically.
    /// </summary>
    public static IncrementalValueProvider<(T1, ImmutableArray<T2>, ImmutableArray<T3>)>
        Combine<T1, T2, T3>(
            this IncrementalValueProvider<T1> source,
            IncrementalValuesProvider<T2> second,
            IncrementalValuesProvider<T3> third) =>
        source
            .Combine(second.Collect())
            .Combine(third.Collect())
            .Select(static (tuple, _) => (tuple.Left.Left, tuple.Left.Right, tuple.Right));

    // ── IncrementalValueProvider (singular) + 3 ─────────────────────────

    /// <summary>
    /// Combines a single-value provider with three additional values providers.
    /// The additional providers are collected into <see cref="ImmutableArray{T}"/> automatically.
    /// </summary>
    public static IncrementalValueProvider<(T1, ImmutableArray<T2>, ImmutableArray<T3>, ImmutableArray<T4>)>
        Combine<T1, T2, T3, T4>(
            this IncrementalValueProvider<T1> source,
            IncrementalValuesProvider<T2> second,
            IncrementalValuesProvider<T3> third,
            IncrementalValuesProvider<T4> fourth) =>
        source
            .Combine(second.Collect())
            .Combine(third.Collect())
            .Combine(fourth.Collect())
            .Select(static (tuple, _) =>
                (tuple.Left.Left.Left, tuple.Left.Left.Right, tuple.Left.Right, tuple.Right));
}

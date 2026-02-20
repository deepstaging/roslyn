// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Expressions;

using Types;

/// <summary>
/// Describes how a method call should be lifted into the Eff monad.
/// </summary>
/// <remarks>
/// Used with <see cref="EffLiftExtensions.Lift"/> to dispatch to the correct
/// <see cref="EffLift"/> method based on the operation's async/sync nature and return shape.
/// </remarks>
public enum LiftingStrategy
{
    /// <summary>Async operation returning a value: <c>async rt =&gt; await expr</c>.</summary>
    AsyncValue,

    /// <summary>Async operation returning void: <c>async rt =&gt; {{ await expr; return unit; }}</c>.</summary>
    AsyncVoid,

    /// <summary>Async operation returning nullable wrapped in Option: <c>async rt =&gt; Optional(await expr)</c>.</summary>
    AsyncOptional,

    /// <summary>Async operation with null-forgiving assertion: <c>async rt =&gt; (await expr)!</c>.</summary>
    AsyncNonNull,

    /// <summary>Sync operation returning a value: <c>rt =&gt; expr</c>.</summary>
    SyncValue,

    /// <summary>Sync operation returning void: <c>rt =&gt; {{ expr; return unit; }}</c>.</summary>
    SyncVoid,

    /// <summary>Sync operation returning nullable wrapped in Option: <c>rt =&gt; Optional(expr)</c>.</summary>
    SyncOptional,

    /// <summary>Sync operation with null-forgiving assertion: <c>rt =&gt; (expr)!</c>.</summary>
    SyncNonNull
}

/// <summary>
/// Extension methods for dispatching <see cref="EffLift"/> via <see cref="LiftingStrategy"/>.
/// </summary>
public static class EffLiftExtensions
{
    /// <summary>
    /// Dispatches to the correct <see cref="EffLift"/> method based on the lifting strategy.
    /// </summary>
    /// <remarks>
    /// For <see cref="LiftingStrategy.AsyncOptional"/> and <see cref="LiftingStrategy.SyncOptional"/>,
    /// <paramref name="resultType"/> is the <em>inner</em> type (e.g., <c>"User"</c>) — the method
    /// wraps it in <c>Option&lt;T&gt;</c> automatically.
    /// For <see cref="LiftingStrategy.AsyncVoid"/> and <see cref="LiftingStrategy.SyncVoid"/>,
    /// <paramref name="resultType"/> is ignored.
    /// </remarks>
    /// <param name="lift">The EffLift builder.</param>
    /// <param name="strategy">The lifting strategy to apply.</param>
    /// <param name="resultType">The raw result type (unwrapped for Optional strategies, ignored for Void strategies).</param>
    /// <param name="expr">The inner expression to lift.</param>
    public static string Lift(this EffLift lift, LiftingStrategy strategy, TypeRef resultType, string expr) =>
        strategy switch
        {
            LiftingStrategy.AsyncValue => lift.Async(resultType, expr),
            LiftingStrategy.AsyncVoid => lift.AsyncVoid(expr),
            LiftingStrategy.AsyncOptional => lift.AsyncOptional(LanguageExtTypes.Option(resultType), expr),
            LiftingStrategy.AsyncNonNull => lift.AsyncNonNull(resultType, expr),
            LiftingStrategy.SyncValue => lift.Sync(resultType, expr),
            LiftingStrategy.SyncVoid => lift.SyncVoid(expr),
            LiftingStrategy.SyncOptional => lift.SyncOptional(LanguageExtTypes.Option(resultType), expr),
            LiftingStrategy.SyncNonNull => lift.SyncNonNull(resultType, expr),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, "Unknown lifting strategy.")
        };

    /// <summary>
    /// Computes the Eff return type for a given lifting strategy.
    /// </summary>
    /// <remarks>
    /// For Optional strategies, wraps <paramref name="resultType"/> in <c>Option&lt;T&gt;</c>.
    /// For Void strategies, returns <c>Unit</c>.
    /// For all others, returns <paramref name="resultType"/> as-is.
    /// </remarks>
    /// <param name="strategy">The lifting strategy.</param>
    /// <param name="resultType">The raw result type.</param>
    public static TypeRef EffReturnType(this LiftingStrategy strategy, TypeRef resultType) =>
        strategy switch
        {
            LiftingStrategy.AsyncOptional or LiftingStrategy.SyncOptional =>
                LanguageExtTypes.Option(resultType),
            LiftingStrategy.AsyncVoid or LiftingStrategy.SyncVoid =>
                LanguageExtTypes.Unit,
            _ => resultType
        };
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Expressions;

using Microsoft.CodeAnalysis;
using static LiftingStrategy;

/// <summary>
/// Extension methods for determining <see cref="LiftingStrategy"/> from Roslyn method symbols.
/// </summary>
public static class LiftingStrategyAnalysis
{
    /// <summary>
    /// Determines the appropriate <see cref="LiftingStrategy"/> for lifting a method into <c>Eff</c>
    /// based on its async nature and return type nullability.
    /// </summary>
    /// <param name="method">The method symbol to analyze.</param>
    public static LiftingStrategy DetermineLiftingStrategy(this ValidSymbol<IMethodSymbol> method) =>
        method.AsyncKind switch
        {
            AsyncMethodKind.Void => AsyncVoid,
            AsyncMethodKind.Value => method.ReturnType.InnerTaskType switch
            {
                { IsEmpty: true } => throw new InvalidOperationException(
                    "Async methods with return type Task must have a type argument."
                ),
                { IsNullable: true } => AsyncOptional,
                _ => AsyncValue
            },
            AsyncMethodKind.NotAsync => method.ReturnType.GetFirstTypeArgument() switch
            {
                { IsEmpty: true } => method.ReturnsVoid ? SyncVoid :
                    method.ReturnType.IsNullable ? SyncOptional : SyncValue,
                { IsNullable: true } => SyncOptional,
                _ => SyncValue
            },
            _ => SyncVoid
        };

    /// <summary>
    /// Computes the raw effect result type for a method given its lifting strategy.
    /// Returns the unwrapped inner type (e.g., <c>"User"</c> not <c>"Option&lt;User&gt;"</c>).
    /// </summary>
    /// <param name="method">The method symbol to extract the result type from.</param>
    /// <param name="strategy">The lifting strategy previously determined for this method.</param>
    public static string EffectResultType(this ValidSymbol<IMethodSymbol> method, LiftingStrategy strategy) =>
        strategy switch
        {
            AsyncVoid or SyncVoid => "Unit",
            AsyncValue or AsyncOptional or AsyncNonNull =>
                method.ReturnType
                    .GetFirstTypeArgument()
                    .Map(type => type.GloballyQualifiedName)
                    .OrThrow("Expected a type argument for async return type."),
            _ => method.ReturnType.GloballyQualifiedName
        };
}

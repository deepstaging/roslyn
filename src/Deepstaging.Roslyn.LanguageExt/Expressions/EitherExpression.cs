// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Expressions;

using Emit.Refs;

/// <summary>
/// Builds <c>Either</c> construction expressions using LanguageExt Prelude functions.
/// </summary>
/// <remarks>
/// These expressions require <c>using static LanguageExt.Prelude</c> in the generated code.
/// Use <see cref="Extensions.TypeBuilderExtensions.AddLanguageExtUsings"/> to add the standard usings.
/// </remarks>
public static class EitherExpression
{
    /// <summary>
    /// <c>Right({expr})</c> — constructs the success (right) side of an <c>Either&lt;L, R&gt;</c>.
    /// </summary>
    /// <param name="expr">The right (success) value expression.</param>
    public static ExpressionRef Right(ExpressionRef expr) =>
        ExpressionRef.From($"Right({expr.Value})");

    /// <summary>
    /// <c>Left({expr})</c> — constructs the failure (left) side of an <c>Either&lt;L, R&gt;</c>.
    /// </summary>
    /// <param name="expr">The left (error) value expression.</param>
    public static ExpressionRef Left(ExpressionRef expr) =>
        ExpressionRef.From($"Left({expr.Value})");
}

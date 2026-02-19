// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Expressions;

/// <summary>
/// Builds <c>Option</c> construction expressions using LanguageExt Prelude functions.
/// </summary>
/// <remarks>
/// These expressions require <c>using static LanguageExt.Prelude</c> in the generated code.
/// Use <see cref="TypeBuilderExtensions.AddLanguageExtUsings"/> to add the standard usings.
/// </remarks>
public static class OptionExpression
{
    /// <summary>
    /// <c>Optional({expr})</c> — wraps a nullable value in <c>Option&lt;T&gt;</c>.
    /// </summary>
    /// <param name="expr">The expression that may be null.</param>
    public static ExpressionRef Optional(ExpressionRef expr) =>
        ExpressionRef.From($"Optional({expr.Value})");

    /// <summary>
    /// <c>Some({expr})</c> — wraps a non-null value in <c>Option&lt;T&gt;</c>. Throws if null.
    /// </summary>
    /// <param name="expr">The expression that must not be null.</param>
    public static ExpressionRef Some(ExpressionRef expr) =>
        ExpressionRef.From($"Some({expr.Value})");

    /// <summary>
    /// <c>None</c> — the empty <c>Option</c> value.
    /// </summary>
    public static ExpressionRef None =>
        ExpressionRef.From("None");
}
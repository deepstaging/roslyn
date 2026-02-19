// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Expressions;

/// <summary>
/// Builds <c>Fin</c> construction expressions using LanguageExt Prelude functions.
/// </summary>
/// <remarks>
/// These expressions require <c>using static LanguageExt.Prelude</c> in the generated code.
/// Use <see cref="TypeBuilderExtensions.AddLanguageExtUsings"/> to add the standard usings.
/// </remarks>
public static class FinExpression
{
    /// <summary>
    /// <c>FinSucc({expr})</c> — constructs a successful <c>Fin&lt;A&gt;</c>.
    /// </summary>
    /// <param name="expr">The success value expression.</param>
    public static ExpressionRef FinSucc(ExpressionRef expr) =>
        ExpressionRef.From($"FinSucc({expr.Value})");

    /// <summary>
    /// <c>FinFail({expr})</c> — constructs a failed <c>Fin&lt;A&gt;</c> from an <c>Error</c>.
    /// </summary>
    /// <param name="expr">The error expression.</param>
    public static ExpressionRef FinFail(ExpressionRef expr) =>
        ExpressionRef.From($"FinFail({expr.Value})");

    /// <summary>
    /// <c>FinFail(Error.New({message}))</c> — constructs a failed <c>Fin&lt;A&gt;</c> from a message string.
    /// </summary>
    /// <param name="message">The error message expression (e.g., a string literal or interpolation).</param>
    public static ExpressionRef FinFailMessage(ExpressionRef message) =>
        ExpressionRef.From($"FinFail(Error.New({message.Value}))");
}
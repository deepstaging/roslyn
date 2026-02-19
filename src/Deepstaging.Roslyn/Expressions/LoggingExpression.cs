// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

/// <summary>
/// Expression factory for <c>Microsoft.Extensions.Logging</c> log method patterns.
/// </summary>
public static class LoggingExpression
{
    /// <summary>Produces <c>logger.LogTrace(args)</c>.</summary>
    public static ExpressionRef LogTrace(ExpressionRef logger, params ExpressionRef[] args) =>
        logger.Call("LogTrace", args);

    /// <summary>Produces <c>logger.LogDebug(args)</c>.</summary>
    public static ExpressionRef LogDebug(ExpressionRef logger, params ExpressionRef[] args) =>
        logger.Call("LogDebug", args);

    /// <summary>Produces <c>logger.LogInformation(args)</c>.</summary>
    public static ExpressionRef LogInformation(ExpressionRef logger, params ExpressionRef[] args) =>
        logger.Call("LogInformation", args);

    /// <summary>Produces <c>logger.LogWarning(args)</c>.</summary>
    public static ExpressionRef LogWarning(ExpressionRef logger, params ExpressionRef[] args) =>
        logger.Call("LogWarning", args);

    /// <summary>Produces <c>logger.LogError(args)</c>.</summary>
    public static ExpressionRef LogError(ExpressionRef logger, params ExpressionRef[] args) =>
        logger.Call("LogError", args);

    /// <summary>Produces <c>logger.LogCritical(args)</c>.</summary>
    public static ExpressionRef LogCritical(ExpressionRef logger, params ExpressionRef[] args) =>
        logger.Call("LogCritical", args);
}

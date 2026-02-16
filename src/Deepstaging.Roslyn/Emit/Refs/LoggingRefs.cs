// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Factory methods for <c>Microsoft.Extensions.Logging</c> types.
/// </summary>
public static class LoggingRefs
{
    /// <summary>Gets the <c>Microsoft.Extensions.Logging</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("Microsoft.Extensions.Logging");

    /// <summary>Gets an <c>ILogger</c> type reference.</summary>
    public static TypeRef ILogger => Namespace.GlobalType("ILogger");

    /// <summary>Creates an <c>ILogger&lt;T&gt;</c> type reference.</summary>
    public static TypeRef ILoggerOf(TypeRef categoryType) =>
        Namespace.GlobalType($"ILogger<{categoryType.Value}>");

    /// <summary>Gets an <c>ILoggerFactory</c> type reference.</summary>
    public static TypeRef ILoggerFactory => Namespace.GlobalType("ILoggerFactory");

    /// <summary>Gets a <c>LogLevel</c> type reference.</summary>
    public static TypeRef LogLevel => Namespace.GlobalType("LogLevel");

    // ── Well-Known API Calls ────────────────────────────────────────────

    /// <summary>Produces a <c>logger.LogTrace(message, args)</c> expression.</summary>
    public static ExpressionRef LogTrace(ExpressionRef logger, params ExpressionRef[] args) =>
        ExpressionRef.From(logger).Call("LogTrace", args);

    /// <summary>Produces a <c>logger.LogDebug(message, args)</c> expression.</summary>
    public static ExpressionRef LogDebug(ExpressionRef logger, params ExpressionRef[] args) =>
        ExpressionRef.From(logger).Call("LogDebug", args);

    /// <summary>Produces a <c>logger.LogInformation(message, args)</c> expression.</summary>
    public static ExpressionRef LogInformation(ExpressionRef logger, params ExpressionRef[] args) =>
        ExpressionRef.From(logger).Call("LogInformation", args);

    /// <summary>Produces a <c>logger.LogWarning(message, args)</c> expression.</summary>
    public static ExpressionRef LogWarning(ExpressionRef logger, params ExpressionRef[] args) =>
        ExpressionRef.From(logger).Call("LogWarning", args);

    /// <summary>Produces a <c>logger.LogError(message, args)</c> expression.</summary>
    public static ExpressionRef LogError(ExpressionRef logger, params ExpressionRef[] args) =>
        ExpressionRef.From(logger).Call("LogError", args);

    /// <summary>Produces a <c>logger.LogCritical(message, args)</c> expression.</summary>
    public static ExpressionRef LogCritical(ExpressionRef logger, params ExpressionRef[] args) =>
        ExpressionRef.From(logger).Call("LogCritical", args);
}
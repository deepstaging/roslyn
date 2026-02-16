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
    public static TypeRef ILogger => Namespace.Type("ILogger");

    /// <summary>Creates an <c>ILogger&lt;T&gt;</c> type reference.</summary>
    public static TypeRef ILoggerOf(TypeRef categoryType) =>
        Namespace.Type($"ILogger<{categoryType.Value}>");

    /// <summary>Gets an <c>ILoggerFactory</c> type reference.</summary>
    public static TypeRef ILoggerFactory => Namespace.Type("ILoggerFactory");

    /// <summary>Gets a <c>LogLevel</c> type reference.</summary>
    public static TypeRef LogLevel => Namespace.Type("LogLevel");
}

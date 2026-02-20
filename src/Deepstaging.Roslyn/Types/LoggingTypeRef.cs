// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// A type-safe wrapper representing an <c>ILogger&lt;T&gt;</c> type reference.
/// Carries the category type for typed expression building.
/// </summary>
public readonly record struct LoggerTypeRef
{
    /// <summary>Gets the category type (e.g., <c>"MyService"</c>).</summary>
    public TypeRef CategoryType { get; }

    /// <summary>Creates a <c>LoggerTypeRef</c> for the given category type.</summary>
    public LoggerTypeRef(TypeRef categoryType) => CategoryType = categoryType;

    /// <summary>Gets the globally qualified <c>ILogger&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::Microsoft.Extensions.Logging.ILogger<{CategoryType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(LoggerTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(LoggerTypeRef self) =>
        self.ToString();
}

/// <summary>
/// Convenience <see cref="TypeRef"/> constants for <c>Microsoft.Extensions.Logging</c> types.
/// </summary>
public static class LoggingTypes
{
    /// <summary>Gets the <c>Microsoft.Extensions.Logging</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("Microsoft.Extensions.Logging");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>ILogger</c> (non-generic).</summary>
    public static TypeRef ILogger => Namespace.GlobalType("ILogger");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>ILoggerFactory</c>.</summary>
    public static TypeRef ILoggerFactory => Namespace.GlobalType("ILoggerFactory");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>LogLevel</c>.</summary>
    public static TypeRef LogLevel => Namespace.GlobalType("LogLevel");

    /// <summary>Creates an <c>ILogger&lt;T&gt;</c> type reference.</summary>
    public static LoggerTypeRef Logger(TypeRef categoryType) => new(categoryType);
}

// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Factory members for common <c>System</c> exception types.
/// </summary>
public static class ExceptionRefs
{
    /// <summary>Gets the <c>System</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System");

    /// <summary>Gets an <c>ArgumentNullException</c> type reference.</summary>
    public static TypeRef ArgumentNull => Namespace.GlobalType("ArgumentNullException");

    /// <summary>Gets an <c>ArgumentException</c> type reference.</summary>
    public static TypeRef Argument => Namespace.GlobalType("ArgumentException");

    /// <summary>Gets an <c>ArgumentOutOfRangeException</c> type reference.</summary>
    public static TypeRef ArgumentOutOfRange => Namespace.GlobalType("ArgumentOutOfRangeException");

    /// <summary>Gets an <c>InvalidOperationException</c> type reference.</summary>
    public static TypeRef InvalidOperation => Namespace.GlobalType("InvalidOperationException");

    /// <summary>Gets an <c>InvalidCastException</c> type reference.</summary>
    public static TypeRef InvalidCast => Namespace.GlobalType("InvalidCastException");

    /// <summary>Gets a <c>FormatException</c> type reference.</summary>
    public static TypeRef Format => Namespace.GlobalType("FormatException");

    /// <summary>Gets a <c>NotSupportedException</c> type reference.</summary>
    public static TypeRef NotSupported => Namespace.GlobalType("NotSupportedException");

    /// <summary>Gets a <c>NotImplementedException</c> type reference.</summary>
    public static TypeRef NotImplemented => Namespace.GlobalType("NotImplementedException");

    // ── Well-Known API Calls ────────────────────────────────────────────

    /// <summary>Produces an <c>ArgumentNullException.ThrowIfNull(value)</c> expression.</summary>
    public static ExpressionRef ThrowIfNull(ExpressionRef value) =>
        ArgumentNull.Call("ThrowIfNull", value);

    /// <summary>Produces an <c>ArgumentException.ThrowIfNullOrEmpty(value)</c> expression.</summary>
    public static ExpressionRef ThrowIfNullOrEmpty(ExpressionRef value) =>
        Argument.Call("ThrowIfNullOrEmpty", value);

    /// <summary>Produces an <c>ArgumentException.ThrowIfNullOrWhiteSpace(value)</c> expression.</summary>
    public static ExpressionRef ThrowIfNullOrWhiteSpace(ExpressionRef value) =>
        Argument.Call("ThrowIfNullOrWhiteSpace", value);

    /// <summary>Produces an <c>ArgumentOutOfRangeException.ThrowIfNegative(value)</c> expression.</summary>
    public static ExpressionRef ThrowIfNegative(ExpressionRef value) =>
        ArgumentOutOfRange.Call("ThrowIfNegative", value);

    /// <summary>Produces an <c>ArgumentOutOfRangeException.ThrowIfZero(value)</c> expression.</summary>
    public static ExpressionRef ThrowIfZero(ExpressionRef value) =>
        ArgumentOutOfRange.Call("ThrowIfZero", value);
}
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
    public static TypeRef ArgumentNull => Namespace.Type("ArgumentNullException");

    /// <summary>Gets an <c>ArgumentException</c> type reference.</summary>
    public static TypeRef Argument => Namespace.Type("ArgumentException");

    /// <summary>Gets an <c>ArgumentOutOfRangeException</c> type reference.</summary>
    public static TypeRef ArgumentOutOfRange => Namespace.Type("ArgumentOutOfRangeException");

    /// <summary>Gets an <c>InvalidOperationException</c> type reference.</summary>
    public static TypeRef InvalidOperation => Namespace.Type("InvalidOperationException");

    /// <summary>Gets an <c>InvalidCastException</c> type reference.</summary>
    public static TypeRef InvalidCast => Namespace.Type("InvalidCastException");

    /// <summary>Gets a <c>FormatException</c> type reference.</summary>
    public static TypeRef Format => Namespace.Type("FormatException");

    /// <summary>Gets a <c>NotSupportedException</c> type reference.</summary>
    public static TypeRef NotSupported => Namespace.Type("NotSupportedException");

    /// <summary>Gets a <c>NotImplementedException</c> type reference.</summary>
    public static TypeRef NotImplemented => Namespace.Type("NotImplementedException");
}

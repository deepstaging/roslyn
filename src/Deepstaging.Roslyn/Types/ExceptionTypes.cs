// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// Convenience <see cref="TypeRef"/> constants for common <c>System</c> exception types.
/// </summary>
public static class ExceptionTypes
{
    /// <summary>Gets a <see cref="TypeRef"/> for <c>Exception</c>.</summary>
    public static TypeRef Exception => TypeRef.Global("System.Exception");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>InvalidOperationException</c>.</summary>
    public static TypeRef InvalidOperation => TypeRef.Global("System.InvalidOperationException");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>ArgumentException</c>.</summary>
    public static TypeRef Argument => TypeRef.Global("System.ArgumentException");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>ArgumentNullException</c>.</summary>
    public static TypeRef ArgumentNull => TypeRef.Global("System.ArgumentNullException");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>ArgumentOutOfRangeException</c>.</summary>
    public static TypeRef ArgumentOutOfRange => TypeRef.Global("System.ArgumentOutOfRangeException");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>NotSupportedException</c>.</summary>
    public static TypeRef NotSupported => TypeRef.Global("System.NotSupportedException");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>NotImplementedException</c>.</summary>
    public static TypeRef NotImplemented => TypeRef.Global("System.NotImplementedException");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>KeyNotFoundException</c>.</summary>
    public static TypeRef KeyNotFound => TypeRef.Global("System.Collections.Generic.KeyNotFoundException");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>ObjectDisposedException</c>.</summary>
    public static TypeRef ObjectDisposed => TypeRef.Global("System.ObjectDisposedException");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>OperationCanceledException</c>.</summary>
    public static TypeRef OperationCanceled => TypeRef.Global("System.OperationCanceledException");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>FormatException</c>.</summary>
    public static TypeRef Format => TypeRef.Global("System.FormatException");
}

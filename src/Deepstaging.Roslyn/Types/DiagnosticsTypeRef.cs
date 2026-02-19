// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// Convenience <see cref="TypeRef"/> constants for <c>System.Diagnostics</c> types.
/// </summary>
public static class DiagnosticsTypes
{
    /// <summary>Gets the <c>System.Diagnostics</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System.Diagnostics");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>Activity</c>.</summary>
    public static TypeRef Activity => Namespace.GlobalType("Activity");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>ActivitySource</c>.</summary>
    public static TypeRef ActivitySource => Namespace.GlobalType("ActivitySource");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>ActivityKind</c>.</summary>
    public static TypeRef ActivityKind => Namespace.GlobalType("ActivityKind");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>ActivityStatusCode</c>.</summary>
    public static TypeRef ActivityStatusCode => Namespace.GlobalType("ActivityStatusCode");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>DiagnosticSource</c>.</summary>
    public static TypeRef DiagnosticSource => Namespace.GlobalType("DiagnosticSource");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>Stopwatch</c>.</summary>
    public static TypeRef Stopwatch => Namespace.GlobalType("Stopwatch");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>Process</c>.</summary>
    public static TypeRef Process => Namespace.GlobalType("Process");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>Debug</c>.</summary>
    public static TypeRef Debug => Namespace.GlobalType("Debug");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>Trace</c>.</summary>
    public static TypeRef Trace => Namespace.GlobalType("Trace");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>Debugger</c>.</summary>
    public static TypeRef Debugger => Namespace.GlobalType("Debugger");
}

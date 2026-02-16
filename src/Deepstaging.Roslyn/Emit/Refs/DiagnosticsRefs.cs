// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Factory members for common <c>System.Diagnostics</c> types.
/// </summary>
public static class DiagnosticsRefs
{
    /// <summary>Gets the <c>System.Diagnostics</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System.Diagnostics");

    // ── OpenTelemetry / Activity ────────────────────────────────────────

    /// <summary>Gets an <c>Activity</c> type reference.</summary>
    public static TypeRef Activity => Namespace.GlobalType("Activity");

    /// <summary>Gets an <c>ActivitySource</c> type reference.</summary>
    public static TypeRef ActivitySource => Namespace.GlobalType("ActivitySource");

    /// <summary>Gets an <c>ActivityKind</c> type reference.</summary>
    public static TypeRef ActivityKind => Namespace.GlobalType("ActivityKind");

    /// <summary>Gets an <c>ActivityStatusCode</c> type reference.</summary>
    public static TypeRef ActivityStatusCode => Namespace.GlobalType("ActivityStatusCode");

    /// <summary>Gets a <c>DiagnosticSource</c> type reference.</summary>
    public static TypeRef DiagnosticSource => Namespace.GlobalType("DiagnosticSource");

    // ── Debug / Trace ───────────────────────────────────────────────────

    /// <summary>Gets a <c>Debug</c> type reference.</summary>
    public static TypeRef Debug => Namespace.GlobalType("Debug");

    /// <summary>Gets a <c>Trace</c> type reference.</summary>
    public static TypeRef Trace => Namespace.GlobalType("Trace");

    /// <summary>Gets a <c>Debugger</c> type reference.</summary>
    public static TypeRef Debugger => Namespace.GlobalType("Debugger");

    /// <summary>Gets a <c>Stopwatch</c> type reference.</summary>
    public static TypeRef Stopwatch => Namespace.GlobalType("Stopwatch");

    /// <summary>Gets a <c>Process</c> type reference.</summary>
    public static TypeRef Process => Namespace.GlobalType("Process");

    // ── Well-Known API Calls ────────────────────────────────────────────

    /// <summary>Produces a <c>source.StartActivity(name)</c> expression.</summary>
    public static ExpressionRef StartActivity(ExpressionRef source, ExpressionRef name) =>
        ExpressionRef.From(source).Call("StartActivity", name);

    /// <summary>Produces a <c>source.StartActivity(name, kind)</c> expression.</summary>
    public static ExpressionRef StartActivity(ExpressionRef source, ExpressionRef name, ExpressionRef kind) =>
        ExpressionRef.From(source).Call("StartActivity", name, kind);

    /// <summary>Produces an <c>activity.SetTag(key, value)</c> expression.</summary>
    public static ExpressionRef SetTag(ExpressionRef activity, ExpressionRef key, ExpressionRef value) =>
        ExpressionRef.From(activity).Call("SetTag", key, value);

    /// <summary>Produces an <c>activity.SetStatus(statusCode)</c> expression.</summary>
    public static ExpressionRef SetStatus(ExpressionRef activity, ExpressionRef statusCode) =>
        ExpressionRef.From(activity).Call("SetStatus", statusCode);

    /// <summary>Produces a <c>Stopwatch.StartNew()</c> expression.</summary>
    public static ExpressionRef StartNew() => Stopwatch.Call("StartNew");

    /// <summary>Produces a <c>Debug.Assert(condition)</c> expression.</summary>
    public static ExpressionRef Assert(ExpressionRef condition) =>
        Debug.Call("Assert", condition);

    /// <summary>Produces a <c>Debug.Assert(condition, message)</c> expression.</summary>
    public static ExpressionRef Assert(ExpressionRef condition, ExpressionRef message) =>
        Debug.Call("Assert", condition, message);
}

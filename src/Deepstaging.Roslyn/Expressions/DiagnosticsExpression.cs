// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

using Deepstaging.Roslyn.Types;

/// <summary>
/// Expression factory for <c>System.Diagnostics</c> operations.
/// </summary>
public static class DiagnosticsExpression
{
    /// <summary>Produces <c>source.StartActivity(name)</c>.</summary>
    public static ExpressionRef StartActivity(ExpressionRef source, ExpressionRef name) =>
        source.Call("StartActivity", name);

    /// <summary>Produces <c>source.StartActivity(name, kind)</c>.</summary>
    public static ExpressionRef StartActivity(ExpressionRef source, ExpressionRef name, ExpressionRef kind) =>
        source.Call("StartActivity", name, kind);

    /// <summary>Produces <c>activity.SetTag(key, value)</c>.</summary>
    public static ExpressionRef SetTag(ExpressionRef activity, ExpressionRef key, ExpressionRef value) =>
        activity.Call("SetTag", key, value);

    /// <summary>Produces <c>activity.SetStatus(statusCode)</c>.</summary>
    public static ExpressionRef SetStatus(ExpressionRef activity, ExpressionRef statusCode) =>
        activity.Call("SetStatus", statusCode);

    /// <summary>Produces <c>Stopwatch.StartNew()</c>.</summary>
    public static ExpressionRef StartNew() => DiagnosticsTypes.Stopwatch.Call("StartNew");

    /// <summary>Produces <c>Debug.Assert(condition)</c>.</summary>
    public static ExpressionRef Assert(ExpressionRef condition) =>
        DiagnosticsTypes.Debug.Call("Assert", condition);

    /// <summary>Produces <c>Debug.Assert(condition, message)</c>.</summary>
    public static ExpressionRef Assert(ExpressionRef condition, ExpressionRef message) =>
        DiagnosticsTypes.Debug.Call("Assert", condition, message);
}

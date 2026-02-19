// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

/// <summary>
/// Builds <c>IDisposable</c> and <c>IAsyncDisposable</c> expressions for use in generated code.
/// </summary>
/// <example>
/// <code>
/// DisposableExpression.Dispose("_connection")
///     // "_connection.Dispose()"
///
/// DisposableExpression.DisposeAsync("_connection")
///     // "await _connection.DisposeAsync()"
///
/// DisposableExpression.ConditionalDispose("_connection")
///     // "(_connection as global::System.IDisposable)?.Dispose()"
/// </code>
/// </example>
public static class DisposableExpression
{
    private static readonly TypeRef IDisposableType =
        TypeRef.Global("System.IDisposable");

    private static readonly TypeRef IAsyncDisposableType =
        TypeRef.Global("System.IAsyncDisposable");

    // ── Synchronous Disposal ────────────────────────────────────────────

    /// <summary>
    /// <c>expr.Dispose()</c> — calls Dispose on the expression.
    /// </summary>
    /// <param name="expr">The disposable instance expression.</param>
    public static ExpressionRef Dispose(ExpressionRef expr) =>
        expr.Call("Dispose");

    /// <summary>
    /// <c>(expr as IDisposable)?.Dispose()</c> — safe disposal with type check.
    /// </summary>
    /// <param name="expr">The expression that may be disposable.</param>
    public static ExpressionRef ConditionalDispose(ExpressionRef expr) =>
        expr.Parenthesize().As(IDisposableType).Parenthesize().NullConditionalCall("Dispose");

    // ── Asynchronous Disposal ───────────────────────────────────────────

    /// <summary>
    /// <c>await expr.DisposeAsync()</c> — calls DisposeAsync and awaits it.
    /// </summary>
    /// <param name="expr">The async-disposable instance expression.</param>
    public static ExpressionRef DisposeAsync(ExpressionRef expr) =>
        expr.Call("DisposeAsync").Await();

    /// <summary>
    /// <c>expr.DisposeAsync()</c> — calls DisposeAsync without awaiting (for use in expression context).
    /// </summary>
    /// <param name="expr">The async-disposable instance expression.</param>
    public static ExpressionRef DisposeAsyncCall(ExpressionRef expr) =>
        expr.Call("DisposeAsync");

    /// <summary>
    /// <c>await ((expr as IAsyncDisposable)?.DisposeAsync() ?? default)</c> — safe async disposal with type check.
    /// </summary>
    /// <param name="expr">The expression that may be async-disposable.</param>
    public static ExpressionRef ConditionalDisposeAsync(ExpressionRef expr) =>
        expr.Parenthesize().As(IAsyncDisposableType).Parenthesize()
            .NullConditionalCall("DisposeAsync")
            .OrDefault("default")
            .Await();
}
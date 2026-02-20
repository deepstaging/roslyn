// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class ExceptionExpressionTests
{
    [Test]
    public async Task New_with_argument_produces_constructor()
    {
        var expr = ExceptionExpression.New(ExceptionTypes.ArgumentNull, "nameof(value)");

        await Assert.That(expr.Value)
            .IsEqualTo("new global::System.ArgumentNullException(nameof(value))");
    }

    [Test]
    public async Task ThrowNew_produces_throw_statement()
    {
        var expr = ExceptionExpression.ThrowNew(ExceptionTypes.InvalidOperation, "\"Item not found\"");

        await Assert.That(expr.Value)
            .IsEqualTo("throw new global::System.InvalidOperationException(\"Item not found\")");
    }

    [Test]
    public async Task ThrowIfNull_produces_guard_clause()
    {
        var expr = ExceptionExpression.ThrowIfNull("value");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.ArgumentNullException.ThrowIfNull(value)");
    }

    [Test]
    public async Task ThrowIfNullOrEmpty_produces_guard_clause()
    {
        var expr = ExceptionExpression.ThrowIfNullOrEmpty("value");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.ArgumentException.ThrowIfNullOrEmpty(value)");
    }

    [Test]
    public async Task ThrowIfNullOrWhiteSpace_produces_guard_clause()
    {
        var expr = ExceptionExpression.ThrowIfNullOrWhiteSpace("value");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.ArgumentException.ThrowIfNullOrWhiteSpace(value)");
    }

    [Test]
    public async Task ThrowIfNegative_produces_guard_clause()
    {
        var expr = ExceptionExpression.ThrowIfNegative("count");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.ArgumentOutOfRangeException.ThrowIfNegative(count)");
    }

    [Test]
    public async Task ThrowIfZero_produces_guard_clause()
    {
        var expr = ExceptionExpression.ThrowIfZero("count");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.ArgumentOutOfRangeException.ThrowIfZero(count)");
    }

    [Test]
    public async Task ThrowIfDisposed_produces_guard_clause()
    {
        var expr = ExceptionExpression.ThrowIfDisposed("_disposed", "this");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.ObjectDisposedException.ThrowIf(_disposed, this)");
    }

    [Test]
    public async Task New_without_arguments_produces_parameterless_constructor()
    {
        var expr = ExceptionExpression.New(ExceptionTypes.InvalidOperation);

        await Assert.That(expr.Value)
            .IsEqualTo("new global::System.InvalidOperationException()");
    }
}

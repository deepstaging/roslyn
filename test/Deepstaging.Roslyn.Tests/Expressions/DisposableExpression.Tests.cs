// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class DisposableExpressionTests
{
    [Test]
    public async Task Dispose_produces_call()
    {
        var expr = DisposableExpression.Dispose("_connection");

        await Assert.That(expr.Value).IsEqualTo("_connection.Dispose()");
    }

    [Test]
    public async Task ConditionalDispose_produces_safe_disposal()
    {
        var expr = DisposableExpression.ConditionalDispose("_field");

        await Assert.That(expr.Value)
            .IsEqualTo("((_field) as global::System.IDisposable)?.Dispose()");
    }

    [Test]
    public async Task DisposeAsync_produces_awaited_call()
    {
        var expr = DisposableExpression.DisposeAsync("_connection");

        await Assert.That(expr.Value).IsEqualTo("await _connection.DisposeAsync()");
    }

    [Test]
    public async Task DisposeAsyncCall_produces_unawaited_call()
    {
        var expr = DisposableExpression.DisposeAsyncCall("_connection");

        await Assert.That(expr.Value).IsEqualTo("_connection.DisposeAsync()");
    }

    [Test]
    public async Task ConditionalDisposeAsync_produces_safe_async_disposal()
    {
        var expr = DisposableExpression.ConditionalDisposeAsync("_field");

        await Assert.That(expr.Value)
            .IsEqualTo("await ((_field) as global::System.IAsyncDisposable)?.DisposeAsync() ?? default");
    }
}
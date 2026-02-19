// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class LazyExpressionTests
{
    [Test]
    public async Task New_produces_constructor()
    {
        var expr = LazyExpression.New("ExpensiveService", "() => new ExpensiveService()");

        await Assert.That(expr.Value)
            .IsEqualTo("new global::System.Lazy<ExpensiveService>(() => new ExpensiveService())");
    }

    [Test]
    public async Task New_with_thread_safety_produces_constructor()
    {
        var expr = LazyExpression.New("IConnection", "() => CreateConnection()", "true");

        await Assert.That(expr.Value)
            .IsEqualTo("new global::System.Lazy<IConnection>(() => CreateConnection(), true)");
    }

    [Test]
    public async Task Value_produces_member_access()
    {
        var expr = LazyExpression.Value("_lazyService");

        await Assert.That(expr.Value).IsEqualTo("_lazyService.Value");
    }

    [Test]
    public async Task IsValueCreated_produces_member_access()
    {
        var expr = LazyExpression.IsValueCreated("_lazyService");

        await Assert.That(expr.Value).IsEqualTo("_lazyService.IsValueCreated");
    }
}
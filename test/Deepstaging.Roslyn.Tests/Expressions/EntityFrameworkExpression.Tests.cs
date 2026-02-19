// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class EntityFrameworkExpressionTests
{
    [Test]
    public async Task Set_produces_expression()
    {
        var expr = EntityFrameworkExpression.Set("context", "Customer");

        await Assert.That(expr.Value).Contains("Set<Customer>()");
    }

    [Test]
    public async Task SaveChangesAsync_produces_expression()
    {
        var expr = EntityFrameworkExpression.SaveChangesAsync("context");

        await Assert.That(expr.Value).Contains("SaveChangesAsync");
    }

    [Test]
    public async Task FindAsync_produces_expression()
    {
        var expr = EntityFrameworkExpression.FindAsync("dbSet", "id");

        await Assert.That(expr.Value).Contains("FindAsync");
    }
}

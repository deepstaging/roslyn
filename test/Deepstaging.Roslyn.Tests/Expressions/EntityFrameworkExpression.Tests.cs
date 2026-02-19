// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class EntityFrameworkExpressionTests
{
    [Test]
    public async Task Set_produces_correct_expression()
    {
        var expr = EntityFrameworkExpression.Set("_context", "Customer");

        await Assert.That(expr.Value).IsEqualTo("_context.Set<Customer>()");
    }

    [Test]
    public async Task SaveChangesAsync_produces_correct_expression()
    {
        var expr = EntityFrameworkExpression.SaveChangesAsync("_context");

        await Assert.That(expr.Value).IsEqualTo("_context.SaveChangesAsync()");
    }

    [Test]
    public async Task SaveChangesAsync_with_cancellation_produces_correct_expression()
    {
        var expr = EntityFrameworkExpression.SaveChangesAsync("_context", "ct");

        await Assert.That(expr.Value).IsEqualTo("_context.SaveChangesAsync(ct)");
    }

    [Test]
    public async Task FindAsync_produces_correct_expression()
    {
        var expr = EntityFrameworkExpression.FindAsync("dbSet", "id");

        await Assert.That(expr.Value).IsEqualTo("dbSet.FindAsync(id)");
    }

    [Test]
    public async Task AddAsync_produces_correct_expression()
    {
        var expr = EntityFrameworkExpression.AddAsync("dbSet", "entity");

        await Assert.That(expr.Value).IsEqualTo("dbSet.AddAsync(entity)");
    }

    [Test]
    public async Task Remove_produces_correct_expression()
    {
        var expr = EntityFrameworkExpression.Remove("dbSet", "entity");

        await Assert.That(expr.Value).IsEqualTo("dbSet.Remove(entity)");
    }
}

// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class TaskExpressionTests
{
    [Test]
    public async Task CompletedTask_produces_expression()
    {
        var expr = TaskExpression.CompletedTask;

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Threading.Tasks.Task.CompletedTask");
    }

    [Test]
    public async Task CompletedValueTask_produces_expression()
    {
        var expr = TaskExpression.CompletedValueTask;

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Threading.Tasks.ValueTask.CompletedTask");
    }

    [Test]
    public async Task FromResult_produces_expression()
    {
        var expr = TaskExpression.FromResult("42");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Threading.Tasks.Task.FromResult(42)");
    }

    [Test]
    public async Task FromResult_with_type_produces_expression()
    {
        var expr = TaskExpression.FromResult("int", "42");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Threading.Tasks.Task.FromResult<int>(42)");
    }

    [Test]
    public async Task Run_produces_expression()
    {
        var expr = TaskExpression.Run("() => DoWork()");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Threading.Tasks.Task.Run(() => DoWork())");
    }

    [Test]
    public async Task WhenAll_produces_expression()
    {
        var expr = TaskExpression.WhenAll("task1", "task2");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Threading.Tasks.Task.WhenAll(task1, task2)");
    }

    [Test]
    public async Task FromValueTaskResult_produces_expression()
    {
        var expr = TaskExpression.FromValueTaskResult("default!");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Threading.Tasks.ValueTask.FromResult(default!)");
    }

    [Test]
    public async Task FromValueTaskResult_with_type_produces_expression()
    {
        var expr = TaskExpression.FromValueTaskResult("string", "default!");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Threading.Tasks.ValueTask.FromResult<string>(default!)");
    }
}
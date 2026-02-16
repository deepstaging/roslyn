// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Refs;

public class TaskRefsTests
{
    [Test]
    public async Task Task_non_generic()
    {
        var typeRef = TaskRefs.Task();

        await Assert.That(typeRef).IsEqualTo("global::System.Threading.Tasks.Task");
    }

    [Test]
    public async Task Task_generic()
    {
        var typeRef = TaskRefs.Task("string");

        await Assert.That(typeRef).IsEqualTo("global::System.Threading.Tasks.Task<string>");
    }

    [Test]
    public async Task ValueTask_non_generic()
    {
        var typeRef = TaskRefs.ValueTask();

        await Assert.That(typeRef).IsEqualTo("global::System.Threading.Tasks.ValueTask");
    }

    [Test]
    public async Task ValueTask_generic()
    {
        var typeRef = TaskRefs.ValueTask("int");

        await Assert.That(typeRef).IsEqualTo("global::System.Threading.Tasks.ValueTask<int>");
    }

    [Test]
    public async Task CompletedTask_expression()
    {
        var typeRef = TaskRefs.CompletedTask;

        await Assert.That(typeRef).IsEqualTo("global::System.Threading.Tasks.Task.CompletedTask");
    }

    [Test]
    public async Task CompletedValueTask_expression()
    {
        var typeRef = TaskRefs.CompletedValueTask;

        await Assert.That(typeRef).IsEqualTo("global::System.Threading.Tasks.ValueTask.CompletedTask");
    }

    [Test]
    public async Task CancellationToken_creates_globally_qualified_type()
    {
        var typeRef = TaskRefs.CancellationToken;

        await Assert.That(typeRef).IsEqualTo("global::System.Threading.CancellationToken");
    }

    #region Composition

    [Test]
    public async Task Task_of_list()
    {
        var typeRef = TaskRefs.Task(CollectionRefs.List("string"));

        await Assert.That(typeRef)
            .IsEqualTo("global::System.Threading.Tasks.Task<global::System.Collections.Generic.List<string>>");
    }

    #endregion
}
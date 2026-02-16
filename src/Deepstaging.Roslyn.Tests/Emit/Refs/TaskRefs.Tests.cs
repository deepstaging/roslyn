// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Refs;

public class TaskRefsTests
{
    [Test]
    public async Task Task_non_generic()
    {
        TypeRef typeRef = TaskRefs.Task();

        await Assert.That((string)typeRef).IsEqualTo("global::System.Threading.Tasks.Task");
    }

    [Test]
    public async Task Task_generic()
    {
        TypeRef typeRef = TaskRefs.Task("string");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Threading.Tasks.Task<string>");
    }

    [Test]
    public async Task ValueTask_non_generic()
    {
        TypeRef typeRef = TaskRefs.ValueTask();

        await Assert.That((string)typeRef).IsEqualTo("global::System.Threading.Tasks.ValueTask");
    }

    [Test]
    public async Task ValueTask_generic()
    {
        TypeRef typeRef = TaskRefs.ValueTask("int");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Threading.Tasks.ValueTask<int>");
    }

    [Test]
    public async Task CompletedTask_expression()
    {
        TypeRef typeRef = TaskRefs.CompletedTask;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Threading.Tasks.Task.CompletedTask");
    }

    [Test]
    public async Task CompletedValueTask_expression()
    {
        TypeRef typeRef = TaskRefs.CompletedValueTask;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Threading.Tasks.ValueTask.CompletedTask");
    }

    [Test]
    public async Task CancellationToken_creates_globally_qualified_type()
    {
        TypeRef typeRef = TaskRefs.CancellationToken;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Threading.CancellationToken");
    }

    #region Composition

    [Test]
    public async Task Task_of_list()
    {
        TypeRef typeRef = TaskRefs.Task(CollectionRefs.List("string"));

        await Assert.That((string)typeRef).IsEqualTo("global::System.Threading.Tasks.Task<global::System.Collections.Generic.List<string>>");
    }

    #endregion
}

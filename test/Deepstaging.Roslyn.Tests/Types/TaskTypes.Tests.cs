// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class TaskTypesTests
{
    [Test]
    public async Task Namespace_returns_system_threading_tasks()
    {
        await Assert.That((string)TaskTypes.Namespace)
            .IsEqualTo("System.Threading.Tasks");
    }

    [Test]
    public async Task Task_creates_globally_qualified_type()
    {
        await Assert.That((string)TaskTypes.Task("int"))
            .IsEqualTo("global::System.Threading.Tasks.Task<int>");
    }

    [Test]
    public async Task ValueTask_creates_globally_qualified_type()
    {
        await Assert.That((string)TaskTypes.ValueTask("string"))
            .IsEqualTo("global::System.Threading.Tasks.ValueTask<string>");
    }

    [Test]
    public async Task CancellationToken_produces_correct_type()
    {
        await Assert.That(TaskTypes.CancellationToken.Value)
            .IsEqualTo("global::System.Threading.CancellationToken");
    }

    [Test]
    public async Task Task_non_generic_returns_globally_qualified_type()
    {
        await Assert.That(TaskTypes.Task().Value)
            .IsEqualTo("global::System.Threading.Tasks.Task");
    }

    [Test]
    public async Task ValueTask_non_generic_returns_globally_qualified_type()
    {
        await Assert.That(TaskTypes.ValueTask().Value)
            .IsEqualTo("global::System.Threading.Tasks.ValueTask");
    }
}

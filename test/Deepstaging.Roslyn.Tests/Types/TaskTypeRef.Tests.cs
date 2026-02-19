// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class TaskTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new TaskTypeRef("int");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Threading.Tasks.Task<int>");
    }

    [Test]
    public async Task Carries_result_type()
    {
        var typeRef = new TaskTypeRef("string");

        await Assert.That((string)typeRef.ResultType).IsEqualTo("string");
    }

    [Test]
    public async Task Implicitly_converts_to_TypeRef()
    {
        TypeRef typeRef = new TaskTypeRef("bool");

        await Assert.That(typeRef.Value)
            .IsEqualTo("global::System.Threading.Tasks.Task<bool>");
    }
}

public class ValueTaskTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new ValueTaskTypeRef("int");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Threading.Tasks.ValueTask<int>");
    }

    [Test]
    public async Task Carries_result_type()
    {
        var typeRef = new ValueTaskTypeRef("string");

        await Assert.That((string)typeRef.ResultType).IsEqualTo("string");
    }

    [Test]
    public async Task Implicitly_converts_to_TypeRef()
    {
        TypeRef typeRef = new ValueTaskTypeRef("bool");

        await Assert.That(typeRef.Value)
            .IsEqualTo("global::System.Threading.Tasks.ValueTask<bool>");
    }
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class FuncTypeRefTests
{
    [Test]
    public async Task Creates_func_with_no_params()
    {
        var typeRef = new FuncTypeRef("int");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Func<int>");
    }

    [Test]
    public async Task Creates_func_with_params()
    {
        var typeRef = new FuncTypeRef(
            System.Collections.Immutable.ImmutableArray.Create<TypeRef>("string", "int"),
            "bool");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Func<string, int, bool>");
    }

    [Test]
    public async Task Carries_return_type()
    {
        var typeRef = new FuncTypeRef("string");

        await Assert.That((string)typeRef.ReturnType).IsEqualTo("string");
    }
}

public class ActionTypeRefTests
{
    [Test]
    public async Task Creates_action_with_no_params()
    {
        var typeRef = new ActionTypeRef(System.Collections.Immutable.ImmutableArray<TypeRef>.Empty);

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Action");
    }

    [Test]
    public async Task Creates_action_with_single_param()
    {
        var typeRef = new ActionTypeRef("string");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Action<string>");
    }

    [Test]
    public async Task Creates_action_with_multiple_params()
    {
        var typeRef = new ActionTypeRef(
            System.Collections.Immutable.ImmutableArray.Create<TypeRef>("string", "int"));

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Action<string, int>");
    }
}

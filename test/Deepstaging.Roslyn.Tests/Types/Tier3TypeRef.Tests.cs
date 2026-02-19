// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class ComparerTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new ComparerTypeRef("int");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Generic.Comparer<int>");
    }

    [Test]
    public async Task Carries_compared_type()
    {
        var typeRef = new ComparerTypeRef("string");

        await Assert.That((string)typeRef.ComparedType).IsEqualTo("string");
    }
}

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

public class ImmutableArrayTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new ImmutableArrayTypeRef("string");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Immutable.ImmutableArray<string>");
    }

    [Test]
    public async Task Carries_element_type()
    {
        var typeRef = new ImmutableArrayTypeRef("int");

        await Assert.That((string)typeRef.ElementType).IsEqualTo("int");
    }
}

public class ImmutableDictionaryTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new ImmutableDictionaryTypeRef("string", "int");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Immutable.ImmutableDictionary<string, int>");
    }

    [Test]
    public async Task Carries_key_and_value_types()
    {
        var typeRef = new ImmutableDictionaryTypeRef("Guid", "User");

        await Assert.That((string)typeRef.KeyType).IsEqualTo("Guid");
        await Assert.That((string)typeRef.ValueType).IsEqualTo("User");
    }
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

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

public class ImmutableListTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new ImmutableListTypeRef("string");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Immutable.ImmutableList<string>");
    }

    [Test]
    public async Task Carries_element_type()
    {
        var typeRef = new ImmutableListTypeRef("Order");

        await Assert.That((string)typeRef.ElementType).IsEqualTo("Order");
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

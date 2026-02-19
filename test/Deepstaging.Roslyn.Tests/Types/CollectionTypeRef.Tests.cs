// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class ListTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new ListTypeRef("string");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Generic.List<string>");
    }

    [Test]
    public async Task Carries_element_type()
    {
        var typeRef = new ListTypeRef("int");

        await Assert.That((string)typeRef.ElementType).IsEqualTo("int");
    }

    [Test]
    public async Task Implicitly_converts_to_TypeRef()
    {
        TypeRef typeRef = new ListTypeRef("string");

        await Assert.That(typeRef.Value)
            .IsEqualTo("global::System.Collections.Generic.List<string>");
    }
}

public class DictionaryTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new DictionaryTypeRef("string", "int");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Generic.Dictionary<string, int>");
    }

    [Test]
    public async Task Carries_key_and_value_types()
    {
        var typeRef = new DictionaryTypeRef("string", "object");

        await Assert.That((string)typeRef.KeyType).IsEqualTo("string");
        await Assert.That((string)typeRef.ValueType).IsEqualTo("object");
    }
}

public class HashSetTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new HashSetTypeRef("int");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Generic.HashSet<int>");
    }

    [Test]
    public async Task Carries_element_type()
    {
        var typeRef = new HashSetTypeRef("string");

        await Assert.That((string)typeRef.ElementType).IsEqualTo("string");
    }
}

public class LazyTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new LazyTypeRef("ExpensiveService");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Lazy<ExpensiveService>");
    }

    [Test]
    public async Task Carries_value_type()
    {
        var typeRef = new LazyTypeRef("IConnection");

        await Assert.That((string)typeRef.ValueType).IsEqualTo("IConnection");
    }
}
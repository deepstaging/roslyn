// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class EnumerableTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new EnumerableTypeRef("string");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Generic.IEnumerable<string>");
    }

    [Test]
    public async Task Carries_element_type()
    {
        var typeRef = new EnumerableTypeRef("Order");

        await Assert.That((string)typeRef.ElementType).IsEqualTo("Order");
    }

    [Test]
    public async Task Implicitly_converts_to_TypeRef()
    {
        TypeRef typeRef = new EnumerableTypeRef("int");

        await Assert.That(typeRef.Value)
            .IsEqualTo("global::System.Collections.Generic.IEnumerable<int>");
    }
}

public class CollectionInterfaceTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new CollectionInterfaceTypeRef("string");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Generic.ICollection<string>");
    }

    [Test]
    public async Task Carries_element_type()
    {
        var typeRef = new CollectionInterfaceTypeRef("int");

        await Assert.That((string)typeRef.ElementType).IsEqualTo("int");
    }
}

public class ListInterfaceTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new ListInterfaceTypeRef("Customer");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Generic.IList<Customer>");
    }

    [Test]
    public async Task Carries_element_type()
    {
        var typeRef = new ListInterfaceTypeRef("Order");

        await Assert.That((string)typeRef.ElementType).IsEqualTo("Order");
    }
}

public class SetInterfaceTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new SetInterfaceTypeRef("string");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Generic.ISet<string>");
    }

    [Test]
    public async Task Carries_element_type()
    {
        var typeRef = new SetInterfaceTypeRef("Tag");

        await Assert.That((string)typeRef.ElementType).IsEqualTo("Tag");
    }
}

public class ReadOnlyListTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new ReadOnlyListTypeRef("string");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Generic.IReadOnlyList<string>");
    }

    [Test]
    public async Task Carries_element_type()
    {
        var typeRef = new ReadOnlyListTypeRef("Order");

        await Assert.That((string)typeRef.ElementType).IsEqualTo("Order");
    }
}

public class ReadOnlyCollectionTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new ReadOnlyCollectionTypeRef("int");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Generic.IReadOnlyCollection<int>");
    }

    [Test]
    public async Task Carries_element_type()
    {
        var typeRef = new ReadOnlyCollectionTypeRef("Item");

        await Assert.That((string)typeRef.ElementType).IsEqualTo("Item");
    }
}

public class DictionaryInterfaceTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new DictionaryInterfaceTypeRef("string", "int");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Generic.IDictionary<string, int>");
    }

    [Test]
    public async Task Carries_key_and_value_types()
    {
        var typeRef = new DictionaryInterfaceTypeRef("Guid", "User");

        await Assert.That((string)typeRef.KeyType).IsEqualTo("Guid");
        await Assert.That((string)typeRef.ValueType).IsEqualTo("User");
    }
}

public class ReadOnlyDictionaryTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new ReadOnlyDictionaryTypeRef("string", "int");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Generic.IReadOnlyDictionary<string, int>");
    }

    [Test]
    public async Task Carries_key_and_value_types()
    {
        var typeRef = new ReadOnlyDictionaryTypeRef("string", "Config");

        await Assert.That((string)typeRef.KeyType).IsEqualTo("string");
        await Assert.That((string)typeRef.ValueType).IsEqualTo("Config");
    }
}

public class KeyValuePairTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new KeyValuePairTypeRef("string", "int");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Generic.KeyValuePair<string, int>");
    }

    [Test]
    public async Task Carries_key_and_value_types()
    {
        var typeRef = new KeyValuePairTypeRef("string", "object");

        await Assert.That((string)typeRef.KeyType).IsEqualTo("string");
        await Assert.That((string)typeRef.ValueType).IsEqualTo("object");
    }
}

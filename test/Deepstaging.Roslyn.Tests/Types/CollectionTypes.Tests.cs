// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class CollectionTypesTests
{
    [Test]
    public async Task Namespace_returns_system_collections_generic()
    {
        await Assert.That((string)CollectionTypes.Namespace)
            .IsEqualTo("System.Collections.Generic");
    }

    [Test]
    public async Task List_creates_globally_qualified_type()
    {
        await Assert.That((string)CollectionTypes.List("string"))
            .IsEqualTo("global::System.Collections.Generic.List<string>");
    }

    [Test]
    public async Task Dictionary_creates_globally_qualified_type()
    {
        await Assert.That((string)CollectionTypes.Dictionary("string", "int"))
            .IsEqualTo("global::System.Collections.Generic.Dictionary<string, int>");
    }

    [Test]
    public async Task HashSet_creates_globally_qualified_type()
    {
        await Assert.That((string)CollectionTypes.HashSet("string"))
            .IsEqualTo("global::System.Collections.Generic.HashSet<string>");
    }

    [Test]
    public async Task Enumerable_creates_globally_qualified_type()
    {
        await Assert.That((string)CollectionTypes.Enumerable("string"))
            .IsEqualTo("global::System.Collections.Generic.IEnumerable<string>");
    }

    [Test]
    public async Task Collection_creates_globally_qualified_type()
    {
        await Assert.That((string)CollectionTypes.Collection("string"))
            .IsEqualTo("global::System.Collections.Generic.ICollection<string>");
    }

    [Test]
    public async Task ListInterface_creates_globally_qualified_type()
    {
        await Assert.That((string)CollectionTypes.ListInterface("string"))
            .IsEqualTo("global::System.Collections.Generic.IList<string>");
    }

    [Test]
    public async Task SetInterface_creates_globally_qualified_type()
    {
        await Assert.That((string)CollectionTypes.SetInterface("string"))
            .IsEqualTo("global::System.Collections.Generic.ISet<string>");
    }

    [Test]
    public async Task ReadOnlyList_creates_globally_qualified_type()
    {
        await Assert.That((string)CollectionTypes.ReadOnlyList("string"))
            .IsEqualTo("global::System.Collections.Generic.IReadOnlyList<string>");
    }

    [Test]
    public async Task ReadOnlyCollection_creates_globally_qualified_type()
    {
        await Assert.That((string)CollectionTypes.ReadOnlyCollection("string"))
            .IsEqualTo("global::System.Collections.Generic.IReadOnlyCollection<string>");
    }

    [Test]
    public async Task DictionaryInterface_creates_globally_qualified_type()
    {
        await Assert.That((string)CollectionTypes.DictionaryInterface("string", "int"))
            .IsEqualTo("global::System.Collections.Generic.IDictionary<string, int>");
    }

    [Test]
    public async Task ReadOnlyDictionary_creates_globally_qualified_type()
    {
        await Assert.That((string)CollectionTypes.ReadOnlyDictionary("string", "int"))
            .IsEqualTo("global::System.Collections.Generic.IReadOnlyDictionary<string, int>");
    }

    [Test]
    public async Task KeyValuePair_creates_globally_qualified_type()
    {
        await Assert.That((string)CollectionTypes.KeyValuePair("string", "int"))
            .IsEqualTo("global::System.Collections.Generic.KeyValuePair<string, int>");
    }
}

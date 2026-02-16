// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Refs;

public class CollectionRefsTests
{
    [Test]
    public async Task List_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.List("string");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.List<string>");
    }

    [Test]
    public async Task Dictionary_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.Dictionary("string", "int");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.Dictionary<string, int>");
    }

    [Test]
    public async Task HashSet_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.HashSet("int");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.HashSet<int>");
    }

    [Test]
    public async Task KeyValuePair_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.KeyValuePair("string", "int");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.KeyValuePair<string, int>");
    }

    [Test]
    public async Task IEnumerable_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.IEnumerable("string");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.IEnumerable<string>");
    }

    [Test]
    public async Task ICollection_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.ICollection("string");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.ICollection<string>");
    }

    [Test]
    public async Task IList_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.IList("string");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.IList<string>");
    }

    [Test]
    public async Task IDictionary_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.IDictionary("string", "int");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.IDictionary<string, int>");
    }

    [Test]
    public async Task ISet_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.ISet("int");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.ISet<int>");
    }

    [Test]
    public async Task IReadOnlyList_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.IReadOnlyList("string");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.IReadOnlyList<string>");
    }

    [Test]
    public async Task IReadOnlyCollection_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.IReadOnlyCollection("string");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.IReadOnlyCollection<string>");
    }

    [Test]
    public async Task IReadOnlyDictionary_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.IReadOnlyDictionary("string", "int");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.IReadOnlyDictionary<string, int>");
    }

    #region Composition

    [Test]
    public async Task List_nullable()
    {
        TypeRef typeRef = CollectionRefs.List("string").Nullable();

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.List<string>?");
    }

    [Test]
    public async Task Dictionary_with_nested_list()
    {
        TypeRef typeRef = CollectionRefs.Dictionary("string", CollectionRefs.List("int"));

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.List<int>>");
    }

    [Test]
    public async Task IReadOnlyList_array()
    {
        TypeRef typeRef = CollectionRefs.IReadOnlyList("int").Array();

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.IReadOnlyList<int>[]");
    }

    #endregion
}

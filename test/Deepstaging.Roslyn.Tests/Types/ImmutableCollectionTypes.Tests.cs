// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class ImmutableCollectionTypesTests
{
    [Test]
    public async Task Namespace_returns_system_collections_immutable()
    {
        await Assert.That((string)ImmutableCollectionTypes.Namespace)
            .IsEqualTo("System.Collections.Immutable");
    }

    [Test]
    public async Task ImmutableArray_creates_globally_qualified_type()
    {
        await Assert.That((string)ImmutableCollectionTypes.ImmutableArray("string"))
            .IsEqualTo("global::System.Collections.Immutable.ImmutableArray<string>");
    }

    [Test]
    public async Task ImmutableList_creates_globally_qualified_type()
    {
        await Assert.That((string)ImmutableCollectionTypes.ImmutableList("int"))
            .IsEqualTo("global::System.Collections.Immutable.ImmutableList<int>");
    }

    [Test]
    public async Task ImmutableDictionary_creates_globally_qualified_type()
    {
        await Assert.That((string)ImmutableCollectionTypes.ImmutableDictionary("string", "int"))
            .IsEqualTo("global::System.Collections.Immutable.ImmutableDictionary<string, int>");
    }
}

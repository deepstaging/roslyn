// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Refs;

public class ImmutableCollectionRefsTests
{
    [Test]
    public async Task ImmutableArray_creates_globally_qualified_type()
    {
        var typeRef = ImmutableCollectionRefs.ImmutableArray("string");

        await Assert.That(typeRef).IsEqualTo("global::System.Collections.Immutable.ImmutableArray<string>");
    }

    [Test]
    public async Task ImmutableList_creates_globally_qualified_type()
    {
        var typeRef = ImmutableCollectionRefs.ImmutableList("string");

        await Assert.That(typeRef).IsEqualTo("global::System.Collections.Immutable.ImmutableList<string>");
    }

    [Test]
    public async Task ImmutableDictionary_creates_globally_qualified_type()
    {
        var typeRef = ImmutableCollectionRefs.ImmutableDictionary("string", "int");

        await Assert.That(typeRef).IsEqualTo("global::System.Collections.Immutable.ImmutableDictionary<string, int>");
    }
}
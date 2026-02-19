// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class EqualityComparerTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new EqualityComparerTypeRef("string");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Generic.EqualityComparer<string>");
    }

    [Test]
    public async Task Carries_compared_type()
    {
        var typeRef = new EqualityComparerTypeRef("int");

        await Assert.That((string)typeRef.ComparedType).IsEqualTo("int");
    }

    [Test]
    public async Task Implicitly_converts_to_TypeRef()
    {
        TypeRef typeRef = new EqualityComparerTypeRef("string");

        await Assert.That(typeRef.Value)
            .IsEqualTo("global::System.Collections.Generic.EqualityComparer<string>");
    }

    [Test]
    public async Task Works_with_nested_generic()
    {
        var typeRef = new EqualityComparerTypeRef((TypeRef)new ListTypeRef("int"));

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Generic.EqualityComparer<global::System.Collections.Generic.List<int>>");
    }
}
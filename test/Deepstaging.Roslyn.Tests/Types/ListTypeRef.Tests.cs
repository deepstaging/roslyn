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

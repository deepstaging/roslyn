// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

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

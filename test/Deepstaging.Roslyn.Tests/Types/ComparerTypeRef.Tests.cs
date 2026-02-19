// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class ComparerTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new ComparerTypeRef("int");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Generic.Comparer<int>");
    }

    [Test]
    public async Task Carries_compared_type()
    {
        var typeRef = new ComparerTypeRef("string");

        await Assert.That((string)typeRef.ComparedType).IsEqualTo("string");
    }
}

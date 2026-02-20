// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class ComparerTypesTests
{
    [Test]
    public async Task EqualityComparer_creates_globally_qualified_type()
    {
        await Assert.That((string)ComparerTypes.EqualityComparer("string"))
            .IsEqualTo("global::System.Collections.Generic.EqualityComparer<string>");
    }

    [Test]
    public async Task Comparer_creates_globally_qualified_type()
    {
        await Assert.That((string)ComparerTypes.Comparer("int"))
            .IsEqualTo("global::System.Collections.Generic.Comparer<int>");
    }
}

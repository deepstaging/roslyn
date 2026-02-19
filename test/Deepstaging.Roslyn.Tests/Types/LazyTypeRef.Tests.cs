// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class LazyTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new LazyTypeRef("ExpensiveService");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Lazy<ExpensiveService>");
    }

    [Test]
    public async Task Carries_value_type()
    {
        var typeRef = new LazyTypeRef("IConnection");

        await Assert.That((string)typeRef.ValueType).IsEqualTo("IConnection");
    }
}

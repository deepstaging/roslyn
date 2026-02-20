// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class SystemTypesTests
{
    [Test]
    public async Task Nullable_creates_globally_qualified_type()
    {
        await Assert.That((string)SystemTypes.Nullable("int"))
            .IsEqualTo("global::System.Nullable<int>");
    }

    [Test]
    public async Task Lazy_creates_globally_qualified_type()
    {
        await Assert.That((string)SystemTypes.Lazy("ExpensiveService"))
            .IsEqualTo("global::System.Lazy<ExpensiveService>");
    }
}

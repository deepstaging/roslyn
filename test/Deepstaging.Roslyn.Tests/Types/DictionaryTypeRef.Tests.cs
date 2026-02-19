// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class DictionaryTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new DictionaryTypeRef("string", "int");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Generic.Dictionary<string, int>");
    }

    [Test]
    public async Task Carries_key_and_value_types()
    {
        var typeRef = new DictionaryTypeRef("string", "object");

        await Assert.That((string)typeRef.KeyType).IsEqualTo("string");
        await Assert.That((string)typeRef.ValueType).IsEqualTo("object");
    }
}

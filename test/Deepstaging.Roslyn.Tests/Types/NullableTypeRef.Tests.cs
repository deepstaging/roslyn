// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class NullableTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new NullableTypeRef("int");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Nullable<int>");
    }

    [Test]
    public async Task Carries_inner_type()
    {
        var typeRef = new NullableTypeRef("double");

        await Assert.That((string)typeRef.InnerType).IsEqualTo("double");
    }

    [Test]
    public async Task Implicitly_converts_to_TypeRef()
    {
        TypeRef typeRef = new NullableTypeRef("int");

        await Assert.That(typeRef.Value)
            .IsEqualTo("global::System.Nullable<int>");
    }
}
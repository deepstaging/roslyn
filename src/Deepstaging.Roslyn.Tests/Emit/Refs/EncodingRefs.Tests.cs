// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Refs;

public class EncodingRefsTests
{
    [Test]
    public async Task UTF8_creates_globally_qualified_expression()
    {
        TypeRef typeRef = EncodingRefs.UTF8;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Text.Encoding.UTF8");
    }

    [Test]
    public async Task ASCII_creates_globally_qualified_expression()
    {
        TypeRef typeRef = EncodingRefs.ASCII;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Text.Encoding.ASCII");
    }

    [Test]
    public async Task Unicode_creates_globally_qualified_expression()
    {
        TypeRef typeRef = EncodingRefs.Unicode;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Text.Encoding.Unicode");
    }
}

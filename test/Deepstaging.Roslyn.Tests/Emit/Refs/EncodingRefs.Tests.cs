// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Refs;

public class EncodingRefsTests
{
    [Test]
    public async Task UTF8_creates_globally_qualified_expression()
    {
        var expr = EncodingRefs.UTF8;

        await Assert.That(expr).IsEqualTo("global::System.Text.Encoding.UTF8");
    }

    [Test]
    public async Task ASCII_creates_globally_qualified_expression()
    {
        var expr = EncodingRefs.ASCII;

        await Assert.That(expr).IsEqualTo("global::System.Text.Encoding.ASCII");
    }

    [Test]
    public async Task Unicode_creates_globally_qualified_expression()
    {
        var expr = EncodingRefs.Unicode;

        await Assert.That(expr).IsEqualTo("global::System.Text.Encoding.Unicode");
    }
}
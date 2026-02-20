// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class EncodingTypesTests
{
    [Test]
    public async Task Namespace_returns_system_text()
    {
        await Assert.That(EncodingTypes.Namespace.Value)
            .IsEqualTo("System.Text");
    }

    [Test]
    public async Task Encoding_returns_globally_qualified_type()
    {
        await Assert.That(EncodingTypes.Encoding.Value)
            .IsEqualTo("global::System.Text.Encoding");
    }

    [Test]
    public async Task UTF8_returns_encoding_utf8_expression()
    {
        await Assert.That(EncodingTypes.UTF8.Value)
            .IsEqualTo("global::System.Text.Encoding.UTF8");
    }

    [Test]
    public async Task ASCII_returns_encoding_ascii_expression()
    {
        await Assert.That(EncodingTypes.ASCII.Value)
            .IsEqualTo("global::System.Text.Encoding.ASCII");
    }

    [Test]
    public async Task Unicode_returns_encoding_unicode_expression()
    {
        await Assert.That(EncodingTypes.Unicode.Value)
            .IsEqualTo("global::System.Text.Encoding.Unicode");
    }

    [Test]
    public async Task UTF32_returns_encoding_utf32_expression()
    {
        await Assert.That(EncodingTypes.UTF32.Value)
            .IsEqualTo("global::System.Text.Encoding.UTF32");
    }

    [Test]
    public async Task Latin1_returns_encoding_latin1_expression()
    {
        await Assert.That(EncodingTypes.Latin1.Value)
            .IsEqualTo("global::System.Text.Encoding.Latin1");
    }
}

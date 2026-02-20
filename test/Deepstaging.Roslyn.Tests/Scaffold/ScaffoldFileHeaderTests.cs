// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Scaffold;

namespace Deepstaging.Roslyn.Tests.Scaffold;

public class ScaffoldFileHeaderTests
{
    [Test]
    public async Task Format_ProducesCorrectHeader()
    {
        var header = new ScaffoldFileHeader
        {
            Package = "deepstaging-web",
            Version = "0.1.0",
            Hash = "abc123",
            ScaffoldName = "api-types"
        };

        var result = header.Format();

        await Assert.That(result).IsEqualTo(
            "// @deepstaging-web v0.1.0 hash:abc123\n// scaffold: api-types");
    }

    [Test]
    public async Task Parse_RoundTrips()
    {
        var original = new ScaffoldFileHeader
        {
            Package = "deepstaging-web-svelte",
            Version = "1.2.3-alpha.1",
            Hash = "deadbeef",
            ScaffoldName = "orders-page"
        };

        var content = original.Format() + "\n\nexport interface Foo {}";
        var parsed = ScaffoldFileHeader.Parse(content);

        await Assert.That(parsed).IsNotNull();
        await Assert.That(parsed!.Package).IsEqualTo("deepstaging-web-svelte");
        await Assert.That(parsed.Version).IsEqualTo("1.2.3-alpha.1");
        await Assert.That(parsed.Hash).IsEqualTo("deadbeef");
        await Assert.That(parsed.ScaffoldName).IsEqualTo("orders-page");
    }

    [Test]
    public async Task Parse_ReturnsNull_ForEmptyContent()
    {
        await Assert.That(ScaffoldFileHeader.Parse("")).IsNull();
        await Assert.That(ScaffoldFileHeader.Parse(null!)).IsNull();
    }

    [Test]
    public async Task Parse_ReturnsNull_ForNonScaffoldContent()
    {
        var content = "export interface Foo {}\nexport interface Bar {}";
        await Assert.That(ScaffoldFileHeader.Parse(content)).IsNull();
    }

    [Test]
    public async Task Parse_ReturnsNull_WhenMissingScaffoldLine()
    {
        var content = "// @deepstaging-web v1.0.0 hash:abc\nexport interface Foo {}";
        await Assert.That(ScaffoldFileHeader.Parse(content)).IsNull();
    }

    [Test]
    public async Task Parse_ReturnsNull_WhenMissingHash()
    {
        var content = "// @deepstaging-web v1.0.0\n// scaffold: api-types";
        await Assert.That(ScaffoldFileHeader.Parse(content)).IsNull();
    }

    [Test]
    public async Task ExtractHash_ReturnsHash()
    {
        var content = "// @deepstaging-web v0.1.0 hash:abc123def\n// scaffold: api-client\n\nexport class Foo {}";
        var hash = ScaffoldFileHeader.ExtractHash(content);
        await Assert.That(hash).IsEqualTo("abc123def");
    }

    [Test]
    public async Task ExtractHash_ReturnsNull_ForNonScaffold()
    {
        var hash = ScaffoldFileHeader.ExtractHash("just plain content");
        await Assert.That(hash).IsNull();
    }
}

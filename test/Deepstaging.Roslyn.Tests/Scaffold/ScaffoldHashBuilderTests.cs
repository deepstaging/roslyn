// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Scaffold;

namespace Deepstaging.Roslyn.Tests.Scaffold;

public class ScaffoldHashBuilderTests
{
    [Test]
    public async Task Build_ReturnsDeterministicHash()
    {
        var hash1 = new ScaffoldHashBuilder()
            .Add("route", "POST /api/orders")
            .Add("prop", "CustomerId:string")
            .Build();

        var hash2 = new ScaffoldHashBuilder()
            .Add("route", "POST /api/orders")
            .Add("prop", "CustomerId:string")
            .Build();

        await Assert.That(hash1).IsEqualTo(hash2);
    }

    [Test]
    public async Task Build_ReturnsLowercaseHex()
    {
        var hash = new ScaffoldHashBuilder()
            .Add("key", "value")
            .Build();

        await Assert.That(hash).Matches("^[0-9a-f]{64}$");
    }

    [Test]
    public async Task Build_DifferentInputs_ProduceDifferentHashes()
    {
        var hash1 = new ScaffoldHashBuilder()
            .Add("route", "POST /api/orders")
            .Build();

        var hash2 = new ScaffoldHashBuilder()
            .Add("route", "GET /api/orders")
            .Build();

        await Assert.That(hash1).IsNotEqualTo(hash2);
    }

    [Test]
    public async Task Build_OrderMatters()
    {
        var hash1 = new ScaffoldHashBuilder()
            .Add("a", "1")
            .Add("b", "2")
            .Build();

        var hash2 = new ScaffoldHashBuilder()
            .Add("b", "2")
            .Add("a", "1")
            .Build();

        await Assert.That(hash1).IsNotEqualTo(hash2);
    }

    [Test]
    public async Task Build_EmptyBuilder_ProducesHash()
    {
        var hash = new ScaffoldHashBuilder().Build();
        await Assert.That(hash).Length().IsEqualTo(64);
    }
}

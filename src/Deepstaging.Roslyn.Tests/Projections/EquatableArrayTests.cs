// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Collections.Immutable;

namespace Deepstaging.Roslyn.Tests.Projections;

public class EquatableArrayTests
{
    [Test]
    public async Task CollectionExpression_Empty()
    {
        EquatableArray<int> array = [];

        await Assert.That(array.Count).IsEqualTo(0);
    }

    [Test]
    public async Task CollectionExpression_WithElements()
    {
        EquatableArray<int> array = [1, 2, 3];

        await Assert.That(array.Count).IsEqualTo(3);
        await Assert.That(array[0]).IsEqualTo(1);
        await Assert.That(array[1]).IsEqualTo(2);
        await Assert.That(array[2]).IsEqualTo(3);
    }

    [Test]
    public async Task CollectionExpression_Spread()
    {
        EquatableArray<string> first = ["a", "b"];
        EquatableArray<string> second = [..first, "c"];

        await Assert.That(second.Count).IsEqualTo(3);
        await Assert.That(second[2]).IsEqualTo("c");
    }

    [Test]
    public async Task CollectionExpression_Equality()
    {
        EquatableArray<int> a = [1, 2, 3];
        EquatableArray<int> b = [1, 2, 3];
        EquatableArray<int> c = [1, 2, 4];

        await Assert.That(a).IsEqualTo(b);
        await Assert.That(a).IsNotEqualTo(c);
    }

    [Test]
    public async Task Create_FromReadOnlySpan()
    {
        var array = EquatableArray.Create<int>([10, 20]);

        await Assert.That(array.Count).IsEqualTo(2);
        await Assert.That(array[0]).IsEqualTo(10);
    }

    [Test]
    public async Task ImplicitConversion_RoundTrip()
    {
        EquatableArray<int> equatable = [5, 10, 15];
        ImmutableArray<int> immutable = equatable;
        EquatableArray<int> backAgain = immutable;

        await Assert.That(backAgain).IsEqualTo(equatable);
    }
}

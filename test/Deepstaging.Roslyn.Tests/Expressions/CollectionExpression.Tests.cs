// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class CollectionExpressionTests
{
    [Test]
    public async Task NewList_produces_constructor()
    {
        var expr = CollectionExpression.NewList("string");

        await Assert.That(expr.Value)
            .IsEqualTo("new global::System.Collections.Generic.List<string>()");
    }

    [Test]
    public async Task NewList_with_capacity_produces_constructor()
    {
        var expr = CollectionExpression.NewList("string", "16");

        await Assert.That(expr.Value)
            .IsEqualTo("new global::System.Collections.Generic.List<string>(16)");
    }

    [Test]
    public async Task NewDictionary_produces_constructor()
    {
        var expr = CollectionExpression.NewDictionary("string", "int");

        await Assert.That(expr.Value)
            .IsEqualTo("new global::System.Collections.Generic.Dictionary<string, int>()");
    }

    [Test]
    public async Task NewHashSet_produces_constructor()
    {
        var expr = CollectionExpression.NewHashSet("int");

        await Assert.That(expr.Value)
            .IsEqualTo("new global::System.Collections.Generic.HashSet<int>()");
    }
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class ComparerExpressionTests
{
    [Test]
    public async Task Default_produces_default_comparer()
    {
        var expr = ComparerExpression.Default("int");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Collections.Generic.Comparer<int>.Default");
    }

    [Test]
    public async Task DefaultCompare_produces_comparison()
    {
        var expr = ComparerExpression.DefaultCompare("int", "left", "right");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Collections.Generic.Comparer<int>.Default.Compare(left, right)");
    }
}

public class ImmutableCollectionExpressionTests
{
    [Test]
    public async Task EmptyArray_produces_empty_expression()
    {
        var expr = ImmutableCollectionExpression.EmptyArray("string");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Collections.Immutable.ImmutableArray<string>.Empty");
    }

    [Test]
    public async Task CreateArray_produces_create_expression()
    {
        var expr = ImmutableCollectionExpression.CreateArray("string", "item1", "item2");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Collections.Immutable.ImmutableArray.Create<string>(item1, item2)");
    }

    [Test]
    public async Task EmptyDictionary_produces_empty_expression()
    {
        var expr = ImmutableCollectionExpression.EmptyDictionary("string", "int");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Collections.Immutable.ImmutableDictionary<string, int>.Empty");
    }

    [Test]
    public async Task CreateDictionaryBuilder_produces_builder_expression()
    {
        var expr = ImmutableCollectionExpression.CreateDictionaryBuilder("string", "int");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Collections.Immutable.ImmutableDictionary.CreateBuilder<string, int>()");
    }

    [Test]
    public async Task EmptyList_produces_empty_expression()
    {
        var expr = ImmutableCollectionExpression.EmptyList("string");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Collections.Immutable.ImmutableList<string>.Empty");
    }

    [Test]
    public async Task CreateList_produces_create_expression()
    {
        var expr = ImmutableCollectionExpression.CreateList("int", "1", "2", "3");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Collections.Immutable.ImmutableList.Create<int>(1, 2, 3)");
    }
}
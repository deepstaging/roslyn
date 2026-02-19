// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.LanguageExt.Expressions;

using Roslyn.LanguageExt.Expressions;
using Roslyn.LanguageExt.Types;

public class SeqExpressionTests
{
    [Test]
    public async Task Seq_single_item()
    {
        var result = SeqExpression.Seq("item");

        await Assert.That(result.Value)
            .IsEqualTo("Seq(item)");
    }

    [Test]
    public async Task Seq_multiple_items()
    {
        var result = SeqExpression.Seq("a", "b", "c");

        await Assert.That(result.Value)
            .IsEqualTo("Seq(a, b, c)");
    }

    [Test]
    public async Task toSeq_converts_enumerable()
    {
        var result = SeqExpression.toSeq("items.Where(x => x.IsActive)");

        await Assert.That(result.Value)
            .IsEqualTo("toSeq(items.Where(x => x.IsActive))");
    }

    [Test]
    public async Task Empty_returns_typed_empty()
    {
        var result = SeqExpression.Empty(LanguageExtRefs.Seq("string"));

        await Assert.That(result.Value)
            .IsEqualTo("global::LanguageExt.Seq<string>.Empty");
    }

    [Test]
    public async Task toSeq_is_chainable()
    {
        var result = SeqExpression.toSeq("items").Call("Map", "x => x.Name");

        await Assert.That(result.Value)
            .IsEqualTo("toSeq(items).Map(x => x.Name)");
    }
}
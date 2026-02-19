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

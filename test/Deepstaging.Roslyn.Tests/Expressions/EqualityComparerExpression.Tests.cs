// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class EqualityComparerExpressionTests
{
    [Test]
    public async Task Default_produces_default_comparer()
    {
        var expr = EqualityComparerExpression.Default("string");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Collections.Generic.EqualityComparer<string>.Default");
    }

    [Test]
    public async Task DefaultEquals_produces_equality_check()
    {
        var expr = EqualityComparerExpression.DefaultEquals("string", "_name", "value");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Collections.Generic.EqualityComparer<string>.Default.Equals(_name, value)");
    }

    [Test]
    public async Task DefaultGetHashCode_produces_hash_call()
    {
        var expr = EqualityComparerExpression.DefaultGetHashCode("string", "_name");

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Collections.Generic.EqualityComparer<string>.Default.GetHashCode(_name)");
    }

    [Test]
    public async Task Default_with_typed_ref()
    {
        var typeRef = new EqualityComparerTypeRef("int");
        var expr = EqualityComparerExpression.Default(typeRef);

        await Assert.That(expr.Value)
            .IsEqualTo("global::System.Collections.Generic.EqualityComparer<int>.Default");
    }
}
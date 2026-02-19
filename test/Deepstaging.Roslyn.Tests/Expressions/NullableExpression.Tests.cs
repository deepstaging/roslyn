// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class NullableExpressionTests
{
    [Test]
    public async Task HasValue_produces_member_access()
    {
        var expr = NullableExpression.HasValue("myNullable");

        await Assert.That(expr.Value).IsEqualTo("myNullable.HasValue");
    }

    [Test]
    public async Task Value_produces_member_access()
    {
        var expr = NullableExpression.Value("myNullable");

        await Assert.That(expr.Value).IsEqualTo("myNullable.Value");
    }

    [Test]
    public async Task GetValueOrDefault_produces_call()
    {
        var expr = NullableExpression.GetValueOrDefault("myNullable");

        await Assert.That(expr.Value).IsEqualTo("myNullable.GetValueOrDefault()");
    }

    [Test]
    public async Task GetValueOrDefault_with_fallback_produces_call()
    {
        var expr = NullableExpression.GetValueOrDefault("myNullable", "42");

        await Assert.That(expr.Value).IsEqualTo("myNullable.GetValueOrDefault(42)");
    }
}
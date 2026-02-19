// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class HttpExpressionTests
{
    [Test]
    public async Task Get_produces_expression()
    {
        var expr = HttpExpression.Get;

        await Assert.That(expr.Value).Contains("HttpMethod.Get");
    }

    [Test]
    public async Task GetAsync_produces_expression()
    {
        var expr = HttpExpression.GetAsync("client", "url");

        await Assert.That(expr.Value).Contains("GetAsync(url)");
    }

    [Test]
    public async Task EnsureSuccessStatusCode_produces_expression()
    {
        var expr = HttpExpression.EnsureSuccessStatusCode("response");

        await Assert.That(expr.Value).Contains("EnsureSuccessStatusCode()");
    }
}

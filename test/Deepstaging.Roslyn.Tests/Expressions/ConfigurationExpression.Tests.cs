// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class ConfigurationExpressionTests
{
    [Test]
    public async Task GetSection_produces_expression()
    {
        var expr = ConfigurationExpression.GetSection("config", "\"Logging\"");

        await Assert.That(expr.Value).Contains("GetSection");
    }

    [Test]
    public async Task GetValue_produces_expression()
    {
        var expr = ConfigurationExpression.GetValue("config", "int", "\"MaxRetries\"");

        await Assert.That(expr.Value).Contains("GetValue<int>");
    }

    [Test]
    public async Task GetConnectionString_produces_expression()
    {
        var expr = ConfigurationExpression.GetConnectionString("config", "\"Default\"");

        await Assert.That(expr.Value).Contains("GetConnectionString");
    }
}

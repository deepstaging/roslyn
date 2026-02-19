// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class ConfigurationExpressionTests
{
    [Test]
    public async Task GetSection_produces_correct_expression()
    {
        var expr = ConfigurationExpression.GetSection("config", "\"Logging\"");

        await Assert.That(expr.Value).IsEqualTo("config.GetSection(\"Logging\")");
    }

    [Test]
    public async Task GetValue_produces_correct_expression()
    {
        var expr = ConfigurationExpression.GetValue("config", "int", "\"MaxRetries\"");

        await Assert.That(expr.Value).IsEqualTo("config.GetValue<int>(\"MaxRetries\")");
    }

    [Test]
    public async Task GetConnectionString_produces_correct_expression()
    {
        var expr = ConfigurationExpression.GetConnectionString("config", "\"Default\"");

        await Assert.That(expr.Value).IsEqualTo("config.GetConnectionString(\"Default\")");
    }

    [Test]
    public async Task Bind_produces_correct_expression()
    {
        var expr = ConfigurationExpression.Bind("config", "\"Settings\"", "options");

        await Assert.That(expr.Value).IsEqualTo("config.GetSection(\"Settings\").Bind(options)");
    }
}

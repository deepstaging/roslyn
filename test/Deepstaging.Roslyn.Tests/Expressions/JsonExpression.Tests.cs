// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class JsonExpressionTests
{
    [Test]
    public async Task Serialize_produces_correct_expression()
    {
        var expr = JsonExpression.Serialize("entity");

        await Assert.That(expr.Value).IsEqualTo("global::System.Text.Json.JsonSerializer.Serialize(entity)");
    }

    [Test]
    public async Task Serialize_with_options_produces_correct_expression()
    {
        var expr = JsonExpression.Serialize("entity", "options");

        await Assert.That(expr.Value).IsEqualTo("global::System.Text.Json.JsonSerializer.Serialize(entity, options)");
    }

    [Test]
    public async Task Deserialize_produces_correct_expression()
    {
        var expr = JsonExpression.Deserialize("Customer", "json");

        await Assert.That(expr.Value).IsEqualTo("global::System.Text.Json.JsonSerializer.Deserialize<Customer>(json)");
    }

    [Test]
    public async Task Deserialize_with_options_produces_correct_expression()
    {
        var expr = JsonExpression.Deserialize("Customer", "json", "options");

        await Assert.That(expr.Value).IsEqualTo("global::System.Text.Json.JsonSerializer.Deserialize<Customer>(json, options)");
    }
}

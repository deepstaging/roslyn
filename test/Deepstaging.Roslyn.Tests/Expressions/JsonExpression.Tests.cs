// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class JsonExpressionTests
{
    [Test]
    public async Task Serialize_produces_expression()
    {
        var expr = JsonExpression.Serialize("value");

        await Assert.That(expr.Value).Contains("Serialize(value)");
    }

    [Test]
    public async Task Deserialize_produces_expression()
    {
        var expr = JsonExpression.Deserialize("string", "json");

        await Assert.That(expr.Value).Contains("Deserialize<string>");
    }
}

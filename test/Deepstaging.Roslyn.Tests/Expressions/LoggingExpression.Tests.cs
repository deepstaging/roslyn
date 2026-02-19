// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class LoggingExpressionTests
{
    [Test]
    public async Task LogInformation_produces_expression()
    {
        var expr = LoggingExpression.LogInformation("logger", "\"Hello\"");

        await Assert.That(expr.Value).Contains("LogInformation");
    }

    [Test]
    public async Task LogError_produces_expression()
    {
        var expr = LoggingExpression.LogError("logger", "ex");

        await Assert.That(expr.Value).Contains("LogError");
    }
}

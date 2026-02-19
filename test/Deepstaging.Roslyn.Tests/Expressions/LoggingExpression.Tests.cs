// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class LoggingExpressionTests
{
    [Test]
    public async Task LogTrace_produces_correct_expression()
    {
        var expr = LoggingExpression.LogTrace("_logger", "\"msg\"");

        await Assert.That(expr.Value).IsEqualTo("_logger.LogTrace(\"msg\")");
    }

    [Test]
    public async Task LogDebug_produces_correct_expression()
    {
        var expr = LoggingExpression.LogDebug("_logger", "\"msg\"");

        await Assert.That(expr.Value).IsEqualTo("_logger.LogDebug(\"msg\")");
    }

    [Test]
    public async Task LogInformation_produces_correct_expression()
    {
        var expr = LoggingExpression.LogInformation("_logger", "\"msg\"");

        await Assert.That(expr.Value).IsEqualTo("_logger.LogInformation(\"msg\")");
    }

    [Test]
    public async Task LogWarning_produces_correct_expression()
    {
        var expr = LoggingExpression.LogWarning("_logger", "\"msg\"");

        await Assert.That(expr.Value).IsEqualTo("_logger.LogWarning(\"msg\")");
    }

    [Test]
    public async Task LogError_produces_correct_expression()
    {
        var expr = LoggingExpression.LogError("_logger", "ex", "\"msg\"");

        await Assert.That(expr.Value).IsEqualTo("_logger.LogError(ex, \"msg\")");
    }

    [Test]
    public async Task LogCritical_produces_correct_expression()
    {
        var expr = LoggingExpression.LogCritical("_logger", "ex");

        await Assert.That(expr.Value).IsEqualTo("_logger.LogCritical(ex)");
    }
}

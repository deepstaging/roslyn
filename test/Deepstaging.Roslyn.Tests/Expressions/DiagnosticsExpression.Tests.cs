// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class DiagnosticsExpressionTests
{
    [Test]
    public async Task StartActivity_produces_expression()
    {
        var expr = DiagnosticsExpression.StartActivity("source", "\"MyOp\"");

        await Assert.That(expr.Value).Contains("StartActivity");
    }

    [Test]
    public async Task StartNew_produces_expression()
    {
        var expr = DiagnosticsExpression.StartNew();

        await Assert.That(expr.Value).Contains("Stopwatch.StartNew()");
    }

    [Test]
    public async Task Assert_produces_expression()
    {
        var expr = DiagnosticsExpression.Assert("condition");

        await Assert.That(expr.Value).Contains("Debug.Assert");
    }
}

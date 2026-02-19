// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class DiagnosticsExpressionTests
{
    [Test]
    public async Task StartActivity_produces_correct_expression()
    {
        var expr = DiagnosticsExpression.StartActivity("source", "\"MyOp\"");

        await Assert.That(expr.Value).IsEqualTo("source.StartActivity(\"MyOp\")");
    }

    [Test]
    public async Task StartActivity_with_kind_produces_correct_expression()
    {
        var expr = DiagnosticsExpression.StartActivity("source", "\"MyOp\"", "kind");

        await Assert.That(expr.Value).IsEqualTo("source.StartActivity(\"MyOp\", kind)");
    }

    [Test]
    public async Task SetTag_produces_correct_expression()
    {
        var expr = DiagnosticsExpression.SetTag("activity", "\"key\"", "value");

        await Assert.That(expr.Value).IsEqualTo("activity.SetTag(\"key\", value)");
    }

    [Test]
    public async Task SetStatus_produces_correct_expression()
    {
        var expr = DiagnosticsExpression.SetStatus("activity", "code");

        await Assert.That(expr.Value).IsEqualTo("activity.SetStatus(code)");
    }

    [Test]
    public async Task StartNew_produces_correct_expression()
    {
        var expr = DiagnosticsExpression.StartNew();

        await Assert.That(expr.Value).IsEqualTo("global::System.Diagnostics.Stopwatch.StartNew()");
    }

    [Test]
    public async Task Assert_produces_correct_expression()
    {
        var expr = DiagnosticsExpression.Assert("condition");

        await Assert.That(expr.Value).IsEqualTo("global::System.Diagnostics.Debug.Assert(condition)");
    }

    [Test]
    public async Task Assert_with_message_produces_correct_expression()
    {
        var expr = DiagnosticsExpression.Assert("condition", "\"message\"");

        await Assert.That(expr.Value).IsEqualTo("global::System.Diagnostics.Debug.Assert(condition, \"message\")");
    }
}

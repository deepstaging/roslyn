// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class PropertyChangedExpressionTests
{
    [Test]
    public async Task NewEventArgs_with_string_produces_constructor()
    {
        var expr = PropertyChangedExpression.NewEventArgs("Name");

        await Assert.That(expr.Value)
            .IsEqualTo("new global::System.ComponentModel.PropertyChangedEventArgs(\"Name\")");
    }

    [Test]
    public async Task NewEventArgs_with_expression_produces_constructor()
    {
        var expr = PropertyChangedExpression.NewEventArgs(ExpressionRef.From("nameof(Name)"));

        await Assert.That(expr.Value)
            .IsEqualTo("new global::System.ComponentModel.PropertyChangedEventArgs(nameof(Name))");
    }

    [Test]
    public async Task NewChangingEventArgs_produces_constructor()
    {
        var expr = PropertyChangedExpression.NewChangingEventArgs("Name");

        await Assert.That(expr.Value)
            .IsEqualTo("new global::System.ComponentModel.PropertyChangingEventArgs(\"Name\")");
    }

    [Test]
    public async Task Raise_produces_null_conditional_invoke()
    {
        var expr = PropertyChangedExpression.Raise("PropertyChanged", "this", "args");

        await Assert.That(expr.Value)
            .IsEqualTo("PropertyChanged?.Invoke(this, args)");
    }
}
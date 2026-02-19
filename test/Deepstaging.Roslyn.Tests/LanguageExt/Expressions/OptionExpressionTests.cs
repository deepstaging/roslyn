// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.LanguageExt.Expressions;

using Roslyn.LanguageExt.Expressions;

public class OptionExpressionTests
{
    [Test]
    public async Task Optional_wraps_expression()
    {
        var result = OptionExpression.Optional("rt.Service.FindAsync(id)");

        await Assert.That(result.Value)
            .IsEqualTo("Optional(rt.Service.FindAsync(id))");
    }

    [Test]
    public async Task Some_wraps_expression()
    {
        var result = OptionExpression.Some("entity");

        await Assert.That(result.Value)
            .IsEqualTo("Some(entity)");
    }

    [Test]
    public async Task None_returns_None()
    {
        var result = OptionExpression.None;

        await Assert.That(result.Value)
            .IsEqualTo("None");
    }

    [Test]
    public async Task Optional_is_chainable()
    {
        var result = OptionExpression.Optional("value").Call("Map", "x => x.Name");

        await Assert.That(result.Value)
            .IsEqualTo("Optional(value).Map(x => x.Name)");
    }
}
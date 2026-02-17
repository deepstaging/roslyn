// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Expressions.LanguageExt;

using Roslyn.LanguageExt.Expressions;

public class FinExpressionTests
{
    [Test]
    public async Task FinSucc_wraps_expression()
    {
        var result = FinExpression.FinSucc("42");

        await Assert.That(result.Value)
            .IsEqualTo("FinSucc(42)");
    }

    [Test]
    public async Task FinFail_wraps_error_expression()
    {
        var result = FinExpression.FinFail("Error.New(ex)");

        await Assert.That(result.Value)
            .IsEqualTo("FinFail(Error.New(ex))");
    }

    [Test]
    public async Task FinFailMessage_wraps_string_message()
    {
        var result = FinExpression.FinFailMessage("\"something went wrong\"");

        await Assert.That(result.Value)
            .IsEqualTo("FinFail(Error.New(\"something went wrong\"))");
    }

    [Test]
    public async Task FinSucc_is_chainable()
    {
        var result = FinExpression.FinSucc("value").Call("Map", "x => x + 1");

        await Assert.That(result.Value)
            .IsEqualTo("FinSucc(value).Map(x => x + 1)");
    }
}

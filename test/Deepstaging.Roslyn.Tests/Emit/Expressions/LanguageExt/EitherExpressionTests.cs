// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Expressions.LanguageExt;

using Roslyn.LanguageExt.Expressions;

public class EitherExpressionTests
{
    [Test]
    public async Task Right_wraps_expression()
    {
        var result = EitherExpression.Right("entity");

        await Assert.That(result.Value)
            .IsEqualTo("Right(entity)");
    }

    [Test]
    public async Task Left_wraps_expression()
    {
        var result = EitherExpression.Left("Error.New(\"not found\")");

        await Assert.That(result.Value)
            .IsEqualTo("Left(Error.New(\"not found\"))");
    }

    [Test]
    public async Task Right_is_chainable()
    {
        var result = EitherExpression.Right("value").Call("Map", "x => x.Id");

        await Assert.That(result.Value)
            .IsEqualTo("Right(value).Map(x => x.Id)");
    }
}

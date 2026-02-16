// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Expressions.LanguageExt;

using Roslyn.LanguageExt.Expressions;

public class EffLiftIOTests
{
    private readonly EffLiftIO _io = EffExpression.LiftIO("Eff<RT, int>", "rt");

    [Test]
    public async Task Async_produces_LiftIO_with_await()
    {
        var result = _io.Async("query(rt).CountAsync(token)");

        await Assert.That(result)
            .IsEqualTo("Eff<RT, int>.LiftIO(async rt => await query(rt).CountAsync(token))");
    }

    [Test]
    public async Task AsyncOptional_produces_LiftIO_with_Optional_wrapper()
    {
        var io = EffExpression.LiftIO("Eff<RT, Option<User>>", "rt");

        var result = io.AsyncOptional("query(rt).FirstOrDefaultAsync(token)");

        await Assert.That(result)
            .IsEqualTo("Eff<RT, Option<User>>.LiftIO(async rt => Optional(await query(rt).FirstOrDefaultAsync(token)))");
    }

    [Test]
    public async Task AsyncNonNull_produces_LiftIO_with_null_forgiving()
    {
        var io = EffExpression.LiftIO("Eff<RT, T>", "rt");

        var result = io.AsyncNonNull("query(rt).MaxAsync(token)");

        await Assert.That(result)
            .IsEqualTo("Eff<RT, T>.LiftIO(async rt => (await query(rt).MaxAsync(token))!)");
    }

    [Test]
    public async Task Body_produces_LiftIO_with_custom_lambda()
    {
        var result = _io.Body("rt => rt.Db.ExecuteSqlAsync(sql)");

        await Assert.That(result)
            .IsEqualTo("Eff<RT, int>.LiftIO(rt => rt.Db.ExecuteSqlAsync(sql))");
    }

    [Test]
    public async Task LiftIO_with_custom_param()
    {
        var io = EffExpression.LiftIO("Eff<RT, bool>", "env");

        var result = io.Async("env.Service.AnyAsync(token)");

        await Assert.That(result)
            .IsEqualTo("Eff<RT, bool>.LiftIO(async env => await env.Service.AnyAsync(token))");
    }
}

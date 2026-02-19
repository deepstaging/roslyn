// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.LanguageExt.Expressions;

using Roslyn.LanguageExt.Expressions;
using Roslyn.LanguageExt.Types;

public class EffLiftIOTests
{
    private readonly EffLiftIO _io = EffExpression.LiftIO(LanguageExtRefs.Eff("RT", "int"), "rt");

    [Test]
    public async Task Async_produces_LiftIO_with_await()
    {
        var result = _io.Async("query(rt).CountAsync(token)");

        await Assert.That(result)
            .IsEqualTo("global::LanguageExt.Eff<RT, int>.LiftIO(async rt => await query(rt).CountAsync(token))");
    }

    [Test]
    public async Task AsyncOptional_produces_LiftIO_with_Optional_wrapper()
    {
        var io = EffExpression.LiftIO(LanguageExtRefs.Eff("RT", LanguageExtRefs.Option("User")), "rt");

        var result = io.AsyncOptional("query(rt).FirstOrDefaultAsync(token)");

        await Assert.That(result)
            .IsEqualTo(
                "global::LanguageExt.Eff<RT, global::LanguageExt.Option<User>>.LiftIO(async rt => Optional(await query(rt).FirstOrDefaultAsync(token)))");
    }

    [Test]
    public async Task AsyncVoid_produces_LiftIO_with_unit_return()
    {
        var io = EffExpression.LiftIO(LanguageExtRefs.Eff("RT", LanguageExtRefs.Unit), "rt");

        var result = io.AsyncVoid("rt.Service.DeleteAsync(token)");

        await Assert.That(result)
            .IsEqualTo(
                "global::LanguageExt.Eff<RT, global::LanguageExt.Unit>.LiftIO(async rt => { await rt.Service.DeleteAsync(token); return unit; })");
    }

    [Test]
    public async Task AsyncNonNull_produces_LiftIO_with_null_forgiving()
    {
        var io = EffExpression.LiftIO(LanguageExtRefs.Eff("RT", "T"), "rt");

        var result = io.AsyncNonNull("query(rt).MaxAsync(token)");

        await Assert.That(result)
            .IsEqualTo("global::LanguageExt.Eff<RT, T>.LiftIO(async rt => (await query(rt).MaxAsync(token))!)");
    }

    [Test]
    public async Task Sync_produces_LiftIO_with_expression_body()
    {
        var result = _io.Sync("rt.Config.ConnectionString");

        await Assert.That(result)
            .IsEqualTo("global::LanguageExt.Eff<RT, int>.LiftIO(rt => rt.Config.ConnectionString)");
    }

    [Test]
    public async Task SyncVoid_produces_LiftIO_with_unit_return()
    {
        var io = EffExpression.LiftIO(LanguageExtRefs.Eff("RT", LanguageExtRefs.Unit), "rt");

        var result = io.SyncVoid("rt.Service.Reset()");

        await Assert.That(result)
            .IsEqualTo(
                "global::LanguageExt.Eff<RT, global::LanguageExt.Unit>.LiftIO(rt => { rt.Service.Reset(); return unit; })");
    }

    [Test]
    public async Task SyncOptional_produces_LiftIO_with_Optional_wrapper()
    {
        var io = EffExpression.LiftIO(LanguageExtRefs.Eff("RT", LanguageExtRefs.Option("string")), "rt");

        var result = io.SyncOptional("rt.Config.TryGetValue(key)");

        await Assert.That(result)
            .IsEqualTo(
                "global::LanguageExt.Eff<RT, global::LanguageExt.Option<string>>.LiftIO(rt => Optional(rt.Config.TryGetValue(key)))");
    }

    [Test]
    public async Task SyncNonNull_produces_LiftIO_with_null_forgiving()
    {
        var result = _io.SyncNonNull("rt.Config.GetValue(key)");

        await Assert.That(result)
            .IsEqualTo("global::LanguageExt.Eff<RT, int>.LiftIO(rt => (rt.Config.GetValue(key))!)");
    }

    [Test]
    public async Task Body_produces_LiftIO_with_custom_lambda()
    {
        var result = _io.Body("rt => rt.Db.ExecuteSqlAsync(sql)");

        await Assert.That(result)
            .IsEqualTo("global::LanguageExt.Eff<RT, int>.LiftIO(rt => rt.Db.ExecuteSqlAsync(sql))");
    }

    [Test]
    public async Task LiftIO_with_custom_param()
    {
        var io = EffExpression.LiftIO(LanguageExtRefs.Eff("RT", "bool"), "env");

        var result = io.Async("env.Service.AnyAsync(token)");

        await Assert.That(result)
            .IsEqualTo("global::LanguageExt.Eff<RT, bool>.LiftIO(async env => await env.Service.AnyAsync(token))");
    }
}
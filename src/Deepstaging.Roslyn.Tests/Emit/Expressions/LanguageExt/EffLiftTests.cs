// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Expressions.LanguageExt;

using Roslyn.LanguageExt.Expressions;

public class EffLiftTests
{
    private readonly EffLift _lift = EffExpression.Lift("RT", "rt");

    // ── Async ───────────────────────────────────────────────────────────

    [Test]
    public async Task Async_produces_liftEff_with_await()
    {
        var result = _lift.Async("int", "rt.Service.GetCountAsync()");

        await Assert.That(result)
            .IsEqualTo("liftEff<RT, int>(async rt => await rt.Service.GetCountAsync())");
    }

    [Test]
    public async Task AsyncVoid_produces_liftEff_with_unit_return()
    {
        var result = _lift.AsyncVoid("rt.Service.DeleteAsync()");

        await Assert.That(result)
            .IsEqualTo("liftEff<RT, Unit>(async rt => { await rt.Service.DeleteAsync(); return unit; })");
    }

    [Test]
    public async Task AsyncOptional_produces_liftEff_with_Optional_wrapper()
    {
        var result = _lift.AsyncOptional("Option<User>", "rt.Service.FindAsync(id)");

        await Assert.That(result)
            .IsEqualTo("liftEff<RT, Option<User>>(async rt => Optional(await rt.Service.FindAsync(id)))");
    }

    // ── Sync ────────────────────────────────────────────────────────────

    [Test]
    public async Task Sync_produces_liftEff_with_expression_body()
    {
        var result = _lift.Sync("string", "rt.Config.ConnectionString");

        await Assert.That(result)
            .IsEqualTo("liftEff<RT, string>(rt => rt.Config.ConnectionString)");
    }

    [Test]
    public async Task SyncVoid_produces_liftEff_with_unit_return()
    {
        var result = _lift.SyncVoid("rt.Service.Reset()");

        await Assert.That(result)
            .IsEqualTo("liftEff<RT, Unit>(rt => { rt.Service.Reset(); return unit; })");
    }

    [Test]
    public async Task SyncOptional_produces_liftEff_with_Optional_wrapper()
    {
        var result = _lift.SyncOptional("Option<string>", "rt.Config.TryGetValue(key)");

        await Assert.That(result)
            .IsEqualTo("liftEff<RT, Option<string>>(rt => Optional(rt.Config.TryGetValue(key)))");
    }

    // ── Body ────────────────────────────────────────────────────────────

    [Test]
    public async Task Body_produces_liftEff_with_custom_lambda()
    {
        var result = _lift.Body("User", "rt => { rt.Db.Users.Add(entity); return entity; }");

        await Assert.That(result)
            .IsEqualTo("liftEff<RT, User>(rt => { rt.Db.Users.Add(entity); return entity; })");
    }

    // ── Custom RT/param ─────────────────────────────────────────────────

    [Test]
    public async Task Lift_with_custom_rt_and_param()
    {
        var lift = EffExpression.Lift("MyRuntime", "env");

        var result = lift.Async("int", "env.Service.GetAsync()");

        await Assert.That(result)
            .IsEqualTo("liftEff<MyRuntime, int>(async env => await env.Service.GetAsync())");
    }
}

// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Expressions.LanguageExt;

using Roslyn.LanguageExt.Expressions;
using Roslyn.LanguageExt.Refs;

public class LiftingStrategyTests
{
    private readonly EffLift _lift = EffExpression.Lift("RT", "rt");

    // ── Dispatch ────────────────────────────────────────────────────────

    [Test]
    public async Task AsyncValue_dispatches_to_Async()
    {
        var result = _lift.Lift(LiftingStrategy.AsyncValue, "int", "rt.Service.GetCountAsync()");

        await Assert.That(result)
            .IsEqualTo("liftEff<RT, int>(async rt => await rt.Service.GetCountAsync())");
    }

    [Test]
    public async Task AsyncVoid_dispatches_to_AsyncVoid()
    {
        var result = _lift.Lift(LiftingStrategy.AsyncVoid, "ignored", "rt.Service.DeleteAsync()");

        await Assert.That(result)
            .IsEqualTo("liftEff<RT, Unit>(async rt => { await rt.Service.DeleteAsync(); return unit; })");
    }

    [Test]
    public async Task AsyncOptional_dispatches_and_wraps_in_Option()
    {
        var result = _lift.Lift(LiftingStrategy.AsyncOptional, "User", "rt.Service.FindAsync(id)");

        await Assert.That(result)
            .IsEqualTo("liftEff<RT, global::LanguageExt.Option<User>>(async rt => Optional(await rt.Service.FindAsync(id)))");
    }

    [Test]
    public async Task AsyncNonNull_dispatches_with_null_forgiving()
    {
        var result = _lift.Lift(LiftingStrategy.AsyncNonNull, "string", "rt.Service.GetNameAsync()");

        await Assert.That(result)
            .IsEqualTo("liftEff<RT, string>(async rt => (await rt.Service.GetNameAsync())!)");
    }

    [Test]
    public async Task SyncValue_dispatches_to_Sync()
    {
        var result = _lift.Lift(LiftingStrategy.SyncValue, "string", "rt.Config.ConnectionString");

        await Assert.That(result)
            .IsEqualTo("liftEff<RT, string>(rt => rt.Config.ConnectionString)");
    }

    [Test]
    public async Task SyncVoid_dispatches_to_SyncVoid()
    {
        var result = _lift.Lift(LiftingStrategy.SyncVoid, "ignored", "rt.Service.Reset()");

        await Assert.That(result)
            .IsEqualTo("liftEff<RT, Unit>(rt => { rt.Service.Reset(); return unit; })");
    }

    [Test]
    public async Task SyncOptional_dispatches_and_wraps_in_Option()
    {
        var result = _lift.Lift(LiftingStrategy.SyncOptional, "string", "rt.Config.TryGetValue(key)");

        await Assert.That(result)
            .IsEqualTo("liftEff<RT, global::LanguageExt.Option<string>>(rt => Optional(rt.Config.TryGetValue(key)))");
    }

    [Test]
    public async Task SyncNonNull_dispatches_with_null_forgiving()
    {
        var result = _lift.Lift(LiftingStrategy.SyncNonNull, "string", "rt.Config.GetValue(key)");

        await Assert.That(result)
            .IsEqualTo("liftEff<RT, string>(rt => (rt.Config.GetValue(key))!)");
    }

    // ── EffReturnType ───────────────────────────────────────────────────

    [Test]
    public async Task EffReturnType_AsyncValue_returns_raw_type()
    {
        var result = LiftingStrategy.AsyncValue.EffReturnType("int");

        await Assert.That(result.Value).IsEqualTo("int");
    }

    [Test]
    public async Task EffReturnType_AsyncOptional_wraps_in_Option()
    {
        var result = LiftingStrategy.AsyncOptional.EffReturnType("User");

        await Assert.That(result.Value).IsEqualTo("global::LanguageExt.Option<User>");
    }

    [Test]
    public async Task EffReturnType_SyncOptional_wraps_in_Option()
    {
        var result = LiftingStrategy.SyncOptional.EffReturnType("string");

        await Assert.That(result.Value).IsEqualTo("global::LanguageExt.Option<string>");
    }

    [Test]
    public async Task EffReturnType_AsyncVoid_returns_Unit()
    {
        var result = LiftingStrategy.AsyncVoid.EffReturnType("ignored");

        await Assert.That(result.Value).IsEqualTo("global::LanguageExt.Unit");
    }

    [Test]
    public async Task EffReturnType_SyncVoid_returns_Unit()
    {
        var result = LiftingStrategy.SyncVoid.EffReturnType("ignored");

        await Assert.That(result.Value).IsEqualTo("global::LanguageExt.Unit");
    }

    [Test]
    public async Task EffReturnType_AsyncNonNull_returns_raw_type()
    {
        var result = LiftingStrategy.AsyncNonNull.EffReturnType("string");

        await Assert.That(result.Value).IsEqualTo("string");
    }
}

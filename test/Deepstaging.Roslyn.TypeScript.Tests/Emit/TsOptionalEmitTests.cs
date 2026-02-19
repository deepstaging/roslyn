// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.TypeScript.Emit;

namespace Deepstaging.Roslyn.TypeScript.Tests.Emit;

public class TsOptionalEmitTests
{
    [Test]
    public async Task ValidateOrThrow_ReturnsValidEmit_OnSuccess()
    {
        var result = TsTypeBuilder.Interface("Foo")
            .AddProperty("x", "number", p => p)
            .Emit();

        var valid = result.ValidateOrThrow();
        await Assert.That(valid.Code).Contains("interface Foo");
    }

    [Test]
    public async Task Success_IsTrueForValidEmit()
    {
        var result = TsTypeBuilder.Interface("Foo")
            .AddProperty("x", "number", p => p)
            .Emit();

        await Assert.That(result.Success).IsTrue();
    }

    [Test]
    public async Task Code_ContainsEmittedSource()
    {
        var result = TsTypeBuilder.TypeAlias("Id", "string").Emit();
        await Assert.That(result.Code).Contains("type Id = string;");
    }

    [Test]
    public async Task Diagnostics_IsEmptyOnSuccess()
    {
        var result = TsTypeBuilder.Interface("Bar").Emit();
        await Assert.That(result.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task TryValidate_ReturnsTrueOnSuccess()
    {
        var result = TsTypeBuilder.Interface("Baz").Emit();
        var success = result.TryValidate(out var valid);

        await Assert.That(success).IsTrue();
        await Assert.That(valid.Code).Contains("interface Baz");
    }

    [Test]
    public async Task ValidEmit_ToString_ReturnsCode()
    {
        var result = TsTypeBuilder.Interface("Qux").Emit();
        var valid = result.ValidateOrThrow();

        await Assert.That(valid.ToString()).IsEqualTo(valid.Code);
    }
}

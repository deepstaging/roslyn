// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.TypeScript.Emit;
using Deepstaging.Roslyn.TypeScript.Testing;

namespace Deepstaging.Roslyn.TypeScript.Tests.Emit;

public class TsValidationTests : TsTestBase
{
    [Test]
    public async Task ValidCode_PassesTscValidation()
    {
        var result = TsTypeBuilder.Interface("User")
            .Exported()
            .AddProperty("id", "number", p => p.AsReadonly())
            .AddProperty("name", "string", p => p)
            .AddProperty("email", "string", p => p.AsOptional())
            .Emit(ValidatedOptions);

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task ValidClass_PassesTscValidation()
    {
        var result = TsTypeBuilder.Class("Counter")
            .Exported()
            .AddField("count", "number", f => f
                .WithAccessibility(TsAccessibility.Private)
                .WithInitializer("0"))
            .AddMethod("increment", m => m
                .WithReturnType("void")
                .WithBody(b => b
                    .AddStatement("this.count++")))
            .AddMethod("getCount", m => m
                .WithReturnType("number")
                .WithExpressionBody("this.count"))
            .Emit(ValidatedOptions);

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task ValidEnum_PassesTscValidation()
    {
        var result = TsTypeBuilder.Enum("Direction")
            .Exported()
            .AddEnumMember("Up", "'up'")
            .AddEnumMember("Down", "'down'")
            .AddEnumMember("Left", "'left'")
            .AddEnumMember("Right", "'right'")
            .Emit(ValidatedOptions);

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task ValidTypeAlias_PassesTscValidation()
    {
        var result = TsTypeBuilder.TypeAlias("Result", "{ success: true; data: unknown } | { success: false; error: Error }")
            .Exported()
            .AddTypeParameter("T")
            .Emit(ValidatedOptions);

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task ValidGenericClass_PassesTscValidation()
    {
        var result = TsTypeBuilder.Class("Repository")
            .Exported()
            .AsAbstract()
            .AddTypeParameter("T")
            .AddMethod("findById", m => m
                .AsAbstract()
                .AddParameter("id", "string")
                .WithReturnType("Promise<T | null>"))
            .AddMethod("save", m => m
                .AsAbstract()
                .AddParameter("entity", "T")
                .WithReturnType("Promise<void>"))
            .Emit(ValidatedOptions);

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task ValidConstructorParameterProperties_PassesTscValidation()
    {
        var result = TsTypeBuilder.Class("Point")
            .Exported()
            .AddConstructor(c => c
                .AddParameter("x", "number", p => p.AsParameterProperty(TsAccessibility.Public).AsReadonlyParameterProperty())
                .AddParameter("y", "number", p => p.AsParameterProperty(TsAccessibility.Public).AsReadonlyParameterProperty()))
            .AddMethod("distanceTo", m => m
                .AddParameter("other", "Point")
                .WithReturnType("number")
                .WithBody(b => b
                    .AddConst("dx", "this.x - other.x")
                    .AddConst("dy", "this.y - other.y")
                    .AddReturn("Math.sqrt(dx * dx + dy * dy)")))
            .Emit(ValidatedOptions);

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task NoValidation_SkipsTsc()
    {
        var result = TsTypeBuilder.Interface("Foo")
            .AddProperty("x", "number", p => p)
            .Emit(); // default options — no validation

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Diagnostics).IsEmpty();
    }
}

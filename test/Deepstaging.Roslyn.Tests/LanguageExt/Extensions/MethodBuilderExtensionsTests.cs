// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.LanguageExt.Extensions;

using Roslyn.Emit;
using Roslyn.LanguageExt;
using Roslyn.LanguageExt.Expressions;
using Roslyn.LanguageExt.Types;

public class MethodBuilderExtensionsTests
{
    [Test]
    public async Task AsEffMethod_adds_RT_type_parameter_and_constraint()
    {
        var method = MethodBuilder
            .Parse("public void GetCount()")
            .AsEffMethod("RT", "IHasDb", "int");

        await Assert.That(method.IsStatic).IsTrue();

        await Assert.That(method.ReturnType)
            .IsEqualTo("global::LanguageExt.Eff<RT, int>");
    }

    [Test]
    public async Task AsEffMethod_with_Option_return_type()
    {
        var method = MethodBuilder
            .Parse("public void FindUser()")
            .AsEffMethod("RT", "IHasDb", LanguageExtTypes.Option("User"));

        await Assert.That(method.ReturnType)
            .IsEqualTo("global::LanguageExt.Eff<RT, global::LanguageExt.Option<User>>");
    }

    [Test]
    public async Task AsEffMethod_with_LiftingStrategy_EffReturnType()
    {
        var strategy = LiftingStrategy.AsyncOptional;
        var returnType = strategy.EffReturnType("User");

        var method = MethodBuilder
            .Parse("public void FindUser(int id)")
            .AsEffMethod("RT", "IHasDb", returnType);

        await Assert.That(method.ReturnType)
            .IsEqualTo("global::LanguageExt.Eff<RT, global::LanguageExt.Option<User>>");
    }
}
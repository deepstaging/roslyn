// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.LanguageExt;

using Deepstaging.Roslyn.Emit;
using Deepstaging.Roslyn.LanguageExt.Extensions;

public class TypeBuilderExtensionsTests : RoslynTestBase
{
    [Test]
    public async Task AddLanguageExtUsings_adds_three_standard_usings()
    {
        var type = TypeBuilder
            .Parse("public partial class MyEffects")
            .AddLanguageExtUsings();

        var result = type.Emit();

        await Assert.That(result.Code).Contains("using LanguageExt;");
        await Assert.That(result.Code).Contains("using LanguageExt.Effects;");
        await Assert.That(result.Code).Contains("using static LanguageExt.Prelude;");
    }
}

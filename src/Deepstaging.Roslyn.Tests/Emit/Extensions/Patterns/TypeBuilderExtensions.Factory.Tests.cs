// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit;
using Deepstaging.Roslyn.Emit.Patterns;

namespace Deepstaging.Roslyn.Tests.Emit.Extensions.Patterns;

public sealed class TypeBuilderExtensionsFactoryTests : RoslynTestBase
{
    [Test]
    public async Task WithEmptyFactory_AddsStaticReadonlyField()
    {
        var type = TypeBuilder.Parse("public partial struct UserId")
            .WithEmptyFactory("new UserId(global::System.Guid.Empty)");

        var result = type.Emit();

        await Assert.That(result.Code)
            .Contains("public static readonly UserId Empty = new UserId(global::System.Guid.Empty);");
    }

    [Test]
    public async Task WithDefaultFactory_AddsDefaultField()
    {
        var type = TypeBuilder.Parse("public partial struct Config")
            .WithDefaultFactory("new Config(\"default\")");

        var result = type.Emit();

        await Assert.That(result.Code).Contains("public static readonly Config Default = new Config(\"default\");");
    }

    [Test]
    public async Task WithNewFactory_AddsStaticMethod()
    {
        var type = TypeBuilder.Parse("public partial struct UserId")
            .WithNewFactory("new UserId(global::System.Guid.NewGuid())");

        var result = type.Emit();

        await Assert.That(result.Code).Contains("public static UserId New()");
        await Assert.That(result.Code).Contains("new UserId(global::System.Guid.NewGuid())");
    }

    [Test]
    public async Task WithEmptyAndNewFactory_AddsBoth()
    {
        var type = TypeBuilder.Parse("public partial struct UserId")
            .WithEmptyAndNewFactory(
                "new UserId(global::System.Guid.Empty)",
                "new UserId(global::System.Guid.NewGuid())");

        var result = type.Emit();

        await Assert.That(result.Code).Contains("public static readonly UserId Empty");
        await Assert.That(result.Code).Contains("public static UserId New()");
    }
}
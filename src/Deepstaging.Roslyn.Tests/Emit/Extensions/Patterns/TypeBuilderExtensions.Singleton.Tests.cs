// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Patterns;

namespace Deepstaging.Roslyn.Tests.Emit.Patterns;

public class TypeBuilderSingletonTests : RoslynTestBase
{
    [Test]
    public async Task AsSingleton_adds_private_constructor_and_instance_property()
    {
        var result = TypeBuilder
            .Class("Configuration")
            .InNamespace("Test")
            .AsSingleton()
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("private Configuration()");
        await Assert.That(result.Code).Contains("static");
        await Assert.That(result.Code).Contains("Instance");
        await Assert.That(result.Code).Contains("Lazy<Configuration>");
    }

    [Test]
    public async Task AsSingleton_with_custom_property_name()
    {
        var result = TypeBuilder
            .Class("Logger")
            .InNamespace("Test")
            .AsSingleton("Current")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Current");
        await Assert.That(result.Code).Contains("_current");
        await Assert.That(result.Code).DoesNotContain("Instance");
    }
}

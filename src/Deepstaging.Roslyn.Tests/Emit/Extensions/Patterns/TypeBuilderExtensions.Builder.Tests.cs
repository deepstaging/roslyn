// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Patterns;

namespace Deepstaging.Roslyn.Tests.Emit.Patterns;

public class TypeBuilderBuilderPatternTests : RoslynTestBase
{
    [Test]
    public async Task WithBuilder_adds_nested_builder_class()
    {
        var result = TypeBuilder
            .Class("Person")
            .InNamespace("Test")
            .AddProperty("Name", "string", p => p.WithAutoPropertyAccessors())
            .AddProperty("Age", "int", p => p.WithAutoPropertyAccessors())
            .WithBuilder()
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("record Builder");
        await Assert.That(result.Code).Contains("WithName");
        await Assert.That(result.Code).Contains("WithAge");
        await Assert.That(result.Code).Contains("this with");
        await Assert.That(result.Code).Contains("Build()");
        await Assert.That(result.Code).Contains("CreateBuilder()");
    }

    [Test]
    public async Task WithBuilder_with_custom_name()
    {
        var result = TypeBuilder
            .Class("Config")
            .InNamespace("Test")
            .AddProperty("Value", "string", p => p.WithAutoPropertyAccessors())
            .WithBuilder("ConfigBuilder")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("record ConfigBuilder");
        await Assert.That(result.Code).Contains("ConfigBuilder CreateBuilder()");
    }
}
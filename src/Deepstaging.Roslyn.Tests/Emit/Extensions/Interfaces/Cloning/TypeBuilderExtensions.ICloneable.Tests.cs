// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Cloning;

namespace Deepstaging.Roslyn.Tests.Emit.Interfaces.Cloning;

public class TypeBuilderCloneableExtensionsTests : RoslynTestBase
{
    [Test]
    public async Task ImplementsICloneable_adds_clone_method()
    {
        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsICloneable()
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("ICloneable");
        await Assert.That(result.Code).Contains("object Clone()");
        await Assert.That(result.Code).Contains("=> this");
    }

    [Test]
    public async Task ImplementsICloneable_with_custom_expression()
    {
        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsICloneable("new(Value)")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("ICloneable");
        await Assert.That(result.Code).Contains("=> new(Value)");
    }
}

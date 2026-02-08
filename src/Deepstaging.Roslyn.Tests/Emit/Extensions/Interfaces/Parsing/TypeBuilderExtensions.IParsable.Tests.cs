// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Parsing;

namespace Deepstaging.Roslyn.Tests.Emit.Interfaces.Parsing;

/// <summary>
/// Tests for TypeBuilder IParsable interface implementation extensions.
/// </summary>
public class TypeBuilderIParsableExtensionsTests : RoslynTestBase
{
    [Test]
    public async Task IParsable_with_Guid_generates_NET7_conditional()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .AddProperty("Value", "global::System.Guid", p => p.AsReadOnly())
            .ImplementsIParsable(WellKnownSymbols.Guid)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#if NET7_0_OR_GREATER");
        await Assert.That(result.Code).Contains("IParsable<MyId>");
        await Assert.That(result.Code).Contains("Guid.Parse(input, provider)");
        await Assert.That(result.Code).Contains("Guid.TryParse");
    }

    [Test]
    public async Task IParsable_with_Int32_generates_int_parse()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsIParsable(WellKnownSymbols.Int32)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("int.Parse(input, provider)");
        await Assert.That(result.Code).Contains("int.TryParse");
    }
}

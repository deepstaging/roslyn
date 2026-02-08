// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Parsing;

namespace Deepstaging.Roslyn.Tests.Emit.Interfaces.Parsing;

/// <summary>
/// Tests for TypeBuilder ISpanParsable interface implementation extensions.
/// </summary>
public class TypeBuilderISpanParsableExtensionsTests : RoslynTestBase
{
    [Test]
    public async Task ISpanParsable_with_Guid_generates_span_parse()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .AddProperty("Value", "global::System.Guid", p => p.AsReadOnly())
            .ImplementsISpanParsable(WellKnownSymbols.Guid)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("ISpanParsable<MyId>");
        await Assert.That(result.Code).Contains("ReadOnlySpan<char>");
        await Assert.That(result.Code).Contains("Guid.Parse(input)");
    }
}

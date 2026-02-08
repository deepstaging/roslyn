// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Parsing;

namespace Deepstaging.Roslyn.Tests.Emit.Interfaces.Parsing;

/// <summary>
/// Tests for TypeBuilder IUtf8SpanParsable interface implementation extensions.
/// </summary>
public class TypeBuilderIUtf8SpanParsableExtensionsTests : RoslynTestBase
{
    [Test]
    public async Task IUtf8SpanParsable_with_Int32_generates_utf8_parse()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsIUtf8SpanParsable(WellKnownSymbols.Int32)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#if NET8_0_OR_GREATER");
        await Assert.That(result.Code).Contains("IUtf8SpanParsable<MyId>");
        await Assert.That(result.Code).Contains("ReadOnlySpan<byte>");
    }

    [Test]
    public async Task IUtf8SpanParsable_with_Guid_is_skipped()
    {
        // Guid doesn't support UTF-8 parsing in .NET 8
        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .AddProperty("Value", "global::System.Guid", p => p.AsReadOnly())
            .ImplementsIUtf8SpanParsable(WellKnownSymbols.Guid)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        // Should NOT contain IUtf8SpanParsable since Guid doesn't support it
        await Assert.That(result.Code).DoesNotContain("IUtf8SpanParsable");
    }
}

// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Formatting;

namespace Deepstaging.Roslyn.Tests.Emit.Interfaces.Formatting;

/// <summary>
/// Tests for TypeBuilder IUtf8SpanFormattable interface implementation extensions.
/// </summary>
public class TypeBuilderIUtf8SpanFormattableExtensionsTests : RoslynTestBase
{
    [Test]
    public async Task IUtf8SpanFormattable_with_Guid_generates_NET8_conditional()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .AddProperty("Value", "global::System.Guid", p => p.AsReadOnly())
            .ImplementsIUtf8SpanFormattable(WellKnownSymbols.Guid, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#if NET8_0_OR_GREATER");
        await Assert.That(result.Code).Contains("IUtf8SpanFormattable");
        await Assert.That(result.Code).Contains("Span<byte>");
    }

    [Test]
    public async Task IUtf8SpanFormattable_with_String_generates_utf8_encoding()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .AddProperty("Value", "string?", p => p.AsReadOnly())
            .ImplementsIUtf8SpanFormattable(WellKnownSymbols.String, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Encoding.UTF8.GetBytes");
    }
}

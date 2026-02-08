// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Formatting;

namespace Deepstaging.Roslyn.Tests.Emit.Interfaces.Formatting;

/// <summary>
/// Tests for TypeBuilder ISpanFormattable interface implementation extensions.
/// </summary>
public class TypeBuilderISpanFormattableExtensionsTests : RoslynTestBase
{
    [Test]
    public async Task ISpanFormattable_with_Guid_generates_NET6_conditional()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .AddProperty("Value", "global::System.Guid", p => p.AsReadOnly())
            .ImplementsISpanFormattable(WellKnownSymbols.Guid, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#if NET6_0_OR_GREATER");
        await Assert.That(result.Code).Contains("ISpanFormattable");
        await Assert.That(result.Code).Contains("TryFormat");
    }

    [Test]
    public async Task ISpanFormattable_with_String_generates_span_copy()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .AddProperty("Value", "string?", p => p.AsReadOnly())
            .ImplementsISpanFormattable(WellKnownSymbols.String, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("AsSpan()");
        await Assert.That(result.Code).Contains("TryCopyTo(destination)");
    }
}

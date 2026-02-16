// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Formatting;

namespace Deepstaging.Roslyn.Tests.Emit.Interfaces.Formatting;

/// <summary>
/// Tests for TypeBuilder IFormattable interface implementation extensions.
/// </summary>
public class TypeBuilderIFormattableExtensionsTests : RoslynTestBase
{
    [Test]
    public async Task IFormattable_with_Guid_generates_with_GuidFormat_attribute()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .AddProperty("Value", "global::System.Guid", p => p.AsReadOnly())
            .ImplementsIFormattable(WellKnownSymbols.Guid.ToSnapshot(), "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IFormattable");
        await Assert.That(result.Code).Contains("Value.ToString(format, formatProvider)");
        await Assert.That(result.Code).Contains("StringSyntax");
        await Assert.That(result.Code).Contains("GuidFormat");
    }

    [Test]
    public async Task IFormattable_with_Int32_generates_conditional_NumericFormat_attribute()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsIFormattable(WellKnownSymbols.Int32.ToSnapshot(), "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        // Generates conditional block for NET7+ only (for numeric types)
        await Assert.That(result.Code).Contains("#if NET7_0_OR_GREATER");
        await Assert.That(result.Code).Contains("IFormattable");
        await Assert.That(result.Code).Contains("StringSyntax");
        await Assert.That(result.Code).Contains("NumericFormat");
    }

    [Test]
    public async Task IFormattable_with_String_generates_null_coalescing()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .AddProperty("Value", "string?", p => p.AsReadOnly())
            .ImplementsIFormattable(WellKnownSymbols.String.ToSnapshot(), "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Value ?? string.Empty");
    }

    #region PropertyBuilder/FieldBuilder Overloads

    [Test]
    public async Task IFormattable_accepts_PropertyBuilder()
    {
        var valueProperty = PropertyBuilder.For("Value", "int").AsReadOnly();

        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty(valueProperty)
            .ImplementsIFormattable(WellKnownSymbols.Int32.ToSnapshot(), valueProperty)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Value.ToString(format, formatProvider)");
    }

    [Test]
    public async Task IFormattable_accepts_FieldBuilder()
    {
        var valueField = FieldBuilder.Parse("private readonly int _count;");

        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddField(valueField)
            .ImplementsIFormattable(WellKnownSymbols.Int32.ToSnapshot(), valueField)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("_count.ToString(format, formatProvider)");
    }

    #endregion
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Operators.Bitwise;

namespace Deepstaging.Roslyn.Tests.Emit.Operators.Bitwise;

public class TypeBuilderBitwiseOperatorsTests : RoslynTestBase
{
    #region Bitwise AND

    [Test]
    public async Task ImplementsBitwiseAndOperator_adds_and_operator()
    {
        var result = TypeBuilder
            .Struct("Flags")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsBitwiseAndOperator(WellKnownSymbols.Int32, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("operator &");
        await Assert.That(result.Code).Contains("left.Value & right.Value");
        await Assert.That(result.Code).Contains("IBitwiseOperators<Flags, Flags, Flags>");
    }

    [Test]
    public async Task ImplementsBitwiseAndOperator_with_custom_expression()
    {
        var result = TypeBuilder
            .Struct("Flags")
            .InNamespace("Test")
            .ImplementsBitwiseAndOperator("new(left.Value & right.Value)")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("operator &");
    }

    #endregion

    #region Bitwise OR

    [Test]
    public async Task ImplementsBitwiseOrOperator_adds_or_operator()
    {
        var result = TypeBuilder
            .Struct("Flags")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsBitwiseOrOperator(WellKnownSymbols.Int32, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("operator |");
        await Assert.That(result.Code).Contains("left.Value | right.Value");
    }

    #endregion

    #region Exclusive OR (XOR)

    [Test]
    public async Task ImplementsExclusiveOrOperator_adds_xor_operator()
    {
        var result = TypeBuilder
            .Struct("Flags")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsExclusiveOrOperator(WellKnownSymbols.Int32, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("operator ^");
        await Assert.That(result.Code).Contains("left.Value ^ right.Value");
    }

    #endregion

    #region Bitwise Complement

    [Test]
    public async Task ImplementsBitwiseComplementOperator_adds_complement_operator()
    {
        var result = TypeBuilder
            .Struct("Flags")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsBitwiseComplementOperator(WellKnownSymbols.Int32, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("operator ~");
        await Assert.That(result.Code).Contains("~operand.Value");
    }

    #endregion

    #region Left Shift

    [Test]
    public async Task ImplementsLeftShiftOperator_adds_shift_operator()
    {
        var result = TypeBuilder
            .Struct("Flags")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsLeftShiftOperator(WellKnownSymbols.Int32, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("operator <<");
        await Assert.That(result.Code).Contains("left.Value << right");
        await Assert.That(result.Code).Contains("IShiftOperators<Flags, int, Flags>");
    }

    #endregion

    #region Right Shift

    [Test]
    public async Task ImplementsRightShiftOperator_adds_shift_operator()
    {
        var result = TypeBuilder
            .Struct("Flags")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsRightShiftOperator(WellKnownSymbols.Int32, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("operator >>");
        await Assert.That(result.Code).Contains("left.Value >> right");
    }

    #endregion

    #region Unsigned Right Shift

    [Test]
    public async Task ImplementsUnsignedRightShiftOperator_adds_shift_operator()
    {
        var result = TypeBuilder
            .Struct("Flags")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsUnsignedRightShiftOperator(WellKnownSymbols.Int32, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("operator >>>");
        await Assert.That(result.Code).Contains("left.Value >>> right");
    }

    #endregion

    #region PropertyBuilder Overloads

    [Test]
    public async Task BitwiseAnd_with_PropertyBuilder()
    {
        var valueProperty = PropertyBuilder.For("Value", "int").AsReadOnly();

        var result = TypeBuilder
            .Struct("Flags")
            .InNamespace("Test")
            .AddProperty(valueProperty)
            .ImplementsBitwiseAndOperator(WellKnownSymbols.Int32, valueProperty)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("left.Value & right.Value");
    }

    #endregion

    #region Non-Integer Types

    [Test]
    public async Task BitwiseAnd_skipped_for_non_integer_types()
    {
        var result = TypeBuilder
            .Struct("Money")
            .InNamespace("Test")
            .AddProperty("Value", "decimal", p => p.AsReadOnly())
            .ImplementsBitwiseAndOperator(WellKnownSymbols.Decimal, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).DoesNotContain("operator &");
    }

    #endregion
}

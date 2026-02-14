// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Operators.Arithmetic;

namespace Deepstaging.Roslyn.Tests.Emit.Operators.Arithmetic;

/// <summary>
/// Tests for TypeBuilder arithmetic operator extensions.
/// </summary>
public class TypeBuilderArithmeticOperatorExtensionsTests : RoslynTestBase
{
    #region Addition Operator

    [Test]
    public async Task ImplementsAdditionOperator_with_Int32()
    {
        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsAdditionOperator(WellKnownSymbols.Int32.ToSnapshot(), "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IAdditionOperators<Counter, Counter, Counter>");
        await Assert.That(result.Code).Contains("operator +(Counter left, Counter right)");
        await Assert.That(result.Code).Contains("new(left.Value + right.Value)");
    }

    [Test]
    public async Task ImplementsAdditionOperator_custom_expression()
    {
        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .ImplementsAdditionOperator("new(left.Value + right.Value)")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("operator +");
    }

    [Test]
    public async Task ImplementsAdditionOperator_non_numeric_returns_unchanged()
    {
        var result = TypeBuilder
            .Struct("Name")
            .InNamespace("Test")
            .ImplementsAdditionOperator(WellKnownSymbols.String.ToSnapshot(), "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).DoesNotContain("IAdditionOperators");
        await Assert.That(result.Code).DoesNotContain("operator +");
    }

    #endregion

    #region Subtraction Operator

    [Test]
    public async Task ImplementsSubtractionOperator_with_Int32()
    {
        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsSubtractionOperator(WellKnownSymbols.Int32.ToSnapshot(), "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("ISubtractionOperators<Counter, Counter, Counter>");
        await Assert.That(result.Code).Contains("operator -(Counter left, Counter right)");
        await Assert.That(result.Code).Contains("new(left.Value - right.Value)");
    }

    #endregion

    #region Increment Operator

    [Test]
    public async Task ImplementsIncrementOperator_with_Int32()
    {
        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsIncrementOperator(WellKnownSymbols.Int32.ToSnapshot(), "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IIncrementOperators<Counter>");
        await Assert.That(result.Code).Contains("operator ++(Counter operand)");
        await Assert.That(result.Code).Contains("new(operand.Value + 1)");
    }

    #endregion

    #region Decrement Operator

    [Test]
    public async Task ImplementsDecrementOperator_with_Int32()
    {
        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsDecrementOperator(WellKnownSymbols.Int32.ToSnapshot(), "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IDecrementOperators<Counter>");
        await Assert.That(result.Code).Contains("operator --(Counter operand)");
        await Assert.That(result.Code).Contains("new(operand.Value - 1)");
    }

    #endregion

    #region Multiply Operator

    [Test]
    public async Task ImplementsMultiplyOperator_with_Int32()
    {
        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsMultiplyOperator(WellKnownSymbols.Int32.ToSnapshot(), "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IMultiplyOperators<Counter, Counter, Counter>");
        await Assert.That(result.Code).Contains("operator *(Counter left, Counter right)");
        await Assert.That(result.Code).Contains("new(left.Value * right.Value)");
    }

    #endregion

    #region Division Operator

    [Test]
    public async Task ImplementsDivisionOperator_with_Int32()
    {
        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsDivisionOperator(WellKnownSymbols.Int32.ToSnapshot(), "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IDivisionOperators<Counter, Counter, Counter>");
        await Assert.That(result.Code).Contains("operator /(Counter left, Counter right)");
        await Assert.That(result.Code).Contains("new(left.Value / right.Value)");
    }

    #endregion

    #region Modulus Operator

    [Test]
    public async Task ImplementsModulusOperator_with_Int32()
    {
        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsModulusOperator(WellKnownSymbols.Int32.ToSnapshot(), "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IModulusOperators<Counter, Counter, Counter>");
        await Assert.That(result.Code).Contains("operator %(Counter left, Counter right)");
        await Assert.That(result.Code).Contains("new(left.Value % right.Value)");
    }

    #endregion

    #region Unary Negation Operator

    [Test]
    public async Task ImplementsUnaryNegationOperator_with_Int32()
    {
        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsUnaryNegationOperator(WellKnownSymbols.Int32.ToSnapshot(), "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IUnaryNegationOperators<Counter, Counter>");
        await Assert.That(result.Code).Contains("operator -(Counter operand)");
        await Assert.That(result.Code).Contains("new(-operand.Value)");
    }

    #endregion

    #region Unary Plus Operator

    [Test]
    public async Task ImplementsUnaryPlusOperator_with_Int32()
    {
        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsUnaryPlusOperator(WellKnownSymbols.Int32.ToSnapshot(), "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IUnaryPlusOperators<Counter, Counter>");
        await Assert.That(result.Code).Contains("operator +(Counter operand)");
        await Assert.That(result.Code).Contains("new(+operand.Value)");
    }

    #endregion

    #region Combined Operators

    [Test]
    public async Task Multiple_operators_can_be_combined()
    {
        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsAdditionOperator(WellKnownSymbols.Int32.ToSnapshot(), "Value")
            .ImplementsSubtractionOperator(WellKnownSymbols.Int32.ToSnapshot(), "Value")
            .ImplementsIncrementOperator(WellKnownSymbols.Int32.ToSnapshot(), "Value")
            .ImplementsDecrementOperator(WellKnownSymbols.Int32.ToSnapshot(), "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("operator +");
        await Assert.That(result.Code).Contains("operator -");
        await Assert.That(result.Code).Contains("operator ++");
        await Assert.That(result.Code).Contains("operator --");
    }

    #endregion

    #region PropertyBuilder/FieldBuilder Overloads

    [Test]
    public async Task ImplementsAdditionOperator_accepts_PropertyBuilder()
    {
        var valueProperty = PropertyBuilder.For("Value", "int").AsReadOnly();

        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty(valueProperty)
            .ImplementsAdditionOperator(WellKnownSymbols.Int32.ToSnapshot(), valueProperty)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("new(left.Value + right.Value)");
    }

    [Test]
    public async Task ImplementsAdditionOperator_accepts_FieldBuilder()
    {
        var valueField = FieldBuilder.Parse("private readonly int _value;");

        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddField(valueField)
            .ImplementsAdditionOperator(WellKnownSymbols.Int32.ToSnapshot(), valueField)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("new(left._value + right._value)");
    }

    #endregion
}

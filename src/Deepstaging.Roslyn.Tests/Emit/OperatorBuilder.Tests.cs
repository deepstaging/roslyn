// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit;

public class OperatorBuilderTests : RoslynTestBase
{
    #region Equality Operators

    [Test]
    public async Task Can_emit_equality_operator()
    {
        var result = TypeBuilder
            .Struct("CustomerId")
            .InNamespace("MyApp.Domain")
            .AddEqualityOperator("left.Value == right.Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public static bool operator ==(CustomerId left, CustomerId right)");
        await Assert.That(result.Code).Contains("left.Value == right.Value");
    }

    [Test]
    public async Task Can_emit_inequality_operator()
    {
        var result = TypeBuilder
            .Struct("CustomerId")
            .InNamespace("MyApp.Domain")
            .AddInequalityOperator("left.Value != right.Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public static bool operator !=(CustomerId left, CustomerId right)");
        await Assert.That(result.Code).Contains("left.Value != right.Value");
    }

    #endregion

    #region Comparison Operators

    [Test]
    public async Task Can_emit_comparison_operators()
    {
        var result = TypeBuilder
            .Struct("Money")
            .InNamespace("MyApp.Domain")
            .AddOperator(OperatorBuilder.LessThan("Money")
                .WithExpressionBody("left.Amount < right.Amount"))
            .AddOperator(OperatorBuilder.GreaterThan("Money")
                .WithExpressionBody("left.Amount > right.Amount"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("operator <(Money left, Money right)");
        await Assert.That(result.Code).Contains("operator>(Money left, Money right)");
    }

    #endregion

    #region Arithmetic Operators

    [Test]
    public async Task Can_emit_arithmetic_operators()
    {
        var result = TypeBuilder
            .Struct("Money")
            .InNamespace("MyApp.Domain")
            .AddOperator(OperatorBuilder.Addition("Money")
                .WithExpressionBody("new Money(left.Amount + right.Amount)"))
            .AddOperator(OperatorBuilder.Subtraction("Money")
                .WithExpressionBody("new Money(left.Amount - right.Amount)"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public static Money operator +(Money left, Money right)");
        await Assert.That(result.Code).Contains("public static Money operator -(Money left, Money right)");
    }

    [Test]
    public async Task Can_emit_unary_operators()
    {
        var result = TypeBuilder
            .Struct("Money")
            .InNamespace("MyApp.Domain")
            .AddOperator(OperatorBuilder.UnaryMinus("Money")
                .WithExpressionBody("new Money(-operand.Amount)"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public static Money operator -(Money operand)");
        await Assert.That(result.Code).Contains("new Money(-operand.Amount)");
    }

    [Test]
    public async Task Can_emit_increment_decrement_operators()
    {
        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("MyApp.Domain")
            .AddOperator(OperatorBuilder.Increment("Counter")
                .WithExpressionBody("new Counter(operand.Value + 1)"))
            .AddOperator(OperatorBuilder.Decrement("Counter")
                .WithExpressionBody("new Counter(operand.Value - 1)"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public static Counter operator ++(Counter operand)");
        await Assert.That(result.Code).Contains("public static Counter operator --(Counter operand)");
    }

    #endregion

    #region Custom Operators

    [Test]
    public async Task Can_emit_binary_operator_with_custom_types()
    {
        var result = TypeBuilder
            .Struct("Vector")
            .InNamespace("MyApp.Math")
            .AddOperator(OperatorBuilder.Binary("*", "Vector", "int", "Vector")
                .WithExpressionBody("new Vector(left.X * right, left.Y * right)"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public static Vector operator *(Vector left, int right)");
    }

    [Test]
    public async Task Can_emit_true_false_operators()
    {
        var result = TypeBuilder
            .Struct("Flag")
            .InNamespace("MyApp.Domain")
            .AddOperator(OperatorBuilder.True("Flag")
                .WithExpressionBody("operand.IsSet"))
            .AddOperator(OperatorBuilder.False("Flag")
                .WithExpressionBody("!operand.IsSet"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("operator true");
        await Assert.That(result.Code).Contains("operator false");
        await Assert.That(result.Code).Contains("(Flag operand)");
    }

    #endregion

    #region Operator Ordering

    [Test]
    public async Task Operators_appear_before_conversion_operators()
    {
        var result = TypeBuilder
            .Struct("CustomerId")
            .InNamespace("MyApp.Domain")
            .AddEqualityOperator("left.Value == right.Value")
            .AddExplicitConversion("int", b => b.WithExpressionBody("new CustomerId(value)"))
            .Emit();

        await Assert.That(result.Success).IsTrue();

        var code = result.Code!;
        var operatorIndex = code.IndexOf("operator ==", StringComparison.Ordinal);
        var conversionIndex = code.IndexOf("explicit operator", StringComparison.Ordinal);

        await Assert.That(operatorIndex).IsGreaterThanOrEqualTo(0);
        await Assert.That(conversionIndex).IsGreaterThanOrEqualTo(0);
        await Assert.That(operatorIndex).IsLessThan(conversionIndex);
    }

    #endregion

    #region Lambda Configuration

    [Test]
    public async Task Operator_with_lambda_configuration()
    {
        var result = TypeBuilder
            .Struct("Point")
            .InNamespace("MyApp.Types")
            .AddOperator(_ => OperatorBuilder.Equality("Point")
                .WithExpressionBody("left.X == right.X && left.Y == right.Y"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public static bool operator ==(Point left, Point right)");
    }

    #endregion

    #region Compilation Validation

    [Test]
    public async Task Emits_valid_compilable_code_with_operators()
    {
        var result = TypeBuilder
            .Struct("UserId")
            .InNamespace("MyApp.Domain")
            .AddProperty("Value", "int", p => p.WithAutoPropertyAccessors())
            .AddEqualityOperator("left.Value == right.Value")
            .AddInequalityOperator("left.Value != right.Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();

        var compilation = CompilationFor(result.Code!);
        var diagnostics = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error);

        await Assert.That(diagnostics).IsEmpty();
    }

    #endregion
}

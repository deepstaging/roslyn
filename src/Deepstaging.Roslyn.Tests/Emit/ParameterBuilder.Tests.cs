// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Patterns;

namespace Deepstaging.Roslyn.Tests.Emit;

public class ParameterBuilderTests : RoslynTestBase
{
    [Test]
    public async Task Can_emit_simple_parameter()
    {
        var param = ParameterBuilder.For("value", "int");

        var method = MethodBuilder
            .For("Process")
            .AddParameter(param)
            .WithBody(b => b.AddStatement("// Use value"));

        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Process(int value)");
    }

    [Test]
    public async Task Can_emit_parameter_with_default_value()
    {
        var param = ParameterBuilder
            .For("timeout", "int")
            .WithDefaultValue("30");

        var method = MethodBuilder
            .For("Connect")
            .AddParameter(param)
            .WithBody(b => b.AddStatement("// Connect"));

        var result = TypeBuilder
            .Class("Client")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Connect(int timeout = 30)");
    }

    [Test]
    public async Task Can_emit_ref_parameter()
    {
        var param = ParameterBuilder
            .For("value", "int")
            .AsRef();

        var method = MethodBuilder
            .For("Increment")
            .AddParameter(param)
            .WithBody(b => b.AddStatement("value++;"));

        var result = TypeBuilder
            .Class("Utils")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Increment(ref int value)");
    }

    [Test]
    public async Task Can_emit_out_parameter()
    {
        var param = ParameterBuilder
            .For("result", "bool")
            .AsOut();

        var method = MethodBuilder
            .For("TryParse")
            .AddParameter("input", "string")
            .AddParameter(param)
            .WithBody(b => b.AddStatements("""
                                           result = true;
                                           return 42;
                                           """));

        var result = TypeBuilder
            .Class("Parser")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("TryParse(string input, out bool result)");
    }

    [Test]
    public async Task Can_emit_in_parameter()
    {
        var param = ParameterBuilder
            .For("value", "LargeStruct")
            .AsIn();

        var method = MethodBuilder
            .For("Process")
            .AddParameter(param)
            .WithBody(b => b.AddStatement("// Process"));

        var result = TypeBuilder
            .Class("Processor")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Process(in LargeStruct value)");
    }

    [Test]
    public async Task Can_emit_params_parameter()
    {
        var param = ParameterBuilder
            .For("values", "int[]")
            .AsParams();

        var method = MethodBuilder
            .For("Sum")
            .AddParameter(param)
            .WithReturnType("int")
            .WithBody(b => b.AddReturn("values.Sum()"));

        var result = TypeBuilder
            .Class("Calculator")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Sum(params int[] values)");
    }

    [Test]
    public async Task Can_emit_nullable_parameter()
    {
        var param = ParameterBuilder
            .For("name", "string?")
            .WithDefaultValue("null");

        var method = MethodBuilder
            .For("SetName")
            .AddParameter(param)
            .WithBody(b => b.AddStatement("// Set name"));

        var result = TypeBuilder
            .Class("Person")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("SetName(string? name = null)");
    }

    #region Null Validation

    [Test]
    public async Task ThrowIfNull_adds_null_check()
    {
        var result = TypeBuilder
            .Struct("CustomerId")
            .InNamespace("Test")
            .AddProperty("Value", "string", p => p.AsReadOnly())
            .AddConstructor(c => c
                .AddParameter("value", "string", p => p
                    .ThrowIfNull()
                    .AssignsTo("Value")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("ArgumentNullException.ThrowIfNull(value)");
        await Assert.That(result.Code).Contains("Value = value;");
    }

    [Test]
    public async Task ThrowIfNullOrEmpty_adds_empty_check()
    {
        var result = TypeBuilder
            .Struct("CustomerId")
            .InNamespace("Test")
            .AddProperty("Value", "string", p => p.AsReadOnly())
            .AddConstructor(c => c
                .AddParameter("value", "string", p => p
                    .ThrowIfNullOrEmpty()
                    .AssignsTo("Value")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("ArgumentException.ThrowIfNullOrEmpty(value)");
        await Assert.That(result.Code).Contains("Value = value;");
    }

    [Test]
    public async Task ThrowIfNullOrWhiteSpace_adds_whitespace_check()
    {
        var result = TypeBuilder
            .Struct("CustomerId")
            .InNamespace("Test")
            .AddProperty("Value", "string", p => p.AsReadOnly())
            .AddConstructor(c => c
                .AddParameter("value", "string", p => p
                    .ThrowIfNullOrWhiteSpace()
                    .AssignsTo("Value")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("ArgumentException.ThrowIfNullOrWhiteSpace(value)");
        await Assert.That(result.Code).Contains("Value = value;");
    }

    #endregion

    #region Range Validation

    [Test]
    public async Task ThrowIfOutOfRange_adds_range_check()
    {
        var result = TypeBuilder
            .Struct("Percentage")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .AddConstructor(c => c
                .AddParameter("value", "int", p => p
                    .ThrowIfOutOfRange("0", "100")
                    .AssignsTo("Value")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("ArgumentOutOfRangeException.ThrowIfLessThan(value, 0)");
        await Assert.That(result.Code).Contains("ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 100)");
        await Assert.That(result.Code).Contains("Value = value;");
    }

    [Test]
    public async Task ThrowIfNotPositive_adds_positive_check()
    {
        var result = TypeBuilder
            .Struct("PositiveInt")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .AddConstructor(c => c
                .AddParameter("value", "int", p => p
                    .ThrowIfNotPositive()
                    .AssignsTo("Value")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value)");
        await Assert.That(result.Code).Contains("Value = value;");
    }

    [Test]
    public async Task ThrowIfNegative_adds_non_negative_check()
    {
        var result = TypeBuilder
            .Struct("NonNegativeInt")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .AddConstructor(c => c
                .AddParameter("value", "int", p => p
                    .ThrowIfNegative()
                    .AssignsTo("Value")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("ArgumentOutOfRangeException.ThrowIfNegative(value)");
        await Assert.That(result.Code).Contains("Value = value;");
    }

    [Test]
    public async Task ThrowIfZero_adds_zero_check()
    {
        var result = TypeBuilder
            .Struct("NonZeroInt")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .AddConstructor(c => c
                .AddParameter("value", "int", p => p
                    .ThrowIfZero()
                    .AssignsTo("Value")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("ArgumentOutOfRangeException.ThrowIfZero(value)");
        await Assert.That(result.Code).Contains("Value = value;");
    }

    #endregion

    #region Validation Ordering

    [Test]
    public async Task Validation_comes_before_assignment()
    {
        var result = TypeBuilder
            .Struct("BoundedString")
            .InNamespace("Test")
            .AddProperty("Value", "string", p => p.AsReadOnly())
            .AddConstructor(c => c
                .AddParameter("value", "string", p => p
                    .ThrowIfNullOrWhiteSpace()
                    .AssignsTo("Value")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        var code = result.Code!;
        var validationIndex = code.IndexOf("ThrowIfNullOrWhiteSpace");
        var assignmentIndex = code.IndexOf("Value = value");
        await Assert.That(validationIndex).IsLessThan(assignmentIndex);
    }

    #endregion

    #region AssignsTo with Builders

    [Test]
    public async Task AssignsTo_with_PropertyBuilder()
    {
        var valueProperty = PropertyBuilder.For("Value", "int").AsReadOnly();

        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty(valueProperty)
            .AddConstructor(c => c
                .AddParameter("value", "int", p => p
                    .AssignsTo(valueProperty)))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Value = value;");
    }

    [Test]
    public async Task AssignsTo_with_FieldBuilder()
    {
        var valueField = FieldBuilder.Parse("private readonly int _value;");

        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddField(valueField)
            .AddConstructor(c => c
                .AddParameter("value", "int", p => p
                    .AssignsTo(valueField)))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("_value = value;");
    }

    #endregion

    #region Multiple Parameters

    [Test]
    public async Task Multiple_parameters_with_validation()
    {
        var result = TypeBuilder
            .Struct("Range")
            .InNamespace("Test")
            .AddProperty("Min", "int", p => p.AsReadOnly())
            .AddProperty("Max", "int", p => p.AsReadOnly())
            .AddConstructor(c => c
                .AddParameter("min", "int", p => p.ThrowIfNegative().AssignsTo("Min"))
                .AddParameter("max", "int", p => p.ThrowIfNegative().AssignsTo("Max")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("ThrowIfNegative(min)");
        await Assert.That(result.Code).Contains("ThrowIfNegative(max)");
        await Assert.That(result.Code).Contains("Min = min;");
        await Assert.That(result.Code).Contains("Max = max;");
    }

    #endregion

    #region Chained Validations

    [Test]
    public async Task Multiple_validations_on_same_parameter()
    {
        var result = TypeBuilder
            .Struct("BoundedInt")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .AddConstructor(c => c
                .AddParameter("value", "int", p => p
                    .ThrowIfNegative()
                    .ThrowIfZero()
                    .AssignsTo("Value")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("ThrowIfNegative(value)");
        await Assert.That(result.Code).Contains("ThrowIfZero(value)");
        await Assert.That(result.Code).Contains("Value = value;");
    }

    #endregion

    #region Method Parameter Validation

    [Test]
    public async Task Method_parameter_validation_works()
    {
        var result = TypeBuilder
            .Class("Calculator")
            .InNamespace("Test")
            .AddMethod("Divide", m => m
                .WithAccessibility(Accessibility.Public)
                .WithReturnType("int")
                .AddParameter("dividend", "int")
                .AddParameter("divisor", "int", p => p.ThrowIfZero())
                .WithBody(b => b.AddStatement("return dividend / divisor;")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("ThrowIfZero(divisor)");
        await Assert.That(result.Code).Contains("return dividend / divisor;");
    }

    [Test]
    public async Task Method_parameter_null_validation()
    {
        var result = TypeBuilder
            .Class("Processor")
            .InNamespace("Test")
            .AddMethod("Process", m => m
                .WithAccessibility(Accessibility.Public)
                .WithReturnType("void")
                .AddParameter("input", "string", p => p.ThrowIfNullOrEmpty())
                .WithBody(b => b.AddStatement("Console.WriteLine(input);")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("ThrowIfNullOrEmpty(input)");
        await Assert.That(result.Code).Contains("Console.WriteLine(input);");
    }

    #endregion
}
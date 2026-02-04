// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
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
}

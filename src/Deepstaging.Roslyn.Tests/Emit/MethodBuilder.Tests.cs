// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Emit;

public class MethodBuilderTests : RoslynTestBase
{
    [Test]
    public async Task Can_emit_simple_method()
    {
        var method = MethodBuilder
            .For("DoSomething")
            .WithAccessibility(Accessibility.Public)
            .WithReturnType("void")
            .WithBody(b => b.AddStatement("Console.WriteLine(\"Hello\")"));

        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public void DoSomething()");
        await Assert.That(result.Code).Contains("Console.WriteLine(\"Hello\")");
    }

    [Test]
    public async Task Can_emit_method_with_return_type()
    {
        var method = MethodBuilder
            .For("GetValue")
            .WithAccessibility(Accessibility.Public)
            .WithReturnType("int")
            .WithBody(b => b.AddReturn("42"));

        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public int GetValue()");
        await Assert.That(result.Code).Contains("return 42;");
    }

    [Test]
    public async Task Can_emit_async_method()
    {
        var method = MethodBuilder
            .For("FetchDataAsync")
            .Async()
            .WithAccessibility(Accessibility.Public)
            .WithReturnType("Task<string>")
            .WithBody(b => b.AddReturn("await Task.FromResult(\"data\")"));

        var result = TypeBuilder
            .Class("Service")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public async Task<string> FetchDataAsync()");
    }

    [Test]
    public async Task Can_emit_static_method()
    {
        var method = MethodBuilder
            .For("Create")
            .AsStatic()
            .WithAccessibility(Accessibility.Public)
            .WithReturnType("MyClass")
            .WithBody(b => b.AddReturn("new MyClass()"));

        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public static MyClass Create()");
    }

    [Test]
    public async Task Can_emit_virtual_method()
    {
        var method = MethodBuilder
            .For("Process")
            .AsVirtual()
            .WithAccessibility(Accessibility.Public)
            .WithReturnType("void")
            .WithBody(b => b.AddStatement("// Base implementation"));

        var result = TypeBuilder
            .Class("Base")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public virtual void Process()");
    }

    [Test]
    public async Task Can_emit_override_method()
    {
        var method = MethodBuilder
            .For("ToString")
            .AsOverride()
            .WithAccessibility(Accessibility.Public)
            .WithReturnType("string")
            .WithBody(b => b.AddReturn("\"Custom\""));

        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public override string ToString()");
    }

    [Test]
    public async Task Can_emit_abstract_method()
    {
        var method = MethodBuilder
            .For("DoWork")
            .AsAbstract()
            .WithAccessibility(Accessibility.Public)
            .WithReturnType("void");

        var result = TypeBuilder
            .Class("AbstractBase")
            .AsAbstract()
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public abstract void DoWork();");
    }

    [Test]
    public async Task Can_emit_method_with_parameters()
    {
        var method = MethodBuilder
            .For("Add")
            .WithAccessibility(Accessibility.Public)
            .WithReturnType("int")
            .AddParameter("a", "int")
            .AddParameter("b", "int")
            .WithBody(b => b.AddReturn("a + b"));

        var result = TypeBuilder
            .Class("Calculator")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public int Add(int a, int b)");
    }

    [Test]
    public async Task Can_emit_method_with_parameter_builder()
    {
        var param = ParameterBuilder
            .For("value", "string")
            .WithDefaultValue("null");

        var method = MethodBuilder
            .For("Process")
            .WithAccessibility(Accessibility.Public)
            .WithReturnType("void")
            .AddParameter(param)
            .WithBody(b => b.AddStatement("// Process value"));

        var result = TypeBuilder
            .Class("Processor")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public void Process(string value = null)");
    }

    [Test]
    public async Task Can_emit_method_with_multiline_body()
    {
        var method = MethodBuilder
            .For("Validate")
            .WithAccessibility(Accessibility.Public)
            .WithReturnType("bool")
            .AddParameter("input", "string")
            .WithBody(b => b.AddStatements("""
                if (string.IsNullOrEmpty(input))
                    return false;
                
                if (input.Length < 3)
                    return false;
                
                return true;
                """));

        var result = TypeBuilder
            .Class("Validator")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public bool Validate(string input)");
        await Assert.That(result.Code).Contains("if (string.IsNullOrEmpty(input))");
    }

    [Test]
    public async Task Can_emit_method_with_lambda_configuration()
    {
        var result = TypeBuilder
            .Class("Service")
            .AddMethod("Execute", method => method
                .WithAccessibility(Accessibility.Public)
                .WithReturnType("void")
                .WithBody(b => b.AddStatement("// Execute")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public void Execute()");
    }

    [Test]
    public async Task Can_append_to_expression_body()
    {
        var result = TypeBuilder
            .Class("Service")
            .AddMethod("GetValue", method => method
                .WithReturnType("int")
                .WithExpressionBody("Calculate()")
                .AppendExpressionBody(".Validate()"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("=> Calculate().Validate()");
    }

    [Test]
    public async Task Append_expression_body_throws_when_no_expression_set()
    {
        var action = () => MethodBuilder
            .For("Test")
            .AppendExpressionBody(".Chain()");

        await Assert.That(action).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task Can_conditionally_append_expression_body()
    {
        var instrumented = true;

        var result = TypeBuilder
            .Class("Service")
            .AddMethod("Execute", method => method
                .WithReturnType("Eff<int>")
                .WithExpressionBody("liftEff(() => 42)")
                .If(instrumented, m => m.AppendExpressionBody(".WithActivity()")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("=> liftEff(() => 42).WithActivity()");
    }
}

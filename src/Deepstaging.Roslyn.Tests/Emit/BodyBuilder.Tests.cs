// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Emit;

public class BodyBuilderTests : RoslynTestBase
{
    [Test]
    public async Task Can_emit_single_statement()
    {
        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod("Print", method => method
                .WithAccessibility(Accessibility.Public)
                .WithBody(b => b.AddStatement("Console.WriteLine(\"Hello\")")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Console.WriteLine(\"Hello\");");
    }

    [Test]
    public async Task Can_emit_multiple_statements()
    {
        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod("Process", method => method
                .WithAccessibility(Accessibility.Public)
                .WithBody(b => b
                    .AddStatement("var x = 10")
                    .AddStatement("var y = 20")
                    .AddStatement("Console.WriteLine(x + y)")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("var x = 10;");
        await Assert.That(result.Code).Contains("var y = 20;");
        await Assert.That(result.Code).Contains("Console.WriteLine(x + y);");
    }

    [Test]
    public async Task Can_emit_multiline_statements()
    {
        var method = MethodBuilder.For("Validate")
            .WithAccessibility(Accessibility.Public)
            .WithReturnType("bool")
            .WithBody(b => b.AddStatements("""
                                           if (value < 0)
                                               return false;

                                           if (value > 100)
                                               return false;

                                           return true;
                                           """));

        var result = TypeBuilder
            .Class("Validator")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("if (value < 0)");
        await Assert.That(result.Code).Contains("if (value > 100)");
        await Assert.That(result.Code).Contains("return true;");
    }

    [Test]
    public async Task Can_emit_return_statement()
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
        await Assert.That(result.Code).Contains("return 42;");
    }

    [Test]
    public async Task Can_emit_throw_statement()
    {
        var method = MethodBuilder
            .For("ThrowError")
            .WithAccessibility(Accessibility.Public)
            .WithBody(b => b.AddThrow("new InvalidOperationException(\"Error\")"));

        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("throw new InvalidOperationException(\"Error\");");
    }

    [Test]
    public async Task Can_emit_custom_syntax()
    {
        var method = MethodBuilder
            .For("UseCustom")
            .WithAccessibility(Accessibility.Public)
            .WithBody(b => b.AddStatement("var scope = BeginScope()"));

        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("var scope = BeginScope();");
    }

    [Test]
    public async Task Can_mix_different_statement_types()
    {
        var method = MethodBuilder
            .For("ComplexMethod")
            .WithAccessibility(Accessibility.Public)
            .WithReturnType("int")
            .WithBody(b => b
                .AddStatement("var x = 10")
                .AddStatements("""
                               if (x < 0)
                                   throw new ArgumentException();
                               """)
                .AddReturn("x * 2"));

        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("var x = 10;");
        await Assert.That(result.Code).Contains("if (x < 0)");
        await Assert.That(result.Code).Contains("return x * 2;");
    }

    [Test]
    public async Task Statements_get_automatic_semicolons()
    {
        var method = MethodBuilder
            .For("Test")
            .WithAccessibility(Accessibility.Public)
            .WithBody(b => b
                .AddStatement("var x = 10") // No semicolon
                .AddStatement("Console.WriteLine(x);")); // Has semicolon

        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        // Both should have semicolons in output
        await Assert.That(result.Code).Contains("var x = 10;");
        await Assert.That(result.Code).Contains("Console.WriteLine(x);");
    }
}
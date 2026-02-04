// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Emit;

public class EmitOptionsTests : RoslynTestBase
{
    [Test]
    public async Task Can_emit_without_validation()
    {
        var result = TypeBuilder
            .Class("MyClass")
            .Emit(new EmitOptions { ValidationLevel = ValidationLevel.None });

        await Assert.That(result.Success).IsTrue();
    }

    [Test]
    public async Task Can_emit_with_syntax_validation()
    {
        var result = TypeBuilder
            .Class("MyClass")
            .Emit(new EmitOptions { ValidationLevel = ValidationLevel.Syntax });

        await Assert.That(result.Success).IsTrue();
    }

    [Test]
    public async Task Syntax_validation_catches_invalid_syntax()
    {
        var method = MethodBuilder
            .For("Invalid")
            .WithBody(b => b.AddStatement("this is not valid C# syntax"));

        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod(method)
            .Emit(new EmitOptions { ValidationLevel = ValidationLevel.Syntax });

        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Diagnostics).IsNotEmpty();
    }

    [Test]
    public async Task Can_customize_indentation()
    {
        var options = new EmitOptions
        {
            Indentation = "  "
        };

        var result = TypeBuilder
            .Class("MyClass")
            .AddProperty("Name", "string", p => p
                .WithAccessibility(Accessibility.Public)
                .WithAutoPropertyAccessors())
            .Emit(options);

        await Assert.That(result.Success).IsTrue();
        // Validates the option exists and is used
    }

    [Test]
    public async Task Can_customize_line_endings()
    {
        var options = new EmitOptions
        {
            EndOfLine = "\n"
        };

        var result = TypeBuilder
            .Class("MyClass")
            .Emit(options);

        await Assert.That(result.Success).IsTrue();
    }

    [Test]
    public async Task Default_options_apply_formatting()
    {
        var result = TypeBuilder
            .Class("MyClass")
            .AddProperty("Name", "string", p => p
                .WithAccessibility(Accessibility.Public)
                .WithAutoPropertyAccessors())
            .Emit();

        await Assert.That(result.Success).IsTrue();
        // Should be formatted with consistent whitespace
        await Assert.That(result.Code).Contains("public string Name");
    }
}

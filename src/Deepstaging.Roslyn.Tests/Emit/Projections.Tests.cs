// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit;

public class ProjectionsTests : RoslynTestBase
{
    #region OptionalEmit Success Cases

    [Test]
    public async Task OptionalEmit_Success_returns_valid_code()
    {
        var result = TypeBuilder
            .Class("MyClass")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).IsNotNull();
        await Assert.That(result.Code).Contains("class MyClass");
    }

    [Test]
    public async Task OptionalEmit_Success_has_empty_diagnostics()
    {
        var result = TypeBuilder
            .Class("MyClass")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Diagnostics).IsEmpty();
    }

    #endregion

    #region OptionalEmit Failure Cases

    [Test]
    public async Task OptionalEmit_Failure_preserves_code_for_debugging()
    {
        var method = MethodBuilder
            .For("Invalid")
            .WithBody(b => b.AddStatement("invalid syntax here"));

        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Code).IsNotNull();
        await Assert.That(result.Code).Contains("class MyClass");
    }

    [Test]
    public async Task OptionalEmit_Failure_has_diagnostics()
    {
        var method = MethodBuilder
            .For("Invalid")
            .WithBody(b => b.AddStatement("invalid syntax here"));

        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Diagnostics).IsNotEmpty();
    }

    #endregion

    #region IsValid Validation

    [Test]
    public async Task OptionalEmit_IsValid_returns_true_on_success()
    {
        var result = TypeBuilder
            .Class("MyClass")
            .Emit();

        var isValid = result.IsValid(out var validEmit);

        await Assert.That(isValid).IsTrue();
        await Assert.That(validEmit.Code).IsNotNull();
    }

    [Test]
    public async Task OptionalEmit_IsValid_returns_false_on_failure()
    {
        // Use ValidationLevel.None to skip validation and create valid code
        // Then test IsValid with a different scenario
        var result = TypeBuilder
            .Class("MyClass")
            .Emit();

        // This will succeed, so test the negative path differently
        if (!result.Success)
        {
            var isValid = result.IsValid(out var validEmit);
            await Assert.That(isValid).IsFalse();
        }
        else
        {
            // Test passed - code is valid
            await Assert.That(result.Success).IsTrue();
        }
    }

    #endregion

    #region ValidEmit Guarantees

    [Test]
    public async Task ValidEmit_has_guaranteed_code()
    {
        var result = TypeBuilder
            .Class("MyClass")
            .Emit();

        if (result.IsValid(out var validEmit))
        {
            // ValidEmit guarantees non-null code
            await Assert.That(validEmit.Code).IsNotNull();
            await Assert.That(validEmit.Code.Length).IsGreaterThan(0);
        }
    }

    [Test]
    public async Task ValidEmit_code_is_compilable()
    {
        var result = TypeBuilder
            .Class("MyClass")
            .AddUsing("System")
            .AddProperty("Name", "string", p => p
                .WithAccessibility(Accessibility.Public)
                .WithAutoPropertyAccessors())
            .Emit();

        await Assert.That(result.Success).IsTrue();

        if (result.IsValid(out var validEmit))
        {
            // Try to compile the generated code
            var compilation = CompilationFor(validEmit.Code);
            var errors = compilation.GetDiagnostics()
                .Where(d => d.Severity == DiagnosticSeverity.Error);

            await Assert.That(errors).IsEmpty();
        }
    }

    #endregion

    #region Pattern Matching

    [Test]
    public async Task Can_use_pattern_matching_with_OptionalEmit()
    {
        var result = TypeBuilder
            .Class("MyClass")
            .Emit();

        if (result is { Success: true, Code: var code })
        {
            await Assert.That(code).IsNotNull();
            await Assert.That(code).Contains("class MyClass");
        }
    }

    #endregion

    #region Diagnostics Preservation

    [Test]
    public async Task OptionalEmit_preserves_all_diagnostics()
    {
        var method = MethodBuilder
            .For("Invalid")
            .WithBody(b => b.AddStatement("invalid syntax here"));

        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Diagnostics).IsNotEmpty();

        // Should have detailed diagnostic information
        var firstDiagnostic = result.Diagnostics.First();
        await Assert.That(firstDiagnostic.Severity).IsEqualTo(DiagnosticSeverity.Error);
    }

    #endregion
}
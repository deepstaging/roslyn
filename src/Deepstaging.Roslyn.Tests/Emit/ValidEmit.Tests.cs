// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.Roslyn.Tests.Emit;

public class ValidEmitTests : RoslynTestBase
{
    #region Part Extraction

    [Test]
    public async Task Usings_returns_using_directives()
    {
        var emit = TypeBuilder
            .Class("MyClass")
            .InNamespace("MyApp")
            .AddUsing("System")
            .AddUsing("System.Collections.Generic")
            .Emit()
            .ValidateOrThrow();

        await Assert.That(emit.Usings.Length).IsEqualTo(2);
        await Assert.That(emit.Usings.Select(u => u.Name?.ToString()))
            .Contains("System");
        await Assert.That(emit.Usings.Select(u => u.Name?.ToString()))
            .Contains("System.Collections.Generic");
    }

    [Test]
    public async Task Types_extracts_type_from_namespace()
    {
        var emit = TypeBuilder
            .Class("MyClass")
            .InNamespace("MyApp")
            .Emit()
            .ValidateOrThrow();

        await Assert.That(emit.Types.Length).IsEqualTo(1);
        var type = emit.Types.First();
        await Assert.That(type).IsTypeOf<ClassDeclarationSyntax>();
    }

    [Test]
    public async Task Types_extracts_type_at_global_scope()
    {
        var emit = TypeBuilder
            .Class("MyClass")
            .Emit()
            .ValidateOrThrow();

        await Assert.That(emit.Types.Length).IsEqualTo(1);
        var type = emit.Types.First();
        await Assert.That(type).IsTypeOf<ClassDeclarationSyntax>();
    }

    [Test]
    public async Task Namespace_returns_namespace_name()
    {
        var emit = TypeBuilder
            .Class("MyClass")
            .InNamespace("MyApp.Domain")
            .Emit()
            .ValidateOrThrow();

        await Assert.That(emit.Namespace).IsEqualTo("MyApp.Domain");
    }

    [Test]
    public async Task Namespace_returns_null_for_global_scope()
    {
        var emit = TypeBuilder
            .Class("MyClass")
            .Emit()
            .ValidateOrThrow();

        await Assert.That(emit.Namespace).IsNull();
    }

    [Test]
    public async Task LeadingTrivia_contains_header_comment()
    {
        var emit = TypeBuilder
            .Class("MyClass")
            .Emit()
            .ValidateOrThrow();

        var triviaText = emit.LeadingTrivia.ToFullString();
        await Assert.That(triviaText).Contains("auto-generated");
    }

    #endregion

    #region Combine

    [Test]
    public async Task Combine_single_emit_returns_same_emit()
    {
        var emit = TypeBuilder
            .Class("MyClass")
            .InNamespace("MyApp")
            .Emit()
            .ValidateOrThrow();

        var combined = ValidEmit.Combine(emit);

        await Assert.That(combined.Types.Length).IsEqualTo(1);
    }

    [Test]
    public async Task Combine_merges_types_from_same_namespace()
    {
        var emit1 = TypeBuilder
            .Class("ClassA")
            .InNamespace("MyApp")
            .Emit()
            .ValidateOrThrow();

        var emit2 = TypeBuilder
            .Class("ClassB")
            .InNamespace("MyApp")
            .Emit()
            .ValidateOrThrow();

        var combined = ValidEmit.Combine(emit1, emit2);

        await Assert.That(combined.Types.Length).IsEqualTo(2);
        await Assert.That(combined.Code).Contains("class ClassA");
        await Assert.That(combined.Code).Contains("class ClassB");
    }

    [Test]
    public async Task Combine_supports_multiple_namespaces()
    {
        var emit1 = TypeBuilder
            .Class("ClassA")
            .InNamespace("MyApp.Domain")
            .Emit()
            .ValidateOrThrow();

        var emit2 = TypeBuilder
            .Class("ClassB")
            .InNamespace("MyApp.Services")
            .Emit()
            .ValidateOrThrow();

        var combined = ValidEmit.Combine(emit1, emit2);

        await Assert.That(combined.Code).Contains("namespace MyApp.Domain");
        await Assert.That(combined.Code).Contains("namespace MyApp.Services");
    }

    [Test]
    public async Task Combine_deduplicates_usings()
    {
        var emit1 = TypeBuilder
            .Class("ClassA")
            .InNamespace("MyApp")
            .AddUsing("System")
            .AddUsing("System.Linq")
            .Emit()
            .ValidateOrThrow();

        var emit2 = TypeBuilder
            .Class("ClassB")
            .InNamespace("MyApp")
            .AddUsing("System")
            .AddUsing("System.Collections.Generic")
            .Emit()
            .ValidateOrThrow();

        var combined = ValidEmit.Combine(emit1, emit2);

        await Assert.That(combined.Usings.Length).IsEqualTo(3);
    }

    [Test]
    public async Task Combine_preserves_static_usings()
    {
        var emit1 = TypeBuilder
            .Class("ClassA")
            .InNamespace("MyApp")
            .AddUsing("System")
            .AddUsing("static System.Math")
            .Emit()
            .ValidateOrThrow();

        var emit2 = TypeBuilder
            .Class("ClassB")
            .InNamespace("MyApp")
            .AddUsing("System.Linq")
            .AddUsing("static System.Console")
            .Emit()
            .ValidateOrThrow();

        var combined = ValidEmit.Combine(emit1, emit2);

        // Should have 2 regular + 2 static = 4 usings
        await Assert.That(combined.Usings.Length).IsEqualTo(4);

        // Verify static usings are present
        var staticUsings = combined.Usings.Where(u => u.StaticKeyword != default).ToList();
        await Assert.That(staticUsings.Count).IsEqualTo(2);
        await Assert.That(staticUsings.Select(u => u.Name?.ToString())).Contains("System.Math");
        await Assert.That(staticUsings.Select(u => u.Name?.ToString())).Contains("System.Console");
    }

    [Test]
    public async Task Combine_preserves_trivia_from_first_emit()
    {
        var emit1 = TypeBuilder
            .Class("ClassA")
            .InNamespace("MyApp")
            .Emit()
            .ValidateOrThrow();

        var emit2 = TypeBuilder
            .Class("ClassB")
            .InNamespace("MyApp")
            .Emit()
            .ValidateOrThrow();

        var combined = ValidEmit.Combine(emit1, emit2);

        // Combined emit should have the auto-generated header from first emit
        await Assert.That(combined.LeadingTrivia.ToFullString()).Contains("auto-generated");
    }

    [Test]
    public async Task Combine_produces_compilable_code()
    {
        var emit1 = TypeBuilder
            .Class("ClassA")
            .InNamespace("MyApp")
            .AddProperty("Name", "string", p => p.WithAutoPropertyAccessors())
            .Emit()
            .ValidateOrThrow();

        var emit2 = TypeBuilder
            .Class("ClassB")
            .InNamespace("MyApp")
            .AddProperty("Value", "int", p => p.WithAutoPropertyAccessors())
            .Emit()
            .ValidateOrThrow();

        var combined = ValidEmit.Combine(emit1, emit2);

        var compilation = CompilationFor(combined.Code);
        var errors = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error);

        await Assert.That(errors).IsEmpty();
    }

    [Test]
    public async Task Combine_with_enumerable_works()
    {
        var emits = new[]
        {
            TypeBuilder.Class("A").Emit().ValidateOrThrow(),
            TypeBuilder.Class("B").Emit().ValidateOrThrow(),
            TypeBuilder.Class("C").Emit().ValidateOrThrow()
        };

        var combined = ValidEmit.Combine(emits);

        await Assert.That(combined.Types.Length).IsEqualTo(3);
    }

    [Test]
    public void Combine_throws_on_empty_collection()
    {
        Assert.Throws<ArgumentException>(() => ValidEmit.Combine(Array.Empty<ValidEmit>()));
    }

    #endregion
}

public class OptionalEmitCombineTests : RoslynTestBase
{
    [Test]
    public async Task Combine_succeeds_when_all_emits_succeed()
    {
        var emit1 = TypeBuilder
            .Class("ClassA")
            .InNamespace("MyApp")
            .Emit();

        var emit2 = TypeBuilder
            .Class("ClassB")
            .InNamespace("MyApp")
            .Emit();

        var combined = OptionalEmit.Combine(emit1, emit2);

        await Assert.That(combined.Success).IsTrue();
        await Assert.That(combined.Code).Contains("class ClassA");
        await Assert.That(combined.Code).Contains("class ClassB");
    }

    [Test]
    public async Task Combine_fails_when_any_emit_fails()
    {
        var successEmit = TypeBuilder
            .Class("ClassA")
            .Emit();

        var failedEmit = TypeBuilder
            .Class("ClassB")
            .AddMethod(MethodBuilder.For("Bad").WithBody(b => b.AddStatement("invalid syntax here")))
            .Emit();

        var combined = OptionalEmit.Combine(successEmit, failedEmit);

        await Assert.That(combined.Success).IsFalse();
        await Assert.That(combined.Diagnostics).IsNotEmpty();
    }

    [Test]
    public async Task Combine_aggregates_diagnostics_from_all_emits()
    {
        var fail1 = TypeBuilder
            .Class("ClassA")
            .AddMethod(MethodBuilder.For("Bad1").WithBody(b => b.AddStatement("invalid syntax here")))
            .Emit();

        var fail2 = TypeBuilder
            .Class("ClassB")
            .AddMethod(MethodBuilder.For("Bad2").WithBody(b => b.AddStatement("another invalid syntax")))
            .Emit();

        var combined = OptionalEmit.Combine(fail1, fail2);

        await Assert.That(combined.Success).IsFalse();
        // Should have diagnostics from both failed emits
        await Assert.That(combined.Diagnostics.Length).IsGreaterThanOrEqualTo(2);
    }

    [Test]
    public async Task Combine_can_be_validated_afterwards()
    {
        var emit1 = TypeBuilder.Class("A").InNamespace("MyApp").Emit();
        var emit2 = TypeBuilder.Class("B").InNamespace("MyApp").Emit();

        var combined = OptionalEmit.Combine(emit1, emit2);

        await Assert.That(combined.IsValid(out var valid)).IsTrue();
        await Assert.That(valid.Types.Length).IsEqualTo(2);
    }

    [Test]
    public async Task Combine_produces_compilable_code()
    {
        var emit1 = TypeBuilder
            .Class("ClassA")
            .InNamespace("MyApp")
            .AddProperty("Name", "string", p => p.WithAutoPropertyAccessors())
            .Emit();

        var emit2 = TypeBuilder
            .Class("ClassB")
            .InNamespace("MyApp")
            .AddProperty("Value", "int", p => p.WithAutoPropertyAccessors())
            .Emit();

        var combined = OptionalEmit.Combine(emit1, emit2);

        await Assert.That(combined.Success).IsTrue();

        var compilation = CompilationFor(combined.Code!);
        var errors = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error);

        await Assert.That(errors).IsEmpty();
    }

    [Test]
    public void Combine_throws_on_empty_collection()
    {
        Assert.Throws<ArgumentException>(() => OptionalEmit.Combine(Array.Empty<OptionalEmit>()));
    }

    [Test]
    public async Task Combine_with_single_emit_returns_same_result()
    {
        var emit = TypeBuilder.Class("MyClass").Emit();

        var combined = OptionalEmit.Combine(emit);

        await Assert.That(combined.Success).IsEqualTo(emit.Success);
    }
}

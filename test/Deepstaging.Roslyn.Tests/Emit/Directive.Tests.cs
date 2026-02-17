// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit;

public class DirectiveTests : RoslynTestBase
{
    #region Interface-Level Directives

    [Test]
    public async Task Can_emit_conditional_interface()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .Implements("IEquatable<MyId>")
            .Implements("ISpanFormattable", Directives.Net6OrGreater)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IEquatable<MyId>");
        await Assert.That(result.Code).Contains("#if NET6_0_OR_GREATER");
        await Assert.That(result.Code).Contains("ISpanFormattable");
        await Assert.That(result.Code).Contains("#endif");
    }

    [Test]
    public async Task Can_emit_multiple_conditional_interfaces_with_same_directive()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .Implements("IEquatable<MyId>")
            .Implements("IParsable<MyId>", Directives.Net7OrGreater)
            .Implements("ISpanParsable<MyId>", Directives.Net7OrGreater)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IEquatable<MyId>");
        await Assert.That(result.Code).Contains("#if NET7_0_OR_GREATER");
        await Assert.That(result.Code).Contains("IParsable<MyId>");
        await Assert.That(result.Code).Contains("ISpanParsable<MyId>");
    }

    [Test]
    public async Task Can_emit_multiple_conditional_interfaces_with_different_directives()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .Implements("IFormattable")
            .Implements("ISpanFormattable", Directives.Net6OrGreater)
            .Implements("IUtf8SpanFormattable", Directives.Net8OrGreater)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IFormattable");
        await Assert.That(result.Code).Contains("#if NET6_0_OR_GREATER");
        await Assert.That(result.Code).Contains("ISpanFormattable");
        await Assert.That(result.Code).Contains("#if NET8_0_OR_GREATER");
        await Assert.That(result.Code).Contains("IUtf8SpanFormattable");
    }

    #endregion

    #region Method-Level Directives

    [Test]
    public async Task Can_emit_conditional_method()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .AddMethod("Parse", m => m
                .When(Directives.Net7OrGreater)
                .AsStatic()
                .WithReturnType("MyId")
                .AddParameter("input", "string")
                .AddParameter("provider", "IFormatProvider?")
                .WithBody(b => b.AddReturn("new MyId()")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#if NET7_0_OR_GREATER");
        await Assert.That(result.Code).Contains("public static MyId Parse(string input, IFormatProvider? provider)");
        await Assert.That(result.Code).Contains("#endif");
    }

    [Test]
    public async Task Can_emit_conditional_property()
    {
        var result = TypeBuilder
            .Class("MyClass")
            .AddProperty("SpanValue", "ReadOnlySpan<char>", p => p
                .When(Directives.NetCoreApp21OrGreater)
                .WithAutoPropertyAccessors())
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#if NETCOREAPP2_1_OR_GREATER");
        await Assert.That(result.Code).Contains("ReadOnlySpan<char> SpanValue");
        await Assert.That(result.Code).Contains("#endif");
    }

    [Test]
    public async Task Can_emit_conditional_field()
    {
        var result = TypeBuilder
            .Class("MyClass")
            .AddField("_spanBuffer", "ReadOnlyMemory<byte>", f => f
                .When(Directives.Net6OrGreater))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#if NET6_0_OR_GREATER");
        await Assert.That(result.Code).Contains("ReadOnlyMemory<byte> _spanBuffer");
        await Assert.That(result.Code).Contains("#endif");
    }

    #endregion

    #region Statement-Level Directives

    [Test]
    public async Task Can_emit_conditional_statements()
    {
        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod("Process", m => m
                .WithBody(b => b
                    .AddStatement("DoSomething()")
                    .When(Directives.Net6OrGreater, cb => cb
                        .AddStatement("DoNet6Thing()"))
                    .AddStatement("DoSomethingElse()")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("DoSomething()");
        await Assert.That(result.Code).Contains("#if NET6_0_OR_GREATER");
        await Assert.That(result.Code).Contains("DoNet6Thing()");
        await Assert.That(result.Code).Contains("#endif");
        await Assert.That(result.Code).Contains("DoSomethingElse()");
    }

    [Test]
    public async Task Can_emit_multiple_conditional_statements()
    {
        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod("Process", m => m
                .WithBody(b => b
                    .When(Directives.Net6OrGreater, cb => cb
                        .AddStatement("var span = value.AsSpan()"))
                    .When(Directives.Net7OrGreater, cb => cb
                        .AddStatement("Utf8.TryWrite(buffer, out written, format)"))))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#if NET6_0_OR_GREATER");
        await Assert.That(result.Code).Contains("var span = value.AsSpan()");
        await Assert.That(result.Code).Contains("#if NET7_0_OR_GREATER");
        await Assert.That(result.Code).Contains("Utf8.TryWrite(buffer, out written, format)");
    }

    #endregion

    #region Directive Combinations

    [Test]
    public async Task Can_combine_directives_with_and()
    {
        var directive = Directives.Net6OrGreater.And(Directives.Custom("ENABLE_SPANS"));

        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod("Process", m => m
                .When(directive)
                .WithBody(b => b.AddReturn()))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#if NET6_0_OR_GREATER && ENABLE_SPANS");
    }

    [Test]
    public async Task Can_combine_directives_with_or()
    {
        var directive = Directives.Net6OrGreater.Or(Directives.NetStandard21OrGreater);

        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod("Process", m => m
                .When(directive)
                .WithBody(b => b.AddReturn()))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#if NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER");
    }

    [Test]
    public async Task Can_negate_directive()
    {
        var directive = Directives.NetFramework.Not();

        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod("Process", m => m
                .When(directive)
                .WithBody(b => b.AddReturn()))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#if !NETFRAMEWORK");
    }

    [Test]
    public async Task Can_use_custom_directive()
    {
        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod("Process", m => m
                .When(Directives.Custom("MY_FEATURE_FLAG"))
                .WithBody(b => b.AddReturn()))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#if MY_FEATURE_FLAG");
    }

    #endregion

    #region Pre-defined Directives

    [Test]
    public async Task All_predefined_directives_have_correct_conditions()
    {
        await Assert.That(Directives.Net5OrGreater.Condition).IsEqualTo("NET5_0_OR_GREATER");
        await Assert.That(Directives.Net6OrGreater.Condition).IsEqualTo("NET6_0_OR_GREATER");
        await Assert.That(Directives.Net7OrGreater.Condition).IsEqualTo("NET7_0_OR_GREATER");
        await Assert.That(Directives.Net8OrGreater.Condition).IsEqualTo("NET8_0_OR_GREATER");
        await Assert.That(Directives.Net9OrGreater.Condition).IsEqualTo("NET9_0_OR_GREATER");

        await Assert.That(Directives.NetCoreApp.Condition).IsEqualTo("NETCOREAPP");
        await Assert.That(Directives.NetCoreApp21OrGreater.Condition).IsEqualTo("NETCOREAPP2_1_OR_GREATER");
        await Assert.That(Directives.NetCoreApp30OrGreater.Condition).IsEqualTo("NETCOREAPP3_0_OR_GREATER");
        await Assert.That(Directives.NetCoreApp31OrGreater.Condition).IsEqualTo("NETCOREAPP3_1_OR_GREATER");

        await Assert.That(Directives.NetStandard.Condition).IsEqualTo("NETSTANDARD");
        await Assert.That(Directives.NetStandard20OrGreater.Condition).IsEqualTo("NETSTANDARD2_0_OR_GREATER");
        await Assert.That(Directives.NetStandard21OrGreater.Condition).IsEqualTo("NETSTANDARD2_1_OR_GREATER");

        await Assert.That(Directives.NetFramework.Condition).IsEqualTo("NETFRAMEWORK");
        await Assert.That(Directives.NetFramework461OrGreater.Condition).IsEqualTo("NET461_OR_GREATER");
        await Assert.That(Directives.NetFramework472OrGreater.Condition).IsEqualTo("NET472_OR_GREATER");
        await Assert.That(Directives.NetFramework48OrGreater.Condition).IsEqualTo("NET48_OR_GREATER");
    }

    #endregion
}
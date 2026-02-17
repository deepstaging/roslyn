// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Comparison;
using Deepstaging.Roslyn.Emit.Interfaces.Equality;
using Deepstaging.Roslyn.Emit.Interfaces.Formatting;
using Deepstaging.Roslyn.Emit.Interfaces.Parsing;

namespace Deepstaging.Roslyn.Tests.Emit;

/// <summary>
/// Integration tests for TypeBuilder interface implementation extensions.
/// These tests exercise full types with multiple interfaces and identify pain points.
/// </summary>
public class TypeBuilderInterfaceExtensionsIntegrationTests : RoslynTestBase
{
    #region Full Integration - Complete Type

    [Test]
    public async Task Can_implement_all_interfaces_for_Guid_backed_type()
    {
        var result = TypeBuilder
            .Struct("OrderId")
            .InNamespace("MyApp.Domain")
            .AsPartial()
            .AddProperty("Value", "global::System.Guid", p => p.AsReadOnly())
            .ImplementsIEquatable(WellKnownSymbols.Guid.ToSnapshot(), "Value")
            .ImplementsIComparable(WellKnownSymbols.Guid.ToSnapshot(), "Value")
            .ImplementsIFormattable(WellKnownSymbols.Guid.ToSnapshot(), "Value")
            .ImplementsISpanFormattable(WellKnownSymbols.Guid.ToSnapshot(), "Value")
            .ImplementsIParsable(WellKnownSymbols.Guid.ToSnapshot())
            .ImplementsISpanParsable(WellKnownSymbols.Guid.ToSnapshot())
            .ImplementsIUtf8SpanFormattable(WellKnownSymbols.Guid.ToSnapshot(), "Value")
            .ImplementsIUtf8SpanParsable(WellKnownSymbols.Guid.ToSnapshot()) // Should be skipped
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IEquatable<OrderId>");
        await Assert.That(result.Code).Contains("IComparable<OrderId>");
        await Assert.That(result.Code).Contains("IFormattable");
        await Assert.That(result.Code).Contains("ISpanFormattable");
        await Assert.That(result.Code).Contains("IParsable<OrderId>");
        await Assert.That(result.Code).Contains("ISpanParsable<OrderId>");
        await Assert.That(result.Code).Contains("IUtf8SpanFormattable");
        // IUtf8SpanParsable should be skipped for Guid
        await Assert.That(result.Code).DoesNotContain("IUtf8SpanParsable");
    }

    [Test]
    public async Task Can_implement_all_interfaces_for_Int_backed_type()
    {
        var result = TypeBuilder
            .Struct("UserId")
            .InNamespace("MyApp.Domain")
            .AsPartial()
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsIEquatable(WellKnownSymbols.Int32.ToSnapshot(), "Value")
            .ImplementsIComparable(WellKnownSymbols.Int32.ToSnapshot(), "Value")
            .ImplementsIFormattable(WellKnownSymbols.Int32.ToSnapshot(), "Value")
            .ImplementsISpanFormattable(WellKnownSymbols.Int32.ToSnapshot(), "Value")
            .ImplementsIParsable(WellKnownSymbols.Int32.ToSnapshot())
            .ImplementsISpanParsable(WellKnownSymbols.Int32.ToSnapshot())
            .ImplementsIUtf8SpanFormattable(WellKnownSymbols.Int32.ToSnapshot(), "Value")
            .ImplementsIUtf8SpanParsable(WellKnownSymbols.Int32.ToSnapshot())
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IUtf8SpanParsable<UserId>");
    }

    [Test]
    public async Task Can_implement_all_interfaces_for_String_backed_type()
    {
        var result = TypeBuilder
            .Struct("Slug")
            .InNamespace("MyApp.Domain")
            .AsPartial()
            .AddProperty("Value", "string?", p => p.AsReadOnly())
            .ImplementsIEquatable(WellKnownSymbols.String.ToSnapshot(), "Value")
            .ImplementsIComparable(WellKnownSymbols.String.ToSnapshot(), "Value")
            .ImplementsIFormattable(WellKnownSymbols.String.ToSnapshot(), "Value")
            .ImplementsISpanFormattable(WellKnownSymbols.String.ToSnapshot(), "Value")
            .ImplementsIParsable(WellKnownSymbols.String.ToSnapshot())
            .ImplementsISpanParsable(WellKnownSymbols.String.ToSnapshot())
            .ImplementsIUtf8SpanFormattable(WellKnownSymbols.String.ToSnapshot(), "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        // Should have null-safe patterns
        await Assert.That(result.Code).Contains("(null, null)");
    }

    #endregion

    #region Pain Point Discovery Tests

    [Test]
    public async Task Pain_point_need_to_call_GetWellKnownType_repeatedly()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .ImplementsIEquatable(WellKnownSymbols.Guid.ToSnapshot(), "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
    }

    [Test]
    public async Task Pain_point_valueAccessor_is_stringly_typed()
    {
        // PAIN POINT: "Value" is a magic string - no compile-time checking
        var result = TypeBuilder
            .Struct("MyId")
            .AddProperty("Value", "global::System.Guid", p => p.AsReadOnly())
            .ImplementsIEquatable(WellKnownSymbols.Guid.ToSnapshot(), "Value") // "Value" could be typo'd
            .ImplementsIComparable(WellKnownSymbols.Guid.ToSnapshot(), "Value") // repeated everywhere
            .ImplementsIFormattable(WellKnownSymbols.Guid.ToSnapshot(), "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
    }

    [Test]
    public async Task Pain_point_backingType_repeated_for_each_interface()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .ImplementsIEquatable(WellKnownSymbols.Guid.ToSnapshot(), "Value")
            .ImplementsIComparable(WellKnownSymbols.Guid.ToSnapshot(), "Value")
            .ImplementsIFormattable(WellKnownSymbols.Guid.ToSnapshot(), "Value")
            .ImplementsISpanFormattable(WellKnownSymbols.Guid.ToSnapshot(), "Value")
            .ImplementsIParsable(WellKnownSymbols.Guid.ToSnapshot())
            .ImplementsISpanParsable(WellKnownSymbols.Guid.ToSnapshot())
            .Emit();

        await Assert.That(result.Success).IsTrue();
        // Would be nicer: .ImplementsCommonInterfaces(guidType, "Value")
    }

    [Test]
    public async Task Pain_point_no_way_to_add_constructor()
    {
        // PAIN POINT: Extensions add interface members but not the constructor
        // User still needs to manually add constructor and Value property
        var result = TypeBuilder
            .Struct("MyId")
            .AddProperty("Value", "global::System.Guid", p => p.AsReadOnly())
            // Missing: constructor that takes Guid
            .ImplementsIEquatable(WellKnownSymbols.Guid.ToSnapshot(), "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        // Generated code won't have a constructor
        await Assert.That(result.Code).DoesNotContain("MyId(");
    }

    [Test]
    public async Task Pain_point_string_comparison_defaults_to_ordinal()
    {
        // Default behavior uses Ordinal comparison
        var result = TypeBuilder
            .Struct("MyId")
            .ImplementsIEquatable(WellKnownSymbols.String.ToSnapshot(), "Value")
            .ImplementsIComparable(WellKnownSymbols.String.ToSnapshot(), "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("StringComparison.Ordinal");
        await Assert.That(result.Code).Contains("string.CompareOrdinal");
    }

    [Test]
    public async Task Can_customize_string_comparison_to_ordinal_ignore_case()
    {
        // Can now customize string comparison
        var result = TypeBuilder
            .Struct("MyId")
            .ImplementsIEquatable(WellKnownSymbols.String.ToSnapshot(), "Value", StringComparison.OrdinalIgnoreCase)
            .ImplementsIComparable(WellKnownSymbols.String.ToSnapshot(), "Value", StringComparison.OrdinalIgnoreCase)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("StringComparison.OrdinalIgnoreCase");
        await Assert.That(result.Code).Contains("StringComparer.OrdinalIgnoreCase.GetHashCode");
    }

    [Test]
    public async Task ConfigureParameter_allows_adding_attributes_after_parse()
    {
        // MethodBuilder.ConfigureParameter enables adding attributes to parsed parameters
        var method = MethodBuilder
            .Parse("public string ToString(string? format, IFormatProvider? provider)")
            .ConfigureParameter("format", p => p.WithAttribute("StringSyntax", a => a.WithArgument("\"GuidFormat\"")))
            .WithExpressionBody("Value.ToString(format, provider)");

        var result = TypeBuilder
            .Struct("MyId")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("[StringSyntax(\"GuidFormat\")]");
        await Assert.That(result.Code).Contains("string? format");
    }

    #endregion
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Operators.Conversions;

namespace Deepstaging.Roslyn.Tests.Emit.Operators.Conversions;

/// <summary>
/// Tests for TypeBuilder backing conversion operator extensions.
/// </summary>
public class TypeBuilderBackingConversionsExtensionsTests : RoslynTestBase
{
    [Test]
    public async Task WithBackingConversions_generates_implicit_from_and_explicit_to()
    {
        var result = TypeBuilder
            .Struct("CustomerId")
            .InNamespace("Test")
            .AddProperty("Value", "global::System.Guid", p => p.AsReadOnly())
            .WithBackingConversions(WellKnownSymbols.Guid, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public static implicit operator CustomerId(global::System.Guid value)");
        await Assert.That(result.Code).Contains("new CustomerId(value)");
        await Assert.That(result.Code).Contains("public static explicit operator global::System.Guid(CustomerId value)");
        await Assert.That(result.Code).Contains("=> value.Value");
    }

    [Test]
    public async Task WithBackingConversions_with_Int32()
    {
        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .WithBackingConversions(WellKnownSymbols.Int32, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("implicit operator Counter(int value)");
        await Assert.That(result.Code).Contains("explicit operator int");
        await Assert.That(result.Code).Contains("Counter value)");
    }

    [Test]
    public async Task WithBackingConversions_custom_expressions()
    {
        var result = TypeBuilder
            .Struct("CustomerId")
            .InNamespace("Test")
            .WithBackingConversions(
                "global::System.Guid",
                "new CustomerId(value)",
                "value.Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("new CustomerId(value)");
    }

    [Test]
    public async Task WithImplicitConversionFromBacking_only_generates_implicit()
    {
        var result = TypeBuilder
            .Struct("CustomerId")
            .InNamespace("Test")
            .WithImplicitConversionFromBacking(WellKnownSymbols.Guid)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public static implicit operator CustomerId");
        await Assert.That(result.Code).DoesNotContain("explicit operator");
    }

    [Test]
    public async Task WithExplicitConversionToBacking_only_generates_explicit()
    {
        var result = TypeBuilder
            .Struct("CustomerId")
            .InNamespace("Test")
            .AddProperty("Value", "global::System.Guid", p => p.AsReadOnly())
            .WithExplicitConversionToBacking(WellKnownSymbols.Guid, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public static explicit operator global::System.Guid");
        await Assert.That(result.Code).DoesNotContain("implicit operator");
    }

    #region PropertyBuilder/FieldBuilder Overloads

    [Test]
    public async Task WithBackingConversions_accepts_PropertyBuilder()
    {
        var valueProperty = PropertyBuilder.For("Value", "global::System.Guid").AsReadOnly();

        var result = TypeBuilder
            .Struct("CustomerId")
            .InNamespace("Test")
            .AddProperty(valueProperty)
            .WithBackingConversions(WellKnownSymbols.Guid, valueProperty)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("=> value.Value");
    }

    [Test]
    public async Task WithBackingConversions_accepts_FieldBuilder()
    {
        var valueField = FieldBuilder.Parse("private readonly global::System.Guid _id;");

        var result = TypeBuilder
            .Struct("CustomerId")
            .InNamespace("Test")
            .AddField(valueField)
            .WithBackingConversions(WellKnownSymbols.Guid, valueField)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("=> value._id");
    }

    #endregion
}

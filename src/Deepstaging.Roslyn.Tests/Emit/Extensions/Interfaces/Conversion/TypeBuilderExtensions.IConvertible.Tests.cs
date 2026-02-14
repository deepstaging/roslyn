// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Conversion;

namespace Deepstaging.Roslyn.Tests.Emit.Interfaces.Conversion;

public class TypeBuilderConvertibleExtensionsTests : RoslynTestBase
{
    [Test]
    public async Task ImplementsIConvertible_adds_all_conversion_methods()
    {
        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsIConvertible(WellKnownSymbols.Int32, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IConvertible");
        await Assert.That(result.Code).Contains("GetTypeCode()");
        await Assert.That(result.Code).Contains("TypeCode.Int32");
        await Assert.That(result.Code).Contains("ToBoolean");
        await Assert.That(result.Code).Contains("ToInt32");
        await Assert.That(result.Code).Contains("ToInt64");
        await Assert.That(result.Code).Contains("ToDouble");
        await Assert.That(result.Code).Contains("ToString");
    }

    [Test]
    public async Task ImplementsIConvertible_with_string_type()
    {
        var result = TypeBuilder
            .Struct("CustomerId")
            .InNamespace("Test")
            .AddProperty("Value", "string", p => p.AsReadOnly())
            .ImplementsIConvertible(WellKnownSymbols.String, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("TypeCode.String");
        await Assert.That(result.Code).Contains("((global::System.IConvertible)Value)");
    }

    [Test]
    public async Task ImplementsIConvertible_with_custom_type_code()
    {
        var result = TypeBuilder
            .Struct("CustomValue")
            .InNamespace("Test")
            .AddProperty("Value", "decimal", p => p.AsReadOnly())
            .ImplementsIConvertible("Decimal", "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("TypeCode.Decimal");
    }

    [Test]
    public async Task ImplementsIConvertible_includes_ToType_method()
    {
        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsIConvertible(WellKnownSymbols.Int32, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("ToType");
        await Assert.That(result.Code).Contains("ToType(conversionType, provider)");
    }

    #region PropertyBuilder/FieldBuilder Overloads

    [Test]
    public async Task ImplementsIConvertible_accepts_PropertyBuilder()
    {
        var valueProperty = PropertyBuilder.For("Value", "int").AsReadOnly();

        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty(valueProperty)
            .ImplementsIConvertible(WellKnownSymbols.Int32, valueProperty)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("((global::System.IConvertible)Value)");
    }

    [Test]
    public async Task ImplementsIConvertible_accepts_FieldBuilder()
    {
        var valueField = FieldBuilder.Parse("private readonly int _count;");

        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddField(valueField)
            .ImplementsIConvertible(WellKnownSymbols.Int32, valueField)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("((global::System.IConvertible)_count)");
    }

    #endregion
}
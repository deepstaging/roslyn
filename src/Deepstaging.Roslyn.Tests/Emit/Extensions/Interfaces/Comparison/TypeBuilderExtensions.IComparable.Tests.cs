// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Comparison;

namespace Deepstaging.Roslyn.Tests.Emit.Interfaces.Comparison;

/// <summary>
/// Tests for TypeBuilder IComparable interface implementation extensions.
/// </summary>
public class TypeBuilderIComparableExtensionsTests : RoslynTestBase
{
    #region IComparable - Semantic API

    [Test]
    public async Task IComparable_with_Guid_generates_value_type_compare()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .AddProperty("Value", "global::System.Guid", p => p.AsReadOnly())
            .ImplementsIComparable(WellKnownSymbols.Guid, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IComparable<MyId>");
        await Assert.That(result.Code).Contains("Value.CompareTo(other.Value)");
        await Assert.That(result.Code).Contains("operator >");
        await Assert.That(result.Code).Contains("operator <");
        await Assert.That(result.Code).Contains("operator >=");
        await Assert.That(result.Code).Contains("operator <=");
    }

    [Test]
    public async Task IComparable_with_String_generates_null_safe_compare()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .AddProperty("Value", "string?", p => p.AsReadOnly())
            .ImplementsIComparable(WellKnownSymbols.String, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("(Value, other.Value) switch");
        await Assert.That(result.Code).Contains("(null, null) => 0");
        await Assert.That(result.Code).Contains("string.CompareOrdinal");
    }

    #endregion

    #region IComparable - Expression API

    [Test]
    public async Task IComparable_with_custom_expression()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .ImplementsIComparable("Value.CustomCompareTo(other.Value)")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Value.CustomCompareTo(other.Value)");
    }

    #endregion

    #region PropertyBuilder/FieldBuilder Overloads

    [Test]
    public async Task IComparable_accepts_PropertyBuilder()
    {
        var valueProperty = PropertyBuilder.For("Value", "int").AsReadOnly();

        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddProperty(valueProperty)
            .ImplementsIComparable(WellKnownSymbols.Int32, valueProperty)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Value.CompareTo(other.Value)");
    }

    [Test]
    public async Task IComparable_accepts_FieldBuilder()
    {
        var valueField = FieldBuilder.Parse("private readonly int _count;");

        var result = TypeBuilder
            .Struct("Counter")
            .InNamespace("Test")
            .AddField(valueField)
            .ImplementsIComparable(WellKnownSymbols.Int32, valueField)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("_count.CompareTo(other._count)");
    }

    #endregion
}

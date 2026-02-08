// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Equality;

namespace Deepstaging.Roslyn.Tests.Emit.Interfaces.Equality;

/// <summary>
/// Tests for TypeBuilder IEquatable interface implementation extensions.
/// </summary>
public class TypeBuilderIEquatableExtensionsTests : RoslynTestBase
{
    #region IEquatable - Semantic API

    [Test]
    public async Task IEquatable_with_Guid_generates_value_type_equals()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .AddProperty("Value", "global::System.Guid", p => p.AsReadOnly())
            .ImplementsIEquatable(WellKnownSymbols.Guid, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IEquatable<MyId>");
        await Assert.That(result.Code).Contains("Value.Equals(other.Value)");
        await Assert.That(result.Code).Contains("Value.GetHashCode()");
        await Assert.That(result.Code).Contains("operator ==");
        await Assert.That(result.Code).Contains("operator !=");
    }

    [Test]
    public async Task IEquatable_with_String_generates_null_safe_equals()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .AddProperty("Value", "string?", p => p.AsReadOnly())
            .ImplementsIEquatable(WellKnownSymbols.String, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("(Value, other.Value) switch");
        await Assert.That(result.Code).Contains("(null, null) => true");
        await Assert.That(result.Code).Contains("Value?.GetHashCode() ?? 0");
    }

    [Test]
    public async Task IEquatable_with_Int32_generates_value_type_equals()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .AddProperty("Value", "int", p => p.AsReadOnly())
            .ImplementsIEquatable(WellKnownSymbols.Int32, "Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Value.Equals(other.Value)");
        await Assert.That(result.Code).Contains("Value.GetHashCode()");
    }

    #endregion

    #region IEquatable - Expression API

    [Test]
    public async Task IEquatable_with_custom_expressions()
    {
        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .ImplementsIEquatable(
                equalsExpression: "Value.CustomEquals(other.Value)",
                hashCodeExpression: "Value.CustomHashCode()")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Value.CustomEquals(other.Value)");
        await Assert.That(result.Code).Contains("Value.CustomHashCode()");
    }

    #endregion

    #region PropertyBuilder/FieldBuilder Overloads

    [Test]
    public async Task IEquatable_accepts_PropertyBuilder()
    {
        var valueProperty = PropertyBuilder.For("Value", "global::System.Guid").AsReadOnly();

        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .AddProperty(valueProperty)
            .ImplementsIEquatable(WellKnownSymbols.Guid, valueProperty)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Value.Equals(other.Value)");
        await Assert.That(result.Code).Contains("Value.GetHashCode()");
    }

    [Test]
    public async Task IEquatable_accepts_FieldBuilder()
    {
        var valueField = FieldBuilder.Parse("private readonly global::System.Guid _id;");

        var result = TypeBuilder
            .Struct("MyId")
            .InNamespace("Test")
            .AddField(valueField)
            .ImplementsIEquatable(WellKnownSymbols.Guid, valueField)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("_id.Equals(other._id)");
        await Assert.That(result.Code).Contains("_id.GetHashCode()");
    }

    #endregion
}

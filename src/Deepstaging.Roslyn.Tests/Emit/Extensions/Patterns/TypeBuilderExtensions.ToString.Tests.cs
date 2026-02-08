// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Patterns;

namespace Deepstaging.Roslyn.Tests.Emit.Patterns;

/// <summary>
/// Tests for TypeBuilder ToString override extensions.
/// </summary>
public class TypeBuilderToStringExtensionsTests : RoslynTestBase
{
    [Test]
    public async Task OverridesToString_delegates_to_value()
    {
        var result = TypeBuilder
            .Struct("CustomerId")
            .InNamespace("Test")
            .AddProperty("Value", "global::System.Guid", p => p.AsReadOnly())
            .OverridesToString("Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public override string ToString()");
        await Assert.That(result.Code).Contains("Value.ToString()");
    }

    [Test]
    public async Task OverridesToStringNullSafe_handles_null()
    {
        var result = TypeBuilder
            .Struct("Name")
            .InNamespace("Test")
            .AddProperty("Value", "string?", p => p.AsReadOnly())
            .OverridesToStringNullSafe("Value")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Value?.ToString() ?? \"\"");
    }

    [Test]
    public async Task OverridesToStringNullSafe_custom_null_value()
    {
        var result = TypeBuilder
            .Struct("Name")
            .InNamespace("Test")
            .AddProperty("Value", "string?", p => p.AsReadOnly())
            .OverridesToStringNullSafe("Value", "\"<null>\"")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Value?.ToString() ?? \"<null>\"");
    }

    [Test]
    public async Task OverridesToString_custom_expression()
    {
        var result = TypeBuilder
            .Struct("CustomerId")
            .InNamespace("Test")
            .AddProperty("Value", "global::System.Guid", p => p.AsReadOnly())
            .OverridesToString("$\"CustomerId: {Value}\"", isCustomExpression: true)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("$\"CustomerId: {Value}\"");
    }

    #region PropertyBuilder/FieldBuilder Overloads

    [Test]
    public async Task OverridesToString_accepts_PropertyBuilder()
    {
        var valueProperty = PropertyBuilder.For("Value", "global::System.Guid").AsReadOnly();

        var result = TypeBuilder
            .Struct("CustomerId")
            .InNamespace("Test")
            .AddProperty(valueProperty)
            .OverridesToString(valueProperty)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Value.ToString()");
    }

    [Test]
    public async Task OverridesToString_accepts_FieldBuilder()
    {
        var valueField = FieldBuilder.Parse("private readonly global::System.Guid _id;");

        var result = TypeBuilder
            .Struct("CustomerId")
            .InNamespace("Test")
            .AddField(valueField)
            .OverridesToString(valueField)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("_id.ToString()");
    }

    [Test]
    public async Task OverridesToStringNullSafe_accepts_PropertyBuilder()
    {
        var valueProperty = PropertyBuilder.For("Value", "string?").AsReadOnly();

        var result = TypeBuilder
            .Struct("Name")
            .InNamespace("Test")
            .AddProperty(valueProperty)
            .OverridesToStringNullSafe(valueProperty)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Value?.ToString() ?? \"\"");
    }

    #endregion
}

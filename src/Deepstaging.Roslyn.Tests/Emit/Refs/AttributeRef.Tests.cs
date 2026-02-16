// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit;
using Deepstaging.Roslyn.Emit.Refs;

namespace Deepstaging.Roslyn.Tests.Emit.Refs;

public class AttributeRefTests
{
    #region Factory Methods

    [Test]
    public async Task From_creates_attribute_ref()
    {
        var attr = AttributeRef.From("System.ObsoleteAttribute");

        await Assert.That(attr.Value).IsEqualTo("System.ObsoleteAttribute");
    }

    [Test]
    public async Task Global_creates_globally_qualified_attribute_ref()
    {
        var attr = AttributeRef.Global("System.ObsoleteAttribute");

        await Assert.That(attr.Value).IsEqualTo("global::System.ObsoleteAttribute");
    }

    [Test]
    public void From_throws_on_null() =>
        Assert.Throws<ArgumentException>(() => AttributeRef.From(null!));

    [Test]
    public void From_throws_on_whitespace() =>
        Assert.Throws<ArgumentException>(() => AttributeRef.From("  "));

    [Test]
    public void Global_throws_on_null() =>
        Assert.Throws<ArgumentException>(() => AttributeRef.Global(null!));

    #endregion

    #region Bridge to AttributeBuilder

    [Test]
    public async Task WithArgument_returns_AttributeBuilder()
    {
        AttributeBuilder builder = AttributeRef.Global("System.ObsoleteAttribute")
            .WithArgument("\"Use NewMethod instead\"");

        await Assert.That(builder.Name).IsEqualTo("global::System.ObsoleteAttribute");
        await Assert.That(builder.Arguments).Count().IsEqualTo(1);
    }

    [Test]
    public async Task WithArguments_returns_AttributeBuilder()
    {
        AttributeBuilder builder = AttributeRef.Global("System.ObsoleteAttribute")
            .WithArguments("\"Use NewMethod instead\"", "true");

        await Assert.That(builder.Arguments).Count().IsEqualTo(2);
    }

    [Test]
    public async Task WithNamedArgument_returns_AttributeBuilder()
    {
        AttributeBuilder builder = AttributeRef.Global("System.ObsoleteAttribute")
            .WithNamedArgument("DiagnosticId", "\"OBS001\"");

        await Assert.That(builder.NamedArguments).Count().IsEqualTo(1);
    }

    [Test]
    public async Task AddUsing_returns_AttributeBuilder()
    {
        AttributeBuilder builder = AttributeRef.Global("System.ObsoleteAttribute")
            .AddUsing("System.Linq");

        await Assert.That(builder.Usings).Count().IsEqualTo(1);
    }

    #endregion

    #region Implicit Conversions

    [Test]
    public async Task Implicit_conversion_to_string()
    {
        string name = AttributeRef.Global("System.SerializableAttribute");

        await Assert.That(name).IsEqualTo("global::System.SerializableAttribute");
    }

    [Test]
    public async Task Implicit_conversion_to_AttributeBuilder()
    {
        AttributeBuilder builder = AttributeRef.Global("System.SerializableAttribute");

        await Assert.That(builder.Name).IsEqualTo("global::System.SerializableAttribute");
    }

    #endregion

    #region NamespaceRef Integration

    [Test]
    public async Task NamespaceRef_Attribute_creates_fully_qualified()
    {
        var ns = NamespaceRef.From("System.ComponentModel.DataAnnotations");
        var attr = ns.Attribute("KeyAttribute");

        await Assert.That(attr.Value).IsEqualTo("System.ComponentModel.DataAnnotations.KeyAttribute");
    }

    [Test]
    public async Task NamespaceRef_GlobalAttribute_creates_globally_qualified()
    {
        var ns = NamespaceRef.From("System.ComponentModel.DataAnnotations");
        var attr = ns.GlobalAttribute("KeyAttribute");

        await Assert.That(attr.Value).IsEqualTo("global::System.ComponentModel.DataAnnotations.KeyAttribute");
    }

    #endregion

    #region Refs Integration

    [Test]
    public async Task EntityFrameworkRefs_Key_is_AttributeRef()
    {
        AttributeRef attr = EntityFrameworkRefs.Key;

        await Assert.That(attr.Value).IsEqualTo("global::System.ComponentModel.DataAnnotations.KeyAttribute");
    }

    [Test]
    public async Task EntityFrameworkRefs_Column_bridges_to_builder()
    {
        AttributeBuilder builder = EntityFrameworkRefs.Column
            .WithArgument("\"name\"");

        await Assert.That(builder.Name)
            .IsEqualTo("global::System.ComponentModel.DataAnnotations.Schema.ColumnAttribute");
        await Assert.That(builder.Arguments).Count().IsEqualTo(1);
    }

    [Test]
    public async Task JsonRefs_Converter_is_AttributeRef()
    {
        AttributeRef attr = JsonRefs.Converter;

        await Assert.That(attr.Value).IsEqualTo("global::System.Text.Json.Serialization.JsonConverter");
    }

    #endregion

    #region ToString

    [Test]
    public async Task ToString_returns_value()
    {
        var attr = AttributeRef.Global("System.ObsoleteAttribute");

        await Assert.That(attr.ToString()).IsEqualTo("global::System.ObsoleteAttribute");
    }

    #endregion
}

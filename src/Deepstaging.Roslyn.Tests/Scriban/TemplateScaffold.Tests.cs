// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Scriban;

namespace Deepstaging.Roslyn.Tests.Scriban;

public class TemplateScaffoldTests : RoslynTestBase
{
    #region Generate - Basic Replacement

    [Test]
    public async Task Generate_replaces_bound_values_with_placeholders()
    {
        var map = new TemplateMap<TestModel>();

        var emit = TypeBuilder.Class(map.Bind("MyClass", m => m.TypeName))
            .InNamespace(map.Bind("TestApp", m => m.Namespace))
            .Emit();

        var customizable = emit.WithUserTemplate("Test/MyType", new TestModel(), map);
        var scaffold = TemplateScaffold.Generate(customizable);

        await Assert.That(scaffold).IsNotNull();
        await Assert.That(scaffold!).Contains("{{ TypeName }}");
        await Assert.That(scaffold).Contains("{{ Namespace }}");
        await Assert.That(scaffold).DoesNotContain("MyClass");
        await Assert.That(scaffold).DoesNotContain("TestApp");
    }

    #endregion

    #region Generate - Nested Property Paths

    [Test]
    public async Task Generate_replaces_nested_property_values()
    {
        var map = new TemplateMap<TestModel>();

        var emit = TypeBuilder.Class("MyClass")
            .AddProperty("Value", map.Bind("int", m => m.BackingType.CodeName), p => p.WithAutoPropertyAccessors())
            .Emit();

        var customizable = emit.WithUserTemplate("Test/MyType", new TestModel(), map);
        var scaffold = TemplateScaffold.Generate(customizable);

        await Assert.That(scaffold).IsNotNull();
        await Assert.That(scaffold!).Contains("{{ BackingType.CodeName }}");
    }

    #endregion

    #region Generate - Longest Value First

    [Test]
    public async Task Generate_replaces_longer_values_first()
    {
        var map = new TemplateMap<TestModel>();

        // "TestApp.OrderId" is longer than "OrderId" — must be replaced first
        var emit = TypeBuilder.Class(map.Bind("OrderId", m => m.TypeName))
            .InNamespace(map.Bind("TestApp.OrderId", m => m.Namespace))
            .Emit();

        var customizable = emit.WithUserTemplate("Test/MyType", new TestModel(), map);
        var scaffold = TemplateScaffold.Generate(customizable);

        await Assert.That(scaffold).IsNotNull();
        // "TestApp.OrderId" should be replaced as a unit, not partially
        await Assert.That(scaffold!).Contains("{{ Namespace }}");
    }

    #endregion

    #region Generate - No Bindings

    [Test]
    public async Task Generate_returns_emit_code_when_no_bindings()
    {
        var emit = TypeBuilder.Class("MyClass").Emit();
        var customizable = emit.WithUserTemplate("Test/MyType", new TestModel());

        var scaffold = TemplateScaffold.Generate(customizable);

        await Assert.That(scaffold).IsNotNull();
        await Assert.That(scaffold!).Contains("class MyClass");
    }

    #endregion

    #region Generate - Invalid Emit

    [Test]
    public async Task Generate_returns_null_when_default_emit_is_invalid()
    {
        var method = MethodBuilder
            .For("Bad")
            .WithBody(b => b.AddStatement("not valid c#"));

        var emit = TypeBuilder.Class("MyClass").AddMethod(method).Emit();
        var customizable = emit.WithUserTemplate("Test/MyType", new TestModel());

        var scaffold = TemplateScaffold.Generate(customizable);

        await Assert.That(scaffold).IsNull();
    }

    #endregion

    #region Test Models

    private sealed class TestModel
    {
        public string TypeName { get; init; } = "";
        public string Namespace { get; init; } = "";
        public BackingTypeModel BackingType { get; init; } = new();
    }

    private sealed class BackingTypeModel
    {
        public string CodeName { get; init; } = "";
    }

    #endregion
}

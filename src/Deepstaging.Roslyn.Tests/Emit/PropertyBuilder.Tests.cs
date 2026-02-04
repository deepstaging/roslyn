// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Emit;

public class PropertyBuilderTests : RoslynTestBase
{
    [Test]
    public async Task Can_emit_auto_property()
    {
        var property = PropertyBuilder
            .For("Name", "string")
            .WithAccessibility(Accessibility.Public)
            .WithAutoPropertyAccessors();

        var result = TypeBuilder
            .Class("Person")
            .AddProperty(property)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public string Name { get; set; }");
    }

    [Test]
    public async Task Can_emit_readonly_auto_property()
    {
        var property = PropertyBuilder
            .For("Id", "Guid")
            .WithAccessibility(Accessibility.Public)
            .WithAutoPropertyAccessors()
            .AsReadOnly();

        var result = TypeBuilder
            .Class("Entity")
            .AddProperty(property)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public Guid Id { get; }");
    }

    [Test]
    public async Task Can_emit_property_with_init_accessor()
    {
        var property = PropertyBuilder
            .For("CreatedAt", "DateTime")
            .WithAccessibility(Accessibility.Public)
            .WithAutoPropertyAccessors();

        var result = TypeBuilder
            .Class("Entity")
            .AddProperty(property)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public DateTime CreatedAt { get; set; }");
    }

    [Test]
    public async Task Can_emit_property_with_initializer()
    {
        var property = PropertyBuilder
            .For("Items", "List<string>")
            .WithAccessibility(Accessibility.Public)
            .WithAutoPropertyAccessors()
            .WithInitializer("new()");

        var result = TypeBuilder
            .Class("Container")
            .AddUsing("System.Collections.Generic")
            .AddProperty(property)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public List<string> Items { get; set; } = new();");
    }

    [Test]
    public async Task Can_emit_expression_bodied_property()
    {
        var property = PropertyBuilder
            .For("FullName", "string")
            .WithAccessibility(Accessibility.Public)
            .WithGetter("$\"{FirstName} {LastName}\"");

        var result = TypeBuilder
            .Class("Person")
            .AddProperty(property)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public string FullName => $\"{FirstName} {LastName}\";");
    }

    [Test]
    public async Task Can_emit_property_with_block_accessors()
    {
        var property = PropertyBuilder
            .For("Name", "string")
            .WithAccessibility(Accessibility.Public)
            .WithGetter(b => b.AddReturn("_name ?? string.Empty"))
            .WithSetter(b => b.AddStatement("_name = value?.Trim()"));

        var result = TypeBuilder
            .Class("Person")
            .AddProperty(property)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("get");
        await Assert.That(result.Code).Contains("_name ?? string.Empty");
        await Assert.That(result.Code).Contains("set");
        await Assert.That(result.Code).Contains("_name = value?.Trim();");
    }

    [Test]
    public async Task Can_emit_static_property()
    {
        var property = PropertyBuilder
            .For("Instance", "Singleton")
            .WithAccessibility(Accessibility.Public)
            .AsStatic()
            .WithAutoPropertyAccessors();

        var result = TypeBuilder
            .Class("Singleton")
            .AddProperty(property)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public static Singleton Instance { get; set; }");
    }

    [Test]
    public async Task Can_emit_virtual_property()
    {
        var property = PropertyBuilder
            .For("Value", "int")
            .WithAccessibility(Accessibility.Public)
            .AsVirtual()
            .WithAutoPropertyAccessors();

        var result = TypeBuilder
            .Class("Base")
            .AddProperty(property)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public virtual int Value { get; set; }");
    }

    [Test]
    public async Task Can_emit_override_property()
    {
        var property = PropertyBuilder
            .For("Value", "int")
            .WithAccessibility(Accessibility.Public)
            .AsOverride()
            .WithAutoPropertyAccessors();

        var result = TypeBuilder
            .Class("Derived")
            .AddProperty(property)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public override int Value { get; set; }");
    }

    [Test]
    public async Task Can_emit_property_with_lambda_configuration()
    {
        var result = TypeBuilder
            .Class("Person")
            .AddProperty("Age", "int", prop => prop
                .WithAccessibility(Accessibility.Public)
                .WithAutoPropertyAccessors())
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public int Age { get; set; }");
    }
}

// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Emit;

public class FieldBuilderTests : RoslynTestBase
{
    [Test]
    public async Task Can_emit_simple_field()
    {
        var field = FieldBuilder
            .For("_value", "int")
            .WithAccessibility(Accessibility.Private);

        var result = TypeBuilder
            .Class("MyClass")
            .AddField(field)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("private int _value;");
    }

    [Test]
    public async Task Can_emit_readonly_field()
    {
        var field = FieldBuilder
            .For("_id", "Guid")
            .WithAccessibility(Accessibility.Private)
            .AsReadonly();

        var result = TypeBuilder
            .Class("Entity")
            .AddField(field)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("private readonly Guid _id;");
    }

    [Test]
    public async Task Can_emit_const_field()
    {
        var field = FieldBuilder
            .For("MaxSize", "int")
            .WithAccessibility(Accessibility.Public)
            .AsConst()
            .WithInitializer("100");

        var result = TypeBuilder
            .Class("Constants")
            .AddField(field)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public const int MaxSize = 100;");
    }

    [Test]
    public async Task Can_emit_static_field()
    {
        var field = FieldBuilder
            .For("_counter", "int")
            .WithAccessibility(Accessibility.Private)
            .AsStatic();

        var result = TypeBuilder
            .Class("Tracker")
            .AddField(field)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("private static int _counter;");
    }

    [Test]
    public async Task Can_emit_static_readonly_field()
    {
        var field = FieldBuilder
            .For("_instance", "Singleton")
            .WithAccessibility(Accessibility.Private)
            .AsStatic()
            .AsReadonly()
            .WithInitializer("new()");

        var result = TypeBuilder
            .Class("Singleton")
            .AddField(field)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("private static readonly Singleton _instance = new();");
    }

    [Test]
    public async Task Can_emit_field_with_initializer()
    {
        var field = FieldBuilder
            .For("_items", "List<string>")
            .WithAccessibility(Accessibility.Private)
            .WithInitializer("new()");

        var result = TypeBuilder
            .Class("Container")
            .AddUsing("System.Collections.Generic")
            .AddField(field)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("private List<string> _items = new();");
    }

    [Test]
    public async Task Can_emit_public_field()
    {
        var field = FieldBuilder
            .For("Name", "string")
            .WithAccessibility(Accessibility.Public);

        var result = TypeBuilder
            .Class("Person")
            .AddField(field)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public string Name;");
    }

    [Test]
    public async Task Can_emit_field_with_lambda_configuration()
    {
        var result = TypeBuilder
            .Class("Service")
            .AddField("_repository", "IRepository", field => field
                .WithAccessibility(Accessibility.Private)
                .AsReadonly())
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("private readonly IRepository _repository;");
    }
}

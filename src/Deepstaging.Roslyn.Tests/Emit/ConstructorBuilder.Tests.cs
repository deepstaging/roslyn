// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Emit;

public class ConstructorBuilderTests : RoslynTestBase
{
    [Test]
    public async Task Can_emit_parameterless_constructor()
    {
        var constructor = ConstructorBuilder
            .For("MyClass")
            .WithAccessibility(Accessibility.Public)
            .WithBody(b => b.AddStatement("// Initialize"));

        var result = TypeBuilder
            .Class("MyClass")
            .AddConstructor(constructor)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public MyClass()");
    }

    [Test]
    public async Task Can_emit_constructor_with_parameters()
    {
        var constructor = ConstructorBuilder
            .For("Person")
            .WithAccessibility(Accessibility.Public)
            .AddParameter("name", "string")
            .AddParameter("age", "int")
            .WithBody(b => b.AddStatements("""
                Name = name;
                Age = age;
                """));

        var result = TypeBuilder
            .Class("Person")
            .AddConstructor(constructor)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public Person(string name, int age)");
        await Assert.That(result.Code).Contains("Name = name;");
    }

    [Test]
    public async Task Can_emit_constructor_with_this_chaining()
    {
        var constructor = ConstructorBuilder
            .For("Person")
            .WithAccessibility(Accessibility.Public)
            .AddParameter("name", "string")
            .CallsThis("name", "0");

        var result = TypeBuilder
            .Class("Person")
            .AddConstructor(constructor)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public Person(string name)");
        await Assert.That(result.Code).Contains(": this(name, 0)");
    }

    [Test]
    public async Task Can_emit_constructor_with_base_chaining()
    {
        var constructor = ConstructorBuilder
            .For("Derived")
            .WithAccessibility(Accessibility.Public)
            .AddParameter("value", "int")
            .CallsBase("value");

        var result = TypeBuilder
            .Class("Derived")
            .AddConstructor(constructor)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public Derived(int value)");
        await Assert.That(result.Code).Contains(": base(value)");
    }

    [Test]
    public async Task Can_emit_private_constructor()
    {
        var constructor = ConstructorBuilder
            .For("Singleton")
            .WithAccessibility(Accessibility.Private)
            .WithBody(b => b.AddStatement("// Private initialization"));

        var result = TypeBuilder
            .Class("Singleton")
            .AddConstructor(constructor)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("private Singleton()");
    }

    [Test]
    public async Task Can_emit_static_constructor()
    {
        var constructor = ConstructorBuilder
            .For("MyClass")
            .AsStatic()
            .WithBody(b => b.AddStatement("_instance = new MyClass();"));

        var result = TypeBuilder
            .Class("MyClass")
            .AddConstructor(constructor)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("static MyClass()");
    }

    [Test]
    public async Task Can_emit_constructor_with_parameter_builder()
    {
        var param = ParameterBuilder
            .For("repository", "IRepository")
            .WithDefaultValue("null");

        var constructor = ConstructorBuilder
            .For("Service")
            .WithAccessibility(Accessibility.Public)
            .AddParameter(param)
            .WithBody(b => b.AddStatement("_repository = repository ?? throw new ArgumentNullException();"));

        var result = TypeBuilder
            .Class("Service")
            .AddConstructor(constructor)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public Service(IRepository repository = null)");
    }

    [Test]
    public async Task Can_emit_constructor_with_lambda_configuration()
    {
        var constructor = ConstructorBuilder
            .For("Entity")
            .WithAccessibility(Accessibility.Public)
            .AddParameter("id", "Guid")
            .WithBody(b => b.AddStatement("Id = id;"));

        var result = TypeBuilder
            .Class("Entity")
            .AddConstructor(constructor)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public Entity(Guid id)");
    }

    [Test]
    public async Task Can_emit_class_with_primary_constructor()
    {
        var result = TypeBuilder
            .Class("Person")
            .WithPrimaryConstructor(c => c
                .AddParameter("name", "string")
                .AddParameter("age", "int"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public class Person(string name, int age)");
    }

    [Test]
    public async Task Can_emit_struct_with_primary_constructor()
    {
        var result = TypeBuilder
            .Struct("Point")
            .WithPrimaryConstructor(c => c
                .AddParameter("x", "double")
                .AddParameter("y", "double"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public struct Point(double x, double y)");
    }

    [Test]
    public async Task Can_emit_primary_constructor_with_default_values()
    {
        var result = TypeBuilder
            .Class("Service")
            .WithPrimaryConstructor(c => c
                .AddParameter("name", "string")
                .AddParameter("timeout", "int", p => p.WithDefaultValue("30")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public class Service(string name, int timeout = 30)");
    }

    [Test]
    public async Task AsPrimary_can_be_used_with_TypeBuilder()
    {
        var constructor = ConstructorBuilder
            .For("Test")
            .AddParameter("value", "string")
            .AsPrimary();

        var result = TypeBuilder
            .Class("Test")
            .WithPrimaryConstructor(constructor)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public class Test(string value)");
    }
}

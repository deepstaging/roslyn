// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit;

namespace Deepstaging.Roslyn.Tests.Emit;

public class ValidSymbolOverloadTests : RoslynTestBase
{
    private const string Source = """
                                  using System;
                                  public class Customer { }
                                  """;

    private ValidSymbol<ITypeSymbol> CustomerType()
        => SymbolsFor(Source).RequireType("Customer");

    private ValidSymbol<INamedTypeSymbol> CustomerNamedType()
        => SymbolsFor(Source).RequireNamedType("Customer");

    [Test]
    public async Task ParameterBuilder_For_uses_globally_qualified_name()
    {
        var param = ParameterBuilder.For("value", CustomerType());
        await Assert.That(param.Type).IsEqualTo("global::Customer");
    }

    [Test]
    public async Task ParameterBuilder_For_accepts_named_type_symbol()
    {
        var param = ParameterBuilder.For("value", CustomerNamedType());
        await Assert.That(param.Type).IsEqualTo("global::Customer");
    }

    [Test]
    public async Task PropertyBuilder_For_uses_globally_qualified_name()
    {
        var prop = PropertyBuilder.For("Value", CustomerType());
        await Assert.That(prop.Type).IsEqualTo("global::Customer");
    }

    [Test]
    public async Task FieldBuilder_For_uses_globally_qualified_name()
    {
        var field = FieldBuilder.For("_value", CustomerType());
        await Assert.That(field.Type).IsEqualTo("global::Customer");
    }

    [Test]
    public async Task EventBuilder_For_uses_globally_qualified_name()
    {
        var type = SymbolsFor("using System; public class Handler : EventArgs { }").RequireType("Handler");
        var evt = EventBuilder.For("Changed", type);
        await Assert.That(evt.Type).IsEqualTo("global::Handler");
    }

    [Test]
    public async Task IndexerBuilder_For_uses_globally_qualified_name()
    {
        var indexer = IndexerBuilder.For(CustomerType());
        await Assert.That(indexer.Type).IsEqualTo("global::Customer");
    }

    [Test]
    public async Task ConstructorBuilder_AddParameter_emits_correct_type()
    {
        var ctor = ConstructorBuilder
            .For("MyClass")
            .AddParameter("value", CustomerNamedType())
            .WithBody(b => b.AddStatement("// init"));

        var result = TypeBuilder
            .Class("MyClass")
            .AddConstructor(ctor)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("global::Customer value");
    }

    [Test]
    public async Task MethodBuilder_AddParameter_emits_correct_type()
    {
        var method = MethodBuilder
            .For("Process")
            .AddParameter("input", CustomerNamedType())
            .WithBody(b => b.AddStatement("// process"));

        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("global::Customer input");
    }

    [Test]
    public async Task MethodBuilder_WithReturnType_emits_correct_type()
    {
        var method = MethodBuilder
            .For("Create")
            .WithReturnType(CustomerNamedType())
            .WithBody(b => b.AddReturn("null!"));

        var result = TypeBuilder
            .Class("MyClass")
            .AddMethod(method)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("global::Customer Create()");
    }

    [Test]
    public async Task TypeBuilder_AddProperty_emits_correct_type()
    {
        var result = TypeBuilder
            .Class("MyClass")
            .AddProperty("Value", CustomerNamedType(), p => p.WithAutoPropertyAccessors())
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("global::Customer Value");
    }

    [Test]
    public async Task TypeBuilder_AddField_emits_correct_type()
    {
        var result = TypeBuilder
            .Class("MyClass")
            .AddField("_value", CustomerNamedType(), f => f.AsReadonly())
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("global::Customer _value");
    }
}
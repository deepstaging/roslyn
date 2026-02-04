// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Projections.Extensions;

public class ValidSymbolTests : RoslynTestBase
{
    [Test]
    public async Task Can_create_from_non_null()
    {
        var code = "public class TestClass { }";
        var type = SymbolsFor(code).RequireNamedType("TestClass").Value;
        
        var valid = ValidSymbol<INamedTypeSymbol>.From(type);

        await Assert.That(valid.HasValue).IsTrue();
        await Assert.That(valid.IsEmpty).IsFalse();
        await Assert.That(valid.Value).IsNotNull();
    }

    [Test]
    public async Task Cannot_create_from_null()
    {
        var action = () => ValidSymbol<INamedTypeSymbol>.From(null!);

        await Assert.That(action).ThrowsException();
    }

    [Test]
    public async Task TryFrom_returns_null_for_null_input()
    {
        var valid = ValidSymbol<INamedTypeSymbol>.TryFrom(null);

        await Assert.That(valid).IsNull();
    }

    [Test]
    public async Task TryFrom_returns_valid_for_non_null()
    {
        var code = "public class TestClass { }";
        var type = SymbolsFor(code).RequireNamedType("TestClass").Value;
        
        var valid = ValidSymbol<INamedTypeSymbol>.TryFrom(type);

        await Assert.That(valid).IsNotNull();
        await Assert.That(valid!.Value.Value).IsNotNull();
    }

    [Test]
    public async Task OrNull_always_returns_value()
    {
        var code = "public class TestClass { }";
        var valid = SymbolsFor(code).RequireNamedType("TestClass");

        await Assert.That(valid.OrNull()).IsNotNull();
    }

    [Test]
    public async Task OrThrow_always_returns_value()
    {
        var code = "public class TestClass { }";
        var valid = SymbolsFor(code).RequireNamedType("TestClass");

        await Assert.That(valid.OrThrow()).IsNotNull();
    }

    [Test]
    public async Task Map_transforms_value()
    {
        var code = "public class TestClass { }";
        var valid = SymbolsFor(code).RequireNamedType("TestClass");
        
        var name = valid.Map(s => s.Name);

        await Assert.That(name).IsEqualTo("TestClass");
    }

    [Test]
    public async Task Where_returns_null_when_predicate_fails()
    {
        var code = "public class TestClass { }";
        var valid = SymbolsFor(code).RequireNamedType("TestClass");
        
        var filtered = valid.Where(s => s.Name == "OtherClass");

        await Assert.That(filtered).IsNull();
    }

    [Test]
    public async Task Where_returns_self_when_predicate_succeeds()
    {
        var code = "public class TestClass { }";
        var valid = SymbolsFor(code).RequireNamedType("TestClass");
        
        var filtered = valid.Where(s => s.Name == "TestClass");

        await Assert.That(filtered).IsNotNull();
        await Assert.That(filtered!.Value.Name).IsEqualTo("TestClass");
    }

    [Test]
    public async Task OfType_casts_to_derived_type()
    {
        var code = "public class TestClass { }";
        var symbol = SymbolsFor(code).RequireNamedType("TestClass").Value;
        
        var asSymbol = ValidSymbol<ISymbol>.From(symbol);
        var asNamed = asSymbol.OfType<INamedTypeSymbol>();

        await Assert.That(asNamed).IsNotNull();
        await Assert.That(asNamed!.Value.Name).IsEqualTo("TestClass");
    }

    [Test]
    public async Task Name_property_returns_symbol_name()
    {
        var code = "public class TestClass { }";
        var valid = SymbolsFor(code).RequireNamedType("TestClass");

        await Assert.That(valid.Name).IsEqualTo("TestClass");
    }

    [Test]
    public async Task FullyQualifiedName_includes_namespace()
    {
        var code = """
            namespace MyApp
            {
                public class TestClass { }
            }
            """;
        
        var valid = SymbolsFor(code).RequireNamedType("TestClass");

        await Assert.That(valid.FullyQualifiedName).Contains("MyApp.TestClass");
    }

    [Test]
    public async Task IsPublic_checks_accessibility()
    {
        var code = """
            public class PublicClass { }
            internal class InternalClass { }
            """;
        
        var context = SymbolsFor(code);

        var publicType = context.RequireNamedType("PublicClass");
        var internalType = context.RequireNamedType("InternalClass");

        await Assert.That(publicType.IsPublic).IsTrue();
        await Assert.That(internalType.IsPublic).IsFalse();
    }

    [Test]
    public async Task IsClass_checks_type_kind()
    {
        var code = """
            public class TestClass { }
            public interface ITestInterface { }
            """;
        
        var context = SymbolsFor(code);

        var classType = context.RequireNamedType("TestClass");
        var interfaceType = context.RequireNamedType("ITestInterface");

        await Assert.That(classType.IsClass).IsTrue();
        await Assert.That(interfaceType.IsClass).IsFalse();
        await Assert.That(interfaceType.IsInterface).IsTrue();
    }

    [Test]
    public async Task IsStatic_checks_static_modifier()
    {
        var code = """
            public static class StaticClass { }
            public class NormalClass { }
            """;
        
        var context = SymbolsFor(code);

        var staticType = context.RequireNamedType("StaticClass");
        var normalType = context.RequireNamedType("NormalClass");

        await Assert.That(staticType.IsStatic).IsTrue();
        await Assert.That(normalType.IsStatic).IsFalse();
    }
}

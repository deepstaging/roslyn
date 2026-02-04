// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Projections.Extensions;

public class OptionalSymbolTests : RoslynTestBase
{
    [Test]
    public async Task Can_create_with_value()
    {
        var code = "public class TestClass { }";
        var type = SymbolsFor(code).RequireNamedType("TestClass").Value;
        
        var optional = OptionalSymbol<INamedTypeSymbol>.WithValue(type);

        await Assert.That(optional.HasValue).IsTrue();
        await Assert.That(optional.IsEmpty).IsFalse();
    }

    [Test]
    public async Task Can_create_empty()
    {
        var optional = OptionalSymbol<INamedTypeSymbol>.Empty();

        await Assert.That(optional.HasValue).IsFalse();
        await Assert.That(optional.IsEmpty).IsTrue();
    }

    [Test]
    public async Task Can_create_from_nullable()
    {
        var code = "public class TestClass { }";
        var type = SymbolsFor(code).RequireNamedType("TestClass").Value;
        
        var withValue = OptionalSymbol<INamedTypeSymbol>.FromNullable(type);
        var withNull = OptionalSymbol<INamedTypeSymbol>.FromNullable(null);

        await Assert.That(withValue.HasValue).IsTrue();
        await Assert.That(withNull.HasValue).IsFalse();
    }

    [Test]
    public async Task OrNull_returns_symbol_or_null()
    {
        var code = "public class TestClass { }";
        var type = SymbolsFor(code).RequireNamedType("TestClass").Value;
        
        var withValue = OptionalSymbol<INamedTypeSymbol>.WithValue(type);
        var empty = OptionalSymbol<INamedTypeSymbol>.Empty();

        await Assert.That(withValue.OrNull()).IsNotNull();
        await Assert.That(empty.OrNull()).IsNull();
    }

    [Test]
    public async Task OrThrow_returns_symbol_or_throws()
    {
        var code = "public class TestClass { }";
        var type = SymbolsFor(code).RequireNamedType("TestClass").Value;
        
        var withValue = OptionalSymbol<INamedTypeSymbol>.WithValue(type);
        var empty = OptionalSymbol<INamedTypeSymbol>.Empty();

        await Assert.That(withValue.OrThrow()).IsNotNull();
        await Assert.That(() => empty.OrThrow()).ThrowsException();
    }

    [Test]
    public async Task Validate_returns_valid_symbol_or_null()
    {
        var code = "public class TestClass { }";
        var type = SymbolsFor(code).RequireNamedType("TestClass").Value;
        
        var withValue = OptionalSymbol<INamedTypeSymbol>.WithValue(type);
        var empty = OptionalSymbol<INamedTypeSymbol>.Empty();

        await Assert.That(withValue.Validate()).IsNotNull();
        await Assert.That(empty.Validate()).IsNull();
    }

    [Test]
    public async Task ValidateOrThrow_returns_valid_or_throws()
    {
        var code = "public class TestClass { }";
        var type = SymbolsFor(code).RequireNamedType("TestClass").Value;
        
        var withValue = OptionalSymbol<INamedTypeSymbol>.WithValue(type);
        var empty = OptionalSymbol<INamedTypeSymbol>.Empty();

        await Assert.That(withValue.ValidateOrThrow().Value).IsNotNull();
        await Assert.That(() => empty.ValidateOrThrow()).ThrowsException();
    }

    [Test]
    public async Task Map_transforms_value()
    {
        var code = "public class TestClass { }";
        var type = SymbolsFor(code).RequireNamedType("TestClass").Value;
        
        var optional = OptionalSymbol<INamedTypeSymbol>.WithValue(type);
        var mapped = optional.Map(s => s.Value.Name);

        await Assert.That(mapped.HasValue).IsTrue();
        await Assert.That(mapped.OrDefault("")).IsEqualTo("TestClass");
    }

    [Test]
    public async Task Where_filters_value()
    {
        var code = """
            public class PublicClass { }
            internal class InternalClass { }
            """;
        
        var publicType = SymbolsFor(code).GetNamedType("PublicClass");
        var filteredPublic = publicType.Where(t => t.DeclaredAccessibility == Accessibility.Public);
        var filteredInternal = publicType.Where(t => t.DeclaredAccessibility == Accessibility.Internal);

        await Assert.That(filteredPublic.HasValue).IsTrue();
        await Assert.That(filteredInternal.HasValue).IsFalse();
    }

    [Test]
    public async Task OfType_casts_to_derived_type()
    {
        var code = "public class TestClass { }";
        var symbol = SymbolsFor(code).RequireNamedType("TestClass").Value;
        
        var asType = OptionalSymbol<ISymbol>.WithValue(symbol);
        var asNamed = asType.OfType<INamedTypeSymbol>();

        await Assert.That(asNamed.HasValue).IsTrue();
        await Assert.That(asNamed.OrNull()?.Name).IsEqualTo("TestClass");
    }

    [Test]
    public async Task Name_property_returns_symbol_name()
    {
        var code = "public class TestClass { }";
        var optional = SymbolsFor(code).GetNamedType("TestClass");

        await Assert.That(optional.Name).IsEqualTo("TestClass");
    }

    [Test]
    public async Task IsPublic_checks_accessibility()
    {
        var code = """
            public class PublicClass { }
            internal class InternalClass { }
            """;
        
        var context = SymbolsFor(code);

        var publicType = context.GetNamedType("PublicClass");
        var internalType = context.GetNamedType("InternalClass");

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

        var classType = context.GetNamedType("TestClass");
        var interfaceType = context.GetNamedType("ITestInterface");

        await Assert.That(classType.IsClass).IsTrue();
        await Assert.That(interfaceType.IsClass).IsFalse();
        await Assert.That(interfaceType.IsInterface).IsTrue();
    }
}

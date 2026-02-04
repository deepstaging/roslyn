// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Projections.Extensions;

public class OptionalNamedTypeSymbolTests : RoslynTestBase
{
    [Test]
    public async Task BaseType_returns_base_type()
    {
        var code = """
            public class BaseClass { }
            public class DerivedClass : BaseClass { }
            """;

        var derived = SymbolsFor(code).GetNamedType("DerivedClass");
        var baseType = derived.BaseType;

        await Assert.That(baseType.HasValue).IsTrue();
        await Assert.That(baseType.Name).IsEqualTo("BaseClass");
    }

    [Test]
    public async Task BaseType_returns_empty_for_object()
    {
        var code = "public class TestClass { }";

        var type = SymbolsFor(code).GetNamedType("TestClass");
        // TestClass inherits from object, which has no base type
        var objectBase = type.BaseType.BaseType;

        await Assert.That(objectBase.IsEmpty).IsTrue();
    }

    [Test]
    public async Task BaseType_returns_empty_for_empty_optional()
    {
        var empty = OptionalSymbol<INamedTypeSymbol>.Empty();

        await Assert.That(empty.BaseType.IsEmpty).IsTrue();
    }

    [Test]
    public async Task GetBaseTypes_returns_inheritance_hierarchy()
    {
        var code = """
            public class GrandParent { }
            public class Parent : GrandParent { }
            public class Child : Parent { }
            """;

        var child = SymbolsFor(code).GetNamedType("Child");
        var baseTypes = child.GetBaseTypes().Select(t => t.Name).ToList();

        await Assert.That(baseTypes).Contains("Parent");
        await Assert.That(baseTypes).Contains("GrandParent");
    }

    [Test]
    public async Task GetBaseTypes_returns_empty_for_empty_optional()
    {
        var empty = OptionalSymbol<INamedTypeSymbol>.Empty();
        var baseTypes = empty.GetBaseTypes().ToList();

        await Assert.That(baseTypes).IsEmpty();
    }

    [Test]
    public async Task GetInterfaces_returns_direct_interfaces()
    {
        var code = """
            public interface IFirst { }
            public interface ISecond { }
            public class TestClass : IFirst, ISecond { }
            """;

        var type = SymbolsFor(code).GetNamedType("TestClass");
        var interfaces = type.GetInterfaces().Select(i => i.Name).ToList();

        await Assert.That(interfaces).Contains("IFirst");
        await Assert.That(interfaces).Contains("ISecond");
    }

    [Test]
    public async Task GetInterfaces_returns_empty_for_empty_optional()
    {
        var empty = OptionalSymbol<INamedTypeSymbol>.Empty();
        var interfaces = empty.GetInterfaces().ToList();

        await Assert.That(interfaces).IsEmpty();
    }

    [Test]
    public async Task GetAllInterfaces_includes_inherited_interfaces()
    {
        var code = """
            public interface IBase { }
            public interface IDerived : IBase { }
            public class TestClass : IDerived { }
            """;

        var type = SymbolsFor(code).GetNamedType("TestClass");
        var allInterfaces = type.GetAllInterfaces().Select(i => i.Name).ToList();

        await Assert.That(allInterfaces).Contains("IDerived");
        await Assert.That(allInterfaces).Contains("IBase");
    }

    [Test]
    public async Task GetAllInterfaces_returns_empty_for_empty_optional()
    {
        var empty = OptionalSymbol<INamedTypeSymbol>.Empty();
        var interfaces = empty.GetAllInterfaces().ToList();

        await Assert.That(interfaces).IsEmpty();
    }

    [Test]
    public async Task ImplementsInterface_returns_true_for_direct_interface()
    {
        var code = """
            public interface IMyInterface { }
            public class TestClass : IMyInterface { }
            """;

        var type = SymbolsFor(code).GetNamedType("TestClass");

        await Assert.That(type.ImplementsInterface("IMyInterface")).IsTrue();
        await Assert.That(type.ImplementsInterface("IUnknown")).IsFalse();
    }

    [Test]
    public async Task ImplementsInterface_returns_true_for_inherited_interface()
    {
        var code = """
            public interface IBase { }
            public interface IDerived : IBase { }
            public class TestClass : IDerived { }
            """;

        var type = SymbolsFor(code).GetNamedType("TestClass");

        await Assert.That(type.ImplementsInterface("IBase")).IsTrue();
        await Assert.That(type.ImplementsInterface("IDerived")).IsTrue();
    }

    [Test]
    public async Task ImplementsInterface_returns_false_for_empty_optional()
    {
        var empty = OptionalSymbol<INamedTypeSymbol>.Empty();

        await Assert.That(empty.ImplementsInterface("IMyInterface")).IsFalse();
    }

    [Test]
    public async Task InheritsFrom_returns_true_for_direct_base()
    {
        var code = """
            public class BaseClass { }
            public class DerivedClass : BaseClass { }
            """;

        var derived = SymbolsFor(code).GetNamedType("DerivedClass");

        await Assert.That(derived.InheritsFrom("BaseClass")).IsTrue();
        await Assert.That(derived.InheritsFrom("UnknownClass")).IsFalse();
    }

    [Test]
    public async Task InheritsFrom_returns_true_for_ancestor()
    {
        var code = """
            public class GrandParent { }
            public class Parent : GrandParent { }
            public class Child : Parent { }
            """;

        var child = SymbolsFor(code).GetNamedType("Child");

        await Assert.That(child.InheritsFrom("Parent")).IsTrue();
        await Assert.That(child.InheritsFrom("GrandParent")).IsTrue();
    }

    [Test]
    public async Task InheritsFrom_returns_false_for_self()
    {
        var code = "public class TestClass { }";

        var type = SymbolsFor(code).GetNamedType("TestClass");

        await Assert.That(type.InheritsFrom("TestClass")).IsFalse();
    }

    [Test]
    public async Task InheritsFrom_returns_false_for_empty_optional()
    {
        var empty = OptionalSymbol<INamedTypeSymbol>.Empty();

        await Assert.That(empty.InheritsFrom("BaseClass")).IsFalse();
    }
}

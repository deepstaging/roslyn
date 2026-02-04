// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Projections.Extensions;

public class ValidSymbolNamespaceTests : RoslynTestBase
{
    [Test]
    public async Task Can_get_namespace_by_name()
    {
        var code = """
            namespace MyNamespace
            {
                public class MyClass { }
            }
            """;

        var context = SymbolsFor(code);
        var ns = context.RequireNamespace("MyNamespace");

        await Assert.That(ns.HasValue).IsTrue();
        await Assert.That(ns.Name).IsEqualTo("MyNamespace");
    }

    [Test]
    public async Task Can_require_namespace_by_name()
    {
        var code = """
            namespace MyNamespace
            {
                public class MyClass { }
            }
            """;

        var context = SymbolsFor(code);
        var ns = context.RequireNamespace("MyNamespace");

        await Assert.That(ns.Name).IsEqualTo("MyNamespace");
    }

    [Test]
    public async Task Can_get_types_from_namespace()
    {
        var code = """
            namespace MyNamespace
            {
                public class ClassA { }
                public class ClassB { }
            }
            """;

        var context = SymbolsFor(code);
        var ns = context.RequireNamespace("MyNamespace");
        var types = ns.GetTypes().ToList();

        await Assert.That(types.Count).IsEqualTo(2);
        await Assert.That(types.Any(t => t.Name == "ClassA")).IsTrue();
        await Assert.That(types.Any(t => t.Name == "ClassB")).IsTrue();
    }

    [Test]
    public async Task Can_get_named_type_from_namespace()
    {
        var code = """
            namespace MyNamespace
            {
                public class MyClass { }
            }
            """;

        var context = SymbolsFor(code);
        var ns = context.RequireNamespace("MyNamespace");
        var type = ns.GetNamedType("MyClass");

        await Assert.That(type.HasValue).IsTrue();
        await Assert.That(type.Name).IsEqualTo("MyClass");
    }

    [Test]
    public async Task Can_require_named_type_from_namespace()
    {
        var code = """
            namespace MyNamespace
            {
                public class MyClass { }
            }
            """;

        var context = SymbolsFor(code);
        var ns = context.RequireNamespace("MyNamespace");
        var type = ns.RequireNamedType("MyClass");

        await Assert.That(type.Name).IsEqualTo("MyClass");
    }

    [Test]
    public async Task Can_get_nested_namespaces()
    {
        var code = """
            namespace Parent
            {
                namespace Child
                {
                    public class MyClass { }
                }
            }
            """;

        var context = SymbolsFor(code);
        var parent = context.RequireNamespace("Parent");
        var childNamespaces = parent.GetNamespaces().ToList();

        await Assert.That(childNamespaces.Count).IsEqualTo(1);
        await Assert.That(childNamespaces[0].Name).IsEqualTo("Child");
    }

    [Test]
    public async Task Can_navigate_nested_namespaces()
    {
        var code = """
            namespace Parent.Child
            {
                public class MyClass { }
            }
            """;

        var context = SymbolsFor(code);
        var parent = context.RequireNamespace("Parent");
        var child = parent.RequireNamespace("Child");
        var type = child.RequireNamedType("MyClass");

        await Assert.That(type.Name).IsEqualTo("MyClass");
    }

    [Test]
    public async Task IsGlobalNamespace_returns_false_for_regular_namespace()
    {
        var code = """
            namespace MyNamespace
            {
                public class MyClass { }
            }
            """;

        var context = SymbolsFor(code);
        var ns = context.RequireNamespace("MyNamespace");

        await Assert.That(ns.IsGlobalNamespace()).IsFalse();
    }

    [Test]
    public async Task Can_query_types_from_namespace()
    {
        var code = """
            namespace MyNamespace
            {
                public class PublicClass { }
                internal class InternalClass { }
            }
            """;

        var context = SymbolsFor(code);
        var ns = context.RequireNamespace("MyNamespace");
        var type = ns.RequireNamedType("PublicClass");
        var methods = type.QueryMethods().GetAll().ToList();

        await Assert.That(type.Name).IsEqualTo("PublicClass");
    }
}

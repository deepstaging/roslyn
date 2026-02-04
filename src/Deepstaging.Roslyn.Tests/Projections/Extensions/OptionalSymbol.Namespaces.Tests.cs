// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Projections.Extensions;

public class OptionalSymbolNamespaceTests : RoslynTestBase
{
    [Test]
    public async Task GetTypes_returns_types_from_namespace()
    {
        var code = """
            namespace MyNamespace
            {
                public class ClassA { }
                public class ClassB { }
            }
            """;

        var context = SymbolsFor(code);
        var ns = context.GetNamespace("MyNamespace");
        var types = ns.GetTypes().ToList();

        await Assert.That(types.Count).IsEqualTo(2);
        await Assert.That(types.All(t => t.HasValue)).IsTrue();
        await Assert.That(types.Any(t => t.Name == "ClassA")).IsTrue();
        await Assert.That(types.Any(t => t.Name == "ClassB")).IsTrue();
    }

    [Test]
    public async Task GetTypes_returns_empty_for_empty_namespace()
    {
        var code = """
            public class MyClass { }
            """;

        var context = SymbolsFor(code);
        var ns = context.GetNamespace("NonExistent");
        var types = ns.GetTypes().ToList();

        await Assert.That(types.Count).IsEqualTo(0);
    }

    [Test]
    public async Task GetNamedType_returns_type_from_namespace()
    {
        var code = """
            namespace MyNamespace
            {
                public class MyClass { }
            }
            """;

        var context = SymbolsFor(code);
        var ns = context.GetNamespace("MyNamespace");
        var type = ns.GetNamedType("MyClass");

        await Assert.That(type.HasValue).IsTrue();
        await Assert.That(type.Name).IsEqualTo("MyClass");
    }

    [Test]
    public async Task GetNamedType_returns_empty_when_namespace_is_empty()
    {
        var code = """
            public class MyClass { }
            """;

        var context = SymbolsFor(code);
        var ns = context.GetNamespace("NonExistent");
        var type = ns.GetNamedType("MyClass");

        await Assert.That(type.HasValue).IsFalse();
    }

    [Test]
    public async Task GetNamedType_returns_empty_when_type_not_found()
    {
        var code = """
            namespace MyNamespace
            {
                public class MyClass { }
            }
            """;

        var context = SymbolsFor(code);
        var ns = context.GetNamespace("MyNamespace");
        var type = ns.GetNamedType("NonExistent");

        await Assert.That(type.HasValue).IsFalse();
    }

    [Test]
    public async Task RequireNamedType_returns_type_from_namespace()
    {
        var code = """
            namespace MyNamespace
            {
                public class MyClass { }
            }
            """;

        var context = SymbolsFor(code);
        var ns = context.GetNamespace("MyNamespace");
        var type = ns.RequireNamedType("MyClass");

        await Assert.That(type.Name).IsEqualTo("MyClass");
    }

    [Test]
    public async Task GetNamespaces_returns_child_namespaces()
    {
        var code = """
            namespace Parent
            {
                namespace Child1 { }
                namespace Child2 { }
            }
            """;

        var context = SymbolsFor(code);
        var parent = context.GetNamespace("Parent");
        var children = parent.GetNamespaces().ToList();

        await Assert.That(children.Count).IsEqualTo(2);
        await Assert.That(children.All(c => c.HasValue)).IsTrue();
        await Assert.That(children.Any(c => c.Name == "Child1")).IsTrue();
        await Assert.That(children.Any(c => c.Name == "Child2")).IsTrue();
    }

    [Test]
    public async Task GetNamespaces_returns_empty_for_empty_namespace()
    {
        var code = """
            public class MyClass { }
            """;

        var context = SymbolsFor(code);
        var ns = context.GetNamespace("NonExistent");
        var children = ns.GetNamespaces().ToList();

        await Assert.That(children.Count).IsEqualTo(0);
    }

    [Test]
    public async Task GetNamespace_returns_child_namespace()
    {
        var code = """
            namespace Parent.Child
            {
                public class MyClass { }
            }
            """;

        var context = SymbolsFor(code);
        var parent = context.GetNamespace("Parent");
        var child = parent.GetNamespace("Child");

        await Assert.That(child.HasValue).IsTrue();
        await Assert.That(child.Name).IsEqualTo("Child");
    }

    [Test]
    public async Task GetNamespace_returns_empty_when_parent_is_empty()
    {
        var code = """
            public class MyClass { }
            """;

        var context = SymbolsFor(code);
        var parent = context.GetNamespace("NonExistent");
        var child = parent.GetNamespace("Child");

        await Assert.That(child.HasValue).IsFalse();
    }

    [Test]
    public async Task GetNamespace_returns_empty_when_child_not_found()
    {
        var code = """
            namespace Parent
            {
                public class MyClass { }
            }
            """;

        var context = SymbolsFor(code);
        var parent = context.GetNamespace("Parent");
        var child = parent.GetNamespace("NonExistent");

        await Assert.That(child.HasValue).IsFalse();
    }

    [Test]
    public async Task RequireNamespace_returns_child_namespace()
    {
        var code = """
            namespace Parent.Child
            {
                public class MyClass { }
            }
            """;

        var context = SymbolsFor(code);
        var parent = context.GetNamespace("Parent");
        var child = parent.RequireNamespace("Child");

        await Assert.That(child.Name).IsEqualTo("Child");
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
        var ns = context.GetNamespace("MyNamespace");

        await Assert.That(ns.IsGlobalNamespace()).IsFalse();
    }

    [Test]
    public async Task IsGlobalNamespace_returns_false_for_empty()
    {
        var code = """
            public class MyClass { }
            """;

        var context = SymbolsFor(code);
        var ns = context.GetNamespace("NonExistent");

        await Assert.That(ns.IsGlobalNamespace()).IsFalse();
    }

    [Test]
    public async Task Can_chain_namespace_navigation()
    {
        var code = """
            namespace Parent.Child.GrandChild
            {
                public class MyClass { }
            }
            """;

        var context = SymbolsFor(code);
        var grandChild = context.GetNamespace("Parent")
            .GetNamespace("Child")
            .GetNamespace("GrandChild");
        var type = grandChild.GetNamedType("MyClass");

        await Assert.That(grandChild.HasValue).IsTrue();
        await Assert.That(type.HasValue).IsTrue();
        await Assert.That(type.Name).IsEqualTo("MyClass");
    }

    [Test]
    public async Task Chained_navigation_stops_at_first_empty()
    {
        var code = """
            namespace Parent
            {
                public class MyClass { }
            }
            """;

        var context = SymbolsFor(code);
        var result = context.GetNamespace("Parent")
            .GetNamespace("NonExistent")
            .GetNamespace("AlsoNonExistent");

        await Assert.That(result.HasValue).IsFalse();
    }
}

// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Extensions.Roslyn;

public class ISymbolTests : RoslynTestBase
{
    [Test]
    public async Task Can_check_if_symbol_is_public()
    {
        var code = """
            public class PublicClass { }
            internal class InternalClass { }
            """;

        var context = SymbolsFor(code);

        var publicClass = context
            .RequireNamedType("PublicClass")
            .Value;
        
        var internalClass = context
            .RequireNamedType("InternalClass")
            .Value;

        await Assert.That(publicClass.IsPublic()).IsTrue();
        await Assert.That(internalClass.IsPublic()).IsFalse();
    }

    [Test]
    public async Task Can_check_if_symbol_is_static()
    {
        var code = """
            public class TestClass
            {
                public static void StaticMethod() { }
                public void InstanceMethod() { }
            }
            """;

        var context = SymbolsFor(code);

        var staticMethod = context
            .RequireNamedType("TestClass")
            .QueryMethods()
            .WithName("StaticMethod")
            .First()
            .Value;
        
        var instanceMethod = context
            .RequireNamedType("TestClass")
            .QueryMethods()
            .WithName("InstanceMethod")
            .First()
            .Value;

        await Assert.That(staticMethod.IsStatic()).IsTrue();
        await Assert.That(instanceMethod.IsStatic()).IsFalse();
    }

    [Test]
    public async Task Can_get_attributes_by_name()
    {
        var code = """
            using System;
            
            [Obsolete("Old class")]
            public class TestClass { }
            """;

        var type = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .Value;
        
        var attributes = type.GetAttributesByName("Obsolete").ToList();

        await Assert.That(attributes.Count).IsEqualTo(1);
    }

    [Test]
    public async Task Can_check_if_symbol_is_obsolete()
    {
        var code = """
            using System;
            
            [Obsolete]
            public class OldClass { }
            public class NewClass { }
            """;

        var context = SymbolsFor(code);

        var oldClass = context
            .RequireNamedType("OldClass")
            .Value;
        
        var newClass = context
            .RequireNamedType("NewClass")
            .Value;

        await Assert.That(oldClass.IsObsolete()).IsTrue();
        await Assert.That(newClass.IsObsolete()).IsFalse();
    }
}

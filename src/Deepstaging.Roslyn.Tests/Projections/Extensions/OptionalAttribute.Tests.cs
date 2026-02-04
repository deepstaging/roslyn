// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Projections.Extensions;

public class OptionalAttributeTests : RoslynTestBase
{
    [Test]
    public async Task Can_create_with_value()
    {
        var code = """
            using System;
            
            [Obsolete("Old class")]
            public class TestClass { }
            """;

        var type = SymbolsFor(code).RequireNamedType("TestClass").Value;
        var attributeData = type.GetAttributes().First();
        
        var optional = OptionalAttribute.WithValue(attributeData);

        await Assert.That(optional.HasValue).IsTrue();
        await Assert.That(optional.IsEmpty).IsFalse();
    }

    [Test]
    public async Task Can_create_empty()
    {
        var optional = OptionalAttribute.Empty();

        await Assert.That(optional.HasValue).IsFalse();
        await Assert.That(optional.IsEmpty).IsTrue();
    }

    [Test]
    public async Task OrNull_returns_attribute_or_null()
    {
        var code = """
            using System;
            
            [Obsolete]
            public class TestClass { }
            """;

        var type = SymbolsFor(code).RequireNamedType("TestClass").Value;
        var attributeData = type.GetAttributes().First();
        
        var withValue = OptionalAttribute.WithValue(attributeData);
        var empty = OptionalAttribute.Empty();

        await Assert.That(withValue.OrNull()).IsNotNull();
        await Assert.That(empty.OrNull()).IsNull();
    }

    [Test]
    public async Task Name_returns_attribute_class_name()
    {
        var code = """
            using System;
            
            [Obsolete]
            public class TestClass { }
            """;

        var type = SymbolsFor(code).RequireNamedType("TestClass").Value;
        var attributeData = type.GetAttributes().First();
        var attribute = OptionalAttribute.WithValue(attributeData);

        await Assert.That(attribute.AttributeClass.Name).IsEqualTo("ObsoleteAttribute");
    }

    [Test]
    public async Task HasConstructorArguments_detects_arguments()
    {
        var code = """
            using System;
            
            [Obsolete("Message")]
            public class WithArgs { }
            
            [Obsolete]
            public class WithoutArgs { }
            """;

        var context = SymbolsFor(code);

        var withArgsData = context.RequireNamedType("WithArgs").Value.GetAttributes().First();
        var withoutArgsData = context.RequireNamedType("WithoutArgs").Value.GetAttributes().First();
        var withArgs = OptionalAttribute.WithValue(withArgsData);
        var withoutArgs = OptionalAttribute.WithValue(withoutArgsData);

        await Assert.That(withArgs.Value.ConstructorArguments.Length > 0).IsTrue();
        await Assert.That(withoutArgs.Value.ConstructorArguments.Length == 0).IsTrue();
    }

    [Test]
    public async Task GetConstructorArguments_returns_arguments()
    {
        var code = """
            using System;
            
            [Obsolete("Old message", true)]
            public class TestClass { }
            """;

        var attributeData = SymbolsFor(code).RequireNamedType("TestClass").Value.GetAttributes().First();
        var attribute = OptionalAttribute.WithValue(attributeData);

        await Assert.That(attribute.Value.ConstructorArguments.Length).IsEqualTo(2);
    }

    [Test]
    public async Task GetFirstConstructorArgument_returns_first_arg()
    {
        var code = """
            using System;
            
            [Obsolete("Message")]
            public class TestClass { }
            """;

        var attributeData = SymbolsFor(code).RequireNamedType("TestClass").Value.GetAttributes().First();
        var attribute = OptionalAttribute.WithValue(attributeData);
        var firstArg = attribute.ConstructorArg<string>(0);

        await Assert.That(firstArg.HasValue).IsTrue();
    }
}

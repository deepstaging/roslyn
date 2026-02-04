// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Projections.Extensions;

public class OptionalSymbolPropertiesTests : RoslynTestBase
{
    [Test]
    public async Task GetReturnType_returns_property_type()
    {
        var code = """
            public class TestClass
            {
                public int Number { get; set; }
                public string Text { get; set; }
            }
            """;

        var numberProp = SymbolsFor(code)
            .GetNamedType("TestClass")
            .QueryProperties()
            .WithName("Number")
            .FirstOrDefault();

        var returnType = numberProp.ReturnType;

        await Assert.That(returnType.HasValue).IsTrue();
        await Assert.That(returnType.Name).IsEqualTo("Int32");
    }

    [Test]
    public async Task GetGetMethod_returns_getter()
    {
        var code = """
            public class TestClass
            {
                public int ReadWrite { get; set; }
                public int ReadOnly { get; }
            }
            """;

        var context = SymbolsFor(code);

        var readWrite = context.GetNamedType("TestClass").QueryProperties().WithName("ReadWrite").FirstOrDefault();
        var readOnly = context.GetNamedType("TestClass").QueryProperties().WithName("ReadOnly").FirstOrDefault();

        await Assert.That(readWrite.GetMethod.HasValue).IsTrue();
        await Assert.That(readOnly.GetMethod.HasValue).IsTrue();
    }

    [Test]
    public async Task GetSetMethod_returns_setter_or_empty()
    {
        var code = """
            public class TestClass
            {
                public int ReadWrite { get; set; }
                public int ReadOnly { get; }
            }
            """;

        var context = SymbolsFor(code);

        var readWrite = context.GetNamedType("TestClass").QueryProperties().WithName("ReadWrite").FirstOrDefault();
        var readOnly = context.GetNamedType("TestClass").QueryProperties().WithName("ReadOnly").FirstOrDefault();

        await Assert.That(readWrite.SetMethod.HasValue).IsTrue();
        await Assert.That(readOnly.SetMethod.HasValue).IsFalse();
    }

    [Test]
    public async Task GetOverriddenProperty_returns_base_property()
    {
        var code = """
            public class BaseClass
            {
                public virtual int Property { get; set; }
            }
            
            public class DerivedClass : BaseClass
            {
                public override int Property { get; set; }
            }
            """;

        var overrideProperty = SymbolsFor(code)
            .GetNamedType("DerivedClass")
            .QueryProperties()
            .WithName("Property")
            .FirstOrDefault();

        var overridden = overrideProperty.OverriddenProperty;

        await Assert.That(overridden.HasValue).IsTrue();
    }
}

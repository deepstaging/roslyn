// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests;

public class PropertyQueryTests : RoslynTestBase
{
    [Test]
    public async Task Can_filter_public_properties()
    {
        var code = """
                   public class TestClass
                   {
                       public int PublicProp { get; set; }
                       private int PrivateProp { get; set; }
                   }
                   """;

        var properties = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryProperties()
            .ThatArePublic()
            .GetAll();

        await Assert.That(properties.Any(p => p.Value.Name == "PublicProp")).IsTrue();
    }

    [Test]
    public async Task Can_filter_readonly_properties()
    {
        var code = """
                   public class TestClass
                   {
                       public int ReadOnlyProp { get; }
                       public int ReadWriteProp { get; set; }
                   }
                   """;

        var readonlyProps = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryProperties()
            .ThatAreReadOnly()
            .GetAll();

        await Assert.That(readonlyProps.Any(p => p.Value.Name == "ReadOnlyProp")).IsTrue();
    }

    [Test]
    public async Task Can_filter_readable_properties()
    {
        var code = """
                   public class TestClass
                   {
                       public int ReadableProp { get; set; }
                       public int WriteOnlyProp { set { } }
                   }
                   """;

        var readableProps = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryProperties()
            .ThatAreReadable()
            .GetAll();

        await Assert.That(readableProps).Count().IsEqualTo(1);
        await Assert.That(readableProps[0].Value.Name).IsEqualTo("ReadableProp");
    }

    [Test]
    public async Task Can_filter_writable_properties()
    {
        var code = """
                   public class TestClass
                   {
                       public int WritableProp { get; set; }
                       public int InitOnlyProp { get; init; }
                       public int ReadOnlyProp { get; }
                   }
                   """;

        var writableProps = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryProperties()
            .ThatAreWritable()
            .GetAll();

        await Assert.That(writableProps).Count().IsEqualTo(2);
        await Assert.That(writableProps.Any(p => p.Value.Name == "WritableProp")).IsTrue();
        await Assert.That(writableProps.Any(p => p.Value.Name == "InitOnlyProp")).IsTrue();
    }
}
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
}

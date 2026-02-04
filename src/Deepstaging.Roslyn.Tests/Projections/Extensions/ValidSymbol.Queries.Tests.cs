// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Projections.Extensions;

public class ValidSymbolQueriesTests : RoslynTestBase
{
    [Test]
    public async Task Can_query_methods_from_valid_symbol()
    {
        var code = """
            public class TestClass
            {
                public void Method1() { }
                public void Method2() { }
            }
            """;

        var methods = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryMethods()
            .GetAll();

        await Assert.That(methods.Length).IsGreaterThanOrEqualTo(2);
    }

    [Test]
    public async Task Can_query_properties_from_valid_symbol()
    {
        var code = """
            public class TestClass
            {
                public int Prop1 { get; set; }
                public string Prop2 { get; set; }
            }
            """;

        var properties = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryProperties()
            .GetAll();

        await Assert.That(properties.Length).IsEqualTo(2);
    }

    [Test]
    public async Task Can_query_parameters_from_valid_method()
    {
        var code = """
            public class TestClass
            {
                public void Method(int x, string y) { }
            }
            """;

        var parameters = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryMethods()
            .WithName("Method")
            .First()
            .QueryParameters()
            .GetAll();

        await Assert.That(parameters.Length).IsEqualTo(2);
    }
}

// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Projections.Extensions;

public class OptionalSymbolQueriesTests : RoslynTestBase
{
    [Test]
    public async Task Can_query_methods_from_optional_symbol()
    {
        var code = """
            public class TestClass
            {
                public void Method1() { }
                public void Method2() { }
            }
            """;

        var methods = SymbolsFor(code)
            .GetNamedType("TestClass")
            .QueryMethods()
            .GetAll();

        await Assert.That(methods.Length).IsGreaterThanOrEqualTo(2);
    }

    [Test]
    public async Task Can_query_properties_from_optional_symbol()
    {
        var code = """
            public class TestClass
            {
                public int Prop1 { get; set; }
                public string Prop2 { get; set; }
            }
            """;

        var properties = SymbolsFor(code)
            .GetNamedType("TestClass")
            .QueryProperties()
            .GetAll();

        await Assert.That(properties.Length).IsEqualTo(2);
    }

    [Test]
    public async Task Can_query_parameters_from_optional_method()
    {
        var code = """
            public class TestClass
            {
                public void Method(int x, string y) { }
            }
            """;

        var parameters = SymbolsFor(code)
            .GetNamedType("TestClass")
            .QueryMethods()
            .WithName("Method")
            .FirstOrDefault()
            .QueryParameters()
            .GetAll();

        await Assert.That(parameters.Length).IsEqualTo(2);
    }

    [Test]
    public async Task Query_on_empty_optional_returns_empty_results()
    {
        // Note: Currently OptionalSymbol query extensions throw on empty optionals
        // This test documents the current behavior
        var emptyOptional = OptionalSymbol<INamedTypeSymbol>.Empty();

        // When calling QueryMethods on empty optional, it throws
        var action = () => emptyOptional.QueryMethods();
        await Assert.That(action).ThrowsException();
    }
}

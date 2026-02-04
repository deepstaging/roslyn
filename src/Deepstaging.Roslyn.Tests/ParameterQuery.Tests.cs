// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests;

public class ParameterQueryTests : RoslynTestBase
{
    [Test]
    public async Task Can_query_method_parameters()
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

    [Test]
    public async Task Can_filter_ref_parameters()
    {
        var code = """
            public class TestClass
            {
                public void Method(ref int x, int y) { }
            }
            """;

        var refParams = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryMethods()
            .WithName("Method")
            .First()
            .QueryParameters()
            .ThatAreRef()
            .GetAll();

        await Assert.That(refParams.Any(p => p.Value.Name == "x")).IsTrue();
    }
}

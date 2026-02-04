// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests;

public class ConstructorQueryTests : RoslynTestBase
{
    [Test]
    public async Task Can_find_public_constructors()
    {
        var code = """
                   public class TestClass
                   {
                       public TestClass() { }
                       private TestClass(int x) { }
                   }
                   """;

        var constructors = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryConstructors()
            .ThatArePublic()
            .GetAll();

        await Assert.That(constructors.Length).IsEqualTo(1);
    }

    [Test]
    public async Task Can_filter_by_parameter_count()
    {
        var code = """
                   public class TestClass
                   {
                       public TestClass() { }
                       public TestClass(int x) { }
                   }
                   """;

        var parameterlessCtors = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryConstructors()
            .WithParameterCount(0)
            .GetAll();

        await Assert.That(parameterlessCtors.Length).IsEqualTo(1);
    }
}
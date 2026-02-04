// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests;

public class MethodQueryTests : RoslynTestBase
{
    [Test]
    public async Task Can_filter_public_methods()
    {
        var code = """
            public class TestClass
            {
                public void PublicMethod() { }
                private void PrivateMethod() { }
            }
            """;

        var methods = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryMethods()
            .ThatArePublic()
            .GetAll();

        await Assert.That(methods.Any(m => m.Value.Name == "PublicMethod")).IsTrue();
    }

    [Test]
    public async Task Can_filter_async_methods()
    {
        var code = """
            using System.Threading.Tasks;
            
            public class TestClass
            {
                public async Task AsyncMethod() => await Task.CompletedTask;
                public void SyncMethod() { }
            }
            """;

        var asyncMethods = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryMethods()
            .ThatAreAsync()
            .GetAll();

        await Assert.That(asyncMethods.Any(m => m.Value.Name == "AsyncMethod")).IsTrue();
    }

    [Test]
    public async Task Can_filter_static_methods()
    {
        var code = """
            public class TestClass
            {
                public static void StaticMethod() { }
                public void InstanceMethod() { }
            }
            """;

        var staticMethods = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryMethods()
            .ThatAreStatic()
            .GetAll();

        await Assert.That(staticMethods.Any(m => m.Value.Name == "StaticMethod")).IsTrue();
    }
}

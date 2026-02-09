// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests;

public class MethodQueryTests : RoslynTestBase
{
    #region Accessibility Filtering

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

    #endregion

    #region Modifier Filtering

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

    [Test]
    public async Task Can_filter_partial_methods()
    {
        var code = """
                   public partial class TestClass
                   {
                       partial void PartialMethod();
                       public void RegularMethod() { }
                   }
                   
                   public partial class TestClass
                   {
                       partial void PartialMethod() { }
                   }
                   """;

        var partialMethods = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryMethods()
            .ThatArePartial()
            .GetAll();

        // Roslyn merges partial definition and implementation into one symbol
        await Assert.That(partialMethods.Count()).IsEqualTo(1);
        await Assert.That(partialMethods[0].Value.Name).IsEqualTo("PartialMethod");
    }

    [Test]
    public async Task Can_filter_partial_definitions()
    {
        var code = """
                   public partial class TestClass
                   {
                       partial void PartialMethod();
                       public void RegularMethod() { }
                   }
                   
                   public partial class TestClass
                   {
                       partial void PartialMethod() { }
                   }
                   """;

        var definitions = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryMethods()
            .ThatArePartialDefinitions()
            .GetAll();

        await Assert.That(definitions.Count()).IsEqualTo(1);
        await Assert.That(definitions[0].Value.Name).IsEqualTo("PartialMethod");
    }

    [Test]
    public async Task Can_filter_extension_methods()
    {
        var code = """
                   public static class StringExtensions
                   {
                       public static string ToUpperFirst(this string s) => s;
                       public static string Regular(string s) => s;
                   }
                   """;

        var extensionMethods = SymbolsFor(code)
            .RequireNamedType("StringExtensions")
            .QueryMethods()
            .ThatAreExtensionMethods()
            .GetAll();

        await Assert.That(extensionMethods.Count()).IsEqualTo(1);
        await Assert.That(extensionMethods[0].Value.Name).IsEqualTo("ToUpperFirst");
    }

    #endregion
}
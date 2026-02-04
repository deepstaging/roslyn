// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Projections.Extensions;

public class ValidSymbolMethodsTests : RoslynTestBase
{
    [Test]
    public async Task GetReturnType_always_returns_valid_type()
    {
        var code = """
                   public class TestClass
                   {
                       public int GetNumber() => 42;
                       public void DoNothing() { }
                   }
                   """;

        var getNumber = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryMethods()
            .WithName("GetNumber")
            .First();

        var returnType = getNumber.ReturnType;

        await Assert.That(returnType.Name).IsEqualTo("Int32");
    }

    [Test]
    public async Task IsAsync_detects_async_methods()
    {
        var code = """
                   using System.Threading.Tasks;

                   public class TestClass
                   {
                       public async Task AsyncMethod() { }
                       public void SyncMethod() { }
                   }
                   """;

        var context = SymbolsFor(code);

        var asyncMethod = context.RequireNamedType("TestClass").QueryMethods().WithName("AsyncMethod").First();
        var syncMethod = context.RequireNamedType("TestClass").QueryMethods().WithName("SyncMethod").First();

        await Assert.That(asyncMethod.IsAsync()).IsTrue();
        await Assert.That(syncMethod.IsAsync()).IsFalse();
    }

    [Test]
    public async Task IsAsyncVoid_detects_task_without_return()
    {
        var code = """
                   using System.Threading.Tasks;

                   public class TestClass
                   {
                       public async Task AsyncVoid() { }
                       public async Task<int> AsyncValue() => 42;
                   }
                   """;

        var context = SymbolsFor(code);

        var asyncVoid = context.RequireNamedType("TestClass").QueryMethods().WithName("AsyncVoid").First();
        var asyncValue = context.RequireNamedType("TestClass").QueryMethods().WithName("AsyncValue").First();

        await Assert.That(asyncVoid.IsAsyncVoid()).IsTrue();
        await Assert.That(asyncValue.IsAsyncVoid()).IsFalse();
    }

    [Test]
    public async Task IsAsyncValue_detects_task_with_return()
    {
        var code = """
                   using System.Threading.Tasks;

                   public class TestClass
                   {
                       public async Task AsyncVoid() { }
                       public async Task<string> AsyncValue() => "hello";
                   }
                   """;

        var context = SymbolsFor(code);

        var asyncVoid = context.RequireNamedType("TestClass").QueryMethods().WithName("AsyncVoid").First();
        var asyncValue = context.RequireNamedType("TestClass").QueryMethods().WithName("AsyncValue").First();

        await Assert.That(asyncVoid.IsAsyncValue()).IsFalse();
        await Assert.That(asyncValue.IsAsyncValue()).IsTrue();
    }

    [Test]
    public async Task GetAsyncReturnType_extracts_type_from_task()
    {
        var code = """
                   using System.Threading.Tasks;

                   public class TestClass
                   {
                       public async Task<string> GetTextAsync() => "hello";
                   }
                   """;

        var method = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryMethods()
            .WithName("GetTextAsync")
            .First();

        var returnType = method.AsyncReturnType;

        await Assert.That(returnType.HasValue).IsTrue();
        await Assert.That(returnType.Name).IsEqualTo("String");
    }

    [Test]
    public async Task IsPublicMethod_checks_accessibility()
    {
        var code = """
                   public class TestClass
                   {
                       public void PublicMethod() { }
                       private void PrivateMethod() { }
                   }
                   """;

        var context = SymbolsFor(code);

        var publicMethod = context.RequireNamedType("TestClass").QueryMethods().WithName("PublicMethod").First();
        var privateMethod = context.RequireNamedType("TestClass").QueryMethods().WithName("PrivateMethod").First();

        await Assert.That(publicMethod.IsPublicMethod()).IsTrue();
        await Assert.That(privateMethod.IsPublicMethod()).IsFalse();
        await Assert.That(privateMethod.IsPrivateMethod()).IsTrue();
    }

    [Test]
    public async Task IsStaticMethod_checks_static_modifier()
    {
        var code = """
                   public class TestClass
                   {
                       public static void StaticMethod() { }
                       public void InstanceMethod() { }
                   }
                   """;

        var context = SymbolsFor(code);

        var staticMethod = context.RequireNamedType("TestClass").QueryMethods().WithName("StaticMethod").First();
        var instanceMethod = context.RequireNamedType("TestClass").QueryMethods().WithName("InstanceMethod").First();

        await Assert.That(staticMethod.IsStaticMethod()).IsTrue();
        await Assert.That(instanceMethod.IsStaticMethod()).IsFalse();
    }

    [Test]
    public async Task IsVirtualMethod_checks_virtual_modifier()
    {
        var code = """
                   public class TestClass
                   {
                       public virtual void VirtualMethod() { }
                       public void NormalMethod() { }
                   }
                   """;

        var context = SymbolsFor(code);

        var virtualMethod = context.RequireNamedType("TestClass").QueryMethods().WithName("VirtualMethod").First();
        var normalMethod = context.RequireNamedType("TestClass").QueryMethods().WithName("NormalMethod").First();

        await Assert.That(virtualMethod.IsVirtualMethod()).IsTrue();
        await Assert.That(normalMethod.IsVirtualMethod()).IsFalse();
    }

    [Test]
    public async Task IsOverrideMethod_checks_override_modifier()
    {
        var code = """
                   public class BaseClass
                   {
                       public virtual void Method() { }
                   }

                   public class DerivedClass : BaseClass
                   {
                       public override void Method() { }
                   }
                   """;

        var context = SymbolsFor(code);

        var baseMethod = context.RequireNamedType("BaseClass").QueryMethods().WithName("Method").First();
        var overrideMethod = context.RequireNamedType("DerivedClass").QueryMethods().WithName("Method").First();

        await Assert.That(baseMethod.IsOverrideMethod()).IsFalse();
        await Assert.That(overrideMethod.IsOverrideMethod()).IsTrue();
    }

    [Test]
    public async Task IsExtensionMethod_checks_extension_methods()
    {
        var code = """
                   namespace TestNamespace
                   {
                       public static class Extensions
                       {
                           public static void ExtensionMethod(this string s) { }
                           public static void NormalMethod(string s) { }
                       }
                   }
                   """;

        var extensions = SymbolsFor(code)
            .RequireNamespace("TestNamespace")
            .RequireNamedType("Extensions");

        var extensionMethod = extensions.QueryMethods().WithName("ExtensionMethod").First();
        var normalMethod = extensions.QueryMethods().WithName("NormalMethod").First();

        await Assert.That(extensionMethod.IsExtensionMethod()).IsTrue();
        await Assert.That(normalMethod.IsExtensionMethod()).IsFalse();
    }

    [Test]
    public async Task GetOverriddenMethod_returns_base_method()
    {
        var code = """
                   public class BaseClass
                   {
                       public virtual void Method() { }
                   }

                   public class DerivedClass : BaseClass
                   {
                       public override void Method() { }
                   }
                   """;

        var overrideMethod = SymbolsFor(code)
            .RequireNamedType("DerivedClass")
            .QueryMethods()
            .WithName("Method")
            .First();

        var overridden = overrideMethod.OverriddenMethod;

        await Assert.That(overridden.HasValue).IsTrue();
        await Assert.That(overridden.ContainingType.Name).IsEqualTo("BaseClass");
    }
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Projections.Extensions;

public class OptionalSymbolMethodsTests : RoslynTestBase
{
    [Test]
    public async Task GetReturnType_returns_method_return_type()
    {
        var code = """
            public class TestClass
            {
                public int GetNumber() => 42;
                public void DoNothing() { }
            }
            """;

        var getNumber = SymbolsFor(code)
            .GetNamedType("TestClass")
            .QueryMethods()
            .WithName("GetNumber")
            .FirstOrDefault();

        var returnType = getNumber.ReturnType;

        await Assert.That(returnType.HasValue).IsTrue();
        await Assert.That(returnType.Name).IsEqualTo("Int32");
    }

    [Test]
    public async Task GetAsyncKind_detects_async_methods()
    {
        var code = """
            using System.Threading.Tasks;
            
            public class TestClass
            {
                public async Task AsyncVoid() { }
                public async Task<int> AsyncValue() => 42;
                public void Sync() { }
            }
            """;

        var context = SymbolsFor(code);

        var asyncVoid = context.GetNamedType("TestClass").QueryMethods().WithName("AsyncVoid").FirstOrDefault();
        var asyncValue = context.GetNamedType("TestClass").QueryMethods().WithName("AsyncValue").FirstOrDefault();
        var sync = context.GetNamedType("TestClass").QueryMethods().WithName("Sync").FirstOrDefault();

        await Assert.That(asyncVoid.IsAsyncVoid()).IsTrue();
        await Assert.That(asyncValue.IsAsyncValue()).IsTrue();
        await Assert.That(sync.IsAsync()).IsFalse();
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
            .GetNamedType("TestClass")
            .QueryMethods()
            .WithName("GetTextAsync")
            .FirstOrDefault();

        var returnType = method.AsyncReturnType;

        await Assert.That(returnType.HasValue).IsTrue();
        await Assert.That(returnType.Name).IsEqualTo("String");
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
            .GetNamedType("DerivedClass")
            .QueryMethods()
            .WithName("Method")
            .FirstOrDefault();

        var overridden = overrideMethod.OverriddenMethod;

        await Assert.That(overridden.HasValue).IsTrue();
    }

    [Test]
    public async Task GetPartialImplementation_returns_impl_part()
    {
        var code = """
            public partial class TestClass
            {
                partial void Method();
            }
            
            public partial class TestClass
            {
                partial void Method() { }
            }
            """;

        var definition = SymbolsFor(code)
            .GetNamedType("TestClass")
            .QueryMethods()
            .WithName("Method")
            .FirstOrDefault();

        var implementation = definition.PartialImplementation;

        await Assert.That(implementation.HasValue).IsTrue();
    }

    [Test]
    public async Task GetAssociatedSymbol_returns_property_for_accessor()
    {
        var code = """
            public class TestClass
            {
                public int Property { get; set; }
            }
            """;

        var property = SymbolsFor(code)
            .GetNamedType("TestClass")
            .QueryProperties()
            .WithName("Property")
            .FirstOrDefault();

        await Assert.That(property.GetMethod.AssociatedSymbol.HasValue).IsTrue();
    }
}

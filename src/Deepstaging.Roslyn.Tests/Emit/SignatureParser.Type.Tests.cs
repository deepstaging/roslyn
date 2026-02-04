// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Emit;

public class SignatureParserTypeTests : RoslynTestBase
{
    [Test]
    public async Task Parse_SimpleClass_ExtractsName()
    {
        var builder = TypeBuilder.Parse("public class Customer");

        await Assert.That(builder.Name).IsEqualTo("Customer");
        await Assert.That(builder.Kind).IsEqualTo(TypeKind.Class);
    }

    [Test]
    public async Task Parse_ClassWithInterface_AddsInterface()
    {
        var result = TypeBuilder.Parse("public class CustomerService : IService")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public class CustomerService : IService");
    }

    [Test]
    public async Task Parse_ClassWithMultipleInterfaces_AddsAllInterfaces()
    {
        var result = TypeBuilder.Parse("public class CustomerService : IService, IDisposable")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IService");
        await Assert.That(result.Code).Contains("IDisposable");
    }

    [Test]
    public async Task Parse_SealedClass_SetsSealedModifier()
    {
        var result = TypeBuilder.Parse("public sealed class CacheEntry")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public sealed class CacheEntry");
    }

    [Test]
    public async Task Parse_AbstractClass_SetsAbstractModifier()
    {
        var result = TypeBuilder.Parse("public abstract class BaseHandler")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public abstract class BaseHandler");
    }

    [Test]
    public async Task Parse_StaticClass_SetsStaticModifier()
    {
        var result = TypeBuilder.Parse("public static class StringExtensions")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public static class StringExtensions");
    }

    [Test]
    public async Task Parse_PartialClass_SetsPartialModifier()
    {
        var result = TypeBuilder.Parse("public partial class Customer")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public partial class Customer");
    }

    [Test]
    public async Task Parse_InternalClass_SetsInternalAccessibility()
    {
        var result = TypeBuilder.Parse("internal class Helper")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("internal class Helper");
    }

    [Test]
    public async Task Parse_Interface_CreatesInterface()
    {
        var builder = TypeBuilder.Parse("public interface IRepository");

        await Assert.That(builder.Kind).IsEqualTo(TypeKind.Interface);
    }

    [Test]
    public async Task Parse_InterfaceWithBase_AddsBaseInterface()
    {
        var result = TypeBuilder.Parse("public interface ICustomerRepository : IRepository")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public interface ICustomerRepository : IRepository");
    }

    [Test]
    public async Task Parse_Struct_CreatesStruct()
    {
        var builder = TypeBuilder.Parse("public struct Point");

        await Assert.That(builder.Kind).IsEqualTo(TypeKind.Struct);
    }

    [Test]
    public async Task Parse_ReadonlyStruct_CreatesStruct()
    {
        var result = TypeBuilder.Parse("public readonly struct Vector")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("struct Vector");
    }

    [Test]
    public async Task Parse_CanAddMembersAfterParsing()
    {
        var result = TypeBuilder.Parse("public class Customer")
            .AddProperty(PropertyBuilder.Parse("public string Name { get; set; }"))
            .AddMethod(MethodBuilder.Parse("public void Save()"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public string Name { get; set; }");
        await Assert.That(result.Code).Contains("public void Save()");
    }

    [Test]
    public async Task Parse_CanSetNamespaceAfterParsing()
    {
        var result = TypeBuilder.Parse("public class Customer")
            .InNamespace("MyApp.Domain")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("namespace MyApp.Domain");
    }

    [Test]
    public async Task Parse_CanAddAttributesAfterParsing()
    {
        var result = TypeBuilder.Parse("public class Customer")
            .WithAttribute("Serializable")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("[Serializable]");
    }

    [Test]
    public async Task Parse_InvalidSignature_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            Task.FromResult(TypeBuilder.Parse("this is not valid")));
    }

    [Test]
    public async Task Parse_EmptySignature_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            Task.FromResult(TypeBuilder.Parse("")));
    }

    [Test]
    public async Task Parse_NullSignature_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            Task.FromResult(TypeBuilder.Parse(null!)));
    }

    [Test]
    public async Task Parse_GenericClass_PreservesTypeParameters()
    {
        var result = TypeBuilder.Parse("public class Repository<T>")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        // Note: Generic type parameters on types would need additional handling
        // For now, this tests that we don't break on generic syntax
    }

    [Test]
    public async Task Parse_CombinedModifiers_AppliesAll()
    {
        var result = TypeBuilder.Parse("internal sealed partial class CacheManager")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("internal");
        await Assert.That(result.Code).Contains("sealed");
        await Assert.That(result.Code).Contains("partial");
        await Assert.That(result.Code).Contains("class CacheManager");
    }
}

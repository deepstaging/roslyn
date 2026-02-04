// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Emit;

public class SignatureParserFieldTests : RoslynTestBase
{
    [Test]
    public async Task Parse_SimpleField_ExtractsNameAndType()
    {
        var builder = FieldBuilder.Parse("private string _name");

        await Assert.That(builder.Name).IsEqualTo("_name");
        await Assert.That(builder.Type).IsEqualTo("string");
    }

    [Test]
    public async Task Parse_ReadonlyField_SetsReadonlyModifier()
    {
        var result = TypeBuilder.Class("Test")
            .AddField(FieldBuilder.Parse("private readonly string _name"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("private readonly string _name");
    }

    [Test]
    public async Task Parse_StaticField_SetsStaticModifier()
    {
        var result = TypeBuilder.Class("Test")
            .AddField(FieldBuilder.Parse("private static int _instanceCount"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("private static int _instanceCount");
    }

    [Test]
    public async Task Parse_ConstField_SetsConstModifier()
    {
        var result = TypeBuilder.Class("Test")
            .AddField(FieldBuilder.Parse("public const int MaxRetries = 3"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public const int MaxRetries = 3");
    }

    [Test]
    public async Task Parse_FieldWithInitializer_SetsInitializer()
    {
        var result = TypeBuilder.Class("Test")
            .AddField(FieldBuilder.Parse("private int _count = 0"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("= 0");
    }

    [Test]
    public async Task Parse_PublicField_SetsPublicAccessibility()
    {
        var result = TypeBuilder.Class("Test")
            .AddField(FieldBuilder.Parse("public string Value"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public string Value");
    }

    [Test]
    public async Task Parse_ProtectedField_SetsProtectedAccessibility()
    {
        var result = TypeBuilder.Class("Test")
            .AddField(FieldBuilder.Parse("protected int _value"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("protected int _value");
    }

    [Test]
    public async Task Parse_InternalField_SetsInternalAccessibility()
    {
        var result = TypeBuilder.Class("Test")
            .AddField(FieldBuilder.Parse("internal string _data"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("internal string _data");
    }

    [Test]
    public async Task Parse_StaticReadonlyField_SetsBothModifiers()
    {
        var result = TypeBuilder.Class("Test")
            .AddField(FieldBuilder.Parse("private static readonly object _lock = new()"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("private static readonly object _lock");
    }

    [Test]
    public async Task Parse_NullableType_PreservesNullability()
    {
        var builder = FieldBuilder.Parse("private string? _optionalName");

        await Assert.That(builder.Type).IsEqualTo("string?");
    }

    [Test]
    public async Task Parse_GenericType_PreservesFullType()
    {
        var builder = FieldBuilder.Parse("private readonly Dictionary<string, int> _cache");

        await Assert.That(builder.Type).IsEqualTo("Dictionary<string, int>");
    }

    [Test]
    public async Task Parse_FieldWithSemicolon_HandlesGracefully()
    {
        var builder = FieldBuilder.Parse("private string _name;");

        await Assert.That(builder.Name).IsEqualTo("_name");
    }

    [Test]
    public async Task Parse_CanAddAttributesAfterParsing()
    {
        var result = TypeBuilder.Class("Test")
            .AddField(FieldBuilder.Parse("private string _name")
                .WithAttribute("NonSerialized"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("[NonSerialized]");
    }

    [Test]
    public async Task Parse_InvalidSignature_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            Task.FromResult(FieldBuilder.Parse("this is not valid")));
    }

    [Test]
    public async Task Parse_EmptySignature_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            Task.FromResult(FieldBuilder.Parse("")));
    }

    [Test]
    public async Task Parse_NullSignature_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            Task.FromResult(FieldBuilder.Parse(null!)));
    }
}

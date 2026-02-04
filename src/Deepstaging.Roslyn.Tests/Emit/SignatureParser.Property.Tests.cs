// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Emit;

public class SignatureParserPropertyTests : RoslynTestBase
{
    [Test]
    public async Task Parse_AutoProperty_ExtractsNameAndType()
    {
        var builder = PropertyBuilder.Parse("public string Name { get; set; }");

        await Assert.That(builder.Name).IsEqualTo("Name");
        await Assert.That(builder.Type).IsEqualTo("string");
    }

    [Test]
    public async Task Parse_AutoProperty_GeneratesGetSet()
    {
        var result = TypeBuilder.Class("Test")
            .AddProperty(PropertyBuilder.Parse("public string Name { get; set; }"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public string Name { get; set; }");
    }

    [Test]
    public async Task Parse_GetOnlyProperty_GeneratesGetOnly()
    {
        var result = TypeBuilder.Class("Test")
            .AddProperty(PropertyBuilder.Parse("public int Count { get; }"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public int Count { get; }");
    }

    [Test]
    public async Task Parse_StaticProperty_SetsStaticModifier()
    {
        var result = TypeBuilder.Class("Test")
            .AddProperty(PropertyBuilder.Parse("public static int InstanceCount { get; set; }"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public static int InstanceCount");
    }

    [Test]
    public async Task Parse_VirtualProperty_SetsVirtualModifier()
    {
        var result = TypeBuilder.Class("Test")
            .AddProperty(PropertyBuilder.Parse("public virtual string DisplayName { get; set; }"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public virtual string DisplayName");
    }

    [Test]
    public async Task Parse_OverrideProperty_SetsOverrideModifier()
    {
        var result = TypeBuilder.Class("Test")
            .AddProperty(PropertyBuilder.Parse("public override string Name { get; set; }"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public override string Name");
    }

    [Test]
    public async Task Parse_PrivateProperty_SetsPrivateAccessibility()
    {
        var result = TypeBuilder.Class("Test")
            .AddProperty(PropertyBuilder.Parse("private string Secret { get; set; }"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("private string Secret");
    }

    [Test]
    public async Task Parse_PropertyWithInitializer_SetsInitializer()
    {
        var result = TypeBuilder.Class("Test")
            .AddProperty(PropertyBuilder.Parse("public List<string> Items { get; set; } = new()"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("= new()");
    }

    [Test]
    public async Task Parse_ExpressionBodiedProperty_SetsGetter()
    {
        var result = TypeBuilder.Class("Test")
            .AddProperty(PropertyBuilder.Parse("public string FullName => FirstName + LastName"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("=> FirstName + LastName");
    }

    [Test]
    public async Task Parse_NullableType_PreservesNullability()
    {
        var builder = PropertyBuilder.Parse("public string? OptionalName { get; set; }");

        await Assert.That(builder.Type).IsEqualTo("string?");
    }

    [Test]
    public async Task Parse_GenericType_PreservesFullType()
    {
        var builder = PropertyBuilder.Parse("public Dictionary<string, List<int>> Data { get; set; }");

        await Assert.That(builder.Type).IsEqualTo("Dictionary<string, List<int>>");
    }

    [Test]
    public async Task Parse_CanAddAttributesAfterParsing()
    {
        var result = TypeBuilder.Class("Test")
            .AddProperty(PropertyBuilder.Parse("public string Name { get; set; }")
                .WithAttribute("Required"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("[Required]");
    }

    [Test]
    public async Task Parse_InvalidSignature_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            Task.FromResult(PropertyBuilder.Parse("this is not valid")));
    }

    [Test]
    public async Task Parse_EmptySignature_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            Task.FromResult(PropertyBuilder.Parse("")));
    }

    [Test]
    public async Task Parse_NullSignature_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            Task.FromResult(PropertyBuilder.Parse(null!)));
    }
}

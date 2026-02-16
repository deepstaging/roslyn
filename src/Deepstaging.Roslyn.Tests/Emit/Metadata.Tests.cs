// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit;

public class MetadataTests : RoslynTestBase
{
    #region TypeBuilder Metadata

    [Test]
    public async Task TypeBuilder_can_store_and_retrieve_metadata()
    {
        var builder = TypeBuilder
            .Class("Customer")
            .WithMetadata("Origin", "test");

        await Assert.That(builder.GetMetadata<string>("Origin")).IsEqualTo("test");
    }

    [Test]
    public async Task TypeBuilder_metadata_throws_for_missing_key()
    {
        var builder = TypeBuilder
            .Class("Customer");

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Task.FromResult(builder.GetMetadata<string>("Missing")));
    }

    [Test]
    public async Task TypeBuilder_metadata_does_not_affect_emitted_code()
    {
        var withMeta = TypeBuilder
            .Class("Customer")
            .InNamespace("MyApp")
            .WithMetadata("Tag", "value")
            .Emit();

        var withoutMeta = TypeBuilder
            .Class("Customer")
            .InNamespace("MyApp")
            .Emit();

        await Assert.That(withMeta.Success).IsTrue();
        await Assert.That(withoutMeta.Success).IsTrue();
        await Assert.That(withMeta.Code).IsEqualTo(withoutMeta.Code);
    }

    [Test]
    public async Task TypeBuilder_can_store_complex_object_as_metadata()
    {
        var info = new TestInfo("StubEmail", "_email");

        var builder = TypeBuilder
            .Class("Customer")
            .WithMetadata("Info", info);

        var retrieved = builder.GetMetadata<TestInfo>("Info");
        await Assert.That(retrieved.Name).IsEqualTo("StubEmail");
        await Assert.That(retrieved.FieldName).IsEqualTo("_email");
    }

    [Test]
    public async Task TypeBuilder_metadata_supports_multiple_keys()
    {
        var builder = TypeBuilder
            .Class("Customer")
            .WithMetadata("Key1", "Value1")
            .WithMetadata("Key2", "Value2");

        await Assert.That(builder.GetMetadata<string>("Key1")).IsEqualTo("Value1");
        await Assert.That(builder.GetMetadata<string>("Key2")).IsEqualTo("Value2");
    }

    #endregion

    #region MethodBuilder Metadata

    [Test]
    public async Task MethodBuilder_can_store_and_retrieve_metadata()
    {
        var builder = MethodBuilder
            .For("Process")
            .WithMetadata("Source", "generated");

        await Assert.That(builder.GetMetadata<string>("Source")).IsEqualTo("generated");
    }

    [Test]
    public async Task MethodBuilder_metadata_throws_for_missing_key()
    {
        var builder = MethodBuilder.For("Process");

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Task.FromResult(builder.GetMetadata<string>("Missing")));
    }

    #endregion

    #region PropertyBuilder Metadata

    [Test]
    public async Task PropertyBuilder_can_store_and_retrieve_metadata()
    {
        var builder = PropertyBuilder
            .For("Name", "string")
            .WithMetadata("Alias", "DisplayName");

        await Assert.That(builder.GetMetadata<string>("Alias")).IsEqualTo("DisplayName");
    }

    [Test]
    public async Task PropertyBuilder_metadata_throws_for_missing_key()
    {
        var builder = PropertyBuilder.For("Name", "string");

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Task.FromResult(builder.GetMetadata<string>("Missing")));
    }

    #endregion

    private record TestInfo(string Name, string FieldName);
}
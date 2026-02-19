// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class JsonConverterTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new JsonConverterTypeRef("MyEnum");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Text.Json.Serialization.JsonConverter<MyEnum>");
    }

    [Test]
    public async Task Carries_value_type()
    {
        var typeRef = new JsonConverterTypeRef("MyEnum");

        await Assert.That((string)typeRef.ValueType).IsEqualTo("MyEnum");
    }
}

public class JsonTypesTests
{
    [Test]
    public async Task Serializer_produces_correct_type()
    {
        await Assert.That(JsonTypes.Serializer.Value)
            .IsEqualTo("global::System.Text.Json.JsonSerializer");
    }

    [Test]
    public async Task SerializerOptions_produces_correct_type()
    {
        await Assert.That(JsonTypes.SerializerOptions.Value)
            .IsEqualTo("global::System.Text.Json.JsonSerializerOptions");
    }

    [Test]
    public async Task Reader_produces_correct_type()
    {
        await Assert.That(JsonTypes.Reader.Value)
            .IsEqualTo("global::System.Text.Json.Utf8JsonReader");
    }

    [Test]
    public async Task Writer_produces_correct_type()
    {
        await Assert.That(JsonTypes.Writer.Value)
            .IsEqualTo("global::System.Text.Json.Utf8JsonWriter");
    }
}

public class JsonAttributesTests
{
    [Test]
    public async Task Converter_produces_valid_attribute()
    {
        await Assert.That(JsonAttributes.Converter.Value)
            .IsEqualTo("global::System.Text.Json.Serialization.JsonConverter");
    }
}

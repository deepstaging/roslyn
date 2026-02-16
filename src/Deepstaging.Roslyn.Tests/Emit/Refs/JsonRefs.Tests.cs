// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Refs;

public class JsonRefsTests
{
    [Test]
    public async Task Serializer_creates_globally_qualified_type()
    {
        var typeRef = JsonRefs.Serializer;

        await Assert.That(typeRef).IsEqualTo("global::System.Text.Json.JsonSerializer");
    }

    [Test]
    public async Task SerializerOptions_creates_globally_qualified_type()
    {
        var typeRef = JsonRefs.SerializerOptions;

        await Assert.That(typeRef).IsEqualTo("global::System.Text.Json.JsonSerializerOptions");
    }

    [Test]
    public async Task Reader_creates_globally_qualified_type()
    {
        var typeRef = JsonRefs.Reader;

        await Assert.That(typeRef).IsEqualTo("global::System.Text.Json.Utf8JsonReader");
    }

    [Test]
    public async Task Writer_creates_globally_qualified_type()
    {
        var typeRef = JsonRefs.Writer;

        await Assert.That(typeRef).IsEqualTo("global::System.Text.Json.Utf8JsonWriter");
    }

    [Test]
    public async Task ConverterOf_creates_generic_globally_qualified_type()
    {
        var typeRef = JsonRefs.ConverterOf("CustomerId");

        await Assert.That(typeRef).IsEqualTo("global::System.Text.Json.Serialization.JsonConverter<CustomerId>");
    }

    [Test]
    public async Task Converter_creates_globally_qualified_attribute()
    {
        AttributeRef attr = JsonRefs.Converter;

        await Assert.That(attr.Value).IsEqualTo("global::System.Text.Json.Serialization.JsonConverter");
    }
}
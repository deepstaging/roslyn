// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Refs;

public class JsonRefsTests
{
    [Test]
    public async Task Serializer_creates_globally_qualified_type()
    {
        TypeRef typeRef = JsonRefs.Serializer;

        await Assert.That(typeRef).IsEqualTo("global::System.Text.Json.JsonSerializer");
    }

    [Test]
    public async Task SerializerOptions_creates_globally_qualified_type()
    {
        TypeRef typeRef = JsonRefs.SerializerOptions;

        await Assert.That(typeRef).IsEqualTo("global::System.Text.Json.JsonSerializerOptions");
    }

    [Test]
    public async Task Reader_creates_globally_qualified_type()
    {
        TypeRef typeRef = JsonRefs.Reader;

        await Assert.That(typeRef).IsEqualTo("global::System.Text.Json.Utf8JsonReader");
    }

    [Test]
    public async Task Writer_creates_globally_qualified_type()
    {
        TypeRef typeRef = JsonRefs.Writer;

        await Assert.That(typeRef).IsEqualTo("global::System.Text.Json.Utf8JsonWriter");
    }

    [Test]
    public async Task Converter_creates_generic_globally_qualified_type()
    {
        TypeRef typeRef = JsonRefs.Converter("CustomerId");

        await Assert.That(typeRef).IsEqualTo("global::System.Text.Json.Serialization.JsonConverter<CustomerId>");
    }

    [Test]
    public async Task ConverterAttribute_creates_globally_qualified_type()
    {
        TypeRef typeRef = JsonRefs.ConverterAttribute;

        await Assert.That(typeRef).IsEqualTo("global::System.Text.Json.Serialization.JsonConverter");
    }
}

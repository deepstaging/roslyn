// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Factory methods for <c>System.Text.Json</c> and <c>System.Text.Json.Serialization</c> types.
/// </summary>
public static class JsonRefs
{
    /// <summary>Gets the <c>System.Text.Json</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System.Text.Json");

    /// <summary>Gets the <c>System.Text.Json.Serialization</c> namespace.</summary>
    public static NamespaceRef SerializationNamespace => NamespaceRef.From("System.Text.Json.Serialization");

    /// <summary>Gets a <c>JsonSerializer</c> type reference.</summary>
    public static TypeRef Serializer => Namespace.Type("JsonSerializer");

    /// <summary>Gets a <c>JsonSerializerOptions</c> type reference.</summary>
    public static TypeRef SerializerOptions => Namespace.Type("JsonSerializerOptions");

    /// <summary>Gets a <c>Utf8JsonReader</c> type reference.</summary>
    public static TypeRef Reader => Namespace.Type("Utf8JsonReader");

    /// <summary>Gets a <c>Utf8JsonWriter</c> type reference.</summary>
    public static TypeRef Writer => Namespace.Type("Utf8JsonWriter");

    /// <summary>Creates a <c>JsonConverter&lt;T&gt;</c> type reference.</summary>
    public static TypeRef Converter(TypeRef valueType) =>
        SerializationNamespace.Type($"JsonConverter<{valueType.Value}>");

    /// <summary>Gets a <c>JsonConverterAttribute</c> type reference.</summary>
    public static TypeRef ConverterAttribute => SerializationNamespace.Type("JsonConverter");
}

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
    public static TypeRef Serializer => Namespace.GlobalType("JsonSerializer");

    /// <summary>Gets a <c>JsonSerializerOptions</c> type reference.</summary>
    public static TypeRef SerializerOptions => Namespace.GlobalType("JsonSerializerOptions");

    /// <summary>Gets a <c>Utf8JsonReader</c> type reference.</summary>
    public static TypeRef Reader => Namespace.GlobalType("Utf8JsonReader");

    /// <summary>Gets a <c>Utf8JsonWriter</c> type reference.</summary>
    public static TypeRef Writer => Namespace.GlobalType("Utf8JsonWriter");

    /// <summary>Creates a <c>JsonConverter&lt;T&gt;</c> type reference.</summary>
    public static TypeRef ConverterOf(TypeRef valueType) =>
        SerializationNamespace.GlobalType($"JsonConverter<{valueType.Value}>");

    /// <summary>Gets a <c>JsonConverterAttribute</c> attribute reference.</summary>
    public static AttributeRef Converter => SerializationNamespace.GlobalAttribute("JsonConverter");

    // ── Well-Known API Calls ────────────────────────────────────────────

    /// <summary>Produces a <c>JsonSerializer.Serialize(value)</c> expression.</summary>
    public static ExpressionRef Serialize(ExpressionRef value) =>
        Serializer.Call("Serialize", value);

    /// <summary>Produces a <c>JsonSerializer.Serialize(value, options)</c> expression.</summary>
    public static ExpressionRef Serialize(ExpressionRef value, ExpressionRef options) =>
        Serializer.Call("Serialize", value, options);

    /// <summary>Produces a <c>JsonSerializer.Deserialize&lt;T&gt;(json)</c> expression.</summary>
    public static ExpressionRef Deserialize(TypeRef type, ExpressionRef json) =>
        Serializer.Call($"Deserialize<{type.Value}>", json);

    /// <summary>Produces a <c>JsonSerializer.Deserialize&lt;T&gt;(json, options)</c> expression.</summary>
    public static ExpressionRef Deserialize(TypeRef type, ExpressionRef json, ExpressionRef options) =>
        Serializer.Call($"Deserialize<{type.Value}>", json, options);
}
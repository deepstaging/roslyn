// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// A type-safe wrapper representing a <c>JsonConverter&lt;T&gt;</c> type reference.
/// Carries the value type for typed expression building.
/// </summary>
public readonly record struct JsonConverterTypeRef
{
    /// <summary>Gets the value type being converted (e.g., <c>"MyEnum"</c>).</summary>
    public TypeRef ValueType { get; }

    /// <summary>Creates a <c>JsonConverterTypeRef</c> for the given value type.</summary>
    public JsonConverterTypeRef(TypeRef valueType) => ValueType = valueType;

    /// <summary>Gets the globally qualified <c>JsonConverter&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Text.Json.Serialization.JsonConverter<{ValueType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(JsonConverterTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(JsonConverterTypeRef self) =>
        self.ToString();
}

/// <summary>
/// Convenience <see cref="TypeRef"/> constants for <c>System.Text.Json</c> types.
/// </summary>
public static class JsonTypes
{
    /// <summary>Gets the <c>System.Text.Json</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System.Text.Json");

    /// <summary>Gets the <c>System.Text.Json.Serialization</c> namespace.</summary>
    public static NamespaceRef SerializationNamespace => NamespaceRef.From("System.Text.Json.Serialization");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>JsonSerializer</c>.</summary>
    public static TypeRef Serializer => Namespace.GlobalType("JsonSerializer");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>JsonSerializerOptions</c>.</summary>
    public static TypeRef SerializerOptions => Namespace.GlobalType("JsonSerializerOptions");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>Utf8JsonReader</c>.</summary>
    public static TypeRef Reader => Namespace.GlobalType("Utf8JsonReader");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>Utf8JsonWriter</c>.</summary>
    public static TypeRef Writer => Namespace.GlobalType("Utf8JsonWriter");
}

/// <summary>
/// Convenience <see cref="AttributeRef"/> constants for <c>System.Text.Json</c> attributes.
/// </summary>
public static class JsonAttributes
{
    /// <summary>Gets an <see cref="AttributeRef"/> for <c>[JsonConverter]</c>.</summary>
    public static AttributeRef Converter => JsonTypes.SerializationNamespace.GlobalAttribute("JsonConverter");
}

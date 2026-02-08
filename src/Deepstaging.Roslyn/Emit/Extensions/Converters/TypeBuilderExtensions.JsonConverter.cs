// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Microsoft.CodeAnalysis;

namespace Deepstaging.Roslyn.Emit.Converters;

/// <summary>
/// Extensions for adding System.Text.Json JsonConverter implementations.
/// </summary>
public static class TypeBuilderJsonConverterExtensions
{
    /// <summary>
    /// Adds a nested System.Text.Json JsonConverter class for a wrapper type with a single value property.
    /// Automatically generates read/write expressions for common backing types:
    /// Guid, int, long, short, byte, sbyte, uint, ulong, ushort, float, double, decimal,
    /// bool, string, DateTime, DateTimeOffset, DateOnly, TimeOnly, TimeSpan.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="converterName">The name of the nested converter class.</param>
    /// <param name="backingType">The backing type symbol (e.g., System.Guid, int, long, string).</param>
    /// <param name="valuePropertyName">The name of the value property (default: "Value").</param>
    /// <param name="addAttribute">Whether to add [JsonConverter] attribute to the parent type.</param>
    /// <example>
    /// <code>
    /// TypeBuilder.Parse("public partial struct UserId")
    ///     .WithJsonConverter("UserIdJsonConverter", guidSymbol);
    /// </code>
    /// </example>
    public static TypeBuilder WithJsonConverter(
        this TypeBuilder builder,
        string converterName,
        ValidSymbol<INamedTypeSymbol> backingType,
        string valuePropertyName = "Value",
        bool addAttribute = true)
    {
        var typeName = builder.Name;
        var specialType = backingType.SpecialType;
        var fullyQualifiedName = backingType.FullyQualifiedName;
        var isGuid = fullyQualifiedName == "System.Guid";

        var readExpression = GetReadExpression(specialType, isGuid, fullyQualifiedName);
        var writeExpression = GetWriteExpression(specialType, isGuid, fullyQualifiedName, valuePropertyName);
        var readAsPropertyNameExpression = GetReadAsPropertyNameExpression(typeName, specialType, isGuid, fullyQualifiedName);
        var writeAsPropertyNameExpression = GetWriteAsPropertyNameExpression(specialType, isGuid, valuePropertyName);

        return builder.WithJsonConverter(
            converterName,
            readExpression,
            writeExpression,
            readAsPropertyNameExpression,
            writeAsPropertyNameExpression,
            addAttribute);
    }

    private static string GetReadExpression(SpecialType specialType, bool isGuid, string fullyQualifiedName) =>
        specialType switch
        {
            SpecialType.System_Int16 => "new(reader.GetInt16())",
            SpecialType.System_Int32 => "new(reader.GetInt32())",
            SpecialType.System_Int64 => "new(reader.GetInt64())",
            SpecialType.System_UInt16 => "new(reader.GetUInt16())",
            SpecialType.System_UInt32 => "new(reader.GetUInt32())",
            SpecialType.System_UInt64 => "new(reader.GetUInt64())",
            SpecialType.System_Byte => "new(reader.GetByte())",
            SpecialType.System_SByte => "new(reader.GetSByte())",
            SpecialType.System_Single => "new(reader.GetSingle())",
            SpecialType.System_Double => "new(reader.GetDouble())",
            SpecialType.System_Decimal => "new(reader.GetDecimal())",
            SpecialType.System_Boolean => "new(reader.GetBoolean())",
            SpecialType.System_String => "new(reader.GetString()!)",
            SpecialType.System_DateTime => "new(reader.GetDateTime())",
            _ when isGuid => "new(reader.GetGuid())",
            _ when fullyQualifiedName == "System.DateTimeOffset" => "new(reader.GetDateTimeOffset())",
            _ when fullyQualifiedName == "System.DateOnly" => "new(global::System.DateOnly.Parse(reader.GetString()!))",
            _ when fullyQualifiedName == "System.TimeOnly" => "new(global::System.TimeOnly.Parse(reader.GetString()!))",
            _ when fullyQualifiedName == "System.TimeSpan" => "new(global::System.TimeSpan.Parse(reader.GetString()!))",
            _ => "default"
        };

    private static string GetWriteExpression(SpecialType specialType, bool isGuid, string fullyQualifiedName, string valueProperty) =>
        specialType switch
        {
            SpecialType.System_Int16 or SpecialType.System_Int32 or SpecialType.System_Int64 or
            SpecialType.System_UInt16 or SpecialType.System_UInt32 or SpecialType.System_UInt64 or
            SpecialType.System_Byte or SpecialType.System_SByte or
            SpecialType.System_Single or SpecialType.System_Double or SpecialType.System_Decimal
                => $"writer.WriteNumberValue(value.{valueProperty})",
            SpecialType.System_Boolean => $"writer.WriteBooleanValue(value.{valueProperty})",
            SpecialType.System_String => $"writer.WriteStringValue(value.{valueProperty})",
            SpecialType.System_DateTime => $"writer.WriteStringValue(value.{valueProperty})",
            _ when isGuid => $"writer.WriteStringValue(value.{valueProperty})",
            _ when fullyQualifiedName == "System.DateTimeOffset" => $"writer.WriteStringValue(value.{valueProperty})",
            _ when fullyQualifiedName is "System.DateOnly" or "System.TimeOnly" or "System.TimeSpan"
                => $"writer.WriteStringValue(value.{valueProperty}.ToString())",
            _ => "writer.WriteNullValue()"
        };

    private static string GetReadAsPropertyNameExpression(string typeName, SpecialType specialType, bool isGuid, string fullyQualifiedName) =>
        specialType switch
        {
            SpecialType.System_Int16 =>
                $"new(short.Parse(reader.GetString() ?? throw new global::System.FormatException(\"The string for the {typeName} property was null\")))",
            SpecialType.System_Int32 =>
                $"new(int.Parse(reader.GetString() ?? throw new global::System.FormatException(\"The string for the {typeName} property was null\")))",
            SpecialType.System_Int64 =>
                $"new(long.Parse(reader.GetString() ?? throw new global::System.FormatException(\"The string for the {typeName} property was null\")))",
            SpecialType.System_UInt16 =>
                $"new(ushort.Parse(reader.GetString() ?? throw new global::System.FormatException(\"The string for the {typeName} property was null\")))",
            SpecialType.System_UInt32 =>
                $"new(uint.Parse(reader.GetString() ?? throw new global::System.FormatException(\"The string for the {typeName} property was null\")))",
            SpecialType.System_UInt64 =>
                $"new(ulong.Parse(reader.GetString() ?? throw new global::System.FormatException(\"The string for the {typeName} property was null\")))",
            SpecialType.System_Byte =>
                $"new(byte.Parse(reader.GetString() ?? throw new global::System.FormatException(\"The string for the {typeName} property was null\")))",
            SpecialType.System_SByte =>
                $"new(sbyte.Parse(reader.GetString() ?? throw new global::System.FormatException(\"The string for the {typeName} property was null\")))",
            SpecialType.System_Single =>
                $"new(float.Parse(reader.GetString() ?? throw new global::System.FormatException(\"The string for the {typeName} property was null\")))",
            SpecialType.System_Double =>
                $"new(double.Parse(reader.GetString() ?? throw new global::System.FormatException(\"The string for the {typeName} property was null\")))",
            SpecialType.System_Decimal =>
                $"new(decimal.Parse(reader.GetString() ?? throw new global::System.FormatException(\"The string for the {typeName} property was null\")))",
            SpecialType.System_Boolean =>
                $"new(bool.Parse(reader.GetString() ?? throw new global::System.FormatException(\"The string for the {typeName} property was null\")))",
            SpecialType.System_String => "new(reader.GetString()!)",
            SpecialType.System_DateTime =>
                $"new(global::System.DateTime.Parse(reader.GetString() ?? throw new global::System.FormatException(\"The string for the {typeName} property was null\")))",
            _ when isGuid =>
                $"new(global::System.Guid.Parse(reader.GetString() ?? throw new global::System.FormatException(\"The string for the {typeName} property was null\")))",
            _ when fullyQualifiedName == "System.DateTimeOffset" =>
                $"new(global::System.DateTimeOffset.Parse(reader.GetString() ?? throw new global::System.FormatException(\"The string for the {typeName} property was null\")))",
            _ when fullyQualifiedName == "System.DateOnly" =>
                $"new(global::System.DateOnly.Parse(reader.GetString() ?? throw new global::System.FormatException(\"The string for the {typeName} property was null\")))",
            _ when fullyQualifiedName == "System.TimeOnly" =>
                $"new(global::System.TimeOnly.Parse(reader.GetString() ?? throw new global::System.FormatException(\"The string for the {typeName} property was null\")))",
            _ when fullyQualifiedName == "System.TimeSpan" =>
                $"new(global::System.TimeSpan.Parse(reader.GetString() ?? throw new global::System.FormatException(\"The string for the {typeName} property was null\")))",
            _ => "default"
        };

    private static string GetWriteAsPropertyNameExpression(SpecialType specialType, bool isGuid, string valueProperty) =>
        specialType switch
        {
            SpecialType.System_String => $"writer.WritePropertyName(value.{valueProperty} ?? string.Empty)",
            _ when isGuid => $"writer.WritePropertyName(value.{valueProperty}.ToString())",
            _ => $"writer.WritePropertyName(value.{valueProperty}.ToString())"
        };

    /// <summary>
    /// Adds a nested System.Text.Json JsonConverter class.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="converterName">The name of the nested converter class.</param>
    /// <param name="readExpression">Expression body for Read method (e.g., "new(reader.GetGuid())").</param>
    /// <param name="writeExpression">Expression body for Write method (e.g., "writer.WriteStringValue(value.Value)").</param>
    /// <param name="readAsPropertyNameExpression">Optional expression for ReadAsPropertyName (NET6+).</param>
    /// <param name="writeAsPropertyNameExpression">Optional expression for WriteAsPropertyName (NET6+).</param>
    /// <param name="addAttribute">Whether to add [JsonConverter] attribute to the parent type.</param>
    /// <example>
    /// <code>
    /// TypeBuilder.Parse("public partial struct UserId")
    ///     .WithJsonConverter(
    ///         "UserIdJsonConverter",
    ///         readExpression: "new(reader.GetGuid())",
    ///         writeExpression: "writer.WriteStringValue(value.Value)");
    /// </code>
    /// </example>
    public static TypeBuilder WithJsonConverter(
        this TypeBuilder builder,
        string converterName,
        string readExpression,
        string writeExpression,
        string? readAsPropertyNameExpression = null,
        string? writeAsPropertyNameExpression = null,
        bool addAttribute = true)
    {
        var typeName = builder.Name;

        var converterType = TypeBuilder
            .Parse($"public partial class {converterName} : global::System.Text.Json.Serialization.JsonConverter<{typeName}>")
            .AddMethod(MethodBuilder
                .Parse($"public override {typeName} Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)")
                .WithExpressionBody(readExpression))
            .AddMethod(MethodBuilder
                .Parse($"public override void Write(global::System.Text.Json.Utf8JsonWriter writer, {typeName} value, global::System.Text.Json.JsonSerializerOptions options)")
                .WithExpressionBody(writeExpression));

        // Add property name methods for NET6+
        if (readAsPropertyNameExpression is not null)
        {
            converterType = converterType.AddMethod(MethodBuilder
                .Parse($"public override {typeName} ReadAsPropertyName(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)")
                .When(Directives.Net6OrGreater)
                .WithExpressionBody(readAsPropertyNameExpression));
        }

        if (writeAsPropertyNameExpression is not null)
        {
            converterType = converterType.AddMethod(MethodBuilder
                .Parse($"public override void WriteAsPropertyName(global::System.Text.Json.Utf8JsonWriter writer, {typeName} value, global::System.Text.Json.JsonSerializerOptions options)")
                .When(Directives.Net6OrGreater)
                .WithExpressionBody(writeAsPropertyNameExpression));
        }

        var result = builder.AddNestedType(converterType);

        if (addAttribute)
        {
            result = result.WithAttribute("global::System.Text.Json.Serialization.JsonConverter", a => a
                .WithArgument($"typeof({converterName})"));
        }

        return result;
    }

    /// <summary>
    /// Adds a nested System.Text.Json JsonConverter class with full configuration.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="converterName">The name of the nested converter class.</param>
    /// <param name="configure">Configuration callback for the converter TypeBuilder.</param>
    /// <param name="addAttribute">Whether to add [JsonConverter] attribute to the parent type.</param>
    public static TypeBuilder WithJsonConverter(
        this TypeBuilder builder,
        string converterName,
        Func<TypeBuilder, TypeBuilder> configure,
        bool addAttribute = true)
    {
        var typeName = builder.Name;

        var converterType = configure(TypeBuilder
            .Parse($"public partial class {converterName} : global::System.Text.Json.Serialization.JsonConverter<{typeName}>"));

        var result = builder.AddNestedType(converterType);

        if (addAttribute)
        {
            result = result.WithAttribute("global::System.Text.Json.Serialization.JsonConverter", a => a
                .WithArgument($"typeof({converterName})"));
        }

        return result;
    }
}

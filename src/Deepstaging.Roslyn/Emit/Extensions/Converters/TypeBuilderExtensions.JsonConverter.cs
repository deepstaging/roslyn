// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Converters;

/// <summary>
/// Extensions for adding System.Text.Json JsonConverter implementations.
/// </summary>
public static class TypeBuilderJsonConverterExtensions
{
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

// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Converters;

namespace Deepstaging.Roslyn.Tests.Emit.Extensions.Converters;

public sealed class TypeBuilderExtensionsJsonConverterTests : RoslynTestBase
{
    [Test]
    public async Task WithJsonConverter_AddsNestedConverterClass()
    {
        var type = TypeBuilder.Parse("public partial struct UserId")
            .WithJsonConverter(
                "UserIdJsonConverter",
                "new(reader.GetGuid())",
                "writer.WriteStringValue(value.Value)");

        var result = type.Emit();

        await Assert.That(result.Code)
            .Contains(
                "public partial class UserIdJsonConverter : global::System.Text.Json.Serialization.JsonConverter<UserId>");

        await Assert.That(result.Code).Contains("public override UserId Read");
        await Assert.That(result.Code).Contains("new(reader.GetGuid())");
        await Assert.That(result.Code).Contains("public override void Write");
        await Assert.That(result.Code).Contains("writer.WriteStringValue(value.Value)");

        await Assert.That(result.Code)
            .Contains("[global::System.Text.Json.Serialization.JsonConverter(typeof(UserIdJsonConverter))]");
    }

    [Test]
    public async Task WithJsonConverter_WithPropertyNameMethods_AddsNet6Methods()
    {
        var type = TypeBuilder.Parse("public partial struct UserId")
            .WithJsonConverter(
                "UserIdJsonConverter",
                "new(reader.GetGuid())",
                "writer.WriteStringValue(value.Value)",
                "new(global::System.Guid.Parse(reader.GetString()!))",
                "writer.WritePropertyName(value.Value.ToString())");

        var result = type.Emit();

        await Assert.That(result.Code).Contains("public override UserId ReadAsPropertyName");
        await Assert.That(result.Code).Contains("public override void WriteAsPropertyName");
        await Assert.That(result.Code).Contains("#if NET6_0_OR_GREATER");
    }

    [Test]
    public async Task WithJsonConverter_WithoutAttribute_DoesNotAddAttribute()
    {
        var type = TypeBuilder.Parse("public partial struct UserId")
            .WithJsonConverter(
                "UserIdJsonConverter",
                "default",
                "writer.WriteNullValue()",
                addAttribute: false);

        var result = type.Emit();

        await Assert.That(result.Code).Contains("public partial class UserIdJsonConverter");
        await Assert.That(result.Code).DoesNotContain("[global::System.Text.Json.Serialization.JsonConverter");
    }

    [Test]
    public async Task WithJsonConverter_ConfigureCallback_AllowsCustomization()
    {
        var type = TypeBuilder.Parse("public partial struct UserId")
            .WithJsonConverter("UserIdJsonConverter", t => t
                .AddMethod(MethodBuilder
                    .Parse(
                        "public override UserId Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)")
                    .WithExpressionBody("default"))
                .AddMethod(MethodBuilder
                    .Parse(
                        "public override void Write(global::System.Text.Json.Utf8JsonWriter writer, UserId value, global::System.Text.Json.JsonSerializerOptions options)")
                    .WithBody(b => b.AddStatement("writer.WriteNullValue()"))));

        var result = type.Emit();

        await Assert.That(result.Code).Contains("public partial class UserIdJsonConverter");
        await Assert.That(result.Code).Contains("writer.WriteNullValue()");
    }
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit;
using Deepstaging.Roslyn.Emit.Converters;

namespace Deepstaging.Roslyn.Tests.Emit.Extensions.Converters;

public sealed class TypeBuilderExtensionsTypeConverterTests : RoslynTestBase
{
    [Test]
    public async Task WithTypeConverter_AddsNestedConverterClass()
    {
        var type = TypeBuilder.Parse("public partial struct UserId")
            .WithTypeConverter(
                "UserIdTypeConverter",
                "return sourceType == typeof(global::System.Guid) || base.CanConvertFrom(context, sourceType);",
                "return value is global::System.Guid g ? new UserId(g) : base.ConvertFrom(context, culture, value);",
                "return sourceType == typeof(global::System.Guid) || base.CanConvertTo(context, sourceType);",
                "return value is UserId id ? id.Value : base.ConvertTo(context, culture, value, destinationType);");

        var result = type.Emit();

        await Assert.That(result.Code)
            .Contains("public partial class UserIdTypeConverter : global::System.ComponentModel.TypeConverter");

        await Assert.That(result.Code).Contains("public override bool CanConvertFrom");
        await Assert.That(result.Code).Contains("public override object? ConvertFrom");
        await Assert.That(result.Code).Contains("[global::System.ComponentModel.TypeConverter(typeof(UserIdTypeConverter))]");
    }

    [Test]
    public async Task WithTypeConverter_WithoutAttribute_DoesNotAddAttribute()
    {
        var type = TypeBuilder.Parse("public partial struct UserId")
            .WithTypeConverter(
                "UserIdTypeConverter",
                "return base.CanConvertFrom(context, sourceType);",
                "return base.ConvertFrom(context, culture, value);",
                "return base.CanConvertTo(context, sourceType);",
                "return base.ConvertTo(context, culture, value, destinationType);",
                false);

        var result = type.Emit();

        await Assert.That(result.Code).Contains("public partial class UserIdTypeConverter");
        await Assert.That(result.Code).DoesNotContain("[global::System.ComponentModel.TypeConverter");
    }

    [Test]
    public async Task WithTypeConverter_ConfigureCallback_AllowsCustomization()
    {
        var type = TypeBuilder.Parse("public partial struct UserId")
            .WithTypeConverter("UserIdTypeConverter", t => t
                .AddMethod(MethodBuilder
                    .Parse(
                        "public override bool CanConvertFrom(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Type sourceType)")
                    .WithBody(b => b.AddStatement("return true;"))));

        var result = type.Emit();

        await Assert.That(result.Code).Contains("public partial class UserIdTypeConverter");
        await Assert.That(result.Code).Contains("return true;");
    }
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit;
using Deepstaging.Roslyn.Emit.Converters;

namespace Deepstaging.Roslyn.Tests.Emit.Extensions.Converters;

public sealed class TypeBuilderExtensionsEfCoreValueConverterTests : RoslynTestBase
{
    [Test]
    public async Task WithEfCoreValueConverter_AddsNestedConverterClass()
    {
        var type = TypeBuilder.Parse("public partial struct UserId")
            .WithEfCoreValueConverter(
                backingType: "global::System.Guid",
                toProviderExpression: "id => id.Value",
                fromProviderExpression: "value => new UserId(value)");

        var result = type.Emit();

        await Assert.That(result.Code).Contains("public partial class EfCoreValueConverter : global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<UserId, global::System.Guid>");
        await Assert.That(result.Code).Contains("id => id.Value");
        await Assert.That(result.Code).Contains("value => new UserId(value)");
    }

    [Test]
    public async Task WithEfCoreValueConverter_CustomConverterName()
    {
        var type = TypeBuilder.Parse("public partial struct UserId")
            .WithEfCoreValueConverter(
                backingType: "global::System.Guid",
                toProviderExpression: "id => id.Value",
                fromProviderExpression: "value => new UserId(value)",
                converterName: "UserIdEfConverter");

        var result = type.Emit();

        await Assert.That(result.Code).Contains("public partial class UserIdEfConverter");
    }

    [Test]
    public async Task WithEfCoreValueConverter_ConfigureCallback_AllowsCustomization()
    {
        var type = TypeBuilder.Parse("public partial struct UserId")
            .WithEfCoreValueConverter("global::System.Guid", t => t
                .AddConstructor(c => c.CallsBase("null")));

        var result = type.Emit();

        await Assert.That(result.Code).Contains("public partial class EfCoreValueConverter");
        await Assert.That(result.Code).Contains(": base(null)");
    }
}

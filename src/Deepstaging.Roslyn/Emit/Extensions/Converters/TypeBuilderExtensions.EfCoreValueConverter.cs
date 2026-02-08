// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Converters;

/// <summary>
/// Extensions for adding Entity Framework Core ValueConverter implementations.
/// </summary>
public static class TypeBuilderEfCoreValueConverterExtensions
{
    /// <summary>
    /// Adds a nested EF Core ValueConverter class.
    /// Uses lambda expressions for conversion in constructor.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="backingType">The backing/provider type (e.g., "global::System.Guid", "int").</param>
    /// <param name="toProviderExpression">Lambda to convert to provider type (e.g., "id => id.Value").</param>
    /// <param name="fromProviderExpression">Lambda to convert from provider type (e.g., "value => new UserId(value)").</param>
    /// <param name="converterName">Optional converter class name. Defaults to "EfCoreValueConverter".</param>
    /// <example>
    /// <code>
    /// TypeBuilder.Parse("public partial struct UserId")
    ///     .WithEfCoreValueConverter(
    ///         backingType: "global::System.Guid",
    ///         toProviderExpression: "id => id.Value",
    ///         fromProviderExpression: "value => new UserId(value)");
    /// </code>
    /// </example>
    public static TypeBuilder WithEfCoreValueConverter(
        this TypeBuilder builder,
        string backingType,
        string toProviderExpression,
        string fromProviderExpression,
        string converterName = "EfCoreValueConverter")
    {
        var typeName = builder.Name;

        var converterType = TypeBuilder
            .Parse($"public partial class {converterName} : global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<{typeName}, {backingType}>")
            .AddConstructor(c => c.CallsThis("null"))
            .AddConstructor(c => c
                .AddParameter("mappingHints",
                    "global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints?",
                    p => p.WithDefaultValue("null"))
                .CallsBase(
                    toProviderExpression,
                    fromProviderExpression,
                    "mappingHints"));

        return builder.AddNestedType(converterType);
    }

    /// <summary>
    /// Adds a nested EF Core ValueConverter class with a simple value accessor pattern.
    /// Uses the common pattern where the type has a Value property.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="backingType">The backing/provider type.</param>
    /// <param name="valueAccessor">The property/field to access the value. Defaults to "Value".</param>
    /// <param name="converterName">Optional converter class name. Defaults to "EfCoreValueConverter".</param>
    public static TypeBuilder WithEfCoreValueConverterForValue(
        this TypeBuilder builder,
        string backingType,
        string valueAccessor = "Value",
        string converterName = "EfCoreValueConverter")
    {
        var typeName = builder.Name;
        return builder.WithEfCoreValueConverter(
            backingType,
            $"id => id.{valueAccessor}",
            $"value => new {typeName}(value)",
            converterName);
    }

    /// <summary>
    /// Adds a nested EF Core ValueConverter class with full configuration.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="backingType">The backing/provider type.</param>
    /// <param name="configure">Configuration callback for the converter TypeBuilder.</param>
    /// <param name="converterName">Optional converter class name.</param>
    public static TypeBuilder WithEfCoreValueConverter(
        this TypeBuilder builder,
        string backingType,
        Func<TypeBuilder, TypeBuilder> configure,
        string converterName = "EfCoreValueConverter")
    {
        var typeName = builder.Name;

        var converterType = configure(TypeBuilder
            .Parse($"public partial class {converterName} : global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<{typeName}, {backingType}>"));

        return builder.AddNestedType(converterType);
    }
}

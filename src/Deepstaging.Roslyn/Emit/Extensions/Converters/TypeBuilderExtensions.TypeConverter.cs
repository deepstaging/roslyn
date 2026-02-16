// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Converters;

/// <summary>
/// Extensions for adding System.ComponentModel.TypeConverter implementations.
/// </summary>
public static class TypeBuilderTypeConverterExtensions
{
    /// <summary>
    /// Adds a nested TypeConverter class for type conversion support.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="converterName">The name of the nested converter class (e.g., "MyTypeConverter").</param>
    /// <param name="canConvertFromBody">The body of CanConvertFrom method.</param>
    /// <param name="convertFromBody">The body of ConvertFrom method.</param>
    /// <param name="canConvertToBody">The body of CanConvertTo method.</param>
    /// <param name="convertToBody">The body of ConvertTo method.</param>
    /// <param name="addAttribute">Whether to add [TypeConverter] attribute to the parent type.</param>
    /// <example>
    /// <code>
    /// TypeBuilder.Parse("public partial struct UserId")
    ///     .WithTypeConverter(
    ///         "UserIdTypeConverter",
    ///         canConvertFromBody: "return sourceType == typeof(Guid) || sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);",
    ///         convertFromBody: "return value switch { Guid g => new UserId(g), string s => new UserId(Guid.Parse(s)), _ => base.ConvertFrom(context, culture, value) };",
    ///         canConvertToBody: "return sourceType == typeof(Guid) || sourceType == typeof(string) || base.CanConvertTo(context, sourceType);",
    ///         convertToBody: "if (value is UserId id) { if (destinationType == typeof(Guid)) return id.Value; if (destinationType == typeof(string)) return id.Value.ToString(); } return base.ConvertTo(context, culture, value, destinationType);"
    ///     );
    /// </code>
    /// </example>
    public static TypeBuilder WithTypeConverter(
        this TypeBuilder builder,
        string converterName,
        string canConvertFromBody,
        string convertFromBody,
        string canConvertToBody,
        string convertToBody,
        bool addAttribute = true)
    {
        var converterType = TypeBuilder
            .Parse($"public partial class {converterName} : global::System.ComponentModel.TypeConverter")
            .AddMethod(MethodBuilder
                .Parse(
                    "public override bool CanConvertFrom(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Type sourceType)")
                .WithBody(b => b.AddStatements(canConvertFromBody)))
            .AddMethod(MethodBuilder
                .Parse(
                    "public override object? ConvertFrom(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Globalization.CultureInfo? culture, object value)")
                .WithBody(b => b.AddStatements(convertFromBody)))
            .AddMethod(MethodBuilder
                .Parse(
                    "public override bool CanConvertTo(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Type? sourceType)")
                .WithBody(b => b.AddStatements(canConvertToBody)))
            .AddMethod(MethodBuilder
                .Parse(
                    "public override object? ConvertTo(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Globalization.CultureInfo? culture, object? value, global::System.Type destinationType)")
                .WithBody(b => b.AddStatements(convertToBody)));

        var result = builder.AddNestedType(converterType);

        if (addAttribute)
            result = result.WithAttribute("global::System.ComponentModel.TypeConverter", a => a
                .WithArgument($"typeof({converterName})"));

        return result;
    }

    /// <summary>
    /// Adds a nested TypeConverter class with configuration callback.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="converterName">The name of the nested converter class.</param>
    /// <param name="configure">Configuration callback for the converter TypeBuilder.</param>
    /// <param name="addAttribute">Whether to add [TypeConverter] attribute to the parent type.</param>
    public static TypeBuilder WithTypeConverter(
        this TypeBuilder builder,
        string converterName,
        Func<TypeBuilder, TypeBuilder> configure,
        bool addAttribute = true)
    {
        var converterType = configure(TypeBuilder
            .Parse($"public partial class {converterName} : global::System.ComponentModel.TypeConverter"));

        var result = builder.AddNestedType(converterType);

        if (addAttribute)
            result = result.WithAttribute("global::System.ComponentModel.TypeConverter", a => a
                .WithArgument($"typeof({converterName})"));

        return result;
    }
}
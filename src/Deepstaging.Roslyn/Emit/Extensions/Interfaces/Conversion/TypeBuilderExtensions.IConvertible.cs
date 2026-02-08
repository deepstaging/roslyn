// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Core;

namespace Deepstaging.Roslyn.Emit.Interfaces.Conversion;

/// <summary>
/// TypeBuilder extensions for implementing IConvertible.
/// Adds conversion methods that delegate to the backing type.
/// </summary>
public static class TypeBuilderConvertibleExtensions
{
    /// <summary>
    /// Implements IConvertible using semantic analysis of the backing type.
    /// Delegates all conversions to the backing value's IConvertible implementation.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="backingType">The backing type symbol to delegate to.</param>
    /// <param name="valueAccessor">The property/field to access the value (e.g., "Value").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIConvertible(
        this TypeBuilder builder,
        ValidSymbol<INamedTypeSymbol> backingType,
        string valueAccessor)
    {
        var info = BackingTypeCore.From(backingType);
        var typeCode = GetTypeCode(info);

        return builder
            .Implements("global::System.IConvertible")
            .AddMethod(MethodBuilder
                .Parse("global::System.TypeCode IConvertible.GetTypeCode()")
                .WithExpressionBody($"global::System.TypeCode.{typeCode}"))
            .AddMethod(BuildConvertMethod("bool", "ToBoolean", valueAccessor))
            .AddMethod(BuildConvertMethod("byte", "ToByte", valueAccessor))
            .AddMethod(BuildConvertMethod("char", "ToChar", valueAccessor))
            .AddMethod(BuildConvertMethod("global::System.DateTime", "ToDateTime", valueAccessor))
            .AddMethod(BuildConvertMethod("decimal", "ToDecimal", valueAccessor))
            .AddMethod(BuildConvertMethod("double", "ToDouble", valueAccessor))
            .AddMethod(BuildConvertMethod("short", "ToInt16", valueAccessor))
            .AddMethod(BuildConvertMethod("int", "ToInt32", valueAccessor))
            .AddMethod(BuildConvertMethod("long", "ToInt64", valueAccessor))
            .AddMethod(BuildConvertMethod("sbyte", "ToSByte", valueAccessor))
            .AddMethod(BuildConvertMethod("float", "ToSingle", valueAccessor))
            .AddMethod(BuildToStringMethod(valueAccessor))
            .AddMethod(BuildToTypeMethod(valueAccessor))
            .AddMethod(BuildConvertMethod("ushort", "ToUInt16", valueAccessor))
            .AddMethod(BuildConvertMethod("uint", "ToUInt32", valueAccessor))
            .AddMethod(BuildConvertMethod("ulong", "ToUInt64", valueAccessor));
    }

    /// <summary>
    /// Implements IConvertible with a custom GetTypeCode value.
    /// Delegates all conversions to the backing value's IConvertible implementation.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="typeCode">The TypeCode to return from GetTypeCode().</param>
    /// <param name="valueAccessor">The property/field to access the value (e.g., "Value").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIConvertible(
        this TypeBuilder builder,
        string typeCode,
        string valueAccessor)
    {
        return builder
            .Implements("global::System.IConvertible")
            .AddMethod(MethodBuilder
                .Parse("global::System.TypeCode IConvertible.GetTypeCode()")
                .WithExpressionBody($"global::System.TypeCode.{typeCode}"))
            .AddMethod(BuildConvertMethod("bool", "ToBoolean", valueAccessor))
            .AddMethod(BuildConvertMethod("byte", "ToByte", valueAccessor))
            .AddMethod(BuildConvertMethod("char", "ToChar", valueAccessor))
            .AddMethod(BuildConvertMethod("global::System.DateTime", "ToDateTime", valueAccessor))
            .AddMethod(BuildConvertMethod("decimal", "ToDecimal", valueAccessor))
            .AddMethod(BuildConvertMethod("double", "ToDouble", valueAccessor))
            .AddMethod(BuildConvertMethod("short", "ToInt16", valueAccessor))
            .AddMethod(BuildConvertMethod("int", "ToInt32", valueAccessor))
            .AddMethod(BuildConvertMethod("long", "ToInt64", valueAccessor))
            .AddMethod(BuildConvertMethod("sbyte", "ToSByte", valueAccessor))
            .AddMethod(BuildConvertMethod("float", "ToSingle", valueAccessor))
            .AddMethod(BuildToStringMethod(valueAccessor))
            .AddMethod(BuildToTypeMethod(valueAccessor))
            .AddMethod(BuildConvertMethod("ushort", "ToUInt16", valueAccessor))
            .AddMethod(BuildConvertMethod("uint", "ToUInt32", valueAccessor))
            .AddMethod(BuildConvertMethod("ulong", "ToUInt64", valueAccessor));
    }

    private static string GetTypeCode(BackingTypeCore info)
    {
        if (info.IsInt32) return "Int32";
        if (info.IsInt64) return "Int64";
        if (info.IsInt16) return "Int16";
        if (info.IsByte) return "Byte";
        if (info.IsSingle) return "Single";
        if (info.IsDouble) return "Double";
        if (info.IsDecimal) return "Decimal";
        if (info.IsString) return "String";
        return "Object";
    }

    private static MethodBuilder BuildConvertMethod(string returnType, string methodName, string valueAccessor)
    {
        return MethodBuilder
            .Parse($"{returnType} IConvertible.{methodName}(global::System.IFormatProvider? provider)")
            .WithExpressionBody($"((global::System.IConvertible){valueAccessor}).{methodName}(provider)");
    }

    private static MethodBuilder BuildToStringMethod(string valueAccessor)
    {
        return MethodBuilder
            .Parse("string IConvertible.ToString(global::System.IFormatProvider? provider)")
            .WithExpressionBody($"((global::System.IConvertible){valueAccessor}).ToString(provider)");
    }

    private static MethodBuilder BuildToTypeMethod(string valueAccessor)
    {
        return MethodBuilder
            .Parse("object IConvertible.ToType(global::System.Type conversionType, global::System.IFormatProvider? provider)")
            .WithExpressionBody($"((global::System.IConvertible){valueAccessor}).ToType(conversionType, provider)");
    }

    /// <summary>
    /// Implements IConvertible using a property as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsIConvertible(
        this TypeBuilder builder,
        ValidSymbol<INamedTypeSymbol> backingType,
        PropertyBuilder property) =>
        builder.ImplementsIConvertible(backingType, property.Name);

    /// <summary>
    /// Implements IConvertible using a field as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsIConvertible(
        this TypeBuilder builder,
        ValidSymbol<INamedTypeSymbol> backingType,
        FieldBuilder field) =>
        builder.ImplementsIConvertible(backingType, field.Name);
}

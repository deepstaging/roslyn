// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Core;

/// <summary>
/// Core backing type information shared across all interface-specific info types.
/// Contains fundamental type properties needed by all TypeBuilder extensions.
/// </summary>
internal readonly struct BackingTypeCore
{
    /// <summary>Gets the analyzed type symbol.</summary>
    public ValidSymbol<INamedTypeSymbol> Type { get; }

    /// <summary>Gets whether the type is a value type.</summary>
    public bool IsValueType { get; }

    /// <summary>Gets whether the type is a reference type.</summary>
    public bool IsReferenceType { get; }

    /// <summary>Gets whether the type is a nullable reference type (e.g., string?).</summary>
    public bool IsNullableReference { get; }

    /// <summary>Gets the globally qualified type name for code generation.</summary>
    public string GloballyQualifiedName => Type.GloballyQualifiedName;

    /// <summary>Gets the simple type name.</summary>
    public string Name => Type.Name;

    /// <summary>
    /// Gets whether the type requires null-safe operations (reference types or nullable).
    /// </summary>
    public bool RequiresNullHandling => IsReferenceType || IsNullableReference;

    /// <summary>Gets whether the type is System.Guid.</summary>
    public bool IsGuid => Type.Value.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.Guid";

    /// <summary>Gets whether the type is System.Int32.</summary>
    public bool IsInt32 => Type.SpecialType == SpecialType.System_Int32;

    /// <summary>Gets whether the type is System.Int64.</summary>
    public bool IsInt64 => Type.SpecialType == SpecialType.System_Int64;

    /// <summary>Gets whether the type is System.Int16.</summary>
    public bool IsInt16 => Type.SpecialType == SpecialType.System_Int16;

    /// <summary>Gets whether the type is System.Byte.</summary>
    public bool IsByte => Type.SpecialType == SpecialType.System_Byte;

    /// <summary>Gets whether the type is System.Single.</summary>
    public bool IsSingle => Type.SpecialType == SpecialType.System_Single;

    /// <summary>Gets whether the type is System.Double.</summary>
    public bool IsDouble => Type.SpecialType == SpecialType.System_Double;

    /// <summary>Gets whether the type is System.Decimal.</summary>
    public bool IsDecimal => Type.SpecialType == SpecialType.System_Decimal;

    /// <summary>Gets whether the type is System.String.</summary>
    public bool IsString => Type.SpecialType == SpecialType.System_String;

    /// <summary>Gets whether the type is a numeric type (integers, floating point, decimal).</summary>
    public bool IsNumericType => IsInt32 || IsInt64 || IsInt16 || IsByte || IsSingle || IsDouble || IsDecimal;

    /// <summary>Gets whether the type is a parsable primitive (Guid, String, or numeric types).</summary>
    public bool IsParsablePrimitive => IsGuid || IsString || IsNumericType;

    /// <summary>
    /// Gets the C# keyword for the type, or null if no keyword applies.
    /// E.g., "int" for Int32, "long" for Int64, "string" for String, etc.
    /// </summary>
    public string? CSharpKeyword => Type.SpecialType switch
    {
        SpecialType.System_Int32 => "int",
        SpecialType.System_Int64 => "long",
        SpecialType.System_Int16 => "short",
        SpecialType.System_Byte => "byte",
        SpecialType.System_Single => "float",
        SpecialType.System_Double => "double",
        SpecialType.System_Decimal => "decimal",
        SpecialType.System_String => "string",
        _ => null
    };

    private BackingTypeCore(
        ValidSymbol<INamedTypeSymbol> type,
        bool isValueType,
        bool isReferenceType,
        bool isNullableReference)
    {
        Type = type;
        IsValueType = isValueType;
        IsReferenceType = isReferenceType;
        IsNullableReference = isNullableReference;
    }

    /// <summary>
    /// Creates a BackingTypeCore from the given type symbol.
    /// </summary>
    public static BackingTypeCore From(ValidSymbol<INamedTypeSymbol> type)
    {
        var symbol = type.Value;
        var isValueType = symbol.IsValueType;
        var isReferenceType = symbol.IsReferenceType;
        var isNullableReference = isReferenceType && symbol.NullableAnnotation == NullableAnnotation.Annotated;

        return new BackingTypeCore(type, isValueType, isReferenceType, isNullableReference);
    }
}

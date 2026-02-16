// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Core;

/// <summary>
/// Core backing type information shared across all interface-specific info types.
/// Contains fundamental type properties needed by all TypeBuilder extensions.
/// Pipeline-safe: accepts only <see cref="TypeSnapshot"/> — no Roslyn symbol dependencies.
/// </summary>
internal readonly struct BackingTypeCore
{
    /// <summary>Gets the globally qualified type name for code generation.</summary>
    public string GloballyQualifiedName { get; }

    /// <summary>Gets the simple type name.</summary>
    public string Name { get; }

    /// <summary>Gets whether the type is a value type.</summary>
    public bool IsValueType { get; }

    /// <summary>Gets whether the type is a reference type.</summary>
    public bool IsReferenceType { get; }

    /// <summary>Gets whether the type is a nullable reference type (e.g., string?).</summary>
    public bool IsNullableReference { get; }

    /// <summary>
    /// Gets whether the type requires null-safe operations (reference types or nullable).
    /// </summary>
    public bool RequiresNullHandling => IsReferenceType || IsNullableReference;

    /// <summary>Gets whether the type is System.Guid.</summary>
    public bool IsGuid => GloballyQualifiedName == "global::System.Guid";

    /// <summary>Gets whether the type is System.Int32.</summary>
    public bool IsInt32 => GloballyQualifiedName == "global::System.Int32";

    /// <summary>Gets whether the type is System.Int64.</summary>
    public bool IsInt64 => GloballyQualifiedName == "global::System.Int64";

    /// <summary>Gets whether the type is System.Int16.</summary>
    public bool IsInt16 => GloballyQualifiedName == "global::System.Int16";

    /// <summary>Gets whether the type is System.Byte.</summary>
    public bool IsByte => GloballyQualifiedName == "global::System.Byte";

    /// <summary>Gets whether the type is System.Single.</summary>
    public bool IsSingle => GloballyQualifiedName == "global::System.Single";

    /// <summary>Gets whether the type is System.Double.</summary>
    public bool IsDouble => GloballyQualifiedName == "global::System.Double";

    /// <summary>Gets whether the type is System.Decimal.</summary>
    public bool IsDecimal => GloballyQualifiedName == "global::System.Decimal";

    /// <summary>Gets whether the type is System.String.</summary>
    public bool IsString => GloballyQualifiedName == "global::System.String";

    /// <summary>Gets whether the type is a numeric type (integers, floating point, decimal).</summary>
    public bool IsNumericType => IsInt32 || IsInt64 || IsInt16 || IsByte || IsSingle || IsDouble || IsDecimal;

    /// <summary>Gets whether the type is a parsable primitive (Guid, String, or numeric types).</summary>
    public bool IsParsablePrimitive => IsGuid || IsString || IsNumericType;

    /// <summary>
    /// Gets the C# keyword for the type, or null if no keyword applies.
    /// E.g., "int" for Int32, "long" for Int64, "string" for String, etc.
    /// </summary>
    public string? CSharpKeyword => GloballyQualifiedName switch
    {
        "global::System.Int32" => "int",
        "global::System.Int64" => "long",
        "global::System.Int16" => "short",
        "global::System.Byte" => "byte",
        "global::System.Single" => "float",
        "global::System.Double" => "double",
        "global::System.Decimal" => "decimal",
        "global::System.String" => "string",
        _ => null
    };

    private BackingTypeCore(
        string globallyQualifiedName,
        string name,
        bool isValueType,
        bool isReferenceType,
        bool isNullableReference)
    {
        GloballyQualifiedName = globallyQualifiedName;
        Name = name;
        IsValueType = isValueType;
        IsReferenceType = isReferenceType;
        IsNullableReference = isNullableReference;
    }

    /// <summary>
    /// Creates a BackingTypeCore from a pipeline-safe type snapshot.
    /// </summary>
    public static BackingTypeCore From(TypeSnapshot type) => new(
        type.GloballyQualifiedName,
        type.Name,
        type.IsValueType,
        type.IsReferenceType,
        type.IsReferenceType && type.IsNullable);
}
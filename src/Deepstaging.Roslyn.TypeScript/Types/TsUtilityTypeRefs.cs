// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Types;

/// <summary>
/// A type-safe wrapper representing a TypeScript <c>Partial&lt;T&gt;</c> utility type reference.
/// Makes all properties of <c>T</c> optional.
/// </summary>
public readonly record struct TsPartialTypeRef
{
    /// <summary>Gets the inner type whose properties become optional.</summary>
    public TsTypeRef InnerType { get; }

    /// <summary>Creates a <c>TsPartialTypeRef</c> for the given inner type.</summary>
    /// <param name="innerType">The type to make partial.</param>
    public TsPartialTypeRef(TsTypeRef innerType) => InnerType = innerType;

    /// <summary>Gets the <c>Partial&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"Partial<{InnerType.Value}>";

    /// <summary>Implicitly converts to <see cref="TsTypeRef"/> for use in type declarations.</summary>
    public static implicit operator TsTypeRef(TsPartialTypeRef self) =>
        TsTypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TsPartialTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing a TypeScript <c>Required&lt;T&gt;</c> utility type reference.
/// Makes all properties of <c>T</c> required.
/// </summary>
public readonly record struct TsRequiredTypeRef
{
    /// <summary>Gets the inner type whose properties become required.</summary>
    public TsTypeRef InnerType { get; }

    /// <summary>Creates a <c>TsRequiredTypeRef</c> for the given inner type.</summary>
    /// <param name="innerType">The type to make required.</param>
    public TsRequiredTypeRef(TsTypeRef innerType) => InnerType = innerType;

    /// <summary>Gets the <c>Required&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"Required<{InnerType.Value}>";

    /// <summary>Implicitly converts to <see cref="TsTypeRef"/> for use in type declarations.</summary>
    public static implicit operator TsTypeRef(TsRequiredTypeRef self) =>
        TsTypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TsRequiredTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing a TypeScript <c>Readonly&lt;T&gt;</c> utility type reference.
/// Makes all properties of <c>T</c> readonly.
/// </summary>
public readonly record struct TsReadonlyTypeRef
{
    /// <summary>Gets the inner type whose properties become readonly.</summary>
    public TsTypeRef InnerType { get; }

    /// <summary>Creates a <c>TsReadonlyTypeRef</c> for the given inner type.</summary>
    /// <param name="innerType">The type to make readonly.</param>
    public TsReadonlyTypeRef(TsTypeRef innerType) => InnerType = innerType;

    /// <summary>Gets the <c>Readonly&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"Readonly<{InnerType.Value}>";

    /// <summary>Implicitly converts to <see cref="TsTypeRef"/> for use in type declarations.</summary>
    public static implicit operator TsTypeRef(TsReadonlyTypeRef self) =>
        TsTypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TsReadonlyTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing a TypeScript <c>Pick&lt;T, K&gt;</c> utility type reference.
/// Constructs a type by picking the set of properties <c>K</c> from <c>T</c>.
/// </summary>
public readonly record struct TsPickTypeRef
{
    /// <summary>Gets the object type to pick properties from.</summary>
    public TsTypeRef ObjectType { get; }

    /// <summary>Gets the keys to pick (e.g., <c>"'name' | 'email'"</c>).</summary>
    public TsTypeRef Keys { get; }

    /// <summary>Creates a <c>TsPickTypeRef</c> for the given object type and keys.</summary>
    /// <param name="objectType">The type to pick properties from.</param>
    /// <param name="keys">The property keys to pick.</param>
    public TsPickTypeRef(TsTypeRef objectType, TsTypeRef keys)
    {
        ObjectType = objectType;
        Keys = keys;
    }

    /// <summary>Gets the <c>Pick&lt;T, K&gt;</c> type string.</summary>
    public override string ToString() =>
        $"Pick<{ObjectType.Value}, {Keys.Value}>";

    /// <summary>Implicitly converts to <see cref="TsTypeRef"/> for use in type declarations.</summary>
    public static implicit operator TsTypeRef(TsPickTypeRef self) =>
        TsTypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TsPickTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing a TypeScript <c>Omit&lt;T, K&gt;</c> utility type reference.
/// Constructs a type by omitting the set of properties <c>K</c> from <c>T</c>.
/// </summary>
public readonly record struct TsOmitTypeRef
{
    /// <summary>Gets the object type to omit properties from.</summary>
    public TsTypeRef ObjectType { get; }

    /// <summary>Gets the keys to omit (e.g., <c>"'password' | 'secret'"</c>).</summary>
    public TsTypeRef Keys { get; }

    /// <summary>Creates a <c>TsOmitTypeRef</c> for the given object type and keys.</summary>
    /// <param name="objectType">The type to omit properties from.</param>
    /// <param name="keys">The property keys to omit.</param>
    public TsOmitTypeRef(TsTypeRef objectType, TsTypeRef keys)
    {
        ObjectType = objectType;
        Keys = keys;
    }

    /// <summary>Gets the <c>Omit&lt;T, K&gt;</c> type string.</summary>
    public override string ToString() =>
        $"Omit<{ObjectType.Value}, {Keys.Value}>";

    /// <summary>Implicitly converts to <see cref="TsTypeRef"/> for use in type declarations.</summary>
    public static implicit operator TsTypeRef(TsOmitTypeRef self) =>
        TsTypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TsOmitTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing a TypeScript <c>Exclude&lt;T, U&gt;</c> utility type reference.
/// Excludes from <c>T</c> those types that are assignable to <c>U</c>.
/// </summary>
public readonly record struct TsExcludeTypeRef
{
    /// <summary>Gets the union type to exclude members from.</summary>
    public TsTypeRef UnionType { get; }

    /// <summary>Gets the members to exclude from the union.</summary>
    public TsTypeRef ExcludedMembers { get; }

    /// <summary>Creates a <c>TsExcludeTypeRef</c> for the given union type and excluded members.</summary>
    /// <param name="unionType">The union type to filter.</param>
    /// <param name="excludedMembers">The members to exclude.</param>
    public TsExcludeTypeRef(TsTypeRef unionType, TsTypeRef excludedMembers)
    {
        UnionType = unionType;
        ExcludedMembers = excludedMembers;
    }

    /// <summary>Gets the <c>Exclude&lt;T, U&gt;</c> type string.</summary>
    public override string ToString() =>
        $"Exclude<{UnionType.Value}, {ExcludedMembers.Value}>";

    /// <summary>Implicitly converts to <see cref="TsTypeRef"/> for use in type declarations.</summary>
    public static implicit operator TsTypeRef(TsExcludeTypeRef self) =>
        TsTypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TsExcludeTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing a TypeScript <c>Extract&lt;T, U&gt;</c> utility type reference.
/// Extracts from <c>T</c> those types that are assignable to <c>U</c>.
/// </summary>
public readonly record struct TsExtractTypeRef
{
    /// <summary>Gets the union type to extract members from.</summary>
    public TsTypeRef UnionType { get; }

    /// <summary>Gets the members to extract from the union.</summary>
    public TsTypeRef ExtractedMembers { get; }

    /// <summary>Creates a <c>TsExtractTypeRef</c> for the given union type and extracted members.</summary>
    /// <param name="unionType">The union type to filter.</param>
    /// <param name="extractedMembers">The members to extract.</param>
    public TsExtractTypeRef(TsTypeRef unionType, TsTypeRef extractedMembers)
    {
        UnionType = unionType;
        ExtractedMembers = extractedMembers;
    }

    /// <summary>Gets the <c>Extract&lt;T, U&gt;</c> type string.</summary>
    public override string ToString() =>
        $"Extract<{UnionType.Value}, {ExtractedMembers.Value}>";

    /// <summary>Implicitly converts to <see cref="TsTypeRef"/> for use in type declarations.</summary>
    public static implicit operator TsTypeRef(TsExtractTypeRef self) =>
        TsTypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TsExtractTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing a TypeScript <c>NonNullable&lt;T&gt;</c> utility type reference.
/// Excludes <c>null</c> and <c>undefined</c> from <c>T</c>.
/// </summary>
public readonly record struct TsNonNullableTypeRef
{
    /// <summary>Gets the inner type to make non-nullable.</summary>
    public TsTypeRef InnerType { get; }

    /// <summary>Creates a <c>TsNonNullableTypeRef</c> for the given inner type.</summary>
    /// <param name="innerType">The type to make non-nullable.</param>
    public TsNonNullableTypeRef(TsTypeRef innerType) => InnerType = innerType;

    /// <summary>Gets the <c>NonNullable&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"NonNullable<{InnerType.Value}>";

    /// <summary>Implicitly converts to <see cref="TsTypeRef"/> for use in type declarations.</summary>
    public static implicit operator TsTypeRef(TsNonNullableTypeRef self) =>
        TsTypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TsNonNullableTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing a TypeScript <c>ReturnType&lt;T&gt;</c> utility type reference.
/// Obtains the return type of a function type <c>T</c>.
/// </summary>
public readonly record struct TsReturnTypeRef
{
    /// <summary>Gets the function type to extract the return type from.</summary>
    public TsTypeRef InnerType { get; }

    /// <summary>Creates a <c>TsReturnTypeRef</c> for the given function type.</summary>
    /// <param name="innerType">The function type to extract the return type from.</param>
    public TsReturnTypeRef(TsTypeRef innerType) => InnerType = innerType;

    /// <summary>Gets the <c>ReturnType&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"ReturnType<{InnerType.Value}>";

    /// <summary>Implicitly converts to <see cref="TsTypeRef"/> for use in type declarations.</summary>
    public static implicit operator TsTypeRef(TsReturnTypeRef self) =>
        TsTypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TsReturnTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing a TypeScript <c>Parameters&lt;T&gt;</c> utility type reference.
/// Obtains the parameter types of a function type <c>T</c> as a tuple.
/// </summary>
public readonly record struct TsParametersTypeRef
{
    /// <summary>Gets the function type to extract parameter types from.</summary>
    public TsTypeRef InnerType { get; }

    /// <summary>Creates a <c>TsParametersTypeRef</c> for the given function type.</summary>
    /// <param name="innerType">The function type to extract parameter types from.</param>
    public TsParametersTypeRef(TsTypeRef innerType) => InnerType = innerType;

    /// <summary>Gets the <c>Parameters&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"Parameters<{InnerType.Value}>";

    /// <summary>Implicitly converts to <see cref="TsTypeRef"/> for use in type declarations.</summary>
    public static implicit operator TsTypeRef(TsParametersTypeRef self) =>
        TsTypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TsParametersTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing a TypeScript <c>InstanceType&lt;T&gt;</c> utility type reference.
/// Obtains the instance type of a constructor function type <c>T</c>.
/// </summary>
public readonly record struct TsInstanceTypeRef
{
    /// <summary>Gets the constructor function type to extract the instance type from.</summary>
    public TsTypeRef InnerType { get; }

    /// <summary>Creates a <c>TsInstanceTypeRef</c> for the given constructor function type.</summary>
    /// <param name="innerType">The constructor function type to extract the instance type from.</param>
    public TsInstanceTypeRef(TsTypeRef innerType) => InnerType = innerType;

    /// <summary>Gets the <c>InstanceType&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"InstanceType<{InnerType.Value}>";

    /// <summary>Implicitly converts to <see cref="TsTypeRef"/> for use in type declarations.</summary>
    public static implicit operator TsTypeRef(TsInstanceTypeRef self) =>
        TsTypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TsInstanceTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing a TypeScript <c>Awaited&lt;T&gt;</c> utility type reference.
/// Recursively unwraps the resolved type of a <c>Promise</c>-like type <c>T</c>.
/// </summary>
public readonly record struct TsAwaitedTypeRef
{
    /// <summary>Gets the inner type to unwrap.</summary>
    public TsTypeRef InnerType { get; }

    /// <summary>Creates a <c>TsAwaitedTypeRef</c> for the given inner type.</summary>
    /// <param name="innerType">The type to unwrap.</param>
    public TsAwaitedTypeRef(TsTypeRef innerType) => InnerType = innerType;

    /// <summary>Gets the <c>Awaited&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"Awaited<{InnerType.Value}>";

    /// <summary>Implicitly converts to <see cref="TsTypeRef"/> for use in type declarations.</summary>
    public static implicit operator TsTypeRef(TsAwaitedTypeRef self) =>
        TsTypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TsAwaitedTypeRef self) =>
        self.ToString();
}

// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

using System.Collections.Immutable;

/// <summary>
/// A type-safe wrapper representing a <c>Func&lt;...&gt;</c> delegate type reference.
/// Carries the parameter types and return type.
/// </summary>
public readonly record struct FuncTypeRef
{
    /// <summary>Gets the parameter types.</summary>
    public ImmutableArray<TypeRef> ParameterTypes { get; }

    /// <summary>Gets the return type (the last type argument of <c>Func</c>).</summary>
    public TypeRef ReturnType { get; }

    /// <summary>Creates a <c>FuncTypeRef</c> for the given parameter and return types.</summary>
    public FuncTypeRef(ImmutableArray<TypeRef> parameterTypes, TypeRef returnType)
    {
        ParameterTypes = parameterTypes;
        ReturnType = returnType;
    }

    /// <summary>Creates a <c>FuncTypeRef</c> with no parameters.</summary>
    public FuncTypeRef(TypeRef returnType)
    {
        ParameterTypes = ImmutableArray<TypeRef>.Empty;
        ReturnType = returnType;
    }

    /// <summary>Gets the globally qualified <c>Func&lt;...&gt;</c> type string.</summary>
    public override string ToString()
    {
        var allTypes = ParameterTypes.IsDefaultOrEmpty
            ? ReturnType.Value
            : string.Join(", ", ParameterTypes.Select(t => t.Value)) + ", " + ReturnType.Value;

        return $"global::System.Func<{allTypes}>";
    }

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(FuncTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(FuncTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing an <c>Action&lt;...&gt;</c> delegate type reference.
/// Carries the parameter types.
/// </summary>
public readonly record struct ActionTypeRef
{
    /// <summary>Gets the parameter types.</summary>
    public ImmutableArray<TypeRef> ParameterTypes { get; }

    /// <summary>Creates an <c>ActionTypeRef</c> for the given parameter types.</summary>
    public ActionTypeRef(ImmutableArray<TypeRef> parameterTypes) =>
        ParameterTypes = parameterTypes;

    /// <summary>Creates an <c>ActionTypeRef</c> with a single parameter type.</summary>
    public ActionTypeRef(TypeRef parameterType) =>
        ParameterTypes = ImmutableArray.Create(parameterType);

    /// <summary>Gets the globally qualified <c>Action&lt;...&gt;</c> type string.</summary>
    public override string ToString()
    {
        if (ParameterTypes.IsDefaultOrEmpty)
            return "global::System.Action";

        var args = string.Join(", ", ParameterTypes.Select(t => t.Value));
        return $"global::System.Action<{args}>";
    }

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(ActionTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(ActionTypeRef self) =>
        self.ToString();
}
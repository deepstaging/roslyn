// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

using System.Collections.Immutable;

/// <summary>
/// Convenience <see cref="NamespaceRef"/> and factory methods for <c>System.Func</c> and <c>System.Action</c> delegate types.
/// </summary>
public static class DelegateTypes
{
    /// <summary>Creates a <c>Func&lt;...&gt;</c> type reference with the given parameter and return types.</summary>
    public static FuncTypeRef Func(TypeRef[] parameterTypes, TypeRef returnType) =>
        new(parameterTypes.ToImmutableArray(), returnType);

    /// <summary>Creates a <c>Func&lt;...&gt;</c> type reference from raw type name strings.</summary>
    public static FuncTypeRef Func(string[] parameterTypes, string returnType) =>
        new(parameterTypes.Select(TypeRef.From).ToImmutableArray(), TypeRef.From(returnType));

    /// <summary>Creates a <c>Func&lt;TResult&gt;</c> type reference with no parameters.</summary>
    public static FuncTypeRef Func(TypeRef returnType) => new(returnType);

    /// <summary>Creates an <c>Action</c> or <c>Action&lt;...&gt;</c> type reference.</summary>
    public static ActionTypeRef Action(params TypeRef[] parameterTypes) => new(parameterTypes.ToImmutableArray());

    /// <summary>Creates an <c>Action</c> or <c>Action&lt;...&gt;</c> type reference from raw type name strings.</summary>
    public static ActionTypeRef Action(params string[] parameterTypes) =>
        new(parameterTypes.Select(TypeRef.From).ToImmutableArray());

    /// <summary>Creates a <c>Predicate&lt;T&gt;</c> type reference.</summary>
    public static TypeRef Predicate(TypeRef type) => TypeRef.Global($"System.Predicate<{type.Value}>");

    /// <summary>Creates a <c>Predicate&lt;T&gt;</c> type reference from a raw type name string.</summary>
    public static TypeRef Predicate(string type) => Predicate(TypeRef.From(type));
}

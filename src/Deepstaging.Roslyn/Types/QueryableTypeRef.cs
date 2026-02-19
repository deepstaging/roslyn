// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// A type-safe wrapper representing an <c>IQueryable&lt;T&gt;</c> type reference.
/// Carries the element type for typed expression building.
/// </summary>
public readonly record struct QueryableTypeRef
{
    /// <summary>Gets the element type (e.g., <c>"Customer"</c>).</summary>
    public TypeRef ElementType { get; }

    /// <summary>Creates a <c>QueryableTypeRef</c> for the given element type.</summary>
    public QueryableTypeRef(TypeRef elementType) => ElementType = elementType;

    /// <summary>Gets the globally qualified <c>IQueryable&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Linq.IQueryable<{ElementType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(QueryableTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(QueryableTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing an <c>IOrderedQueryable&lt;T&gt;</c> type reference.
/// Carries the element type for typed expression building.
/// </summary>
public readonly record struct OrderedQueryableTypeRef
{
    /// <summary>Gets the element type (e.g., <c>"Customer"</c>).</summary>
    public TypeRef ElementType { get; }

    /// <summary>Creates an <c>OrderedQueryableTypeRef</c> for the given element type.</summary>
    public OrderedQueryableTypeRef(TypeRef elementType) => ElementType = elementType;

    /// <summary>Gets the globally qualified <c>IOrderedQueryable&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Linq.IOrderedQueryable<{ElementType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(OrderedQueryableTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(OrderedQueryableTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing an <c>Expression&lt;TDelegate&gt;</c> type reference from <c>System.Linq.Expressions</c>.
/// Carries the delegate type for typed expression building.
/// </summary>
public readonly record struct LinqExpressionTypeRef
{
    /// <summary>Gets the delegate type (e.g., <c>"Func&lt;Customer, bool&gt;"</c>).</summary>
    public TypeRef DelegateType { get; }

    /// <summary>Creates a <c>LinqExpressionTypeRef</c> for the given delegate type.</summary>
    public LinqExpressionTypeRef(TypeRef delegateType) => DelegateType = delegateType;

    /// <summary>Gets the globally qualified <c>Expression&lt;TDelegate&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::System.Linq.Expressions.Expression<{DelegateType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(LinqExpressionTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(LinqExpressionTypeRef self) =>
        self.ToString();
}

// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// Convenience <see cref="NamespaceRef"/> and factory methods for <c>System.Linq</c> types.
/// </summary>
public static class LinqTypes
{
    /// <summary>Gets the <c>System.Linq</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System.Linq");

    /// <summary>Gets the <c>System.Linq.Expressions</c> namespace.</summary>
    public static NamespaceRef ExpressionsNamespace => NamespaceRef.From("System.Linq.Expressions");

    /// <summary>Creates an <c>IQueryable&lt;T&gt;</c> type reference.</summary>
    public static QueryableTypeRef Queryable(TypeRef elementType) => new(elementType);

    /// <summary>Creates an <c>IOrderedQueryable&lt;T&gt;</c> type reference.</summary>
    public static OrderedQueryableTypeRef OrderedQueryable(TypeRef elementType) => new(elementType);

    /// <summary>Creates an <c>Expression&lt;TDelegate&gt;</c> type reference.</summary>
    public static LinqExpressionTypeRef Expression(TypeRef delegateType) => new(delegateType);
}

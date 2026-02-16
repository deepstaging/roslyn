// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Factory methods for <c>System.Linq</c> and <c>System.Linq.Expressions</c> types.
/// </summary>
public static class LinqRefs
{
    /// <summary>Gets the <c>System.Linq</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System.Linq");

    /// <summary>Gets the <c>System.Linq.Expressions</c> namespace.</summary>
    public static NamespaceRef ExpressionsNamespace => NamespaceRef.From("System.Linq.Expressions");

    /// <summary>Creates an <c>IQueryable&lt;T&gt;</c> type reference.</summary>
    public static TypeRef IQueryable(TypeRef elementType) =>
        Namespace.Type($"IQueryable<{elementType.Value}>");

    /// <summary>Creates an <c>IOrderedQueryable&lt;T&gt;</c> type reference.</summary>
    public static TypeRef IOrderedQueryable(TypeRef elementType) =>
        Namespace.Type($"IOrderedQueryable<{elementType.Value}>");

    /// <summary>Creates an <c>Expression&lt;TDelegate&gt;</c> type reference.</summary>
    public static TypeRef Expression(TypeRef delegateType) =>
        ExpressionsNamespace.Type($"Expression<{delegateType.Value}>");
}
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

using Types;

/// <summary>
/// Expression factory for Entity Framework Core operations.
/// </summary>
public static class EntityFrameworkExpression
{
    /// <summary>Produces <c>context.Set&lt;T&gt;()</c>.</summary>
    public static ExpressionRef Set(ExpressionRef context, TypeRef entityType) =>
        ExpressionRef.From($"{context.Value}.Set<{entityType.Value}>()");

    /// <summary>Produces <c>context.SaveChangesAsync(cancellationToken)</c>.</summary>
    public static ExpressionRef SaveChangesAsync(ExpressionRef context, ExpressionRef cancellationToken) =>
        context.Call("SaveChangesAsync", cancellationToken);

    /// <summary>Produces <c>context.SaveChangesAsync()</c>.</summary>
    public static ExpressionRef SaveChangesAsync(ExpressionRef context) =>
        context.Call("SaveChangesAsync");

    /// <summary>Produces <c>dbSet.FindAsync(keys)</c>.</summary>
    public static ExpressionRef FindAsync(ExpressionRef dbSet, params ExpressionRef[] keys) =>
        dbSet.Call("FindAsync", keys);

    /// <summary>Produces <c>dbSet.AddAsync(entity)</c>.</summary>
    public static ExpressionRef AddAsync(ExpressionRef dbSet, ExpressionRef entity) =>
        dbSet.Call("AddAsync", entity);

    /// <summary>Produces <c>dbSet.Remove(entity)</c>.</summary>
    public static ExpressionRef Remove(ExpressionRef dbSet, ExpressionRef entity) =>
        dbSet.Call("Remove", entity);
}

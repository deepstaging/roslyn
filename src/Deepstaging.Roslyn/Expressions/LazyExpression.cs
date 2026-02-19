// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

/// <summary>
/// Builds <c>Lazy&lt;T&gt;</c> expressions for use in generated code.
/// </summary>
/// <example>
/// <code>
/// LazyExpression.New("ExpensiveService", "() => new ExpensiveService()")
///     // "new global::System.Lazy&lt;ExpensiveService&gt;(() => new ExpensiveService())"
///
/// LazyExpression.Value("_lazyService")
///     // "_lazyService.Value"
/// </code>
/// </example>
public static class LazyExpression
{
    /// <summary>
    /// <c>new Lazy&lt;T&gt;(factory)</c> — creates a lazy instance with a value factory.
    /// </summary>
    /// <param name="valueType">The value type.</param>
    /// <param name="factory">The factory expression (e.g., <c>"() =&gt; new Service()"</c>).</param>
    public static ExpressionRef New(TypeRef valueType, ExpressionRef factory) =>
        ((TypeRef)new LazyTypeRef(valueType)).New(factory);

    /// <summary>
    /// <c>new Lazy&lt;T&gt;(factory, isThreadSafe)</c> — creates a lazy instance with thread-safety control.
    /// </summary>
    /// <param name="valueType">The value type.</param>
    /// <param name="factory">The factory expression.</param>
    /// <param name="isThreadSafe">The thread-safety expression (e.g., <c>"true"</c>).</param>
    public static ExpressionRef New(TypeRef valueType, ExpressionRef factory, ExpressionRef isThreadSafe) =>
        ((TypeRef)new LazyTypeRef(valueType)).New(factory, isThreadSafe);

    /// <summary>
    /// <c>expr.Value</c> — accesses the lazily-initialized value.
    /// </summary>
    /// <param name="expr">The Lazy instance expression.</param>
    public static ExpressionRef Value(ExpressionRef expr) =>
        expr.Member("Value");

    /// <summary>
    /// <c>expr.IsValueCreated</c> — checks whether the value has been created.
    /// </summary>
    /// <param name="expr">The Lazy instance expression.</param>
    public static ExpressionRef IsValueCreated(ExpressionRef expr) =>
        expr.Member("IsValueCreated");
}
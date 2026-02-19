// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

using Emit;

/// <summary>
/// TypeBuilder extensions for adding <c>Lazy&lt;T&gt;</c> backed fields with
/// a corresponding property that exposes the <c>.Value</c>.
/// </summary>
/// <example>
/// <code>
/// // Produces:
/// //   private readonly Lazy&lt;ExpensiveService&gt; _service = new Lazy&lt;ExpensiveService&gt;(() =&gt; new ExpensiveService());
/// //   public ExpensiveService Service =&gt; _service.Value;
/// builder.WithLazyField("ExpensiveService", "_service", "() => new ExpensiveService()", "Service");
/// </code>
/// </example>
public static class TypeBuilderLazyFieldExtensions
{
    /// <summary>
    /// Adds a <c>Lazy&lt;T&gt;</c> backing field and a read-only property that exposes <c>.Value</c>.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="valueType">The type of the lazily-initialized value.</param>
    /// <param name="fieldName">The backing field name (e.g., <c>"_service"</c>).</param>
    /// <param name="factory">The factory expression (e.g., <c>"() =&gt; new ExpensiveService()"</c>).</param>
    /// <param name="propertyName">The public property name (e.g., <c>"Service"</c>).</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder WithLazyField(
        this TypeBuilder builder,
        TypeRef valueType,
        string fieldName,
        ExpressionRef factory,
        string propertyName)
    {
        var lazyType = new LazyTypeRef(valueType);

        return builder
            .AddField(fieldName, lazyType, f => f
                .AsReadonly()
                .WithInitializer(LazyExpression.New(valueType, factory)))
            .AddProperty(propertyName, valueType, p => p
                .WithGetter($"{fieldName}.Value"));
    }
}
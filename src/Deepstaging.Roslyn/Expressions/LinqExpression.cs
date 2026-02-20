// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

using System.Collections.Immutable;

/// <summary>
/// Fluent builder for LINQ query-syntax expressions (<c>from … select</c>).
/// Returns an <see cref="ExpressionRef"/> suitable for expression bodies, return statements, or assignments.
/// Immutable — each method returns a new instance.
/// </summary>
/// <example>
/// <code>
/// // Simple select
/// LinqExpression
///     .From("x", "source")
///     .Select("x.Name")
///
/// // Where + OrderBy + Select
/// LinqExpression
///     .From("x", "source")
///     .Where("x.IsActive")
///     .OrderBy("x.Name")
///     .Select("x")
///
/// // Nested from (SelectMany)
/// LinqExpression
///     .From("customer", "customers")
///     .ThenFrom("order", "customer.Orders")
///     .Where("order.Total > 100")
///     .Select("new { customer.Name, order.Id }")
///
/// // Group by with into continuation
/// LinqExpression
///     .From("x", "source")
///     .GroupBy("x", "x.Category", into: "g")
///     .ThenFrom("item", "g")
///     .Select("new { Group = g.Key, Item = item.Name }")
/// </code>
/// </example>
public readonly record struct LinqExpression
{
    private ImmutableArray<string> Clauses { get; init; }

    private LinqExpression(ImmutableArray<string> clauses) => Clauses = clauses;

    /// <summary>
    /// Starts a LINQ query expression with a <c>from</c> clause.
    /// </summary>
    /// <param name="variable">The range variable name (e.g., <c>"x"</c>).</param>
    /// <param name="source">The source expression (e.g., <c>"customers"</c>).</param>
    public static LinqExpression From(string variable, string source) =>
        new(ImmutableArray.Create($"from {variable} in {source}"));

    /// <summary>
    /// Adds an additional <c>from</c> clause (cross join / SelectMany).
    /// </summary>
    /// <param name="variable">The range variable name.</param>
    /// <param name="source">The source expression.</param>
    public LinqExpression ThenFrom(string variable, string source) =>
        this with { Clauses = Clauses.Add($"from {variable} in {source}") };

    /// <summary>
    /// Adds a <c>let</c> clause that introduces a computed variable.
    /// </summary>
    /// <param name="variable">The variable name.</param>
    /// <param name="expression">The expression to assign.</param>
    public LinqExpression Let(string variable, string expression) =>
        this with { Clauses = Clauses.Add($"let {variable} = {expression}") };

    /// <summary>
    /// Adds a <c>where</c> clause that filters elements.
    /// </summary>
    /// <param name="condition">The filter condition.</param>
    public LinqExpression Where(string condition) =>
        this with { Clauses = Clauses.Add($"where {condition}") };

    /// <summary>
    /// Adds an <c>orderby</c> clause (ascending).
    /// </summary>
    /// <param name="key">The sort key expression.</param>
    public LinqExpression OrderBy(string key) =>
        this with { Clauses = Clauses.Add($"orderby {key}") };

    /// <summary>
    /// Adds an <c>orderby … descending</c> clause.
    /// </summary>
    /// <param name="key">The sort key expression.</param>
    public LinqExpression OrderByDescending(string key) =>
        this with { Clauses = Clauses.Add($"orderby {key} descending") };

    /// <summary>
    /// Adds a <c>join</c> clause.
    /// </summary>
    /// <param name="variable">The range variable for the joined source.</param>
    /// <param name="source">The source to join.</param>
    /// <param name="left">The left key expression.</param>
    /// <param name="right">The right key expression.</param>
    public LinqExpression Join(string variable, string source, string left, string right) =>
        this with { Clauses = Clauses.Add($"join {variable} in {source} on {left} equals {right}") };

    /// <summary>
    /// Adds a <c>join … into</c> clause (group join).
    /// </summary>
    /// <param name="variable">The range variable for the joined source.</param>
    /// <param name="source">The source to join.</param>
    /// <param name="left">The left key expression.</param>
    /// <param name="right">The right key expression.</param>
    /// <param name="into">The group variable name.</param>
    public LinqExpression Join(string variable, string source, string left, string right, string into) =>
        this with { Clauses = Clauses.Add($"join {variable} in {source} on {left} equals {right} into {into}") };

    /// <summary>
    /// Terminates the query with a <c>group … by</c> clause.
    /// </summary>
    /// <param name="element">The element expression to group.</param>
    /// <param name="key">The grouping key expression.</param>
    public ExpressionRef GroupBy(string element, string key) =>
        Build($"group {element} by {key}");

    /// <summary>
    /// Adds a <c>group … by … into</c> clause with a continuation, allowing further query clauses.
    /// </summary>
    /// <param name="element">The element expression to group.</param>
    /// <param name="key">The grouping key expression.</param>
    /// <param name="into">The continuation variable name.</param>
    public LinqExpression GroupBy(string element, string key, string into) =>
        this with { Clauses = Clauses.Add($"group {element} by {key} into {into}") };

    /// <summary>
    /// Terminates the query with a <c>select</c> clause and returns the composed <see cref="ExpressionRef"/>.
    /// </summary>
    /// <param name="expression">The projection expression.</param>
    public ExpressionRef Select(string expression) =>
        Build($"select {expression}");

    private ExpressionRef Build(string terminal) =>
        ExpressionRef.From(string.Join("\n", Clauses.Add(terminal)));
}

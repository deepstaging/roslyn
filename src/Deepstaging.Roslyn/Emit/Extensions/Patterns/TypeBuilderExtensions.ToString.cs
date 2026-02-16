// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Patterns;

/// <summary>
/// TypeBuilder extensions for overriding ToString().
/// </summary>
public static class TypeBuilderToStringExtensions
{
    /// <summary>
    /// Overrides ToString() to delegate to the backing value's ToString().
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="valueAccessor">The property/field name to access the backing value (e.g., "Value").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder OverridesToString(
        this TypeBuilder builder,
        string valueAccessor) => builder.AddMethod(MethodBuilder
        .Parse("public override string ToString()")
        .WithExpressionBody($"{valueAccessor}.ToString()"));

    /// <summary>
    /// Overrides ToString() to delegate to the backing value's ToString(),
    /// with null-safe handling for reference types.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="valueAccessor">The property/field name to access the backing value (e.g., "Value").</param>
    /// <param name="nullValue">The string to return when the backing value is null. Defaults to empty string.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder OverridesToStringNullSafe(
        this TypeBuilder builder,
        string valueAccessor,
        string nullValue = "\"\"") => builder.AddMethod(MethodBuilder
        .Parse("public override string ToString()")
        .WithExpressionBody($"{valueAccessor}?.ToString() ?? {nullValue}"));

    /// <summary>
    /// Overrides ToString() using a custom expression.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="expression">The expression body for ToString() (e.g., "$\"Id: {Value}\"").</param>
    /// <param name="isCustomExpression">Disambiguation parameter (always pass true).</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder OverridesToString(
        this TypeBuilder builder,
        string expression,
        bool isCustomExpression) => builder.AddMethod(MethodBuilder
        .Parse("public override string ToString()")
        .WithExpressionBody(expression));

    /// <summary>
    /// Overrides ToString() using a property as the backing value.
    /// </summary>
    public static TypeBuilder OverridesToString(
        this TypeBuilder builder,
        PropertyBuilder property) =>
        builder.OverridesToString(property.Name);

    /// <summary>
    /// Overrides ToString() using a field as the backing value.
    /// </summary>
    public static TypeBuilder OverridesToString(
        this TypeBuilder builder,
        FieldBuilder field) =>
        builder.OverridesToString(field.Name);

    /// <summary>
    /// Overrides ToString() with null-safe handling using a property.
    /// </summary>
    public static TypeBuilder OverridesToStringNullSafe(
        this TypeBuilder builder,
        PropertyBuilder property,
        string nullValue = "\"\"") =>
        builder.OverridesToStringNullSafe(property.Name, nullValue);

    /// <summary>
    /// Overrides ToString() with null-safe handling using a field.
    /// </summary>
    public static TypeBuilder OverridesToStringNullSafe(
        this TypeBuilder builder,
        FieldBuilder field,
        string nullValue = "\"\"") =>
        builder.OverridesToStringNullSafe(field.Name, nullValue);
}
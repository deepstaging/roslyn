// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Operators.Arithmetic;

/// <summary>
/// TypeBuilder extensions for implementing the subtraction operator (-).
/// Implements ISubtractionOperators&lt;TSelf, TSelf, TSelf&gt; (NET7+).
/// </summary>
public static class TypeBuilderSubtractionOperatorExtensions
{
    /// <summary>
    /// Implements the subtraction operator (-) using semantic analysis of the backing type.
    /// Generates ISubtractionOperators&lt;T, T, T&gt; interface implementation (NET7+).
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="backingType">The backing type symbol to delegate to.</param>
    /// <param name="valueAccessor">The property/field name to access the backing value (e.g., "Value").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsSubtractionOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        string valueAccessor)
    {
        var typeName = builder.Name;
        var info = ArithmeticTypeInfo.From(backingType);

        if (!info.SupportsArithmetic)
            return builder;

        return builder
            .Implements(
                $"global::System.Numerics.ISubtractionOperators<{typeName}, {typeName}, {typeName}>",
                Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .Subtraction(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody($"new(left.{valueAccessor} - right.{valueAccessor})"));
    }

    /// <summary>
    /// Implements the subtraction operator (-) using a custom expression.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="subtractionExpression">Expression body for the - operator (e.g., "new(left.Value - right.Value)").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsSubtractionOperator(
        this TypeBuilder builder,
        string subtractionExpression)
    {
        var typeName = builder.Name;

        return builder
            .Implements(
                $"global::System.Numerics.ISubtractionOperators<{typeName}, {typeName}, {typeName}>",
                Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .Subtraction(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody(subtractionExpression));
    }

    /// <summary>
    /// Implements the subtraction operator (-) using a property as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsSubtractionOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        PropertyBuilder property) =>
        builder.ImplementsSubtractionOperator(backingType, property.Name);

    /// <summary>
    /// Implements the subtraction operator (-) using a field as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsSubtractionOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        FieldBuilder field) =>
        builder.ImplementsSubtractionOperator(backingType, field.Name);
}
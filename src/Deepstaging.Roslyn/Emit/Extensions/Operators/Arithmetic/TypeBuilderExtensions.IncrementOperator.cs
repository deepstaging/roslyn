// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Operators.Arithmetic;

/// <summary>
/// TypeBuilder extensions for implementing the increment operator (++).
/// Implements IIncrementOperators&lt;TSelf&gt; (NET7+).
/// </summary>
public static class TypeBuilderIncrementOperatorExtensions
{
    /// <summary>
    /// Implements the increment operator (++) using semantic analysis of the backing type.
    /// Generates IIncrementOperators&lt;T&gt; interface implementation (NET7+).
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="backingType">The backing type symbol to delegate to.</param>
    /// <param name="valueAccessor">The property/field name to access the backing value (e.g., "Value").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIncrementOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        string valueAccessor)
    {
        var typeName = builder.Name;
        var info = ArithmeticTypeInfo.From(backingType);

        if (!info.SupportsArithmetic)
            return builder;

        return builder
            .Implements($"global::System.Numerics.IIncrementOperators<{typeName}>", Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .Increment(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody($"new(operand.{valueAccessor} + 1)"));
    }

    /// <summary>
    /// Implements the increment operator (++) using a custom expression.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="incrementExpression">Expression body for the ++ operator (e.g., "new(operand.Value + 1)").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIncrementOperator(
        this TypeBuilder builder,
        string incrementExpression)
    {
        var typeName = builder.Name;

        return builder
            .Implements($"global::System.Numerics.IIncrementOperators<{typeName}>", Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .Increment(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody(incrementExpression));
    }

    /// <summary>
    /// Implements the increment operator (++) using a property as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsIncrementOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        PropertyBuilder property) =>
        builder.ImplementsIncrementOperator(backingType, property.Name);

    /// <summary>
    /// Implements the increment operator (++) using a field as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsIncrementOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        FieldBuilder field) =>
        builder.ImplementsIncrementOperator(backingType, field.Name);
}

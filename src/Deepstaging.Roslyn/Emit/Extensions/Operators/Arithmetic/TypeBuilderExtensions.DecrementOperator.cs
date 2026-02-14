// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Operators.Arithmetic;

/// <summary>
/// TypeBuilder extensions for implementing the decrement operator (--).
/// Implements IDecrementOperators&lt;TSelf&gt; (NET7+).
/// </summary>
public static class TypeBuilderDecrementOperatorExtensions
{
    /// <summary>
    /// Implements the decrement operator (--) using semantic analysis of the backing type.
    /// Generates IDecrementOperators&lt;T&gt; interface implementation (NET7+).
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="backingType">The backing type symbol to delegate to.</param>
    /// <param name="valueAccessor">The property/field name to access the backing value (e.g., "Value").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsDecrementOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        string valueAccessor)
    {
        var typeName = builder.Name;
        var info = ArithmeticTypeInfo.From(backingType);

        if (!info.SupportsArithmetic)
            return builder;

        return builder
            .Implements($"global::System.Numerics.IDecrementOperators<{typeName}>", Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .Decrement(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody($"new(operand.{valueAccessor} - 1)"));
    }

    /// <summary>
    /// Implements the decrement operator (--) using a custom expression.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="decrementExpression">Expression body for the -- operator (e.g., "new(operand.Value - 1)").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsDecrementOperator(
        this TypeBuilder builder,
        string decrementExpression)
    {
        var typeName = builder.Name;

        return builder
            .Implements($"global::System.Numerics.IDecrementOperators<{typeName}>", Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .Decrement(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody(decrementExpression));
    }

    /// <summary>
    /// Implements the decrement operator (--) using a property as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsDecrementOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        PropertyBuilder property) =>
        builder.ImplementsDecrementOperator(backingType, property.Name);

    /// <summary>
    /// Implements the decrement operator (--) using a field as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsDecrementOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        FieldBuilder field) =>
        builder.ImplementsDecrementOperator(backingType, field.Name);
}

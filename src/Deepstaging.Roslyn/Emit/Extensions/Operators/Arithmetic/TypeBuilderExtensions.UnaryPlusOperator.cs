// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Operators.Arithmetic;

/// <summary>
/// TypeBuilder extensions for implementing the unary plus operator (+).
/// Implements IUnaryPlusOperators&lt;TSelf, TSelf&gt; (NET7+).
/// </summary>
public static class TypeBuilderUnaryPlusOperatorExtensions
{
    /// <summary>
    /// Implements the unary plus operator (+) using semantic analysis of the backing type.
    /// Generates IUnaryPlusOperators&lt;T, T&gt; interface implementation (NET7+).
    /// </summary>
    public static TypeBuilder ImplementsUnaryPlusOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        string valueAccessor)
    {
        var typeName = builder.Name;
        var info = ArithmeticTypeInfo.From(backingType);

        if (!info.SupportsArithmetic)
            return builder;

        return builder
            .Implements($"global::System.Numerics.IUnaryPlusOperators<{typeName}, {typeName}>",
                Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .UnaryPlus(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody($"new(+operand.{valueAccessor})"));
    }

    /// <summary>
    /// Implements the unary plus operator (+) using a custom expression.
    /// </summary>
    public static TypeBuilder ImplementsUnaryPlusOperator(
        this TypeBuilder builder,
        string plusExpression)
    {
        var typeName = builder.Name;

        return builder
            .Implements($"global::System.Numerics.IUnaryPlusOperators<{typeName}, {typeName}>",
                Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .UnaryPlus(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody(plusExpression));
    }

    /// <summary>
    /// Implements the unary plus operator (+) using a property as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsUnaryPlusOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        PropertyBuilder property) =>
        builder.ImplementsUnaryPlusOperator(backingType, property.Name);

    /// <summary>
    /// Implements the unary plus operator (+) using a field as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsUnaryPlusOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        FieldBuilder field) =>
        builder.ImplementsUnaryPlusOperator(backingType, field.Name);
}
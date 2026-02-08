// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Operators.Arithmetic;

/// <summary>
/// TypeBuilder extensions for implementing the unary negation operator (-).
/// Implements IUnaryNegationOperators&lt;TSelf, TSelf&gt; (NET7+).
/// </summary>
public static class TypeBuilderUnaryNegationOperatorExtensions
{
    /// <summary>
    /// Implements the unary negation operator (-) using semantic analysis of the backing type.
    /// Generates IUnaryNegationOperators&lt;T, T&gt; interface implementation (NET7+).
    /// </summary>
    public static TypeBuilder ImplementsUnaryNegationOperator(
        this TypeBuilder builder,
        ValidSymbol<INamedTypeSymbol> backingType,
        string valueAccessor)
    {
        var typeName = builder.Name;
        var info = ArithmeticTypeInfo.From(backingType);

        if (!info.SupportsArithmetic)
            return builder;

        return builder
            .Implements($"global::System.Numerics.IUnaryNegationOperators<{typeName}, {typeName}>", Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .UnaryMinus(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody($"new(-operand.{valueAccessor})"));
    }

    /// <summary>
    /// Implements the unary negation operator (-) using a custom expression.
    /// </summary>
    public static TypeBuilder ImplementsUnaryNegationOperator(
        this TypeBuilder builder,
        string negationExpression)
    {
        var typeName = builder.Name;

        return builder
            .Implements($"global::System.Numerics.IUnaryNegationOperators<{typeName}, {typeName}>", Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .UnaryMinus(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody(negationExpression));
    }

    /// <summary>
    /// Implements the unary negation operator (-) using a property as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsUnaryNegationOperator(
        this TypeBuilder builder,
        ValidSymbol<INamedTypeSymbol> backingType,
        PropertyBuilder property) =>
        builder.ImplementsUnaryNegationOperator(backingType, property.Name);

    /// <summary>
    /// Implements the unary negation operator (-) using a field as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsUnaryNegationOperator(
        this TypeBuilder builder,
        ValidSymbol<INamedTypeSymbol> backingType,
        FieldBuilder field) =>
        builder.ImplementsUnaryNegationOperator(backingType, field.Name);
}

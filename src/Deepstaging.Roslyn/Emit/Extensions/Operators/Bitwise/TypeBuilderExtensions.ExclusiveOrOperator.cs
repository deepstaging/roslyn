// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Operators.Arithmetic;

namespace Deepstaging.Roslyn.Emit.Operators.Bitwise;

/// <summary>
/// TypeBuilder extensions for implementing the exclusive OR (XOR) operator (^).
/// Implements IBitwiseOperators&lt;TSelf, TSelf, TSelf&gt; (NET7+).
/// </summary>
public static class TypeBuilderExclusiveOrOperatorExtensions
{
    /// <summary>
    /// Implements the exclusive OR operator (^) using semantic analysis of the backing type.
    /// </summary>
    public static TypeBuilder ImplementsExclusiveOrOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        string valueAccessor)
    {
        var typeName = builder.Name;
        var info = ArithmeticTypeInfo.From(backingType);

        if (!info.SupportsBitwise)
            return builder;

        return builder
            .Implements($"global::System.Numerics.IBitwiseOperators<{typeName}, {typeName}, {typeName}>", Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .ExclusiveOr(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody($"new(left.{valueAccessor} ^ right.{valueAccessor})"));
    }

    /// <summary>
    /// Implements the exclusive OR operator (^) using a custom expression.
    /// </summary>
    public static TypeBuilder ImplementsExclusiveOrOperator(
        this TypeBuilder builder,
        string exclusiveOrExpression)
    {
        var typeName = builder.Name;

        return builder
            .Implements($"global::System.Numerics.IBitwiseOperators<{typeName}, {typeName}, {typeName}>", Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .ExclusiveOr(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody(exclusiveOrExpression));
    }

    /// <summary>
    /// Implements the exclusive OR operator (^) using a property as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsExclusiveOrOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        PropertyBuilder property) =>
        builder.ImplementsExclusiveOrOperator(backingType, property.Name);

    /// <summary>
    /// Implements the exclusive OR operator (^) using a field as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsExclusiveOrOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        FieldBuilder field) =>
        builder.ImplementsExclusiveOrOperator(backingType, field.Name);
}

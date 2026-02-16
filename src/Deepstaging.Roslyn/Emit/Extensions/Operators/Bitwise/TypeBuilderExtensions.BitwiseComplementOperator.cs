// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Operators.Arithmetic;

namespace Deepstaging.Roslyn.Emit.Operators.Bitwise;

/// <summary>
/// TypeBuilder extensions for implementing the bitwise complement operator (~).
/// Implements IBitwiseOperators&lt;TSelf, TSelf, TSelf&gt; (NET7+).
/// </summary>
public static class TypeBuilderBitwiseComplementOperatorExtensions
{
    /// <summary>
    /// Implements the bitwise complement operator (~) using semantic analysis of the backing type.
    /// </summary>
    public static TypeBuilder ImplementsBitwiseComplementOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        string valueAccessor)
    {
        var typeName = builder.Name;
        var info = ArithmeticTypeInfo.From(backingType);

        if (!info.SupportsBitwise)
            return builder;

        return builder
            .Implements($"global::System.Numerics.IBitwiseOperators<{typeName}, {typeName}, {typeName}>",
                Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .BitwiseComplement(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody($"new(~operand.{valueAccessor})"));
    }

    /// <summary>
    /// Implements the bitwise complement operator (~) using a custom expression.
    /// </summary>
    public static TypeBuilder ImplementsBitwiseComplementOperator(
        this TypeBuilder builder,
        string bitwiseComplementExpression)
    {
        var typeName = builder.Name;

        return builder
            .Implements($"global::System.Numerics.IBitwiseOperators<{typeName}, {typeName}, {typeName}>",
                Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .BitwiseComplement(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody(bitwiseComplementExpression));
    }

    /// <summary>
    /// Implements the bitwise complement operator (~) using a property as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsBitwiseComplementOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        PropertyBuilder property) =>
        builder.ImplementsBitwiseComplementOperator(backingType, property.Name);

    /// <summary>
    /// Implements the bitwise complement operator (~) using a field as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsBitwiseComplementOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        FieldBuilder field) =>
        builder.ImplementsBitwiseComplementOperator(backingType, field.Name);
}
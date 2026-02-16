// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Operators.Arithmetic;

namespace Deepstaging.Roslyn.Emit.Operators.Bitwise;

/// <summary>
/// TypeBuilder extensions for implementing the bitwise AND operator (&amp;).
/// Implements IBitwiseOperators&lt;TSelf, TSelf, TSelf&gt; (NET7+).
/// </summary>
public static class TypeBuilderBitwiseAndOperatorExtensions
{
    /// <summary>
    /// Implements the bitwise AND operator (&amp;) using semantic analysis of the backing type.
    /// Generates IBitwiseOperators&lt;T, T, T&gt; interface implementation (NET7+).
    /// </summary>
    public static TypeBuilder ImplementsBitwiseAndOperator(
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
                .BitwiseAnd(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody($"new(left.{valueAccessor} & right.{valueAccessor})"));
    }

    /// <summary>
    /// Implements the bitwise AND operator (&amp;) using a custom expression.
    /// </summary>
    public static TypeBuilder ImplementsBitwiseAndOperator(
        this TypeBuilder builder,
        string bitwiseAndExpression)
    {
        var typeName = builder.Name;

        return builder
            .Implements($"global::System.Numerics.IBitwiseOperators<{typeName}, {typeName}, {typeName}>",
                Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .BitwiseAnd(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody(bitwiseAndExpression));
    }

    /// <summary>
    /// Implements the bitwise AND operator (&amp;) using a property as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsBitwiseAndOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        PropertyBuilder property) =>
        builder.ImplementsBitwiseAndOperator(backingType, property.Name);

    /// <summary>
    /// Implements the bitwise AND operator (&amp;) using a field as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsBitwiseAndOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        FieldBuilder field) =>
        builder.ImplementsBitwiseAndOperator(backingType, field.Name);
}
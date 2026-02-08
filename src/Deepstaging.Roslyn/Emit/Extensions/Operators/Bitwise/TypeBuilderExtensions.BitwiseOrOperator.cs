// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Operators.Arithmetic;

namespace Deepstaging.Roslyn.Emit.Operators.Bitwise;

/// <summary>
/// TypeBuilder extensions for implementing the bitwise OR operator (|).
/// Implements IBitwiseOperators&lt;TSelf, TSelf, TSelf&gt; (NET7+).
/// </summary>
public static class TypeBuilderBitwiseOrOperatorExtensions
{
    /// <summary>
    /// Implements the bitwise OR operator (|) using semantic analysis of the backing type.
    /// </summary>
    public static TypeBuilder ImplementsBitwiseOrOperator(
        this TypeBuilder builder,
        ValidSymbol<INamedTypeSymbol> backingType,
        string valueAccessor)
    {
        var typeName = builder.Name;
        var info = ArithmeticTypeInfo.From(backingType);

        if (!info.SupportsBitwise)
            return builder;

        return builder
            .Implements($"global::System.Numerics.IBitwiseOperators<{typeName}, {typeName}, {typeName}>", Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .BitwiseOr(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody($"new(left.{valueAccessor} | right.{valueAccessor})"));
    }

    /// <summary>
    /// Implements the bitwise OR operator (|) using a custom expression.
    /// </summary>
    public static TypeBuilder ImplementsBitwiseOrOperator(
        this TypeBuilder builder,
        string bitwiseOrExpression)
    {
        var typeName = builder.Name;

        return builder
            .Implements($"global::System.Numerics.IBitwiseOperators<{typeName}, {typeName}, {typeName}>", Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .BitwiseOr(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody(bitwiseOrExpression));
    }

    /// <summary>
    /// Implements the bitwise OR operator (|) using a property as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsBitwiseOrOperator(
        this TypeBuilder builder,
        ValidSymbol<INamedTypeSymbol> backingType,
        PropertyBuilder property) =>
        builder.ImplementsBitwiseOrOperator(backingType, property.Name);

    /// <summary>
    /// Implements the bitwise OR operator (|) using a field as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsBitwiseOrOperator(
        this TypeBuilder builder,
        ValidSymbol<INamedTypeSymbol> backingType,
        FieldBuilder field) =>
        builder.ImplementsBitwiseOrOperator(backingType, field.Name);
}

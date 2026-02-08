// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Operators.Arithmetic;

namespace Deepstaging.Roslyn.Emit.Operators.Bitwise;

/// <summary>
/// TypeBuilder extensions for implementing the right shift operator (&gt;&gt;).
/// Implements IShiftOperators&lt;TSelf, int, TSelf&gt; (NET7+).
/// </summary>
public static class TypeBuilderRightShiftOperatorExtensions
{
    /// <summary>
    /// Implements the right shift operator (&gt;&gt;) using semantic analysis of the backing type.
    /// </summary>
    public static TypeBuilder ImplementsRightShiftOperator(
        this TypeBuilder builder,
        ValidSymbol<INamedTypeSymbol> backingType,
        string valueAccessor)
    {
        var typeName = builder.Name;
        var info = ArithmeticTypeInfo.From(backingType);

        if (!info.SupportsShift)
            return builder;

        return builder
            .Implements($"global::System.Numerics.IShiftOperators<{typeName}, int, {typeName}>", Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .RightShift(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody($"new(left.{valueAccessor} >> right)"));
    }

    /// <summary>
    /// Implements the right shift operator (&gt;&gt;) using a custom expression.
    /// </summary>
    public static TypeBuilder ImplementsRightShiftOperator(
        this TypeBuilder builder,
        string rightShiftExpression)
    {
        var typeName = builder.Name;

        return builder
            .Implements($"global::System.Numerics.IShiftOperators<{typeName}, int, {typeName}>", Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .RightShift(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody(rightShiftExpression));
    }

    /// <summary>
    /// Implements the right shift operator (&gt;&gt;) using a property as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsRightShiftOperator(
        this TypeBuilder builder,
        ValidSymbol<INamedTypeSymbol> backingType,
        PropertyBuilder property) =>
        builder.ImplementsRightShiftOperator(backingType, property.Name);

    /// <summary>
    /// Implements the right shift operator (&gt;&gt;) using a field as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsRightShiftOperator(
        this TypeBuilder builder,
        ValidSymbol<INamedTypeSymbol> backingType,
        FieldBuilder field) =>
        builder.ImplementsRightShiftOperator(backingType, field.Name);
}

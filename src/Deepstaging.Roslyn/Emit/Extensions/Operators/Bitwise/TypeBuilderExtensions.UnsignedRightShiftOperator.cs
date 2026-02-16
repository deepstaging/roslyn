// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Operators.Arithmetic;

namespace Deepstaging.Roslyn.Emit.Operators.Bitwise;

/// <summary>
/// TypeBuilder extensions for implementing the unsigned right shift operator (&gt;&gt;&gt;).
/// Implements IShiftOperators&lt;TSelf, int, TSelf&gt; (NET7+).
/// </summary>
public static class TypeBuilderUnsignedRightShiftOperatorExtensions
{
    /// <summary>
    /// Implements the unsigned right shift operator (&gt;&gt;&gt;) using semantic analysis of the backing type.
    /// </summary>
    public static TypeBuilder ImplementsUnsignedRightShiftOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        string valueAccessor)
    {
        var typeName = builder.Name;
        var info = ArithmeticTypeInfo.From(backingType);

        if (!info.SupportsShift)
            return builder;

        return builder
            .Implements($"global::System.Numerics.IShiftOperators<{typeName}, int, {typeName}>",
                Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .UnsignedRightShift(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody($"new(left.{valueAccessor} >>> right)"));
    }

    /// <summary>
    /// Implements the unsigned right shift operator (&gt;&gt;&gt;) using a custom expression.
    /// </summary>
    public static TypeBuilder ImplementsUnsignedRightShiftOperator(
        this TypeBuilder builder,
        string unsignedRightShiftExpression)
    {
        var typeName = builder.Name;

        return builder
            .Implements($"global::System.Numerics.IShiftOperators<{typeName}, int, {typeName}>",
                Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .UnsignedRightShift(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody(unsignedRightShiftExpression));
    }

    /// <summary>
    /// Implements the unsigned right shift operator (&gt;&gt;&gt;) using a property as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsUnsignedRightShiftOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        PropertyBuilder property) =>
        builder.ImplementsUnsignedRightShiftOperator(backingType, property.Name);

    /// <summary>
    /// Implements the unsigned right shift operator (&gt;&gt;&gt;) using a field as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsUnsignedRightShiftOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        FieldBuilder field) =>
        builder.ImplementsUnsignedRightShiftOperator(backingType, field.Name);
}
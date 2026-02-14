// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Operators.Arithmetic;

namespace Deepstaging.Roslyn.Emit.Operators.Bitwise;

/// <summary>
/// TypeBuilder extensions for implementing the left shift operator (&lt;&lt;).
/// Implements IShiftOperators&lt;TSelf, int, TSelf&gt; (NET7+).
/// </summary>
public static class TypeBuilderLeftShiftOperatorExtensions
{
    /// <summary>
    /// Implements the left shift operator (&lt;&lt;) using semantic analysis of the backing type.
    /// </summary>
    public static TypeBuilder ImplementsLeftShiftOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        string valueAccessor)
    {
        var typeName = builder.Name;
        var info = ArithmeticTypeInfo.From(backingType);

        if (!info.SupportsShift)
            return builder;

        return builder
            .Implements($"global::System.Numerics.IShiftOperators<{typeName}, int, {typeName}>", Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .LeftShift(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody($"new(left.{valueAccessor} << right)"));
    }

    /// <summary>
    /// Implements the left shift operator (&lt;&lt;) using a custom expression.
    /// </summary>
    public static TypeBuilder ImplementsLeftShiftOperator(
        this TypeBuilder builder,
        string leftShiftExpression)
    {
        var typeName = builder.Name;

        return builder
            .Implements($"global::System.Numerics.IShiftOperators<{typeName}, int, {typeName}>", Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .LeftShift(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody(leftShiftExpression));
    }

    /// <summary>
    /// Implements the left shift operator (&lt;&lt;) using a property as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsLeftShiftOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        PropertyBuilder property) =>
        builder.ImplementsLeftShiftOperator(backingType, property.Name);

    /// <summary>
    /// Implements the left shift operator (&lt;&lt;) using a field as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsLeftShiftOperator(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        FieldBuilder field) =>
        builder.ImplementsLeftShiftOperator(backingType, field.Name);
}

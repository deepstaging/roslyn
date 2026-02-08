// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Operators.Arithmetic;

/// <summary>
/// TypeBuilder extensions for implementing the division operator (/).
/// Implements IDivisionOperators&lt;TSelf, TSelf, TSelf&gt; (NET7+).
/// </summary>
public static class TypeBuilderDivisionOperatorExtensions
{
    /// <summary>
    /// Implements the division operator (/) using semantic analysis of the backing type.
    /// Generates IDivisionOperators&lt;T, T, T&gt; interface implementation (NET7+).
    /// </summary>
    public static TypeBuilder ImplementsDivisionOperator(
        this TypeBuilder builder,
        ValidSymbol<INamedTypeSymbol> backingType,
        string valueAccessor)
    {
        var typeName = builder.Name;
        var info = ArithmeticTypeInfo.From(backingType);

        if (!info.SupportsArithmetic)
            return builder;

        return builder
            .Implements($"global::System.Numerics.IDivisionOperators<{typeName}, {typeName}, {typeName}>", Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .Division(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody($"new(left.{valueAccessor} / right.{valueAccessor})"));
    }

    /// <summary>
    /// Implements the division operator (/) using a custom expression.
    /// </summary>
    public static TypeBuilder ImplementsDivisionOperator(
        this TypeBuilder builder,
        string divisionExpression)
    {
        var typeName = builder.Name;

        return builder
            .Implements($"global::System.Numerics.IDivisionOperators<{typeName}, {typeName}, {typeName}>", Directives.Net7OrGreater)
            .AddOperator(OperatorBuilder
                .Division(typeName)
                .When(Directives.Net7OrGreater)
                .WithExpressionBody(divisionExpression));
    }

    /// <summary>
    /// Implements the division operator (/) using a property as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsDivisionOperator(
        this TypeBuilder builder,
        ValidSymbol<INamedTypeSymbol> backingType,
        PropertyBuilder property) =>
        builder.ImplementsDivisionOperator(backingType, property.Name);

    /// <summary>
    /// Implements the division operator (/) using a field as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsDivisionOperator(
        this TypeBuilder builder,
        ValidSymbol<INamedTypeSymbol> backingType,
        FieldBuilder field) =>
        builder.ImplementsDivisionOperator(backingType, field.Name);
}
